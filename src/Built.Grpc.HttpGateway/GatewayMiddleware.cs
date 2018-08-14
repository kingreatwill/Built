using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Built.Grpc.HttpGateway
{
    public class GatewayMiddleware
    {
        #region ctor

        private readonly RequestDelegate _next;

        /// <summary>
        /// Middleware pipeline to be executed on every server request.
        /// </summary>
        private readonly Pipeline MiddlewarePipeline;

        /// <summary>
        /// GatewayMiddleware
        /// </summary>
        public GatewayMiddleware(RequestDelegate next, Pipeline pipeline)
        {
            _next = next;
            MiddlewarePipeline = pipeline;
        }

        #endregion ctor

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                string requestPath = httpContext.Request.Path.Value;

                if (!GrpcServiceMethodFactory.Handers.Any() || !GrpcServiceMethodFactory.Handers.TryGetValue(requestPath, out GrpcMethodHandlerInfo method))
                {
                    if (_next != null) { await _next(httpContext); }
                    return;
                }

                IDictionary<string, string> headers = GetRequestHeaders(httpContext);

                string requestJson = GetRequestJson(httpContext);

                object requestObject = DeselializeFromJson(requestJson, method.GetJsonRequestType());

                object responseObject = await CallGrpcAsync(method, headers, requestObject);

                string responseJson = SerializeToJson(responseObject);

                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsync(responseJson);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        /// <param name="json"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private object DeselializeFromJson(string json, Type objectType)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(json, objectType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string SerializeToJson(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        private string GetRequestJson(HttpContext context)
        {
            if (context.Request.Method == "GET")
            {
                JObject o = new JObject();
                foreach (var q in context.Request.Query)
                {
                    o.Add(q.Key, q.Value.ToString());
                }
                return SerializeToJson(o);
            }
            else
            {
                using (var sr = new System.IO.StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private IDictionary<string, string> GetRequestHeaders(HttpContext context)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (string key in context.Request.Headers.Keys)
            {
                string grpcKey = null;
                string prefix = "grpc.";
                if (key.Length > prefix.Length && key.StartsWith(prefix))
                {
                    grpcKey = key.Substring(prefix.Length);
                }
                else
                {
                    continue;
                }
                Microsoft.Extensions.Primitives.StringValues value = context.Request.Headers[key];
                headers.Add(grpcKey, value.FirstOrDefault());
            }
            return headers;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public Task<object> CallGrpcAsync(GrpcMethodHandlerInfo method, IDictionary<string, string> headers, object requestObject)
        {
            object requests;

            if (requestObject != null && typeof(IEnumerable<>).MakeGenericType(method.RequestType).IsAssignableFrom(requestObject.GetType()))
            {
                requests = requestObject;
            }
            else
            {
                Array ary = Array.CreateInstance(method.RequestType, 1);
                ary.SetValue(requestObject, 0);
                requests = ary;
            }

            System.Reflection.MethodInfo m = typeof(GatewayMiddleware).GetMethod("CallGrpcAsyncCore", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            Task<object> task = (Task<object>)m.MakeGenericMethod(new Type[] { method.RequestType, method.ResponseType }).Invoke(this, new object[] { method, headers, requests });

            return task;
        }

        private Task<object> CallGrpcAsyncCore<TRequest, TResponse>(GrpcMethodHandlerInfo method, IDictionary<string, string> headers, IEnumerable<TRequest> requests) where TRequest : class where TResponse : class
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            CallInvoker invoker = new MiddlewareCallInvoker(channel, MiddlewarePipeline);

            CallOptions option = CreateCallOptions(headers);

            Method<TRequest, TResponse> rpc = (Method<TRequest, TResponse>)method.Method;

            switch (rpc.Type)
            {
                case MethodType.Unary:

                    Task<TResponse> taskUnary = AsyncUnaryCall(invoker, rpc, option, requests.FirstOrDefault());

                    var s = taskUnary.Result;
                    return Task.FromResult<object>(taskUnary.Result);

                //case MethodType.ClientStreaming:

                //    Task<TResponse> taskClientStreaming = AsyncClientStreamingCall(invoker, rpc, option, requests);

                //    return Task.FromResult<object>(taskClientStreaming.Result);

                //case MethodType.ServerStreaming:

                //    Task<IList<TResponse>> taskServerStreaming = AsyncServerStreamingCall(invoker, rpc, option, requests.FirstOrDefault());

                //    return Task.FromResult<object>(taskServerStreaming.Result);

                //case MethodType.DuplexStreaming:

                //    Task<IList<TResponse>> taskDuplexStreaming = AsyncDuplexStreamingCall(invoker, rpc, option, requests);

                //    return Task.FromResult<object>(taskDuplexStreaming.Result);

                default:
                    throw new NotSupportedException(string.Format("MethodType '{0}' is not supported.", rpc.Type));
            }
        }

        private CallOptions CreateCallOptions(IDictionary<string, string> headers)
        {
            Metadata meta = new Metadata();

            foreach (KeyValuePair<string, string> entry in headers)
            {
                meta.Add(entry.Key, entry.Value);
            }

            CallOptions option = new CallOptions(meta);

            return option;
        }

        private Task<TResponse> AsyncUnaryCall<TRequest, TResponse>(CallInvoker invoker, Method<TRequest, TResponse> method, CallOptions option, TRequest request) where TRequest : class where TResponse : class
        {
            return invoker.AsyncUnaryCall(method, null, option, request).ResponseAsync;
        }
    }
}
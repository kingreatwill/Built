using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;

namespace Built.Grpc.HttpGateway
{
    public class GatewayMiddleware
    {
        #region ctor

        /// <summary>
        /// GatewayMiddleware
        /// </summary>
        public GatewayMiddleware()
        {
        }

        /// <summary>
        /// GatewayMiddleware
        /// </summary>
        public GatewayMiddleware(RequestDelegate next)
        {
        }

        #endregion ctor

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public Task<object> CallGrpcAsync(GrpcServiceMethod method, IDictionary<string, string> headers, object requestObject)
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

        private Task<object> CallGrpcAsyncCore<TRequest, TResponse>(GrpcServiceMethod method, IDictionary<string, string> headers, IEnumerable<TRequest> requests) where TRequest : class where TResponse : class
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            CallInvoker invoker = new DefaultCallInvoker(channel);

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
            var s = invoker.BlockingUnaryCall(method, null, option, request);

            return invoker.AsyncUnaryCall(method, null, option, request).ResponseAsync;
        }
    }
}
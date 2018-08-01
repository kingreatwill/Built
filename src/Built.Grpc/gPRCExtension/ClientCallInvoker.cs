using Grpc.Core;
using Grpc.Core.Interceptors;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FM.ConsulInterop
{
    internal sealed class ClientCallInvoker : CallInvoker
    {
        private Channel grpcChannel;
        private ClientCallActionCollection _callActionCollection;

        /// <summary>
        /// Middleware pipeline to be executed on every server request.
        /// </summary>
        private Pipeline MiddlewarePipeline { get; set; }

        public ClientCallInvoker(Channel channel)
        {
            this.grpcChannel = channel;
        }

        public ClientCallInvoker(Channel channel, Pipeline pipeline) : this(channel)
        {
            this.MiddlewarePipeline = pipeline;
        }

        public ClientCallInvoker(Channel channel, ClientCallActionCollection clientCallActionCollection) : this(channel)
        {
            this._callActionCollection = clientCallActionCollection;
        }

        private TResponse Call<TResponse>(Func<CallInvoker, MiddlewareContext, TResponse> call, MiddlewareContext context)
        {
            ServerCallInvoker callInvoker = new ServerCallInvoker(grpcChannel);

            // 实现方式2  继承Interceptor
            //var ss = callInvoker.Intercept(new ClientCallbackInterceptor(
            //        () => Console.WriteLine("-----------ClientCallbackInterceptor----------------------")
            //        ));
            // callInvoker.Intercept(new Interceptor());

            /*
             .Intercept(new ClientCallbackInterceptor(() => stringBuilder.Append("array1")),
                new ClientCallbackInterceptor(() => stringBuilder.Append("array2")),
                new ClientCallbackInterceptor(() => stringBuilder.Append("array3")))
            .Intercept(metadata =>
            {
                stringBuilder.Append("interceptor2");
                return metadata;
            }).Intercept(metadata =>
            {
                stringBuilder.Append("interceptor3");
                return metadata;
            });
             */
            TResponse response = default(TResponse);
            if (MiddlewarePipeline != null)
            {
                context.HandlerExecutor = async () =>
                {
                    response = await Task.FromResult(call(callInvoker, context));
                    context.Response = response;
                };
                MiddlewarePipeline.RunPipeline(context).ConfigureAwait(false);
            }
            else
            {
                response = call(callInvoker, context);
            }
            return response;
            /*
             *
             *
             *
             //catch (RpcException rpcEx)
            //{
            //    if (rpcEx.Status.StatusCode == StatusCode.Unavailable)
            //    {
            //        // 注销服务;剔除服务;
            //    }
            //    throw;
            //}
              var times = 0;
                if (options.Headers != null)
                {
                    var retry = options.Headers.FirstOrDefault(t => t.Key == "grpc-retry");
                    if (!string.IsNullOrWhiteSpace(retry?.Value))
                    {
                        times = int.Parse(retry?.Value);
                    }
                }
                Policy
                .Handle<Exception>()
                .Retry(times, (exception, retryCount) =>
                {
                    response = callInvoker.BlockingUnaryCall(method, host, options, request);
                });
             */
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method,
            string host, CallOptions options, TRequest request)
        {
            return Call((ci, context) => ci.BlockingUnaryCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options, (TRequest)context.Request), new MiddlewareContext
            {
                Host = host,
                Method = method,
                Options = options,
                Request = request,
                Response = null
            });
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            return Call((ci, context) => ci.AsyncUnaryCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options, (TRequest)context.Request), new MiddlewareContext
            {
                Host = host,
                Method = method,
                Options = options,
                Request = request,
                Response = null
            });
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options,
            TRequest request)
        {
            return Call((ci, context) => ci.AsyncServerStreamingCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options, (TRequest)context.Request), new MiddlewareContext
            {
                Host = host,
                Method = method,
                Options = options,
                Request = request,
                Response = null
            });
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call((ci, context) => ci.AsyncClientStreamingCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options), new MiddlewareContext
            {
                Host = host,
                Method = method,
                Options = options,
                Request = null,
                Response = null
            });
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            return Call((ci, context) => ci.AsyncDuplexStreamingCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options), new MiddlewareContext
            {
                Host = host,
                Method = method,
                Options = options,
                Request = null,
                Response = null
            });
        }
    }
}
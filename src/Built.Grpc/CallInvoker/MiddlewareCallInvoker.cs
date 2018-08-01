using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace Built.Grpc
{
    //DefaultCallInvoker
    internal sealed class MiddlewareCallInvoker : DefaultCallInvoker
    {
        private readonly Channel grpcChannel;

        /// <summary>
        /// Middleware pipeline to be executed on every server request.
        /// </summary>
        private Pipeline MiddlewarePipeline { get; set; }

        public MiddlewareCallInvoker(Channel channel) : base(channel)
        {
            this.grpcChannel = channel;
        }

        public MiddlewareCallInvoker(Channel channel, Pipeline pipeline) : this(channel)
        {
            this.MiddlewarePipeline = pipeline;
        }

        private TResponse Call<TResponse>(Func<MiddlewareContext, TResponse> call, MiddlewareContext context)
        {
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
                    response = await Task.FromResult(call(context));
                    context.Response = response;
                };
                MiddlewarePipeline.RunPipeline(context).ConfigureAwait(false);
            }
            else
            {
                response = call(context);
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
            return Call((context) => base.BlockingUnaryCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options, (TRequest)context.Request), new MiddlewareContext
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
            return Call((context) => base.AsyncUnaryCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options, (TRequest)context.Request), new MiddlewareContext
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
            return Call((context) => base.AsyncServerStreamingCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options, (TRequest)context.Request), new MiddlewareContext
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
            return Call((context) => base.AsyncClientStreamingCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options), new MiddlewareContext
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
            return Call((context) => base.AsyncDuplexStreamingCall((Method<TRequest, TResponse>)context.Method, context.Host, context.Options), new MiddlewareContext
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
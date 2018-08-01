using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FM.ConsulInterop
{
    /*

            var helper = new MockServiceHelper(Host);
            const string MetadataKey = "x-interceptor";
            const string MetadataValue = "hello world";
            var interceptor = new ServerCallContextInterceptor(ctx => ctx.RequestHeaders.Add(new Metadata.Entry(MetadataKey, MetadataValue)));
            helper.UnaryHandler = new UnaryServerMethod<string, string>((request, context) =>
            {
                var interceptorHeader = context.RequestHeaders.Last(m => (m.Key == MetadataKey)).Value;
                Assert.AreEqual(interceptorHeader, MetadataValue);
                return Task.FromResult("PASS");
            });
            helper.ServiceDefinition = helper.ServiceDefinition.Intercept(interceptor);
            var server = helper.GetServer();
            server.Start();
            var channel = helper.GetChannel();
            Assert.AreEqual("PASS", Calls.BlockingUnaryCall(helper.CreateUnaryCall(), ""));
         */

    public class ServerCallContextInterceptor : Interceptor
    {
        private readonly Action<ServerCallContext> interceptor;

        public ServerCallContextInterceptor(Action<ServerCallContext> interceptor)
        {
            GrpcPreconditions.CheckNotNull(interceptor, nameof(interceptor));
            this.interceptor = interceptor;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            interceptor(context);
            return continuation(request, context);
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            interceptor(context);
            return continuation(requestStream, context);
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            interceptor(context);
            return continuation(request, responseStream, context);
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            interceptor(context);
            return continuation(requestStream, responseStream, context);
        }
    }
}
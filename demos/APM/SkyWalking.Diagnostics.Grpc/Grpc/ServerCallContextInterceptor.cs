using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Core.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SkyWalking.Diagnostics.Grpc
{
    public class ServerCallContextInterceptor : Interceptor
    {
        private static readonly DiagnosticListener listener = new DiagnosticListener(GrpcDiagnosticListenerExtensions.DiagnosticListenerName);

        public ServerCallContextInterceptor()
        {
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var opId = listener.ServerRequest(request);

            var result = continuation(request, context);

            listener.ServerResponse(opId.ToString(), "OK");//cath error
            return result;
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var result = continuation(requestStream, context);
            return result;
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var result = continuation(request, responseStream, context);
            return result;
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var result = continuation(requestStream, responseStream, context);
            return result;
        }
    }
}
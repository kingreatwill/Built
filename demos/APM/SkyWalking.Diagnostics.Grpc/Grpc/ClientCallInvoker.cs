using Grpc.Core;
using Grpc.Core.Utils;
using SkyWalking.Context;
using System.Diagnostics;

namespace SkyWalking.Diagnostics.Grpc
{
    /// <summary>
    /// Invokes client RPCs using <see cref="Calls"/>.
    /// </summary>
    public class ClientCallInvoker : CallInvoker
    {
        private readonly Channel channel;
        private static readonly DiagnosticListener listener = new DiagnosticListener(GrpcDiagnosticListenerExtensions.DiagnosticListenerName);

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientCallInvoker"/> class.
        /// </summary>
        /// <param name="channel">Channel to use.</param>
        public ClientCallInvoker(Channel channel)
        {
            this.channel = GrpcPreconditions.CheckNotNull(channel);
        }

        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var sd = ContextManager.GlobalTraceId;
            var result = Calls.BlockingUnaryCall(CreateCall(method, host, options), request);
            return result;
        }

        /// <summary>
        /// Invokes a simple remote call asynchronously.
        /// </summary>
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var result = Calls.AsyncUnaryCall(CreateCall(method, host, options), request);
            return result;
        }

        /// <summary>
        /// Invokes a server streaming call asynchronously.
        /// In server streaming scenario, client sends on request and server responds with a stream of responses.
        /// </summary>
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var result = Calls.AsyncServerStreamingCall(CreateCall(method, host, options), request);
            return result;
        }

        /// <summary>
        /// Invokes a client streaming call asynchronously.
        /// In client streaming scenario, client sends a stream of requests and server responds with a single response.
        /// </summary>
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var result = Calls.AsyncClientStreamingCall(CreateCall(method, host, options));
            return result;
        }

        /// <summary>
        /// Invokes a duplex streaming call asynchronously.
        /// In duplex streaming scenario, client sends a stream of requests and server responds with a stream of responses.
        /// The response stream is completely independent and both side can be sending messages at the same time.
        /// </summary>
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var result = Calls.AsyncDuplexStreamingCall(CreateCall(method, host, options));
            return result;
        }

        /// <summary>Creates call invocation details for given method.</summary>
        protected virtual CallInvocationDetails<TRequest, TResponse> CreateCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
                where TRequest : class
                where TResponse : class
        {
            return new CallInvocationDetails<TRequest, TResponse>(channel, method, host, options);
        }
    }
}
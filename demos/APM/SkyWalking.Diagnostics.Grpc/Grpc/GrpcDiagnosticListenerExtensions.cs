using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SkyWalking.Diagnostics.Grpc
{
    public static class GrpcDiagnosticListenerExtensions
    {
        public const string DiagnosticListenerName = "GrpcDiagnosticListener";

        private const string Prefix = "DotNetCore.Grpc.";

        public const string ServerRequestMessageStore = Prefix + nameof(ServerRequest);

        public const string ServerResponseMessageStore = Prefix + nameof(ServerResponse);

        public const string ClientRequestMessageStore = Prefix + nameof(ClientRequest);

        public const string ClientResponseMessageStore = Prefix + nameof(ClientResponse);

        public static Guid ServerRequest(this DiagnosticListener listener, object request, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(ServerRequestMessageStore))
            {
                var operationId = Guid.NewGuid();

                listener.Write(ServerRequestMessageStore, new
                {
                    OperationId = operationId,
                    Operation = operation,
                    MessageContent = request
                });

                return operationId;
            }

            return Guid.Empty;
        }

        public static void ServerResponse(this DiagnosticListener listener, string operationId, object response, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(ServerResponseMessageStore))
            {
                listener.Write(ServerResponseMessageStore, new
                {
                    OperationId = operationId,
                    Operation = operation,
                    MessageContent = response
                });
            }
        }

        public static Guid ClientRequest(this DiagnosticListener listener, object request, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(ClientRequestMessageStore))
            {
                var operationId = Guid.NewGuid();

                listener.Write(ClientRequestMessageStore, new
                {
                    OperationId = operationId,
                    Operation = operation,
                    MessageContent = request
                });

                return operationId;
            }

            return Guid.Empty;
        }

        public static void ClientResponse(this DiagnosticListener listener, string operationId, object response, [CallerMemberName] string operation = "")
        {
            if (listener.IsEnabled(ClientResponseMessageStore))
            {
                listener.Write(ClientResponseMessageStore, new
                {
                    OperationId = operationId,
                    Operation = operation,
                    MessageContent = response
                });
            }
        }
    }
}
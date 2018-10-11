using System;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using SkyWalking.Utilities.DependencyInjection;

namespace SkyWalking.Diagnostics.Grpc
{
    public class GrpcEventData
    {
        public Guid OperationId { get; set; }

        public string Operation { get; set; }

        public Metadata Headers { get; set; }
    }
}
using Grpc.Core;
using System.Collections.Generic;

namespace Built.Grpc
{
    public class ClientAgentOption
    {
        /// <summary>
        /// Middleware Pipeline
        /// </summary>
        public Pipeline MiddlewarePipeline { get; set; }

        /// <summary>
        /// grpc channel options
        /// </summary>
        public IEnumerable<ChannelOption> ChannelOptions { get; set; }
    }
}
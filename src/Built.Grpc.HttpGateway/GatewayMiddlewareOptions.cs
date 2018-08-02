using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Grpc.HttpGateway
{
    public class GatewayMiddlewareOptions : IOptions<GatewayMiddlewareOptions>
    {
        private readonly Pipeline Pipeline;
        private readonly ServiceDiscoveryProvider Discovery;

        public GatewayMiddlewareOptions()
        {
            TimoutMilliseconds = 60000;
        }

        public int TimoutMilliseconds { get; set; }

        GatewayMiddlewareOptions IOptions<GatewayMiddlewareOptions>.Value
        {
            get
            {
                return this;
            }
        }
    }

    public class ServiceDiscoveryProvider
    {
        public string Host { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// Consul/Eureka/Etcd/Zookeeper
        /// </summary>
        public string Type { get; set; } = "Consul";

        /// <summary>
        /// PollingInterval=0 则每次都从注册中心获取服务地址
        /// </summary>
        public int PollingInterval { get; set; }
    }
}
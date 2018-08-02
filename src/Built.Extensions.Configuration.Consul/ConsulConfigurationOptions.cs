using Consul;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Extensions.Configuration.Consul
{
    public class ConsulConfigurationOptions : ConsulClientConfiguration, IOptions<ConsulConfigurationOptions>
    {
        public string Prefix { get; set; }

        ConsulConfigurationOptions IOptions<ConsulConfigurationOptions>.Value
        {
            get
            {
                return this;
            }
        }
    }
}
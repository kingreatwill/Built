using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Grpc.HttpGateway
{
    public class SwaggerMiddlewareOptions : IOptions<SwaggerMiddlewareOptions>
    {
        private readonly string RoutePrefix;
        private readonly string ApiPrefix;

        public SwaggerMiddlewareOptions()
        {
        }

        SwaggerMiddlewareOptions IOptions<SwaggerMiddlewareOptions>.Value
        {
            get
            {
                return this;
            }
        }
    }
}
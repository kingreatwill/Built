using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Built.Grpc.HttpGateway
{
    public class GatewayMiddleware
    {
        #region ctor

        /// <summary>
        /// GatewayMiddleware
        /// </summary>
        public GatewayMiddleware(RequestDelegate next)
        {
        }

        #endregion ctor

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
        }
    }
}
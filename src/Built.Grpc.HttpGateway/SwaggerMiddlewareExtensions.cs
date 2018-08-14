using Built.Grpc.HttpGateway.Swagger;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Grpc.HttpGateway
{
    public static class SwaggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseBuiltGrpcSwagger(this IApplicationBuilder app, SwaggerOptions options)
        {
            return app.UseMiddleware<SwaggerMiddleware>(options);
        }
    }
}
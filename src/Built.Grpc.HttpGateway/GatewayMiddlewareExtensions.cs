using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Built.Grpc.HttpGateway
{
    public static class GatewayMiddlewareExtensions
    {
        public static IApplicationBuilder UseGrpcHttpGateway(this IApplicationBuilder app, Pipeline pipeline)
        {
            // 启用插件;
            return app.HttpGatewayInit().UseMiddleware<GatewayMiddleware>(pipeline);
            //return app.UseMiddleware<GatewayMiddleware>(pipeline);
        }

        public static IApplicationBuilder MonitorFileEnable(this IApplicationBuilder app)
        {
            DirectoryMonitor monitor = new DirectoryMonitor(GrpcServiceMethodFactory.PluginPath);
            monitor.Change += (string filePath) =>
            {
                GrpcServiceMethodFactory.LoadAsync(filePath);
            };
            monitor.Start();
            return app;
        }

        public static IApplicationBuilder HttpGatewayInit(this IApplicationBuilder app)
        {
            GrpcServiceMethodFactory.InitAsync().Wait();
            return app;
        }
    }
}
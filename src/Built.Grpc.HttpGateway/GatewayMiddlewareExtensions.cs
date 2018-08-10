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
        }

        public static IApplicationBuilder MonitorDllFileEnable(this IApplicationBuilder app)
        {
            DirectoryMonitor monitor = new DirectoryMonitor(GrpcServiceMethodFactory.PluginPath, "*.dll");
            monitor.Change += (string filePath) =>
            {
                GrpcServiceMethodFactory.LoadAsync(filePath);
            };
            monitor.Start();
            return app;
        }

        public static IApplicationBuilder MonitorProtoFileEnable(this IApplicationBuilder app)
        {
            DirectoryMonitor monitor = new DirectoryMonitor(GrpcServiceMethodFactory.PluginPath, "*.proto");
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
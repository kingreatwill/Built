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
            // 注册编码;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // 启用插件;
            return app.HttpGatewayInit().UseMiddleware<GatewayMiddleware>(pipeline);
        }

        public static IApplicationBuilder UseGrpcMonitorDllFileEnable(this IApplicationBuilder app)
        {
            DirectoryMonitor monitor = new DirectoryMonitor(GrpcServiceMethodFactory.PluginPath, "*.dll");
            monitor.Change += (string filePath) =>
            {
                InnerLogger.Log(LoggerLevel.Debug, filePath);
                GrpcServiceMethodFactory.DllQueue.Enqueue(filePath);
            };
            monitor.Start();
            return app;
        }

        public static IApplicationBuilder UseGrpcMonitorProtoFileEnable(this IApplicationBuilder app)
        {
            DirectoryMonitor monitor = new DirectoryMonitor(GrpcServiceMethodFactory.ProtoPath, "*.proto");
            monitor.Change += (string filePath) =>
            {
                InnerLogger.Log(LoggerLevel.Debug, filePath);
                GrpcServiceMethodFactory.ProtoQueue.Enqueue(filePath);
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
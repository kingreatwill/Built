using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Built.Grpc.HttpGateway
{
    public static class GrpcSrvPlugin
    {
        private static readonly string PluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GatewayClients");
        public static IList<GrpcServiceMethod> Handles = new List<GrpcServiceMethod>();
        public static ConcurrentDictionary<string, GrpcServiceMethod> Handers = new ConcurrentDictionary<string, GrpcServiceMethod>();

        public static void AddOrUpdate(GrpcServiceMethod method)
        {
            Handers.AddOrUpdate(method.GetHashString(), method);
        }

        public static IApplicationBuilder HttpGatewayEnable(this IApplicationBuilder app)
        {
            var watcher = new FileSystemWatcher
            {
                Path = PluginPath,
                NotifyFilter = NotifyFilters.Attributes |
                                   NotifyFilters.CreationTime |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.FileName |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Security |
                                   NotifyFilters.Size,
                Filter = "*.dll"
            };
            watcher.Changed += (object source, FileSystemEventArgs e) =>
            {
                Console.WriteLine("文件{0}已经被修改,修改类型{1}", e.FullPath, e.ChangeType.ToString());
            };
            watcher.Created += (object source, FileSystemEventArgs e) =>
            {
                Console.WriteLine("文件{0}被建立", e.FullPath);
            };
            watcher.Deleted += (object source, FileSystemEventArgs e) =>
            {
                Console.WriteLine("文件{0}已经被删除", e.FullPath);
            };
            watcher.Renamed += (object source, RenamedEventArgs e) =>
            {
                Console.WriteLine("文件{0}的名称已经从{1}变成了{2}", e.OldFullPath, e.OldName, e.Name);
            };
            // 为true表示开启FileSystemWatcher组件，反之我们的监控将不启作用
            watcher.EnableRaisingEvents = true;

            return app;
        }

        public static IApplicationBuilder HttpGatewayInit(this IApplicationBuilder app)
        {
            var clients = Directory.GetFiles(PluginPath, "*.dll");
            var baseClient = typeof(ClientBase);
            foreach (var clientPath in clients)
            {
                byte[] assemblyBuf = File.ReadAllBytes(clientPath);
                var assembly = Assembly.Load(assemblyBuf);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.Name.EndsWith("Base"))
                    {
                        if (type.ReflectedType == null) continue;
                        // 获取__ServiceName
                        FieldInfo f_key = type.ReflectedType.GetField("__ServiceName", BindingFlags.Static | BindingFlags.NonPublic);
                        if (f_key == null) continue;
                        var ServiceName = f_key.GetValue(type.ReflectedType);
                        var methods = GetGrpcMethods(ServiceName.ToString(), type);

                        foreach (var method in methods)
                        {
                            if (method.Method.Name == "Gets")
                            {
                                var str = Newtonsoft.Json.JsonConvert.SerializeObject(new
                                {
                                    PageIndex = 1,
                                    PageSize = 10,
                                });
                                var requestObject = Newtonsoft.Json.JsonConvert.DeserializeObject(str, method.RequestType);

                                object responseObject = new GatewayMiddleware().CallGrpcAsync(method, new Dictionary<string, string>(), requestObject).Result;
                            }
                        }
                    }
                }
            }
            return app;
        }

        public static IList<GrpcServiceMethod> GetGrpcMethods(string serviceName, Type serviceType)
        {
            return GetGrpcMethods(serviceName, serviceType, GrpcMarshallerFactory.DefaultInstance);
        }

        public static IList<GrpcServiceMethod> GetGrpcMethods(string serviceName, Type serviceType, IGrpcMarshallerFactory marshallerFactory)
        {
            List<GrpcServiceMethod> methods = new List<GrpcServiceMethod>();

            foreach (GrpcMethodHandlerInfo handler in GrpcReflection.EnumerateServiceMethods(serviceType))
            {
                IMethod method = GrpcReflection.CreateMethod(serviceName, handler, marshallerFactory);

                AddOrUpdate(new GrpcServiceMethod(method, handler.RequestType, handler.ResponseType));
                methods.Add(new GrpcServiceMethod(method, handler.RequestType, handler.ResponseType));
            }

            return methods;
        }
    }
}
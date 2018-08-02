using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using System;
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
                //Assembly.LoadFile Assembly.LoadFrom 不能释放文件句柄，不能实现热更新
                byte[] assemblyBuf = File.ReadAllBytes(clientPath);
                var assembly = Assembly.Load(assemblyBuf);
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.Name.EndsWith("Base"))
                    {
                        // 获取__ServiceName
                        FieldInfo f_key = type.ReflectedType.GetField("__ServiceName", BindingFlags.Static | BindingFlags.NonPublic);
                        var ServiceName = f_key.GetValue(type.ReflectedType);
                        var methods = GetGrpcMethods(ServiceName.ToString(), type);
                        // http header  转grpc header  grpc-timeout
                        /*
                         /// <summary>
                        ///
                        /// </summary>
                        /// <param name="httpHeaderKey"></param>
                        /// <param name="grpcHeaderKey"></param>
                        /// <returns></returns>
                        private static bool IsGrpcRequestHeader(string httpHeaderKey, out string grpcHeaderKey)
                        {
                            string prefix = "grpc.";

                            if (httpHeaderKey.Length > prefix.Length && httpHeaderKey.StartsWith(prefix))
                            {
                                grpcHeaderKey = httpHeaderKey.Substring(prefix.Length);
                                return true;
                            }

                            grpcHeaderKey = null;
                            return false;
                        }
                         */
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
                    if (type.IsSubclassOf(baseClient))
                    {
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (method.Name == "Gets")
                            {
                                ParameterInfo[] parameters = method.GetParameters();
                                if (parameters.Length == 4)
                                {
                                    Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
                                    var pipeline = new PipelineBuilder()
                                      .Use<PolicyMiddleware>(new PolicyMiddlewareOptions
                                      {
                                          RetryTimes = 2,
                                          TimoutMilliseconds = 100
                                      })
                                  ;
                                    //pipeline.Use<LoggingMiddleware>();// pipeline.UseWhen<LoggingMiddleware>(ctx => ctx.Context.Method.EndsWith("SayHello"));
                                    //pipeline.Use<TimeoutMiddleware>(new TimeoutMiddlewareOptions { TimoutMilliseconds = 1000 });
                                    //console logger
                                    pipeline.Use(async (ctx, next) =>
                                    {
                                        Console.WriteLine(ctx.Request.ToString());
                                        await next(ctx);
                                        Console.WriteLine(ctx.Response.ToString());
                                    });
                                    MiddlewareCallInvoker callInvoker = new MiddlewareCallInvoker(channel, pipeline.Build());

                                    object testClass = Activator.CreateInstance(type, callInvoker);
                                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(new
                                    {
                                        PageIndex = 1,
                                        PageSize = 10,
                                    });
                                    var objParams = Newtonsoft.Json.JsonConvert.DeserializeObject(str, parameters[0].ParameterType);
                                    var res = method.Invoke(testClass, new object[] { objParams, null, null, null });
                                }
                            }
                            // 异步调用;
                            if (method.Name == "GetsAsync")
                            {
                                ParameterInfo[] parameters = method.GetParameters();
                                if (parameters.Length == 4)
                                {
                                    Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
                                    object testClass = Activator.CreateInstance(type, channel);
                                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(new
                                    {
                                        PageIndex = 1,
                                        PageSize = 10,
                                    });
                                    var objParams = Newtonsoft.Json.JsonConvert.DeserializeObject(str, parameters[0].ParameterType);

                                    // Task<object> task = (Task<object>)method.MakeGenericMethod(new Type[] { parameters[0].ParameterType, method.ReturnType }).Invoke(testClass, new object[] { objParams, null, null, null });

                                    var res = method.Invoke(testClass, new object[] { objParams, null, null, null });

                                    // var r = res.ResponseAsync.Result;
                                }
                            }
                        }
                    }
                }
                //ClientBase
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

                methods.Add(new GrpcServiceMethod(method, handler.RequestType, handler.ResponseType));
            }

            return methods;
        }
    }
}
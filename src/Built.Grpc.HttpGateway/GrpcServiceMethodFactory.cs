using Grpc.Core;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Built.Grpc.HttpGateway
{
    public static class GrpcServiceMethodFactory
    {
        public static readonly string PluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GatewayClients");
        public static ConcurrentDictionary<string, GrpcServiceMethod> Handers = new ConcurrentDictionary<string, GrpcServiceMethod>();

        public static async Task ReLoadAsync()
        {
            //todo 为了热更新，不能初始化handers
            await InitAsync();
        }

        public static async Task InitAsync()
        {
            Handers = new ConcurrentDictionary<string, GrpcServiceMethod>();
            var dllFiles = Directory.GetFiles(PluginPath, "*.dll");
            foreach (var file in dllFiles)
            {
                await LoadAsync(file);
            }
        }

        public static Task LoadAsync(string fileFullPath)
        {
            return Task.Run(() =>
            {
                if (!File.Exists(fileFullPath))
                {
                    ReLoadAsync().Wait();
                    return;
                }
                byte[] assemblyBuf = File.ReadAllBytes(fileFullPath);
                var assembly = Assembly.Load(assemblyBuf);
                HandersAddOrUpdate(assembly);
            });
        }

        private static void HandersAddOrUpdate(Assembly assembly)
        {
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
                    GetGrpcMethods(ServiceName.ToString(), type);
                }
            }
        }

        public static void GetGrpcMethods(string serviceName, Type serviceType)
        {
            GetGrpcMethods(serviceName, serviceType, GrpcMarshallerFactory.DefaultInstance);
        }

        public static void GetGrpcMethods(string serviceName, Type serviceType, IGrpcMarshallerFactory marshallerFactory)
        {
            foreach (GrpcMethodHandlerInfo handler in GrpcReflection.EnumerateServiceMethods(serviceType))
            {
                IMethod method = GrpcReflection.CreateMethod(serviceName, handler, marshallerFactory);
                var srvMethod = new GrpcServiceMethod(method, handler.RequestType, handler.ResponseType);
                Handers.AddOrUpdate(srvMethod.GetHashString(), srvMethod);
                Console.WriteLine(srvMethod.GetHashString());
            }
        }
    }
}
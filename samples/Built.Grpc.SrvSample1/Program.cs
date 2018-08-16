using Built.Grpc.ContractsSample1.HelloApiDemo;
using Built.Grpc.ContractsSample1.HelloDemo;
using Built.Grpc.ContractsSample1.ProductBasic;
using Built.Grpc.Extensions;
using Built.Grpc.ImplSample1;
using Consul;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Core.Interceptors;
using System;

namespace Built.Grpc.SrvSample1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Port = 50051;
            Server server = new Server
            {
                Services = {
                    Get.BindService(new HelloworldApiImpl()),
                    BuiltHelloDemoSrv.BindService(new HelloworldImpl()).Intercept(
                            new ServerCallContextInterceptor(ctx =>
                                {
                                    Console.WriteLine(ctx);
                                }
                            )
                    ),
                    ProductBasicSrv.BindService(new ProductBasicImpl()),
                    ProductPriceSrv.BindService(new ProductPriceImpl())
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) },
            };
            var s = ProductBasicSrv.Descriptor.FullName;
            var dsfs = ProductBasicReflection.Descriptor;
            var ds = HelloworldApiReflection.Descriptor;
            server.StartAndRegisterConsul();

            Console.WriteLine("Greeter server listening on port " + Port);

            Console.ReadKey();
            var client = new ConsulClient(p =>
             {
                 p.Address = new Uri("http://127.0.0.1:8500");
             });
            var sd = client.Health.Service("HelloDemo.BuiltHelloDemoSrv", "", true).Result;
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();

            Console.ReadLine();
        }
    }
}
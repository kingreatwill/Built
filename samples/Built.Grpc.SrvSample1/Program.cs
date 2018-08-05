using Built.Grpc.ContractsSample1.HelloDemo;
using Built.Grpc.ContractsSample1.ProductBasic;
using Built.Grpc.Extensions;
using Built.Grpc.ImplSample1;
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
           
            server.StartAndRegisterConsul();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();

            Console.ReadLine();
        }
    }
}
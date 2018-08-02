using Built.Grpc.ContractsSample1.HelloDemo;
using Built.Grpc.ContractsSample1.ProductBasic;
using Built.Grpc.ImplSample1;
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
                                    Console.WriteLine(ctx.Host);
                                }
                            )
                    ),
                    ProductBasicSrv.BindService(new ProductBasicImpl())
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("Greeter server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();

            Console.ReadLine();
        }
    }
}
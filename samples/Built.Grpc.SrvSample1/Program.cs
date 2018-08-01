using Built.Grpc.ContractsSample1.HelloDemo;
using Built.Grpc.ImplSample1;
using Grpc.Core;
using System;

namespace Built.Grpc.SrvSample1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server
            {
                Services = { BuiltHelloDemoSrv.BindService(new HelloworldImpl()) },
                Ports = { new ServerPort("", 0, ServerCredentials.Insecure) }
            };

            server.Start();

            server.ShutdownAsync().Wait();

            Console.ReadLine();
        }
    }
}
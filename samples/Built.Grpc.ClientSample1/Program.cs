using Built.Grpc.ContractsSample1.HelloDemo;
using Grpc.Core;
using System;

namespace Built.Grpc.ClientSample1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new BuiltHelloDemoSrv.BuiltHelloDemoSrvClient(channel);
            String user = "you";

            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    //// var client = new ClientFactory().Get<Greeter.GreeterClient>();
    //public class ClientFactory
    //{
    //    private readonly ClientCallInvoker _callInvoker;

    //    public ClientFactory(IEndpointStrategy strategy)
    //    {
    //        _callInvoker = new ClientCallInvoker(strategy, 1);
    //    }

    //    public T Get<T>()
    //    {
    //        var client = (T)Activator.CreateInstance(typeof(T), _callInvoker);
    //        return client;
    //    }
    //}
}
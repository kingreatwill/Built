using Built.Grpc.ContractsSample1.HelloDemo;
using Built.Grpc.ContractsSample1.ProductBasic;
using Grpc.Core;
using System;

namespace Built.Grpc.ClientSample1
{
    internal class Program
    {
        private static void Main(string[] args)//async
        {
            var pipeline = new PipelineBuilder()
              //.Use<ExceptionMiddleware>()
              //.Use<TimerMiddleware>()
              //.Use<LoggingMiddleware>()
              //.Use<TimeoutMiddleware>()
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

            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            MiddlewareCallInvoker callInvoker = new MiddlewareCallInvoker(channel, pipeline.Build());

            var client2 = new ProductBasicSrv.ProductBasicSrvClient(callInvoker);
            var request = new ProductBasicGetsRequest
            {
                PageIndex = 1,
                PageSize = 10,
            };
            request.Items.Add(1);
            request.PType = ProductBasicGetsRequest.Types.PhoneType.Work;
            var result2 = client2.Gets(request);
            //var sd = await client2.GetsAsync(request);
            foreach (var r in result2.Result)
            {
                Console.WriteLine($"{r.ProductId}|---------|{r.ProductName}");
            }

            var client = new BuiltHelloDemoSrv.BuiltHelloDemoSrvClient(callInvoker);
            String user = "you";

            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void Main2(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new BuiltHelloDemoSrv.BuiltHelloDemoSrvClient(channel);
            String user = "you";

            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine("Greeting: " + reply.Message);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            //MiddlewareCallInvoker
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
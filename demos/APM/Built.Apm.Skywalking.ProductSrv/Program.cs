using Grpc.Core;
using Grpc.Core.Interceptors;
using Product;
using SkyWalking.Diagnostics.Grpc;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using User;
using static Product.ProductService;

namespace Built.Apm.Skywalking.ProductSrv
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Port = 50052;
            Server server = new Server
            {
                Services = {
                    ProductService.BindService(new ProductServiceImpl()).Intercept( new ServerCallContextInterceptor()),
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) },
            };
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }

    public class ProductServiceImpl : ProductServiceBase
    {
        public static ConcurrentDictionary<long, ProductGetResponse> store = new ConcurrentDictionary<long, ProductGetResponse>();

        public ProductServiceImpl()
        {
        }

        public override Task<ProductGetResponse> Get(ProductGetRequest request, ServerCallContext context)
        {
            if (store.ContainsKey(request.ProductId))
            {
                return Task.FromResult(store[request.ProductId]);
            }
            return Task.FromResult(default(ProductGetResponse));
        }

        public override Task<ProductGetResponse> Create(ProductCreateRequest request, ServerCallContext context)
        {
            if (store.ContainsKey(request.ProductId))
            {
                return Task.FromResult(default(ProductGetResponse));
            }

            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            ClientCallInvoker callInvoker = new ClientCallInvoker(channel);
            var userClient = new UserService.UserServiceClient(callInvoker);

            var product = store[request.UserId];
            product.ProductId = request.ProductId;
            product.ProductName = request.ProductName;
            product.User = userClient.Get(new UserGetRequest
            {
                UserId = request.UserId
            });
            userClient.Update(new UserGetResponse
            {
                UserId = product.User.UserId,
                UserName = product.User.UserName,
                ProductCount = product.User.ProductCount + 1
            });
            //关闭
            channel.ShutdownAsync().Wait();
            return Task.FromResult(product);
        }
    }
}
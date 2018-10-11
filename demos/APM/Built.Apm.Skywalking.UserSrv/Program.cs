using Grpc.Core;
using Grpc.Core.Interceptors;
using SkyWalking.Diagnostics.Grpc;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using User;
using static User.UserService;

namespace Built.Apm.Skywalking.UserSrv
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var Port = 50051;
            Server server = new Server
            {
                Services = {
                    UserService.BindService(new UserServiceImpl()).Intercept( new ServerCallContextInterceptor()),
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) },
            };
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
            server.ShutdownAsync().Wait();
            Console.ReadLine();
        }
    }

    public class UserServiceImpl : UserServiceBase
    {
        public static ConcurrentDictionary<long, UserGetResponse> store = new ConcurrentDictionary<long, UserGetResponse>();

        public UserServiceImpl()
        {
            store.TryAdd(1, new UserGetResponse
            {
                ProductCount = 0,
                UserId = 1,
                UserName = "u01"
            });
            store.TryAdd(2, new UserGetResponse
            {
                ProductCount = 0,
                UserId = 2,
                UserName = "u02"
            });
        }

        public override Task<UserGetResponse> Get(UserGetRequest request, ServerCallContext context)
        {
            if (store.ContainsKey(request.UserId))
            {
                return Task.FromResult(store[request.UserId]);
            }
            return Task.FromResult(default(UserGetResponse));
        }

        public override Task<UserGetResponse> Update(UserGetResponse request, ServerCallContext context)
        {
            if (store.ContainsKey(request.UserId))
            {
                store[request.UserId] = request;
                return Task.FromResult(store[request.UserId]);
            }
            return Task.FromResult(default(UserGetResponse));
        }
    }
}
using Built.Grpc.ContractsSample1.HelloDemo;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Built.Grpc.ImplSample1
{
    public class HelloworldImpl : BuiltHelloDemoSrv.BuiltHelloDemoSrvBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }
    }
}
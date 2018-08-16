using Built.Grpc.ContractsSample1.HelloApiDemo;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Built.Grpc.ImplSample1
{
    public class HelloworldApiImpl : Get.GetBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello Api " + request.Name });
        }
    }
}
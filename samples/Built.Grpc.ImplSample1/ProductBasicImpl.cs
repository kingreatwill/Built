using Built.Grpc.ContractsSample1.HelloDemo;
using Built.Grpc.ContractsSample1.ProductBasic;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Built.Grpc.ImplSample1
{
    public class ProductBasicImpl : ProductBasicSrv.ProductBasicSrvBase
    {
        public override Task<ProductBasicGetsResponse> Gets(ProductBasicGetsRequest request, ServerCallContext context)
        {
            ProductBasicGetsResponse response = new ProductBasicGetsResponse();
            for (var i = 0; i < request.PageSize; i++)
            {
                response.Result.Add(new ProductBasicGetsResult
                {
                    ProductId = (request.PageIndex - 1) * request.PageSize + i + 1,
                    ProductName = $"{ request.PageIndex }_{i}_{request.Items}_{request.PType}"
                });
            }
            return Task.FromResult(response);
        }
    }
}
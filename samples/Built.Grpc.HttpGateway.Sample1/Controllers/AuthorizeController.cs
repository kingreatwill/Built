using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Built.Grpc.HttpGateway.Sample1.Controllers
{
    /// <summary>
    /// 授权
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private IConfiguration _configuration;

        /// <summary>
        /// AuthorizeController
        /// </summary>
        /// <param name="configuration"></param>
        public AuthorizeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// GET api/Authorize
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return GrpcServiceMethodFactory.Handers.Select(t => t.Key).ToArray();
        }

        /// <summary>
        /// GET api/Authorize/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<KeyValuePair<string, string>>> Get(int id)
        {
            return _configuration.AsEnumerable().ToArray();
        }
    }
}
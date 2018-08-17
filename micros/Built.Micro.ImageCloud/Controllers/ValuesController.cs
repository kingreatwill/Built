using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Built.Micro.ImageCloud.Controllers
{
    /// <summary>
    /// ValuesController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var constr = _configuration.GetConnectionString("ImageCloud");
            MaterialRepository repo = new MaterialRepository(constr);
            //Insert
            Material item = new Material()
            {
                Username = "username",
                Password = "password"
            };
            repo.Insert(item);
            //repo.Collection.Database
            //bucket = new GridFSBucket(_database); //这个是初始化gridFs存储的
            //GridFSFileInfo
            //var id = bucket.UploadFromBytes("filename", null); //source字节数组
            //var id = await bucket.UploadFromBytesAsync("filename", source);
            return new string[] { item.Id };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<KeyValuePair<string, string>>> Get(int id)
        {
            return _configuration.AsEnumerable().ToArray();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
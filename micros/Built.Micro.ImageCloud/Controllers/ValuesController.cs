using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Built.Micro.ImageCloud.Domain.Services;
using Built.Micro.ImageCloud.Mongo;
using Microsoft.AspNetCore.Http;
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
        private readonly IMaterialService _materialService;

        public ValuesController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _materialService.Repository.Insert(new Material { Name = "呵呵" });
            //repo.Collection.Database
            //bucket = new GridFSBucket(_database); //这个是初始化gridFs存储的
            //GridFSFileInfo
            //var id = bucket.UploadFromBytes("filename", null); //source字节数组
            //var id = await bucket.UploadFromBytesAsync("filename", source);

            return new string[] { "" };
        }

        // POST api/values
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Post(IFormCollection files)
        {
            foreach (var formFile in files.Files)
            {
                if (formFile.Length > 0)
                {
                    string fileExt = Path.GetExtension(formFile.FileName); //文件扩展名，不含“.”
                    long fileSize = formFile.Length; //获得文件大小，以字节为单位

                    using (var stream = new MemoryStream())
                    {
                        await formFile.CopyToAsync(stream);
                        await stream.FlushAsync();
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        var id = await _materialService.UploadAsync(formFile.FileName, bytes);
                    }
                }
            }
            return new JsonResult(0);
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
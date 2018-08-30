using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Built.Micro.ImageCloud.Domain.Services;
using Built.Mongo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

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
        private readonly IUnitOfWork _unitOfWork;

        public ValuesController(IMaterialService materialService, IUnitOfWork unitOfWork)

        {
            _materialService = materialService;
            _unitOfWork = unitOfWork;
        }

        // GET /api/values/unit
        [Route("unit")]
        [HttpGet]
        public async Task<ActionResult> Unit()
        {
            var rpo = _unitOfWork.GetRepository<Material>();
            var material = new Material
            {
                Author = "Enter" + DateTime.Now,
                Version = 1
            };
            var sd = rpo.FindByPaged(0, 2);
            await rpo.InsertAsync(material);
            //_unitOfWork.AbortTransaction();
            // _unitOfWork.CommitTransaction();

            _unitOfWork.StartTransaction();
            //rpo = _unitOfWork.GetRepository<Material>();
            await rpo.InsertAsync(new Material
            {
                Author = "Enter" + DateTime.Now,
                Version = 2
            });
            // _unitOfWork.AbortTransaction();
            return new JsonResult(material);
        }

        // GET /api/values/add
        [Route("add")]
        [HttpGet]
        public ActionResult Add()
        {
            var material = new Material
            {
                Author = "Enter",
                Content = "描述",
                Version = 1
            };
            _materialService.Repository.Insert(material);
            return new JsonResult(material);
        }

        // GET /api/values/5b77ce8e6e527040a04c9471
        [HttpGet("{id}")]
        public async Task<ActionResult> GetByMaterialId(string id)
        {
            var material = _materialService.Repository.Get(id);
            return File(await _materialService.Repository.Bucket.OpenDownloadStreamAsync(new ObjectId(material.FileId)), material.ContentType);
        }

        // POST api/values
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Post(IFormCollection files)
        {
            var result = new List<ObjectId>();
            foreach (var formFile in files.Files)
            {
                if (formFile.Length > 0)
                {
                    // 计算MD5;
                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    var hash = md5.ComputeHash(formFile.OpenReadStream());
                    StringBuilder md5sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        md5sb.Append(hash[i].ToString("x2"));
                    }
                    var md5Str = md5sb.ToString();
                    // 处理重复文件;
                    ObjectId fileId;
                    var fileList = _materialService.Repository.Bucket.Find(new { md5 = md5Str }.ToBsonDocument(), new GridFSFindOptions
                    {
                        Limit = 1
                    }).ToList();
                    if (fileList.Any())
                    {
                        var fInfo = fileList.First();
                        fileId = fInfo.Id;
                    }
                    else
                    {
                        fileId = await _materialService.Repository.Bucket.UploadFromStreamAsync(formFile.FileName, formFile.OpenReadStream(), new GridFSUploadOptions
                        {
                            Metadata = new BsonDocument().AddRange(formFile.Headers?.Select(t => new BsonElement(t.Key, t.Value.ToString())))
                        });
                    }

                    var suffix = Path.GetExtension(formFile.FileName).ToLower();
                    var material = new Material
                    {
                        Author = "Enter",
                        Content = "描述",
                        ContentType = formFile.ContentType,
                        FileId = fileId.ToString(),
                        FileName = formFile.FileName,
                        FileSize = formFile.Length,
                        MD5 = md5Str,
                        Name = string.IsNullOrWhiteSpace(formFile.Name) ? Path.GetFileNameWithoutExtension(formFile.FileName) : formFile.Name,
                        Suffix = suffix,
                        Type = MaterialExtension.GetMaterialTypeBySuffix(suffix),
                        Version = 1
                    };
                    await _materialService.Repository.InsertAsync(material);
                    result.Add(fileId);
                }
            }
            return new JsonResult(result);
        }
    }
}
using Built.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Built.Micro.ImageCloud.Domain.Services
{
    public interface IMaterialService : IService<Material>
    {
        Task<ObjectId> UploadAsync(string filename, byte[] bytes);

        Task<byte[]> DownloadAsync(string fileId);
    }

    public class MaterialService : IMaterialService
    {
        public IRepository<Material> Repository { get; }

        public MaterialService(IRepository<Material> materialRepository)
        {
            Repository = materialRepository;

            // 创建索引;
            //Repository.Collection.Indexes.CreateOne(
            //       new CreateIndexModel<Material>(
            //           Builders<Material>.IndexKeys.Ascending(x => x.MD5),
            //           new CreateIndexOptions
            //           {
            //               Unique = true
            //           }
            //       )
            //   );
        }

        public async Task<ObjectId> UploadAsync(string filename, byte[] bytes)
        {
            var bucket = new GridFSBucket(Repository.Collection.Database); //这个是初始化gridFs存储的

            // 计算MD5;
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("x2"));
            }
            var md5Str = result.ToString();
            // 处理重复文件;
            ObjectId fileId;
            var fileList = bucket.Find(new { md5 = md5Str }.ToBsonDocument(), new GridFSFindOptions
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
                fileId = await bucket.UploadFromBytesAsync(filename, bytes, new GridFSUploadOptions
                {
                    DisableMD5 = false
                });
            }
            // 插入素材记录;
            var entity = new Material
            {
                FileId = fileId.ToString(),
                Name = filename,
                MD5 = md5Str
            };
            Repository.Insert(entity);
            return entity.ObjectId;
        }

        public async Task<byte[]> DownloadAsync(string fileId)
        {
            var bucket = new GridFSBucket(Repository.Collection.Database);
            return await bucket.DownloadAsBytesAsync(new ObjectId(fileId));
        }
    };
}
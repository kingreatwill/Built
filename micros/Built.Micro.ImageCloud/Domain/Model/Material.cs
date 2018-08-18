using Built.Micro.ImageCloud.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Built.Micro.ImageCloud
{
    /*
     voice_count	语音总数量
video_count	视频总数量
image_count	图片总数量
news_count	图文总数量
media_id
title	是	标题
thumb_media_id	是	图文消息的封面图片素材id（必须是永久mediaID）
author	是	作者
digest	是	图文消息的摘要，仅有单图文消息才有摘要，多图文此处为空
show_cover_pic	是	是否显示封面，0为false，即不显示，1为true，即显示
content	是	图文消息的具体内容，支持HTML标签，必须少于2万字符，小于1M，且此处会去除JS
content_source_url	是	图文消息的原文地址，即点击“阅读原文”后的URL
         */

    /// <summary>
    /// 素材
    /// </summary>
    [ConnectionName("ImageCloud")]
    public class Material : Entity
    {
        [BsonElement("_fid", Order = 3)]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId { get; set; }

        /// <summary>
        /// 素材名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 素材文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Content-Type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 素材文件后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 素材类型
        /// </summary>
        public MaterialType Type { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件创建时间
        /// </summary>
        public DateTime FileCreated { get; set; }

        /// <summary>
        /// 文件修改时间
        /// </summary>
        public DateTime FileUpdated { get; set; }

        /// <summary>
        /// 文件版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// MD5
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// SHA1
        /// </summary>
        public string SHA1 { get; set; }

        /// <summary>
        /// CRC32
        /// </summary>
        public string CRC32 { get; set; }
    }

    public enum MaterialType
    {
        Image = 1,

        Audio = 3,

        Video = 5,

        Doc = 7,

        Execl = 9,

        Pdf = 11,

        Text = 13,

        Markdown = 15,

        Html = 17,

        CompressFiles = 19,

        Other = 999,
    }
}
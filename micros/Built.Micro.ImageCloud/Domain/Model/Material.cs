using Built.Micro.ImageCloud.Mongo;
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

    //if you are able to define your model
    public class Material : Entity
    {
        public string Name { get; set; }
        public string FileName { get; set; }

        public MaterialType Type { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }
    }

    public enum MaterialType
    {
        Image = 1,
        Video = 3,
        Doc = 5,
    }
}
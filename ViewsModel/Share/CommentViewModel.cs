using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Share
{
    /// <summary>
    /// 评论保存 ViewModel
    /// </summary>
    public class CommentPostViewModel
    {
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文章Code
        /// </summary>
        public string ArticleCode { get; set; }

        /// <summary>
        /// 根评论Code
        /// </summary>
        public string CommentCode { get; set; }

        /// <summary>
        /// 被评论人Code
        /// </summary>
        public string UserCodeTo { get; set; }

        /// <summary>
        /// 被评论人名称
        /// </summary>
        public string UserNameTo { get; set; }
    }

    /// <summary>
    /// 评论/回复 ViewModel
    /// </summary>
    public class CommentViewModel : CommentBaseViewModel
    {
        /// <summary>
        /// 回复列表
        /// </summary>
        public List<CommentBaseViewModel> SubComment { set; get; }
    }

    /// <summary>
    /// 评论/回复Base ViewModel
    /// </summary>
    public class CommentBaseViewModel
    {
        /// <summary>
        /// 评论/回复Code
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 评论/回复内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 发布人Code
        /// </summary>
        public string UserCodeFrom { set; get; }
        /// <summary>
        /// 发布人Name
        /// </summary>
        public string UserNameFrom { set; get; }
        /// <summary>
        /// 被回复人Code
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserCodeTo { set; get; }
        /// <summary>
        /// 被回复人Name
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserNameTo { set; get; }
        /// <summary>
        /// 评论/回复时间
        /// </summary>
        public string CreateTime { set; get; }
    }
}
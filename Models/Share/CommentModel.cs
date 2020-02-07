using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Share
{
    /// <summary>
    /// 评论
    /// </summary>
    [ORTableMapping("office.S_Comment")]
    public class CommentModel : BaseModel
    {
        /// <summary>
        /// 文章编码
        /// </summary>
        [ORFieldMapping("ArticleCode")]
        public string ArticleCode { get; set; }

        /// <summary>
        /// 评论编码
        /// </summary>
        [ORFieldMapping("CommentCode")]
        public string CommentCode { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }
        
        /// <summary>
        /// 评论人编码
        /// </summary>
        [ORFieldMapping("UserCodeFrom")]
        public string UserCodeFrom { get; set; }

        /// <summary>
        /// 评论人名称
        /// </summary>
        [ORFieldMapping("UserNameFrom")]
        public string UserNameFrom { get; set; }

        /// <summary>
        /// 被评论人编码
        /// </summary>
        [ORFieldMapping("UserCodeTo")]
        public string UserCodeTo { get; set; }

        /// <summary>
        /// 被评论人名称
        /// </summary>
        [ORFieldMapping("UserNameTo")]
        public string UserNameTo { get; set; }
    }

    /// <summary>
    /// 评论集合
    /// </summary>
    public class CommentCollection : EditableDataObjectCollectionBase<CommentModel>
    {

    }
}
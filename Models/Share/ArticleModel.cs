using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Seagull2.YuanXin.AppApi.Models.Share
{
    /// <summary>
    /// 文章实体
    /// </summary>
    [ORTableMapping("office.S_Article")]
    public class ArticleModel : BaseModel
    {
        /// <summary>
        /// 图文Code
        /// </summary>
        [ORFieldMapping("GroupCode")]
        public string GroupCode { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        [ORFieldMapping("Author")]
        public string Author { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 原文链接
        /// </summary>
        [ORFieldMapping("Link")]
        public string Link { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        [ORFieldMapping("Views")]
        public int Views { get; set; }

        /// <summary>
        /// 封面图
        /// </summary>
        [ORFieldMapping("Cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [ORFieldMapping("Summary")]
        public string Summary { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否为文档
        /// </summary>
        [ORFieldMapping("IsFile")]
        public bool IsFile { set; get; }

        /// <summary>
        /// 文档路径
        /// </summary>
        [ORFieldMapping("FileUrl")]
        public string FileUrl { set; get; }
    }

    /// <summary>
    /// 文章集合
    /// </summary>
    public class ArticleCollection : EditableDataObjectCollectionBase<ArticleModel>
    {

    }
}
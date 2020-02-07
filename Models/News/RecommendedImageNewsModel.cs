using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 推荐图片新闻实体
    /// </summary>
    public class RecommendedImageNewsModel
    {
        /// <summary>
        /// 新闻链接地址
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string ShortTitle { get; set; }
        /// <summary>
        /// 图片顺序
        /// </summary>
        public int PicOrder { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string TopPicAddress { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [NoMapping]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否最新
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// 跳转url
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 图片src
        /// </summary>
        public String ImgSrc { get; set; }

        public String Id { get; set; }
        public String ListId { get; set; }
        public String WebId { get; set; }
    }

    [Serializable]
    public class RecommendedImageNewsModelCollection : EditableDataObjectCollectionBase<RecommendedImageNewsModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 新闻实体
    /// </summary>
    public class NewsModel
    {
        /// <summary>
        /// 新闻链接地址
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 新闻GUID
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebName { get; set; }
        /// <summary>
        /// 网站短名称
        /// </summary>
        public string WebShortName { get; set; }
        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebLink { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [NoMapping]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建时间字符串
        /// </summary>
        [NoMapping]
        public string CreateTimeString { get; set; }
        /// <summary>
        /// 是否最新
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// 跳转url
        /// </summary>
        public String Url { get; set; }
        /// <summary>
        /// 新闻内容
        /// </summary>
        public String PublishingPageContent { get; set; }
        public String Author { get; set; }
        public String Id { get; set; }
        public String ListId { get; set; }
        public String WebId { get; set; }
        public String NewsType { get; set; }
        public String VideoAddress { get; set; }
        public String NewsCategory { get; set; }
        /// <summary>
        /// 文章发布时间
        /// </summary>
        public DateTime ArticleStartDate { get; set; }
        //点击数量
        public int ReadCount { get; set; }
        public string ArticleStartDateString { get; set; }
        public string Publisher { get; set; }
        public string VideoPicAddress { get; set; }
        public string IsTop { get; set; }
        public string IsVideo { get; set; }
    }

    /// <summary>
    /// 新闻首次加载 （数）实体
    /// </summary>
    public class NewsLoadModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [NoMapping]
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [NoMapping]
        public string Type { get; set; }
        /// <summary>
        /// 未读总数
        /// </summary>
        [NoMapping]
        public int NoReadCount { get; set; }
        /// <summary>
        /// 未读的所有Code集合
        /// </summary>
        [NoMapping]
        public List<string> NoReadCodeList { get; set; }
    }
    [Serializable]
    public class NewsModelCollection : EditableDataObjectCollectionBase<NewsModel>
    {

    }
}
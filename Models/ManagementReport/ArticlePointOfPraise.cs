using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.ManagementReport
{
    /// <summary>
    /// 
    /// </summary>
    [ORTableMapping("Table_ArticlePointOfPraise")]
    public class ArticlePointOfPraise
    {
        /// <summary>
        /// ID
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public int ID { get; set; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        [ORFieldMapping("PointPraise", PrimaryKey = true)]
        public string PointPraise { get; set; }
        /// <summary>
        /// 点赞的链接
        /// </summary>
        [ORFieldMapping("ArticleUrl", PrimaryKey = true)]
        public string ArticleUrl { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ArticlePointOfPraiseCollection : EditableDataObjectCollectionBase<ArticlePointOfPraise>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 文章点赞Model实体
    /// </summary>
    [ORTableMapping("dbo.Table_ArticlePointOfPraise")]
    public class ArticlePointModel
    {
        /// <summary>
        /// 标识种子，标识增量1
        /// </summary>
        [ORFieldMapping("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 点赞量
        /// </summary>
        [ORFieldMapping("PointOfPraise")]
        public int PointOfPraise { get; set; }

        /// <summary>
        /// 文章URL
        /// </summary>
        [ORFieldMapping("ArticleUrl")]
        public string ArticleUrl { get; set; }
    }
    /// <summary>
    /// 文章点赞集合
    /// </summary>
    public class ArticlePointCollection : EditableDataObjectCollectionBase<ArticlePointModel>
    {

    }
}
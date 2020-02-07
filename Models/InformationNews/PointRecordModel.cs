using System;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 点赞记录Model实体
    /// </summary>
    [ORTableMapping("dbo.Table_PointPraiseRecord")]
    public class PointPraiseRecordModel
    {
        /// <summary>
        /// 标识种子，标识增量1
        /// </summary>
        [ORFieldMapping("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// 点赞类型（1：文章点赞；0：评论点赞）
        /// </summary>
        [ORFieldMapping("PointPraiseType")]
        public int PointPraiseType { get; set; }

        /// <summary>
        /// 点赞时间
        /// </summary>
        [ORFieldMapping("PointPraiseTime")]
        public DateTime PointPraiseTime { get; set; }

        /// <summary>
        /// 文章ID
        /// </summary>
        [ORFieldMapping("RelationID")]
        public int RelationID { get; set; }

    }
    /// <summary>
    /// 点赞记录集合
    /// </summary>
    public class PointPraiseRecordCollection : EditableDataObjectCollectionBase<PointPraiseRecordModel>
    {

    }
}
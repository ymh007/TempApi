using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.ManagementReport
{
    /// <summary>
    /// 点赞记录实体
    /// </summary>
    [ORTableMapping("dbo.Table_PointPraiseRecord")]
    public class PointPraiseRecordModel
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public int ID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }
        /// <summary>
        /// 点赞类型:
        /// 1：对文章点赞
        /// 0：对评论点赞
        /// </summary>
        [ORFieldMapping("PointPraiseType")]
        public string PointPraiseType { get; set; }
        /// <summary>
        /// 点赞时间
        /// </summary>
        [ORFieldMapping("PointPraiseTime")]
        public DateTime PointPraiseTime { get; set; }
        /// <summary>
        /// 管理报告ID或评论ID
        /// </summary>
        [ORFieldMapping("RelationID")]
        public int RelationID { get; set; }
    }

    /// <summary>
    /// 点赞记录列表
    /// </summary>
    public class PointPraiseRecordCollection : EditableDataObjectCollectionBase<PointPraiseRecordModel>
    {

    }
}
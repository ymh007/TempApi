using System;
using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;


namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 评论Model实体
    /// </summary>
    [ORTableMapping("dbo.Table_Comment")]
    public class CommentModel
    {
        /// <summary>
        /// 标识种子，标识增量1
        /// </summary>
        [ORFieldMapping("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 评论类型（1：文章评论、0：回复）
        /// </summary>
        [ORFieldMapping("CommentType")]
        public int CommentType { get; set; }

        /// <summary>
        /// CommentType为1时：文章编号、CommentType为0时：回复编号
        /// </summary>
        [ORFieldMapping("RelationID")]
        public int RelationID { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [ORFieldMapping("CommentContent")]
        public string CommentContent { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        [ORFieldMapping("CommentTime")]
        public DateTime CommentTime { set; get; }
        
        /// <summary>
        /// 评论人
        /// </summary>
        [ORFieldMapping("CommentPeople")]
        public string CommentPeople { get; set; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        [ORFieldMapping("PointOfPraise")]
        public int PointOfPraise { get; set; }

        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public int TotalPointPraise { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [ORFieldMapping("OperationStatue")]
        public short OperationStatue { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [ORFieldMapping("OperationTime")]
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 评论人ID
        /// </summary>
        [ORFieldMapping("CommentPeopleID")]
        public string CommentPeopleID { get; set; }
    }

    /// <summary>
    /// 评论集合
    /// </summary>
    public class CommentCollection : EditableDataObjectCollectionBase<CommentModel>
    {

    }
}
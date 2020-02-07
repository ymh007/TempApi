using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.ManagementReport
{
    /// <summary>
    /// 评论Model
    /// </summary>
    [ORTableMapping("dbo.Table_Comment")]
    public class CommentModel
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true, IsIdentity = true)]
        public int ID { set; get; }

        /// <summary>
        /// 评论类型 1：文章评论、0：回复评论
        /// </summary>
        [ORFieldMapping("CommentType")]
        public int CommentType { set; get; }

        /// <summary>
        /// 关系编号 如果CommentType=1：文章编号；如果CommentType=0：评论编号
        /// </summary>
        [ORFieldMapping("RelationID")]
        public int RelationID { set; get; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [ORFieldMapping("CommentContent")]
        public string CommentContent { set; get; }

        /// <summary>
        /// 评论时间
        /// </summary>
        [ORFieldMapping("CommentTime")]
        public DateTime CommentTime { set; get; }

        /// <summary>
        /// 评论人名称（如：v-姓名）
        /// </summary>
        [ORFieldMapping("CommentPeople")]
        public string CommentPeople { set; get; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        [ORFieldMapping("PointOfPraise")]
        public int PointOfPraise { set; get; }

        /// <summary>
        /// 操作状态
        /// </summary>
        [ORFieldMapping("OperationStatue")]
        public int OperationStatue { set; get; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [ORFieldMapping("OperationTime")]
        public DateTime OperationTime { set; get; }

        /// <summary>
        /// 评论人帐号（如：v-sun）
        /// </summary>
        [ORFieldMapping("CommentPeopleID")]
        public string CommentPeopleID { set; get; }
    }

    /// <summary>
    /// 评论Collection
    /// </summary>
    public class CommentCollection : EditableDataObjectCollectionBase<CommentModel>
    {

    }
}
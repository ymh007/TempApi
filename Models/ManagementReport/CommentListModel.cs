using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.ManagementReport
{
    /// <summary>
    /// 评论列表实体
    /// </summary>
    public class CommentListModel
    {
        /// <summary>
        /// 评论编号
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime Time { set; get; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int PraiseCount { set; get; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public int MyPraiseCount { set; get; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { set; get; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { set; get; }
    }

    /// <summary>
    /// 评论列表集合
    /// </summary>
    public class CommentListCollection : EditableDataObjectCollectionBase<CommentListModel>
    {

    }
}
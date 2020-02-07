using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Seagull2.YuanXin.AppApi.Controllers.EmployeeController;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ManagementReport
{
    /// <summary>
    /// 管理报告评论
    /// </summary>
    public class ManagementReportCommentViewModel
    {
        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 评论报告ID编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string Userid { get; set; }
        /// <summary>
        /// 评论人姓名
        /// </summary>
        public string CommentPeople { get; set; }
        /// <summary>
        /// 评论时间
        /// </summary>
        public string CommentTime { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string CommentContent { get; set; }
        /// <summary>
        /// 评论点赞数量
        /// </summary>
        public int PointOfPraise { get; set; }
        
        /// <summary>
        /// 点赞状态
        /// </summary>
        public bool PointState { get; set; }
    }

    /// <summary>
    /// 管理报告评论集合
    /// </summary>
    [Serializable]
    public class ManagementReportCommentViewModelCollection : EditableDataObjectCollectionBase<ManagementReportCommentViewModel>
    {

    }
}
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using Seagull2.YuanXin.AppApi.ZhongChouData.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class ProjectView
    {
        /// <summary>
        /// 项目信息
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public ContactsModel UserInfo { get; set; }

        /// <summary>
        /// 用户评价信息集合
        /// </summary>
        public UserEvaluationCollection EvaluationCollection { get; set; }


        /// <summary>
        /// 用户评论（留言）
        /// </summary>
        public CommentViewDataCollection UserCommentCollection { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public List<string> ProjectTagNames { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsFocus { get; set; }
    }
}
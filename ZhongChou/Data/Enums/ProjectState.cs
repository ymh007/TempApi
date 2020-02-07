using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 项目状态
    /// </summary>
    public enum ProjectState
    {
        /// <summary>
        /// 草稿箱
        /// </summary>
        [EnumItemDescription("草稿箱", Category = "1,2,6,7")]
        Draft = 0,
        /// <summary>
        /// 审核中
        /// </summary>
        [EnumItemDescription("审核中", Category = "1,2,6,7")]
        Auditing = 1,
        /// <summary>
        /// 审核未通过
        /// </summary>
        [EnumItemDescription("审核未通过", Category = "1,2,6,7")]
        AuditFailed = 2,
        /// <summary>
        /// 即将开始
        /// </summary>
        [EnumItemDescription("即将开始", Category = "1,2,6,7")]
        Soon = 3,
        /// <summary>
        /// 众筹中
        /// </summary>
        [EnumItemDescription("众筹中", Category = "1,2")]
        Progressing = 4,
        /// <summary>
        /// 众筹成功
        /// </summary>
        [EnumItemDescription("众筹成功" ,Category = "1,2")]
        Successed = 5,
        /// <summary>
        /// 众筹失败
        /// </summary>
        [EnumItemDescription("众筹失败", Category = "1,2")]
        Failed = 6,


        /// <summary>
        /// 报名中
        /// </summary>
        [EnumItemDescription("报名中", Category = "6")]
        Enrolling = 4,
        /// <summary>
        /// 报名结束
        /// </summary>
        [EnumItemDescription("报名结束", Category = "6")]
        EnrollEnd = 5,
        /// <summary>
        /// 投票中
        /// </summary>
        [EnumItemDescription("投票中", Category = "6")]
        Polling = 6,
        /// <summary>
        /// 评选中
        /// </summary>
        [EnumItemDescription("评选中", Category = "6")]
        Selection = 7,
        /// <summary>
        /// 活动结束
        /// </summary>
        [EnumItemDescription("已结束", Category = "6")]
        ActivityEnds = 8,
        /// <summary>
        /// 征集中
        /// </summary>
        [EnumItemDescription("征集中", Category = "7")]
        Collecting = 4,
        /// <summary>
        /// 投票中
        /// </summary>
        [EnumItemDescription("投票中", Category = "7")]
        Voting = 5,
        /// <summary>
        /// 评选中
        /// </summary>
        [EnumItemDescription("评选中", Category = "7")]
        InSelection = 6,
        /// <summary>
        /// 已结束
        /// </summary>
        [EnumItemDescription("已结束", Category = "7")]
        ActivityEnd = 7,

    }
}

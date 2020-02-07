using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 意见类型
    /// </summary>
    public enum OpinionType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumItemDescription("未知")]
        None = 0,
        /// <summary>
        /// 审核公司
        /// </summary>
        [EnumItemDescription("审核公司")]
        Company = 1,
        /// <summary>
        /// 审核项目
        /// </summary>
        [EnumItemDescription("审核项目")]
        Project = 2,
        /// <summary>
        /// 实名认证
        /// </summary>
        [EnumItemDescription("实名认证")]
        RealNameAuth = 3
    }
}

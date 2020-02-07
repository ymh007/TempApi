using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum AuditStatus
    {
        /// <summary>
        /// 草稿箱
        /// </summary>
        [EnumItemDescription("草稿箱")]
        None = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        [EnumItemDescription("待审核")]
        Auditing = 1,
        /// <summary>
        /// 未通过
        /// </summary>
        [EnumItemDescription("未通过")]
        Faid = 2,
        /// <summary>
        /// 已通过
        /// </summary>
        [EnumItemDescription("已通过")]
        Success = 3
    }
}

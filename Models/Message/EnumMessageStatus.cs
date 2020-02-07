using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 消息状态（枚举）
    /// </summary>
    public enum EnumMessageStatus
    {
        /// <summary>
        /// 新消息
        /// </summary>
        [EnumItemDescription(Description = "新消息")]
        New = 0,
        /// <summary>
        /// 已读消息
        /// </summary>
        [EnumItemDescription(Description = "已读消息")]
        IsRead = 1,
        /// <summary>
        /// 过期消息
        /// </summary>
        [EnumItemDescription(Description = "过期消息")]
        IsOverdue = 2
    }
}
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    /// <summary>
    /// 用户消息状态
    /// </summary>
    public enum UserMessageStatus
    {
         /// <summary>
        /// 未读
        /// </summary>
        [EnumItemDescription("未读")]
        Unread = 0,
        /// <summary>
        /// 已读
        /// </summary>
        [EnumItemDescription("已读")]
        Read = 1,
        /// <summary>
        /// 已删
        /// </summary>
        [EnumItemDescription("已删")]
        Deleted = 2,
        
    }
}

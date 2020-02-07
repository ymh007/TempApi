using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    public enum JourneyStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [EnumItemDescription("空闲")]
        None = 0,

        /// <summary>
        /// 已占用
        /// </summary>
        [EnumItemDescription("已占用")]
        Occupy = 1,

        /// <summary>
        /// 已过期
        /// </summary>
        [EnumItemDescription("已过期")]
        Overdue = 2
    }
}

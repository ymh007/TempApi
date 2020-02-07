using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 流程类型
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumItemDescription("未知")]
        None = 0,
        /// <summary>
        /// 生活家资料
        /// </summary>
        [EnumItemDescription("生活家资料")]
        LiferInfo = 1,

         /// <summary>
        /// 订单状态
        /// </summary>
        [EnumItemDescription("订单状态")]
        OrderStatus = 2
    }
}

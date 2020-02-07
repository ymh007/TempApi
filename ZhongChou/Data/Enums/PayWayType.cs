using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 支付方式类型
    /// </summary>
    public enum PayWayType
    {
        /// <summary>
        /// 无
        /// </summary>
        [EnumItemDescription("无")]
        None = 0,
        /// <summary>
        /// 支付宝
        /// </summary>
        [EnumItemDescription("支付宝")]
        Alipay = 1,
        /// <summary>
        /// 微信支付
        /// </summary>
        [EnumItemDescription("微信支付")]
        Wxpay = 2
    }
}

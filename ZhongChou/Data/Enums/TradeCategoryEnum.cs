using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 交易类型
    /// </summary>
    public enum TradeCategoryEnum
    {
        /// <summary>
        /// 红包交易
        /// </summary>
        [EnumItemDescription("红包交易")]
        Redep = 10,
        /// <summary>
        /// 现金支付（微信支付）
        /// </summary>
        [EnumItemDescription("现金支付")]
        Cash = 20,
    }
}

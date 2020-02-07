using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    public enum ReasonType
    {
    
        /// <summary>
        /// 取消订单
        /// </summary>
        [EnumItemDescription("取消订单")]
        CancleOrder = 1,

        /// <summary>
        /// 拒绝订单
        /// </summary>
        [EnumItemDescription("拒绝订单")]
        RefuseOrder = 2,

        /// <summary>
        /// 未发货申请退款
        /// </summary>
        [EnumItemDescription("申请退款")]
        ApplyRefund_NoDeliver = 3,

        /// <summary>
        /// 已发货申请退款
        /// </summary>
        [EnumItemDescription("申请退款")]
        ApplyRefund_Delivered = 4,

        ///// <summary>
        ///// 拒绝退款
        ///// </summary>
        //[EnumItemDescription("拒绝退款")]
        //RefuseRefund = 5,


    }
}

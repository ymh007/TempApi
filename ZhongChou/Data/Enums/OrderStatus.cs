using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 空
        /// </summary>
        [EnumItemDescription("空", Category = "0")]
        None = 00,

        #region 在售房订单状态(待支付->已支付->已确认->已退款)

        /// <summary>
        /// 待支付
        /// </summary>
        [EnumItemDescription("待支付", Category = "1,2")]
        ZaiShou_None = 0,
        /// <summary>
        /// 已支付
        /// </summary>
        [EnumItemDescription("已支付", Category = "1,2")]
        ZaiShou_Paid = 1,
        /// <summary>
        /// 已退款
        /// </summary>
        [EnumItemDescription("已退款", Category = "1,2")]
        ZaiShou_Refunded = 2,
        /// <summary>
        /// 已确认
        /// </summary>
        [EnumItemDescription("已确认", Category = "1,2")]
        ZaiShou_Confirmed = 3,
        /// <summary>
        /// 已关闭(仅仅起显示作用，不改变数据库数据)
        /// </summary>
        [EnumItemDescription("已关闭", Category = "1,2")]
        ZaiShou_Close = 4,
        #endregion


        #region 特价房订单状态(待支付->带现场确认->已确认)
        /// <summary>
        /// 待分享
        /// </summary>
        [EnumItemDescription("待分享", Category = "1,2")]
        Tejiafang_NotShared = 10,
        /// <summary>
        /// 待现场确认（已分享）
        /// </summary>
        [EnumItemDescription("待现场确认", Category = "1,2")]
        Tejiafang_Shared = 11,
        /// <summary>
        /// 已确认
        /// </summary>
        [EnumItemDescription("已确认", Category = "1,2")]
        Tejiafang_Confirmed = 12,
        /// <summary>
        /// 已完成
        /// </summary>
        [EnumItemDescription("已完成", Category = "1,2")]
        Tejiafang_Completed = 13,
        /// <summary>
        /// 待支付
        /// </summary>
        [EnumItemDescription("待支付", Category = "1,2")]
        Tejiafang_None = 14,
        /// <summary>
        /// 已支付(暂不支持该状态，供应商接口不识别该状态)
        /// </summary>
        [EnumItemDescription("已支付", Category = "1,2")]
        Tejiafang_Paid = 15,
        /// <summary>
        /// 已退款
        /// </summary>
        [EnumItemDescription("已退款", Category = "1,2")]
        Tejiafang_Refunded = 16,
        /// <summary>
        /// 已关闭(仅仅起显示作用，不改变数据库数据--未支付的订单过期后显示已关闭)
        /// </summary>
        [EnumItemDescription("已关闭", Category = "1,2")]
        Tejiafang_Close = 17,
        #endregion


        #region 案场活动订单状态

        /// <summary>
        /// 待支付
        /// </summary>
        [EnumItemDescription("待支付", Category = "6")]
        Anchang_NotEnroll = 20,
        /// <summary>
        /// 已报名
        /// </summary>
        [EnumItemDescription("已报名", Category = "6")]
        Anchang_Enrolled = 21,
        /// <summary>
        /// 已签到
        /// </summary>
        [EnumItemDescription("已签到", Category = "6")]
        Anchang_Signed = 22,
        /// <summary>
        /// 已评价
        /// </summary>
        [EnumItemDescription("已评价", Category = "6")]
        Anchang_Evaluated = 23,


        #endregion


        #region 生活家订单状态

        /// <summary>
        /// 待接受
        /// </summary>
        [EnumItemDescription("待接受", Category = "8")]
        Topic_Unaccepted = 30,
        /// <summary>
        /// 待支付
        /// </summary>
        [EnumItemDescription("待支付", Category = "8")]
        Topic_Unpaid = 31,
        /// <summary>
        /// 待见面
        /// </summary>
        [EnumItemDescription("待见面", Category = "8")]
        Topic_Unconfirmed = 32,
        /// <summary>
        /// 待评价
        /// </summary>
        [EnumItemDescription("待评价", Category = "8")]
        Topic_Unevaluated = 33,
        /// <summary>
        /// 已完成/待结算
        /// </summary>
        [EnumItemDescription("已完成", Category = "8")]
        Topic_Completed = 34,
        /// <summary>
        /// 已拒绝
        /// </summary>
        [EnumItemDescription("已拒绝", Category = "8")]
        Topic_Refused = 35,
        /// <summary>
        /// 已取消
        /// </summary>
        [EnumItemDescription("已取消", Category = "8")]
        Topic_Cancled = 36,

        #endregion


        #region 优惠券订单状态

        /// <summary>
        /// 待付款
        /// </summary>
        [EnumItemDescription("待付款", Category = "9")]
        Coupon_Unpaid = 40,

        /// <summary>
        /// 待发货
        /// </summary>
        [EnumItemDescription("待发货", Category = "9")]
        Coupon_UnDeliver = 41,

        /// <summary>
        /// 待收货
        /// </summary>
        [EnumItemDescription("待收货", Category = "9")]
        Coupon_UnReceipted = 42,

        /// <summary>
        /// 已完成
        /// </summary>
        [EnumItemDescription("已完成", Category = "9")]
        Coupon_Completed = 43,

        /// <summary>
        /// 已取消
        /// </summary>
        [EnumItemDescription("已取消", Category = "9")]
        Coupon_Cancled = 44,

        /// <summary>
        /// 退款中
        /// </summary>
        [EnumItemDescription("退款中", Category = "9")]
        Coupon_Refunding = 45,

        /// <summary>
        /// 已退款
        /// </summary>
        [EnumItemDescription("已退款", Category = "9")]
        Coupon_Refunded = 46,

        #endregion

        #region 展会通已支付
        [EnumItemDescription("已支付", Category = "10")]
        Zhanhui_Paid = 54,
        #endregion

    }
}

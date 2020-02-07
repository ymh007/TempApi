using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 订单类型（与projectTypeEnum一致）
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumItemDescription("未知")]
        None = 0,
        /// <summary>
        /// 特价房
        /// </summary>
        [EnumItemDescription("特价房")]
        Tejiafang = 1,
        /// <summary>
        /// 在售项目众筹
        /// </summary>
        [EnumItemDescription("在售项目众筹")]
        ZaiShou = 2,
        /// <summary>
        /// 在建项目众筹
        /// </summary>
        [EnumItemDescription("在建项目众筹")]
        ZaiJian = 3,
        /// <summary>
        /// 土地众筹
        /// </summary>
        [EnumItemDescription("土地众筹")]
        Land = 4,
        /// <summary>
        /// 产品线众筹
        /// </summary>
        [EnumItemDescription("产品线众筹")]
        ProdLine = 5,
        /// <summary>
        /// 案场活动
        /// </summary>
        [EnumItemDescription("案场活动")]
        Anchang = 6,
        /// <summary>
        /// 征集活动
        /// </summary>
        [EnumItemDescription("征集活动")]
        Online = 7,
        /// <summary>
        /// 生活家话题
        /// </summary>
        [EnumItemDescription("生活家话题")]
        Topic = 8,
        /// <summary>
        /// 优惠券
        /// </summary>
        [EnumItemDescription("优惠券")]
        Coupon = 9,
        /// <summary>
        /// 展会通
        /// </summary>
        [EnumItemDescription("展会通")]
        Zhanhui = 10,
    }
}

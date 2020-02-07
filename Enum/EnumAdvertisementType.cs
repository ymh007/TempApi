using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 广告类型
    /// </summary>
    public enum EnumAdvertisementType
    {
        /// <summary>
        /// 开屏广告
        /// </summary>
        [EnumItemDescription("开屏广告")]
        AppStart = 0,
        ///// <summary>
        ///// 工作通banner
        ///// </summary>
        //[EnumItemDescription("工作通banner")]
        //WorkRegionBanner = 1,
        /// <summary>
        /// 工作通banner新
        /// </summary>
        [EnumItemDescription("工作通banner")]
        WorkRegionBannerNew = 2,

        /// <summary>
        /// 工作通背景运营
        /// </summary>
        [EnumItemDescription("工作通背景运营")]
        WorkRegionBagOper = 3,
    }
}
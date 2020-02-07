using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 首页轮播图类型
    /// </summary>
    public enum EnumAppHomeSlideType
    {
        /// <summary>
        /// 网址
        /// </summary>
        [EnumItemDescription("网址")]
        View = 0,
        /// <summary>
        /// 直播
        /// </summary>
        [EnumItemDescription("直播")]
        Live = 1,
        /// <summary>
        /// 会议
        /// </summary>
        [EnumItemDescription("会议")]
        Conference = 2,
    }
}
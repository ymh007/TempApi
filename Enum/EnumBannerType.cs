using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 置顶轮播类型
    /// </summary>
    public enum EnumBannerType
    {
        /// <summary>
        /// 会议
        /// </summary>
        [EnumItemDescription("会议")]
        Conference = 0,
        /// <summary>
        /// 热议（话题）
        /// </summary>
        [EnumItemDescription("热议")]
        Topic = 1,
        /// <summary>
        /// H5直播直播
        /// </summary>
        [EnumItemDescription("H5直播")]
        Live = 2,
        /// <summary>
        /// SDK直播
        /// </summary>
        [EnumItemDescription("SDK直播")]
        LiveSDK = 3
    }

    /// <summary>
    /// 置顶轮播模块
    /// </summary>
    public enum EnumBannerModel
    {
        /// <summary>
        /// 会议
        /// </summary>
        [EnumItemDescription("企业圈")]
        Conference = 0
    }
}
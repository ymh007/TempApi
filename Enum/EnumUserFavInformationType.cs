using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 用户资讯收藏类型
    /// </summary>
    public enum EnumUserFavInformationType
    {
        /// <summary>
        /// 新闻中心
        /// </summary>
        [EnumItemDescription("新闻中心")]
        New = 1,

        /// <summary>
        /// 重要发文
        /// </summary>
        [EnumItemDescription("重要发文")]
        Important = 2,

        /// <summary>
        /// 单位通知
        /// </summary>
        [EnumItemDescription("单位通知")]
        Unit = 3,

        /// <summary>
        /// 部门通知
        /// </summary>
        [EnumItemDescription("部门通知")]
        Department = 4,

        /// <summary>
        /// 通知纪要
        /// </summary>
        [EnumItemDescription("会议纪要")]
        Meeting = 5,

        /// <summary>
        /// 党建新闻
        /// </summary>
        [EnumItemDescription("党建新闻")]
        PartyNew = 6,

        /// <summary>
        /// 党建发文
        /// </summary>
        [EnumItemDescription("党建发文")]
        PartyNotice = 7,
    }
}
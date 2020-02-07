using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 用户动态类型
    /// </summary>
    public enum NewsFeedAction
    {
        /// <summary>
        /// 发表话题
        /// </summary>
        [EnumItemDescription("发表话题")]
        Topic = 1,
        /// <summary>
        /// 评价活动
        /// </summary>
        [EnumItemDescription("评价活动")]
        Evaluate = 2,
        /// <summary>
        /// 参加的征集
        /// </summary>
        [EnumItemDescription("参与征集")]
        Join = 3,
        /// <summary>
        /// 报名的活动
        /// </summary>
        [EnumItemDescription("报名活动")]
        Enlist = 4

    }
}

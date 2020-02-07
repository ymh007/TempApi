using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 意见反馈状态
    /// </summary>
    public enum EvaluationType
    {
        /// <summary>
        /// 案场活动评价
        /// </summary>
        [EnumItemDescription("案场活动评价")]
        Anchang = 6,

         /// <summary>
        /// 案场活动评价
        /// </summary>
        [EnumItemDescription("生活家评价")]
        Topic = 8

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew
{
    /// <summary>
    /// 活动记录 ViewModel
    /// </summary>
    public class ActivityRecordViewModel
    {
        /// <summary>
        /// 关注/取消关注
        /// </summary>
        public class FollowViewModel
        {
            /// <summary>
            /// 活动编码
            /// </summary>
            public string ActivityCode { get; set; }
        }

        /// <summary>
        /// 统计ViewModel
        /// </summary>
        public class StatisticsViewModel
        {
            /// <summary>
            /// 报名人数
            /// </summary>
            public int ApplyCount { get; set; }

            /// <summary>
            /// 关注人数
            /// </summary>
            public int FollowCount { get; set; }

            /// <summary>
            /// 浏览人数
            /// </summary>
            public int ViewCount { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// 添加用户行为日志实体
    /// </summary>
    public class UserBehaviorLogViewModel
    {
        /// <summary>
        /// 模块
        /// </summary>
        public string Module { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long TimeStart { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public long TimeEnd { set; get; }
        /// <summary>
        /// 将Unix时间戳转换为DateTime
        /// </summary>
        public DateTime ConvertTime(long Unix)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            time = startTime.AddMilliseconds(Unix);
            return time;
        }
    }
}
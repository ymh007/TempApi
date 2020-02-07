using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Extensions
{
    /// <summary>
    /// 时间的相关扩展方法
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 将DateTime类型转换成DateTimeOffset
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime sender)
        {
            return new DateTimeOffset(sender, TimeSpan.Zero);
        }

        /// <summary>
        /// 远洋平台系统默认时间最小值
        /// </summary>
        public static readonly DateTime DefaultMinDate = DateTime.Parse(@"1/1/1753 12:00:00 AM");

        /// <summary>
        /// 远洋平台秕默认时间最大值
        /// </summary>
        public static readonly DateTime DefaultMaxDate = DateTime.Parse(@"12/31/9999 11:59:59 PM");
        /// <summary>
        /// 1970-1-1至今的毫秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetTimeStamp(this DateTime dateTime)
        {
            return ((long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
        }
        /// <summary>
        /// 1970-1-1至今的秒数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeStampTotalSeconds(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        /// <summary>
        ///秒数转当前时间
        /// </summary>
        /// <param name="epoch"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(this long epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
        }
    }
}
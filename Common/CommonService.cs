using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 常用工具类
    /// </summary>
    public class CommonService
    {
        #region 判断是否为手机号码
        /// <summary>
        /// 判断是否为手机号码
        /// </summary>
        public static bool IsHandset(string content)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(content, @"^1[3|4|5|6|7|8|9]\d{9}$");
        }
        #endregion

        #region 计算两个坐标之间的距离

        private const double EARTH_RADIUS = 6378.137; //地球半径

        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 计算两个坐标之间的距离
        /// </summary>
        public static double Distance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLat2 = Rad(lat2);

            double radLng1 = Rad(lng1);
            double radLng2 = Rad(lng2);

            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));

            s = s * EARTH_RADIUS;

            s = Math.Round(s * 10000) / 10000;

            return s;
        }

        #endregion

        #region 获取url中的查询字符串参数
        /// <summary>
        /// 获取url中的查询字符串参数
        /// </summary>
        public static NameValueCollection ExtractQueryParams(string url)
        {
            int startIndex = url.IndexOf("?");
            NameValueCollection values = new NameValueCollection();

            if (startIndex <= 0)
            {
                return values;
            }

            string[] nameValues = url.Substring(startIndex + 1).Split('&');

            foreach (string s in nameValues)
            {
                string[] pair = s.Split('=');

                string name = pair[0];
                string value = string.Empty;

                if (pair.Length > 1)
                    value = pair[1];

                values.Add(name, value);
            }
            return values;
        }
        #endregion


        /// <summary>
        /// 获取某月第一天
        /// </summary>
        public static DateTime FirstDayOfMounth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }
        /// <summary>
        /// 获取某月最后一天
        /// </summary>
        public static DateTime LastDayOfMounth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }

    }
}
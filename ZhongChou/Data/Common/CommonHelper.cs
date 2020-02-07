using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    public class CommonHelper
    {
        /// <summary>
        /// 根据参数日期，获取下次双色球开奖时间
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime NextLotteryDate(DateTime date)
        {
            date = date.Date;
            DayOfWeek week = date.DayOfWeek;

            switch (week)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Saturday:
                    date = date.AddDays(1); break;

                case DayOfWeek.Tuesday:
                case DayOfWeek.Friday:
                case DayOfWeek.Sunday:
                    date = date.AddDays(2); break;

                case DayOfWeek.Thursday:
                    date = date.AddDays(3); break;
            }

            return date.AddHours(21).AddMinutes(30);
        }

        /// <summary>
        /// 获取系统配置路径
        /// </summary>
        /// <returns></returns>
        public static string GetSysSettingPath()
        {
            return ConfigurationManager.AppSettings["sysSettingPath"];
        }
        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <returns></returns>
        public static string GetProjectFileUrl()
        {
            return ConfigurationManager.AppSettings["GetProjectFileUrl"];
        }
        public static bool IsTestMode()
        {
            return true;
        }

        public static float GetAppVersion()
        {
            return float.Parse(ConfigurationManager.AppSettings["appversion"]);
        }

        public static bool GetApppublished()
        {
            return bool.Parse(ConfigurationManager.AppSettings["apppublished"]);
        }

        public static string GetConnectionName()
        {
            return DatabaseUtility.CFOWDFUNDING;
        }


        /// <summary>
        /// 加密手机号，中间四位设置为*
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static string EncryptPhone(string phone)
        {
            if (phone.Length != 11) return phone;

            string result = string.Empty;

            string section1 = phone.Substring(0, 3);
            string section2 = "****";
            string section3 = phone.Substring(7, 4);

            return string.Format("{0}{1}{2}", section1, section2, section3);
        }

        public static string EncryptNickName(string nickName)
        {
            if (nickName.Contains("yxzc_")) return nickName;

            if (nickName.Contains("用户") && nickName.Length >= 13)
            {
                string section1 = nickName.Substring(0, 2);
                string section2 = "****";
                string section3 = nickName.Substring(nickName.Length - 4, 4);

                return string.Format("{0}{1}{2}", section1, section2, section3);
            }

            return nickName;
        }

        public static string GetFormatAppVersion(string version)
        {
            string result = "0";

            if (string.IsNullOrEmpty(version)) return result;

            string[] strarr = version.Split('.');

            return string.Format(strarr[0] + "." + strarr[1]);
        }
        /// <summary>
        /// APP日期比较格式化
        /// </summary>
        /// <param name="date">当前时间</param>
        /// <param name="TargeTime">比较时间</param>
        /// <returns></returns>
        public static string APPDateFormateDiff(DateTime date, DateTime TargeTime)
        {
            string result = "";
            TimeSpan date_ts = new TimeSpan(date.Ticks);
            TimeSpan target_ts = new TimeSpan(TargeTime.Ticks);
            TimeSpan ts = date_ts.Subtract(target_ts).Duration();
            //string dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";

            //float nYear = ((float)ts.Days) / 365;//
            //return nYear.ToString();//dateDiff
            if (ts.Days > 7)
            {
                result = date.ToString("MM月dd日");
            }
            else if (ts.Days >= 3)
            {
                result = ts.Days.ToString() + "天前";
            }
            else if (ts.Days == 2)
            {
                result = "前天";
            }
            else if (ts.Days == 1)
            {
                result = "昨天";
            }
            //else if (ts.Days == 0 && ts.Hours > 12)
            //{
            //    result = "今天";
            //}
            else if (ts.Days == 0 && ts.Hours >= 1)
            {
                result = ts.Hours.ToString() + "小时前";
            }
            else if (ts.Days == 0 && ts.Hours < 1 && ts.Minutes >= 1)
            {
                result = ts.Minutes.ToString() + "分钟前";
            }
            else if (ts.Days == 0 && ts.Hours < 1 && ts.Minutes < 1)
            {
                result = "刚刚";
            }
            //result += dateDiff;
            return result;
        }

        /// <summary>
        /// 获取活动房间户型
        /// </summary>
        /// <param name="BedroomNo">卧室数量</param>
        /// <param name="LivingroomNo">客厅数量</param>
        /// <param name="ToiletNo">卫生间数量</param>
        /// <returns>X室X厅X卫</returns>
        public static string GetActivityRoomHouseType(int BedroomNo, int LivingroomNo, int ToiletNo)
        {
            string[] chinaNum = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            return chinaNum[BedroomNo] + "室" + chinaNum[LivingroomNo] + "厅" + chinaNum[ToiletNo] + "卫";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using MobileBusiness.Common.Data;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    public sealed class DatabaseUtility
    {
        /// <summary>
        /// 众筹数据库  连接字符串名称
        /// </summary>
        public static readonly string CFOWDFUNDING = "Crowdfunding";
        /// <summary>
        /// 众筹【测试】数据库  连接字符串名称
        /// </summary>
        public static readonly string CFOWDFUNDINGTEST = "CrowdfundingTest";
        /// <summary>
        /// 平台资金账户
        /// </summary>
        public static readonly string PLATFORMFUNDACCOUNT = "7194BF56-2534-4B1D-8867-0C103A359D5D";

        /// <summary>
        /// 获取数据库中时间最大值
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDbDateTimeMaxValue()
        {
            return Convert.ToDateTime(DateTime.MaxValue.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static string CreateOrderNo(OrderType orderType)
        {
            return SystemNumAdapter.Instance.Create(orderType.ToString("D"));
        }

        /// <summary>
        /// 生成交易单号
        /// </summary>
        /// <param name="tradeCategory"></param>
        /// <returns></returns>
        public static string CreateTradeNo(TradeCategoryEnum tradeCategory)
        {
            return SystemNumAdapter.Instance.Create(tradeCategory.ToString("D"));
        }

        /// <summary>
        /// 生成签约码
        /// </summary>
        /// <returns></returns>
        public static string CreateSignupCode()
        {
            string year = DateTime.Now.ToString("yy");
            return SystemNumAdapter.Instance.Create(year);
        }
        /// <summary>
        /// 生成土地合伙人标识码
        /// </summary>
        /// <returns></returns>
        public static string CreateLandCateCode()
        {
            string year = DateTime.Now.ToString("yy");
            return "LDC" + SystemNumAdapter.Instance.Create(year);
        }

    }
}

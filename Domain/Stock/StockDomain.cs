using log4net;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.ScheduleManage;
using Seagull2.YuanXin.AppApi.Adapter.Stock;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Extensions;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.ScheduleManage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Domain.Stock
{
    /// <summary>
    /// 远洋股票 Domain
    /// </summary>
    public class StockDomain
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly StockDomain Instance = new StockDomain();
        /// <summary>
        /// 刷新股票数据
        /// </summary>
        public void RefreshStockData()
        {
            var url = System.Configuration.ConfigurationManager.AppSettings["StockApiUrl"];

            string json = HttpService.Get(url);

            if (!string.IsNullOrWhiteSpace(json))
            {
                var model = StockAdapter.Instance.Load(w => { }).SingleOrDefault();
                if (model == null)
                {
                    model = new Models.Stock.StockModel();
                    model.Code = Guid.NewGuid().ToString();
                    model.JsonDataString = json;
                    model.RefreshTime = DateTime.Now;
                    model.Creator = "System.Timers.Timer";
                    model.CreateTime = DateTime.Now;
                    model.ValidStatus = true;
                    StockAdapter.Instance.Update(model);
                }
                else
                {
                    model.JsonDataString = json;
                    model.RefreshTime = DateTime.Now;
                    model.Modifier = "System.Timers.Timer";
                    model.ModifyTime = DateTime.Now;
                    StockAdapter.Instance.Update(model);
                }
            }
        }
    }
}
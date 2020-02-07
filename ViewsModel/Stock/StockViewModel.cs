using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Stock
{
    /// <summary>
    /// 远洋股票信息
    /// </summary>
    public class StockViewModel
    {
        /// <summary>
        /// 最后更新值
        /// </summary>
        public string Last { set; get; }
        /// <summary>
        /// 变化
        /// </summary>
        public string Change { set; get; }
        /// <summary>
        /// 预览地址
        /// </summary>
        public string WebUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["StockWebUrl"];
            }
        }
    }
}
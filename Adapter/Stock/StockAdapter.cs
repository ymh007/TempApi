using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Stock
{
    /// <summary>
    /// 股票信息（第三方拉取） Adapter
    /// </summary>
    public class StockAdapter : UpdatableAndLoadableAdapterBase<StockModel, StockCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly StockAdapter Instance = new StockAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取单个股票信息对象
        /// </summary>
        public StockModel GetSingle()
        {
            try
            {
                var model = Load(w => { }).SingleOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
    }
}
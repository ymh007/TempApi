using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Stock
{
    /// <summary>
    /// 股票信息（第三方拉取） Model
    /// </summary>
    [ORTableMapping("office.Stock")]
    public class StockModel : BaseModel
    {
        /// <summary>
        /// 股票信息字符串
        /// </summary>
        [ORFieldMapping("JsonDataString")]
        public string JsonDataString { get; set; }

        /// <summary>
        /// 刷新时间
        /// </summary>
        [ORFieldMapping("RefreshTime")]
        public DateTime RefreshTime { get; set; }
    }

    /// <summary>
    /// 股票信息（第三方拉取） Collection
    /// </summary>
    public class StockCollection : EditableDataObjectCollectionBase<StockModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Weather
{
    /// <summary>
    /// 天气 Model
    /// </summary>
    [ORTableMapping("office.Weather")]
    public class WeatherModel : BaseModel
    {
        /// <summary>
        /// 地理位置
        /// </summary>
        [ORFieldMapping("Location")]
        public string Location { set; get; }

        /// <summary>
        /// 天气信息字符串
        /// </summary>
        [ORFieldMapping("JsonDataString")]
        public string JsonDataString { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [ORFieldMapping("ExpiredTime")]
        public DateTime ExpiredTime { get; set; }

        /// <summary>
        /// 刷新时间
        /// </summary>
        [ORFieldMapping("RefreshTime")]
        public DateTime RefreshTime { get; set; }
    }

    /// <summary>
    /// 天气 Collection
    /// </summary>
    public class WeatherCollection : EditableDataObjectCollectionBase<WeatherModel>
    {

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Adavertisement
{
    /// <summary>
    /// 广告Model
    /// </summary>
    [ORTableMapping("office.Advertisement")]
    public class AdvertisementModel : BaseModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 广告图
        /// </summary>
        [ORFieldMapping("Images")]
        public string Images { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [ORFieldMapping("Link")]
        public string Link { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        [ORFieldMapping("EndTime")]
        public DateTime EndTime { get; set; }
    }

    /// <summary>
    /// 广告Collection
    /// </summary>
    public class AdvertisementCollection : EditableDataObjectCollectionBase<AdvertisementModel>
    {

    }
}
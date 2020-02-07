using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    [ORTableMapping("office.SiteService")]
    /// <summary>
    /// 现场服务
    /// </summary>
    public class SiteServiceModel : ConferenceBase
    {
        /// <summary>
        /// 现场服务类型
        /// </summary>
        [ORFieldMapping("SiteServiceTypeID")]
        public string SiteServiceTypeID { get; set; }
        [NoMapping]
        public string SiteServiceTypeName
        {
            get
            {
                return SiteServiceTypeAdapter.Instance.LoadSiteServiceType(this.SiteServiceTypeID).Name;
            }
        }

        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }

        /// <summary>
        /// 附注
        /// </summary>
        [ORFieldMapping("Remarks")]
        public string Remarks { get; set; }
    }
    [Serializable]
    public class SiteServiceCollection : EditableDataObjectCollectionBase<SiteServiceModel>
    {

    }
}
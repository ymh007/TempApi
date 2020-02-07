using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    /// <summary>
    /// 会议服务
    /// </summary>
    [ORTableMapping("office.SiteServiceType")]
    public class SiteServiceTypeModel: ConferenceBase
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
    }
    [Serializable]
    public class SiteServiceTypeCollection : EditableDataObjectCollectionBase<SiteServiceTypeModel>
    {

    }
}
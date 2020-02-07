using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.ActivityNew
{
    /// <summary>
    /// 活动名范围设置
    /// </summary>
    [ORTableMapping("office.ActivityApplySet")]
    public class ActivityApplySetModel : BaseModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        [ORFieldMapping("ActivityCode")]
        public string ActivityCode { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }
    }

    /// <summary>
    /// 活动名范围设置集合
    /// </summary>
    public class ActivityApplySetCollection : EditableDataObjectCollectionBase<ActivityApplySetModel>
    {

    }
}
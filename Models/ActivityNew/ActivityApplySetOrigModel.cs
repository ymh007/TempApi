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
    /// 活动名范围设置（原始）
    /// </summary>
    [ORTableMapping("office.ActivityApplySetOrig")]
    public class ActivityApplySetOrigModel : BaseModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        [ORFieldMapping("ActivityCode")]
        public string ActivityCode { get; set; }

        /// <summary>
        /// 用户编码或部门编码
        /// </summary>
        [ORFieldMapping("SelectCode")]
        public string SelectCode { get; set; }

        /// <summary>
        /// 用户或部门（Users 或 Organizations）
        /// </summary>
        [ORFieldMapping("SelectType")]
        public string SelectType { get; set; }
    }

    /// <summary>
    /// 活动名范围设置集合
    /// </summary>
    public class ActivityApplySetOrigCollection : EditableDataObjectCollectionBase<ActivityApplySetOrigModel>
    {

    }
}
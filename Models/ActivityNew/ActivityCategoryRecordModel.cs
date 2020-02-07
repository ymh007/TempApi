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
    /// 活动分类关联
    /// </summary>
    [ORTableMapping("office.ActivityCategoryRecord")]
    public class ActivityCategoryRecordModel : BaseModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        [ORFieldMapping("ActivityCode")]
        public string ActivityCode { get; set; }

        /// <summary>
        /// 活动类型编码
        /// </summary>
        [ORFieldMapping("ActivityCategoryCode")]
        public string ActivityCategoryCode { get; set; }
    }

    /// <summary>
    /// 活动分类关联集合
    /// </summary>
    public class ActivityCategoryRecordCollection : EditableDataObjectCollectionBase<ActivityCategoryRecordModel>
    {

    }
}
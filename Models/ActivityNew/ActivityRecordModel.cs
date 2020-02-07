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
    /// 活动记录实体
    /// </summary>
    [ORTableMapping("office.ActivityRecord")]
    public class ActivityRecordModel : BaseModel
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

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public ActivityRecordType Type { get; set; }
    }

    /// <summary>
    /// 记录类型
    /// </summary>
    public enum ActivityRecordType
    {
        /// <summary>
        /// 浏览记录
        /// </summary>
        View = 0,
        /// <summary>
        /// 关注记录
        /// </summary>
        Follow = 1
    }

    /// <summary>
    /// 活动记录集合
    /// </summary>
    public class ActivityRecordCollection : EditableDataObjectCollectionBase<ActivityRecordModel>
    {

    }
}
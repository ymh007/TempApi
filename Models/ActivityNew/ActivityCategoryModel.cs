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
    /// 活动类型实体
    /// </summary>
    [ORTableMapping("office.ActivityCategory")]
    public class ActivityCategoryModel : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [ORFieldMapping("Icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }

    /// <summary>
    /// 活动类型集合
    /// </summary>
    public class ActivityCategoryCollection : EditableDataObjectCollectionBase<ActivityCategoryModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 工作人员类型-Model
    /// </summary>
    [ORTableMapping("office.WorkerType")]
    public class WorkerTypeModel : ConferenceBase
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { set; get; }
        /// <summary>
        /// 关联ID
        /// </summary>
        [ORFieldMapping("ContactId")]
        public string ContactId { set; get; }

    }

    /// <summary>
    /// 工作人员类型-Collection
    /// </summary>
    [Serializable]
    public class WorkerTypeCollection : EditableDataObjectCollectionBase<WorkerTypeModel>
    {

    }
}
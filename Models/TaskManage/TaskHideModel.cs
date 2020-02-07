using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.TaskManage
{
    /// <summary>
    /// 
    /// </summary>
    [ORTableMapping("office.TaskHide")]
    public class TaskHideModel : BaseModel
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        [ORFieldMapping("TaskCode")]
        public string TaskCode { get; set; }

    }

    /// <summary>
    /// TaskColeection
    /// </summary>

    public class TaskHideCollection : EditableDataObjectCollectionBase<TaskHideModel>
    {

    }
}
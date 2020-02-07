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
    [ORTableMapping("office.TaskCopyPerson")]
    public class TaskCopyPersonModel : BaseModel
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        [ORFieldMapping("TaskCode")]
        public string TaskCode { get; set; }

        /// <summary>
        /// 人员编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 人员名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

    }

    /// <summary>
    /// TaskCopyPersonColeection
    /// </summary>

    public class TaskCopyPersonCollection : EditableDataObjectCollectionBase<TaskCopyPersonModel>
    {

    }
}
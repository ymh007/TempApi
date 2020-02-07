using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.TaskManage
{
    /// <summary>
    /// 任务消息
    /// </summary>
    [ORTableMapping("office.TaskMessage")]
    public class TaskMessageModel : BaseModel
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        [ORFieldMapping("TaskCode")]
        public string TaskCode { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }

		/// <summary>
		/// 创建人名称
		/// </summary>
		public string CreatorName { get; set; }
    }
    /// <summary>
    /// TaskMessageColeection
    /// </summary>

    public class TaskMessageCollection : EditableDataObjectCollectionBase<TaskMessageModel>
    {

    }
}
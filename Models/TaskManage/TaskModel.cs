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
    [ORTableMapping("office.Task")]
    public class TaskModel : BaseModel
    {
        /// <summary>
        /// 标题内容
        /// </summary>
        [ORFieldMapping("TitleContent")]
        public string TitleContent { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        [ORFieldMapping("ParentCode")]
        public string ParentCode { get; set; }

		/// <summary>
		/// 截止时间
		/// </summary>
		[ORFieldMapping("Deadline")]
		public DateTime Deadline { get; set; }


		/// <summary>
		/// 优先级
		/// </summary>
		[ORFieldMapping("Priority")]
        public int Priority { get; set; }


        /// <summary>
        /// 执行人
        /// </summary>
        [ORFieldMapping("Executor")]
        public string Executor { get; set; }


        /// <summary>
        /// 执行人名称
        /// </summary>
        [ORFieldMapping("ExecutorName")]
        public string ExecutorName { get; set; }


        /// <summary>
        /// 完成状态
        /// </summary>
        [ORFieldMapping("CompletionState")]
        public int CompletionState { get; set; }

		/// <summary>
		/// 创建人名称
		/// </summary>
		[ORFieldMapping("CreatorName")]
		public string CreatorName { get; set; }
	}
    /// <summary>
    /// TaskColeection
    /// </summary>

    public class TaskCollection : EditableDataObjectCollectionBase<TaskModel>
    {

    }
}
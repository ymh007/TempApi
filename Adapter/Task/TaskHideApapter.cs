using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Seagull2.YuanXin.AppApi.Models.TaskManage.TaskHideModel;

namespace Seagull2.YuanXin.AppApi.Adapter.Task
{
    /// <summary>
    /// 隐藏任务Adapter
    /// </summary>
    public class TaskHideApapter : UpdatableAndLoadableAdapterBase<TaskHideModel, TaskHideCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly TaskHideApapter Instance = new TaskHideApapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

		/// <summary>
		/// 根据任务编码删除隐藏任务
		/// </summary>
		/// <param name="code"></param>
		public void DeleteHideTaskByTaskCode(string code)
		{
			Instance.Delete(m => m.AppendItem("TaskCode", code));
		}
    }
}
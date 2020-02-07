using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 工作人员类型-Adapter
    /// </summary>
    public class WorkerTypeAdapter : UpdatableAndLoadableAdapterBase<WorkerTypeModel, WorkerTypeCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly WorkerTypeAdapter Instance = new WorkerTypeAdapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName()
		{
			return Models.ConnectionNameDefine.YuanXinBusiness;
		}
	}
}
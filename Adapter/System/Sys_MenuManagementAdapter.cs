using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter
{
	/// <summary>
	/// Sys_MenuManagement-Adapter
	/// </summary>
	public class Sys_MenuManagementAdapter : UpdatableAndLoadableAdapterBase<Sys_MenuManagementModel, Sys_MenuManagementCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly Sys_MenuManagementAdapter Instance = new Sys_MenuManagementAdapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

	}

}
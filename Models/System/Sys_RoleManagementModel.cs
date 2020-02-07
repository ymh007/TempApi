using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models
{
	/// <summary>
	/// Sys_RoleManagement-Model
	/// </summary>
	[ORTableMapping("office.Sys_RoleManagement")]
	public class Sys_RoleManagementModel : BaseModel
	{
		/// <summary>
		/// 菜单名称
		/// </summary>
		[ORFieldMapping("Name")]
		public string Name { get; set; }
	}

	/// <summary>
	/// Sys_RoleManagement-Collection
	/// </summary>
	public class Sys_RoleManagementCollection : EditableDataObjectCollectionBase<Sys_RoleManagementModel>
	{

	}
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models
{
	/// <summary>
	/// Sys_AuthorityManagement-Model
	/// </summary>
	[ORTableMapping("office.Sys_AuthorityManagement")]
	public class Sys_AuthorityManagementModel : BaseModel
	{
		/// <summary>
		/// 角色编码
		/// </summary>
		[ORFieldMapping("RoleCode")]
		public string RoleCode { get; set; }
		/// <summary>
		/// 菜单编码
		/// </summary>
		[ORFieldMapping("MenuCode")]
		public string MenuCode { get; set; }
	}

	/// <summary>
	/// Sys_AuthorityManagement-Collection
	/// </summary>
	public class Sys_AuthorityManagementCollection : EditableDataObjectCollectionBase<Sys_AuthorityManagementModel>
	{

	}
}
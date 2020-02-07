using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models
{
	/// <summary>
	/// Sys_RoleUserManagement-Model
	/// </summary>
	[ORTableMapping("office.Sys_RoleUserManagement")]
	public class Sys_RoleUserManagementModel : BaseModel
	{
		/// <summary>
		/// 用户编码
		/// </summary>
		[ORFieldMapping("UserCode")]
		public string UserCode { get; set; }
		/// <summary>
		/// 用户姓名
		/// </summary>
		[ORFieldMapping("UserName")]
		public string UserName { get; set; }
		/// <summary>
		/// 角色编码
		/// </summary>
		[ORFieldMapping("RoleCode")]
		public string RoleCode { get; set; }
		/// <summary>
		/// 组织机构路径
		/// </summary>
		[ORFieldMapping("FullPath")]
		public string FullPath { get; set; }
		/// <summary>
		/// 是否是主职
		/// </summary>
		[ORFieldMapping("IsDefault")]
		public int IsDefault { get; set; }
	}

	/// <summary>
	/// Sys_RoleUserManagement-Collection
	/// </summary>
	public class Sys_RoleUserManagementCollection : EditableDataObjectCollectionBase<Sys_RoleUserManagementModel>
	{

	}
}
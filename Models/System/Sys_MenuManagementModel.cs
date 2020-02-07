using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models
{
	/// <summary>
	/// Sys_MenuManagement-Model
	/// </summary>
	[ORTableMapping("office.Sys_MenuManagement")]
	public class Sys_MenuManagementModel : BaseModel
	{
		/// <summary>
		/// 菜单名称
		/// </summary>
		[ORFieldMapping("Namet")]
		public string Namet { get; set; }
		/// <summary>
		/// 父级编码
		/// </summary>
		[ORFieldMapping("ParentCode")]
		public string ParentCode { get; set; }
		/// <summary>
		/// 菜单表示
		/// </summary>
		[ORFieldMapping("Flag")]
		public string Flag { get; set; }
		/// <summary>
		/// 排序
		/// </summary>
		[ORFieldMapping("Sort")]
		public int Sort { get; set; }
	}

	/// <summary>
	/// Sys_MenuManagement-Collection
	/// </summary>
	public class Sys_MenuManagementCollection : EditableDataObjectCollectionBase<Sys_MenuManagementModel>
	{

	}
}
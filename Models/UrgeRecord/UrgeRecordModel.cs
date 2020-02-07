using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.UrgeRecord
{
	/// <summary>
	/// 催办记录-Model
	/// </summary>
	[ORTableMapping("office.UrgeRecord")]
	public class UrgeRecordModel : BaseModel
	{
		/// <summary>
		/// 资源ID
		/// </summary>
		[ORFieldMapping("ResourceID")]
		public string ResourceID { get; set; }

		/// <summary>
		/// 流程ID
		/// </summary>
		[ORFieldMapping("ProcessID")]
		public string ProcessID { get; set; }

		/// <summary>
		/// 催办类型
		/// </summary>
		[ORFieldMapping("Category")]
		public string Category { get; set; }

		/// <summary>
		/// 环节ID
		/// </summary>
		[ORFieldMapping("ActivityID")]
		public string ActivityID { get; set; }

		/// <summary>
		/// 催办人编码
		/// </summary>
		[ORFieldMapping("UserCode")]
		public string UserCode { get; set; }

		/// <summary>
		/// 催办人姓名
		/// </summary>
		[ORFieldMapping("UserName")]
		public string UserName { get; set; }
	}

	/// <summary>
	/// 催办记录-Collection
	/// </summary>
	public class UrgeRecordCollection : EditableDataObjectCollectionBase<UrgeRecordModel> { }
}
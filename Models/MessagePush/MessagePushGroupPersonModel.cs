using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.MessagePush
{
	/// <summary>
	/// 群组人员 Model
	/// </summary>
	[ORTableMapping("[office].[MessagePushGroupPerson]")]
	public class MessagePushGroupPersonModel : BaseModel
	{
		/// <summary>
		/// 群组Code
		/// </summary>
		[ORFieldMapping("PushGroupCode")]
		public string PushGroupCode { get; set; }

		/// <summary>
		/// 用户编码
		/// </summary>
		[ORFieldMapping("UserCode")]
		public string UserCode { get; set; }

		/// <summary>
		/// 用户名称
		/// </summary>
		[ORFieldMapping("UserName")]
		public string UserName { get; set; }

        /// <summary>
		/// 用户类型
		/// </summary>
		[ORFieldMapping("ObjectType")]
        public string ObjectType { get; set; }

    }


	/// <summary>
	/// 群组人员 Collection
	/// </summary>
	public class MessagePushGroupPersonCollection : EditableDataObjectCollectionBase<MessagePushGroupPersonModel>
	{

	}
}
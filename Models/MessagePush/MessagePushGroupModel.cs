using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.MessagePush
{
	/// <summary>
	/// 消息推送群组 Model
	/// </summary>
	[ORTableMapping("[office].[MessagePushGroup]")]
	public class MessagePushGroupModel : BaseModel
	{
		/// <summary>
		/// 群组名称
		/// </summary>
		[ORFieldMapping("Name")]
		public string Name { get; set; }


        /// 群组类型  消息的群组的时候为‘1’  会议的对应的群组的时候存的位会议的id
		/// </summary>
		[ORFieldMapping("SourceType")]
        public string SourceType { get; set; }
    }

	/// <summary>
	/// 消息推送群组 Collection
	/// </summary>
	public class MessagePushGroupCollection : EditableDataObjectCollectionBase<MessagePushGroupModel>
	{

	}
}
using MCS.Library.OGUPermission;
using Seagull2.YuanXin.AppApi.Enum;
using SinoOcean.Seagull2.TransactionData.Meeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
	/// <summary>
	/// 添加会议提醒信息类（远洋移动办公添加消息专用）
	/// </summary>
	public class MessageForMeetingYuanXin : MessageForYxBase
	{
		/// <summary>
		/// 消息类型(0-签到，1-会议)
		/// </summary>
		public const string MessageTypeCode = "1";
		/// <summary>
		/// 会议消息类型（会议->会议开始,会议邀请,会议取消，签到->签到提醒）
		/// </summary>
		public EnumMessageTitle MessageTitleCode;

		/// <summary>
		/// 会议编码
		/// </summary>
		public string MeetingCode { get; set; }
		/// <summary>
		/// 会议室类型
		/// </summary>
		public EnumMessageModuleType ModuleType { get; set; }
		/// <summary>
		/// 创建人编码
		/// </summary>
		public string Creator { get; set; }
		/// <summary>
		/// 创建人姓名
		/// </summary>
		public string CreatorName { get; set; }
		/// <summary>
		/// 接收人
		/// </summary> 
		public List<MessageForMeetingYuanXinPeople> ReceivePerson { get; set; }
		/// <summary>
		/// 过期时间(会议开始时间)
		/// </summary>
		public DateTime OverdueTime { get; set; }
	}
	/// <summary>
	/// 接收人
	/// </summary>
	public class MessageForMeetingYuanXinPeople
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { set; get; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string DisplsyName
        {
            get
            {
                try
                {
                    return Adapter.AddressBook.ContactsAdapter.Instance.LoadByCode(UserCode).DisplayName;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// 添加签到提醒信息类（远洋移动办公添加消息专用）
    /// </summary>
    public class MessageForSignYuanXin : MessageForYxBase
    {
        /// <summary>
        /// 消息类型(0-签到，1-会议)
        /// </summary>
        public const string MessageTypeCode = "0";
        /// <summary>
        ///会议消息类型（会议->会议开始,会议邀请,会议取消，签到->签到提醒）
        /// </summary>
        public const EnumMessageTitle MessageTitleCode = EnumMessageTitle.Signin;
        /// <summary>
        /// 接收人(Dictionary 编码,人名)
        /// </summary> 
        public Dictionary<string, string> ReceivePerson { get; set; }
    }
}
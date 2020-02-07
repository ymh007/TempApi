using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Meeting
{
    /// <summary>
    /// 预定会议室信息实体
    /// </summary>
    public class MeetingModel
    {
        /// <summary>
        /// 构造
        /// </summary>
        public MeetingModel()
        {
            MeetingMan = new List<Man>();
        }
        /// <summary>
        /// 预定编码
        /// </summary>
        public string MeetingCode { set; get; }
        /// <summary>
        /// 会议名称（主题）
        /// </summary>
        public string Subject { set; get; }
        /// <summary>
        /// 会议内容
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { set; get; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string CreatorCode { set; get; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatorName { set; get; }
        /// <summary>
        /// 会议室名称
        /// </summary>
        public string MeetingRoomName { set; get; }
        /// <summary>
        /// 参会人列表
        /// </summary>
        public List<Man> MeetingMan { set; get; }

        /// <summary>
        /// 参会人
        /// </summary>
        public class Man
        {
            /// <summary>
            /// 参会人编码
            /// </summary>
            public string Code { set; get; }
            /// <summary>
            /// 参会人姓名
            /// </summary>
            public string Name
            {
                get
                {
                    try
                    {
                        return Adapter.AddressBook.ContactsAdapter.Instance.LoadByCode(Code).DisplayName;
                    }
                    catch
                    {
                        return string.Empty;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 我的会议
    /// </summary>
    public class MyMeetingModel
    {
        /// <summary>
        /// 预定编码
        /// </summary>
        public string MeetingCode { set; get; }
        /// <summary>
        /// 会议名称（主题）
        /// </summary>
        public string Subject { set; get; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { set; get; }
    }
}
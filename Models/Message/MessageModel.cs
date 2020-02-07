using System;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Message
{
    /// <summary>
    /// 提醒信息类
    /// </summary>
    [ORTableMapping("dbo.Message")]
    public class MessageModel
    {
        /// <summary>
        /// 消息编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 会议编码
        /// </summary>
        [ORFieldMapping("MeetingCode")]
        public string MeetingCode { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [ORFieldMapping("MessageContent")]
        public string MessageContent { get; set; }

        /// <summary>
        /// 消息状态编码（新消息、已读消息、过期消息）
        /// </summary>
        [ORFieldMapping("MessageStatusCode")]
        public EnumMessageStatus MessageStatusCode { get; set; }

        /// <summary>
        /// 消息状态名称（新消息、已读消息、过期消息）
        /// </summary>
        [NoMapping]
        public string MessageStatusName
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(this.MessageStatusCode);
            }
        }

        /// <summary>
        /// 消息类型（0签到、1会议、2系统、3会务...）
        /// </summary>
        [ORFieldMapping("MessageTypeCode")]
        public string MessageTypeCode { get; set; }

        /// <summary>
        /// 消息标题编码（会议开始提醒、会议邀请通知...）
        /// </summary>
        [ORFieldMapping("MessageTitleCode")]
        public EnumMessageTitle MessageTitleCode { get; set; }

        /// <summary>
        /// 消息标题名称（会议开始提醒、会议邀请通知...）
        /// </summary>
        [NoMapping]
        public string MessageTitleName
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(this.MessageTitleCode);
            }
        }

        /// <summary>
        /// 模块类型（投票、待办...）
        /// </summary>
        [ORFieldMapping("ModuleType")]
        public string ModuleType { get; set; }

        /// <summary>
        /// 创建人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [ORFieldMapping("CreatorName")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 接收人编码
        /// </summary> 
        [ORFieldMapping("ReceivePersonCode")]
        public string ReceivePersonCode { get; set; }

        /// <summary>
        /// 接收人姓名
        /// </summary>
        [ORFieldMapping("ReceivePersonName")]
        public string ReceivePersonName { get; set; }

        /// <summary>
        /// 接收人会议操作类型编码（会议接受通知、会议拒绝通知）
        /// </summary>
        [ORFieldMapping("ReceivePersonMeetingTypeCode")]
        public string ReceivePersonMeetingTypeCode { get; set; }

        /// <summary>
        /// 接收人会议操作类型名称（会议接受通知、会议拒绝通知）
        /// </summary>
        [NoMapping]
        public string ReceivePersonMeetingTypeName
        {
            get
            {
                if (this.ReceivePersonMeetingTypeCode == "2" || this.ReceivePersonMeetingTypeCode == "3")
                {
                    EnumMessageTitle meetType = (EnumMessageTitle)(int.Parse(this.ReceivePersonMeetingTypeCode));
                    return EnumItemDescriptionAttribute.GetDescription(meetType);
                }
                return "";
            }
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        [ORFieldMapping("OverdueTime")]
        public DateTime OverdueTime { get; set; }

        /// <summary>
        /// 过期时间字符串
        /// </summary>
        [NoMapping]
        public string OverdueTimeStr
        {
            get
            {
                return this.OverdueTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间字符串
        /// </summary>
        [NoMapping]
        public string CreateTimeStr
        {
            get
            {
                return this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 时间格式
        /// </summary>
        [NoMapping]
        public string CreateTimeFormat
        {
            get
            {
                var timeSpan = DateTime.Now - CreateTime;
                if (timeSpan.TotalSeconds < 60)
                {
                    return "刚刚";
                }
                if (timeSpan.TotalMinutes < 60)
                {
                    return $"{(int)timeSpan.TotalMinutes}分钟前";
                }
                if (timeSpan.TotalHours < 24)
                {
                    return $"{(int)timeSpan.TotalHours}小时前";
                }
                return CreateTime.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [ORFieldMapping("ReadTime")]
        public DateTime ReadTime { get; set; }


        /// <summary>
        /// MsgView
        /// </summary>
        [ORFieldMapping("MsgView")]
        public string MsgView { get; set; }


    }

    /// <summary>
    /// 消息提醒集合
    /// </summary>
    public class MessageCollection : EditableDataObjectCollectionBase<MessageModel>
    {

    }
}
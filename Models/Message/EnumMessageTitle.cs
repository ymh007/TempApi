using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 消息标题类型（枚举）
    /// </summary>
    public enum EnumMessageTitle
    {
        /// <summary>
        /// 会议开始提醒
        /// </summary>
        [EnumItemDescription(Description = "会议开始提醒")]
        MeetingBegin = 0,
        /// <summary>
        /// 会议邀请通知
        /// </summary>
        [EnumItemDescription(Description = "会议邀请通知")]
        MeetingRequest = 1,
        /// <summary>
        /// 会议接受通知
        /// </summary>
        [EnumItemDescription(Description = "会议接受通知")]
        ReceiveMeeting = 2,
        /// <summary>
        /// 会议拒绝通知
        /// </summary>
        [EnumItemDescription(Description = "会议拒绝通知")]
        RefuseMeeting = 3,
        /// <summary>
        /// 会议取消通知
        /// </summary>
        [EnumItemDescription(Description = "会议取消通知")]
        CancelMeeting = 4,
        /// <summary>
        /// 会议邀请回复提醒
        /// </summary>
        [EnumItemDescription(Description = "会议邀请回复提醒")]
        Other = 5,
        /// <summary>
        /// 打卡提醒
        /// </summary>
        [EnumItemDescription(Description = "打卡提醒")]
        Signin = 6,
        /// <summary>
        /// 系统提醒
        /// </summary>
        [EnumItemDescription(Description = "系统提醒")]
        System = 7,
        /// <summary>
        /// 会务提醒
        /// </summary>
        [EnumItemDescription(Description = "会务提醒")]
        Conference = 8,
        /// <summary>
        /// 会议修改提醒
        /// </summary>
        [EnumItemDescription(Description = "会议修改提醒")]
        MeetingUpdate = 9,
        /// <summary>
        /// 时间管理
        /// </summary>
        [EnumItemDescription(Description = "时间管理")]
        TimeManage = 10,
        /// <summary>
		/// 计划开始提醒
		/// </summary>
		[EnumItemDescription(Description = "计划开始提醒")]
        PlanManageStart = 11,
        /// <summary>
		/// 计划结束提醒
		/// </summary>
		[EnumItemDescription(Description = "计划结束提醒")]
        PlanManageEnd = 12,
        /// <summary>
		/// 企业邮箱
		/// </summary>
		[EnumItemDescription(Description = "企业邮箱")]
        EnterpriseMailbox = 13,
        /// <summary>
        /// 日程管理
        /// </summary>
        [EnumItemDescription(Description = "日程管理")]
        ScheduleManage = 14,

        /// <summary>
        /// 打卡提醒
        /// </summary>
        [EnumItemDescription(Description = "打卡补写提醒")]
        SigninException = 15,

        /// <summary>
        /// 平台消息 后台推送消息
        /// </summary>
        [EnumItemDescription(Description = "平台消息")]
         SysMsgPush= 16,

        /// <summary>
        /// 平台消息 意见反馈
        /// </summary>
        [EnumItemDescription(Description = "意见反馈")]
        Feedback = 17,


        /// <summary>
        /// 平台消息 销售日报
        /// </summary>
        [EnumItemDescription(Description = "销售日报")]
        SalesDaily = 18,

        /// <summary>
        /// 平台消息 会议通告
        /// </summary>
        [EnumItemDescription(Description = "会议通告")]
        ConferenceNotice = 18,

        /// <summary>
        /// 计划提醒 生日提醒
        /// </summary>
        [EnumItemDescription(Description = "生日提醒")]
        BirthdayNotify = 19,

    }
}
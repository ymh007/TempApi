using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
	/// <summary>
	/// 消息模块类型（针对ModuleType字段）
	/// </summary>
	public enum EnumMessageModuleType
	{
		/// <summary>
		/// 高管会议室
		/// </summary>
		[EnumItemDescription("高管会议室")]
		MeetingRoomManager = 1,
		/// <summary>
		/// 投票
		/// </summary>
		[EnumItemDescription("投票")]
		Vote = 2,
		/// <summary>
		/// 任务管理
		/// </summary>
		[EnumItemDescription("任务管理")]
		TaskManage = 3,
		/// <summary>
		/// 催办任务
		/// </summary>
		[EnumItemDescription("催办任务")]
		Task = 4,
		/// <summary>
		/// 时间管理
		/// </summary>
		[EnumItemDescription("时间管理")]
		TimeManage = 5,
		/// <summary>
		/// 客研数据
		/// </summary>
		[EnumItemDescription("客研数据")]
		ResearchData = 6,
		/// <summary>
		/// 活动
		/// </summary>
		[EnumItemDescription("活动")]
		Activity = 7,
        /// <summary>
        /// 工作汇报
        /// </summary>
        [EnumItemDescription("工作汇报")]
        WorkReport = 8,
        /// <summary>
        /// 企业邮箱
        /// </summary>
        [EnumItemDescription("企业邮箱")]
        EnterpriseMailbox = 9,
        /// <summary>
        /// 日程管理
        /// </summary>
        [EnumItemDescription("日程管理")]
        ScheduleManage = 10,

        /// <summary>
        /// 打卡补写提醒
        /// </summary>
        [EnumItemDescription(Description = "打卡补写提醒")]
        SigninException = 15,

        /// <summary>
        /// 平台消息 后台推送消息
        /// </summary>
        [EnumItemDescription(Description = "平台消息")]
        SysMsgPush = 16,

        /// <summary>
        /// 平台消息 意见反馈
        /// </summary>
        [EnumItemDescription(Description = "意见反馈")]
        Feedback = 17,

        /// <summary>
        /// 平台消息 会议通告
        /// </summary>
        [EnumItemDescription(Description = "会议通告")]
        ConferenceNotice = 18,

    }

	/// <summary>
	/// 会议类型枚举
	/// </summary>
	public enum EnumMessageType
	{
        /// <summary>
        /// 打卡提醒
        /// </summary>
        [EnumItemDescription(Description = "打卡提醒")]
		Signin = 0,
		/// <summary>
		/// 会议提醒
		/// </summary>
		[EnumItemDescription(Description = "会议提醒")]
		Meeting = 1,
		/// <summary>
		/// 系统提醒
		/// </summary>
		[EnumItemDescription(Description = "系统提醒")]
		System = 2,
		/// <summary>
		/// 会务提醒
		/// </summary>
		[EnumItemDescription(Description = "会务提醒")]
		Conference = 3
	}

}
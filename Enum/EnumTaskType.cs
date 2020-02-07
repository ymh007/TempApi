using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
	/// <summary>
	/// 任务类型枚举
	/// </summary>
	public enum EnumTaskType
	{
		/// <summary>
		/// 未完成
		/// </summary>
		[EnumItemDescription("未完成")]
		Unfinished = 0,
		/// <summary>
		/// 完成
		/// </summary>
		[EnumItemDescription("完成")]
		Finished = 1,
		/// <summary>
		/// 我创建的
		/// </summary>
		[EnumItemDescription("我创建的")]
		ICreate = 2,
		/// <summary>
		/// 我执行的
		/// </summary>
		[EnumItemDescription("我执行的")]
		IExecute = 3,
		/// <summary>
		/// 抄送我的
		/// </summary>
		[EnumItemDescription("抄送我的")]
		CopyMe = 4,
	}
	/// <summary>
	/// 任务消息枚举
	/// </summary>
	public enum EnumTaskMessageType
	{
		/// <summary>
		/// 全部消息
		/// </summary>
		[EnumItemDescription("全部")]
		All = 0,
		/// <summary>
		/// 评论
		/// </summary>
		[EnumItemDescription("评论")]
		Comment = 1
	}

	/// <summary>
	/// 任务状态枚举
	/// </summary>
	public enum EnumTaskCompletion
	{
		/// <summary>
		/// 未完成
		/// </summary>
		[EnumItemDescription("未完成")]
		Unfinished = 0,
		/// <summary>
		/// 完成
		/// </summary>
		[EnumItemDescription("完成")]
		Finished = 1
	}
}
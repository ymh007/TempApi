using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.TaskManage
{
	/// <summary>
	/// 任务管理-ViewModel
	/// </summary>
	public class TaskManageViewModel
	{
		#region 新增编辑ViewModel
		public class SaveViewModel
		{
			/// <summary>
			/// 任务编码
			/// </summary>
			[StringLength(36)]//字段长度
			public string Code { get; set; }

			/// <summary>
			/// 标题内容
			/// </summary>
			[Required]
			public string TitleContent { get; set; }

			/// <summary>
			/// 父级编码
			/// </summary>
			[StringLength(36)]//字段长度
			public string ParentCode { get; set; }

			/// <summary>
			/// 截止时间
			/// </summary>
			public DateTime Deadline { get; set; }

			/// <summary>
			/// 优先级
			/// </summary>
			public int Priority { get; set; }

			/// <summary>
			/// 执行人编码
			/// </summary>
			[StringLength(36)]
			public string Executor { get; set; }

			/// <summary>
			/// 执行人名称
			/// </summary>
			public string ExecutorName { get; set; }

			/// <summary>
			/// 抄送人
			/// </summary>
			public List<CopyPerson> CopyPersons { get; set; }
		}

		/// <summary>
		/// 抄送人
		/// </summary>
		public class CopyPerson
		{
			/// <summary>
			/// 用户编码
			/// </summary>
			[StringLength(36)]
			public string UserCode { get; set; }
		}
		#endregion

		#region RedisViewModel
		public class RedisViewModel
		{
			/// <summary>
			/// 任务编码
			/// </summary>
			[StringLength(36)]
			[Required]
			public string Code { get; set; }

			/// <summary>
			/// 提醒时间
			/// </summary>
			[Required]
			public string RemindTime { get; set; }

			/// <summary>
			/// 提醒字符串
			/// </summary>
			[Required]
			public string RemindStr { get; set; }
		}
		#endregion

		#region RedisSaveViewModel
		/// <summary>
		/// RedisViewModel
		/// </summary>
		public class RedisSaveViewModel
		{
			/// <summary>
			/// 任务编码
			/// </summary>
			public string Code { get; set; }


			/// <summary>
			/// 截止时间
			/// </summary>
			public DateTime RemindTime { get; set; }
		}
		#endregion

		#region 新增任务评论消息ViewModel
		/// <summary>
		/// 新增任务评论消息ViewModel
		/// </summary>
		public class TaskAddMessageViewModel
		{
			/// <summary>
			/// 任务编码
			/// </summary>
			[Required]
			[StringLength(36)]
			public string TaskCode { get; set; }

			/// <summary>
			/// 消息内容
			/// </summary>
			[Required]
			[StringLength(200)]//字段长度
			public string Content { get; set; }

		}
		#endregion

		#region 任务消息
		/// <summary>
		/// 任务消息
		/// </summary>
		public class TaskMessage
		{
			/// <summary>
			/// 编码
			/// </summary>
			public string Code { get; set; }

			/// <summary>
			/// 任务编码
			/// </summary>
			public string TaskCode { get; set; }

			/// <summary>
			/// 类型 0：全部；1：评论
			/// </summary>
			public int Type { get; set; }

			/// <summary>
			/// 内容
			/// </summary>
			public string Content { get; set; }

			/// <summary>
			/// 创建人编码
			/// </summary>
			public string Creator { get; set; }

			/// <summary>
			/// 创建人姓名
			/// </summary>
			public string CreatorName { get; set; }
			/// <summary>
			/// 创建时间
			/// </summary>
			public string CreateTime { get; set; }
		}

		/// <summary>
		/// 任务消息列表
		/// </summary>
		public class TaskMessageList
		{
			/// <summary>
			/// 全部消息总量
			/// </summary>
			public int AllCount { get; set; }

			/// <summary>
			/// 评论总量
			/// </summary>
			public int CommentCount { get; set; }

			/// <summary>
			/// 消息列表
			/// </summary>
			public List<TaskMessage> List { get; set; }
		}

		#endregion

		#region 任务列表返回数据格式ViewModel
		/// <summary>
		/// 任务列表ViewModel
		/// </summary>
		public class TaskListViewModel
		{
			/// <summary>
			/// 编码
			/// </summary>
			public string Code { get; set; }

			/// <summary>
			/// 是否是主任务
			/// </summary>
			public string ParentCode { get; set; }

			/// <summary>
			/// 任务名称
			/// </summary>
			public string TitleContent { get; set; }

			/// <summary>
			/// 父任务名称
			/// </summary>
			public string ParentTitle { get; set; }

			/// <summary>
			/// 创建者ID
			/// </summary>
			public string Creator { get; set; }

			/// <summary>
			/// 创建人名称
			/// </summary>
			public string CreatorName { get; set; }

			/// <summary>
			/// 创建时间
			/// </summary>
			public string CreateTime { get; set; }

			/// <summary>
			/// 截止时间
			/// </summary>
			public string Deadline { get; set; }

			/// <summary>
			/// 是否过期
			/// </summary>
			public bool IsOverDue { get; set; }

			/// <summary>
			/// 完成状态
			/// </summary>
			public int CompletionState { get; set; }

			/// <summary>
			/// 父任务完成状态
			/// </summary>
			public int ParentCompletionState { get; set; }

			/// <summary>
			/// 执行人code
			/// </summary>
			public string Executor { get; set; }

			/// <summary>
			/// 执行人名称
			/// </summary>
			public string ExecutorName { get; set; }

			/// <summary>
			/// 优先级
			/// </summary>
			public int Priority { get; set; }

			/// <summary>
			/// 子任务数量
			/// </summary>
			public int ChildTaskCount { get; set; }

			/// <summary>
			/// 子任务完成数
			/// </summary>
			public int CompleteChildTaskCount { get; set; }
		}
		#endregion

		#region 任务列表ViewModel
		/// <summary>
		/// 任务列表ViewModel
		/// </summary>
		public class TaskListsViewModel
		{
			/// <summary>
			/// 编码
			/// </summary>
			public string Code { get; set; }

			/// <summary>
			/// 父级编码
			/// </summary>
			public string ParentCode { get; set; }

			/// <summary>
			/// 任务名称
			/// </summary>
			public string TitleContent { get; set; }

			/// <summary>
			/// 创建者ID
			/// </summary>
			public string Creator { get; set; }

			/// <summary>
			/// 创建者
			/// </summary>
			public string CreatorName { get; set; }

			/// <summary>
			/// 创建时间
			/// </summary>
			public DateTime CreateTime { get; set; }

			/// <summary>
			/// 截止时间
			/// </summary>
			public DateTime Deadline { get; set; }

			/// <summary>
			/// 当前时间
			/// </summary>
			public DateTime NowTime { get; set; }

			/// <summary>
			/// 父任务名称
			/// </summary>
			public string ParentTitle { get; set; }

			/// <summary>
			/// 父任务完成状态
			/// </summary>
			public int ParentCompletionState { get; set; }

			/// <summary>
			/// 完成状态
			/// </summary>
			public int CompletionState { get; set; }

			/// <summary>
			/// 执行人code
			/// </summary>
			public string Executor { get; set; }

			/// <summary>
			/// 执行人名称
			/// </summary>
			public string ExecutorName { get; set; }

			/// <summary>
			/// 任务数
			/// </summary>
			public int TaskNumber { get; set; }

			/// <summary>
			/// 任务完成数
			/// </summary>
			public int TaskCompleteNumber { get; set; }

			/// 优先级
			/// </summary>
			public int Priority { get; set; }
		}
		#endregion

		#region 任务详情TaskDetails
		/// <summary>
		/// 任务详情TaskDetails
		/// </summary>
		public class TaskDetails
		{
			/// <summary>
			/// 编码
			/// </summary>
			public string Code { get; set; }

			/// <summary>
			/// 父级编码
			/// </summary>
			public string ParentCode { get; set; }

			/// <summary>
			/// 父任务名称
			/// </summary>
			public string ParentTitle { get; set; }

			/// <summary>
			/// 任务名称
			/// </summary>
			public string TitleContent { get; set; }

			/// <summary>
			/// 创建者编码
			/// </summary>
			public string Creator { get; set; }

			/// <summary>
			/// 创建者名称
			/// </summary>
			public string CreatorName { get; set; }

			/// <summary>
			/// 创建时间
			/// </summary>
			public string CreateTime { get; set; }

			/// <summary>
			/// 截止时间
			/// </summary>
			public string Deadline { get; set; }

            /// <summary>
            /// 完成状态
            /// </summary>
            public int CompletionState { get; set; }
            /// <summary>
            /// 父任务完成状态
            /// </summary>
            public int ParentCompletionState { get; set; }

            /// <summary>
            /// 执行人code
            /// </summary>
            public string Executor { get; set; }

			/// <summary>
			/// 执行人名称
			/// </summary>
			public string ExecutorName { get; set; }

			/// <summary>
			/// 优先级
			/// </summary>
			public int Priority { get; set; }

			/// <summary>
			/// 是否是主任务
			/// </summary>
			public bool IsMainTask { get; set; }

			/// <summary>
			/// 是否过期
			/// </summary>
			public bool IsOverdue { get; set; }

			/// <summary>
			/// 子任务数量
			/// </summary>
			public int ChildCount { get; set; }

			/// <summary>
			/// 子任务完成数量
			/// </summary>
			public int ChildCompleteCount { get; set; }

			/// <summary>
			/// 抄送人列表
			/// </summary>
			public List<BaseViewCodeName> CopyPersons { get; set; }

			/// <summary>
			/// 子任务列表
			/// </summary>
			public List<TaskDetailsChildOptins> ChildList { get; set; }
		}
		#endregion

		#region 任务详情TaskDetailsChildOptins辅助类
		/// <summary>
		/// 任务详情TaskDetailsChildOptins
		/// </summary>
		public class TaskDetailsChildOptins
		{
			/// <summary>
			/// 编码
			/// </summary>
			public string Code { get; set; }

			/// <summary>
			/// 任务名称
			/// </summary>
			public string TitleContent { get; set; }

			/// <summary>
			/// 完成状态
			/// </summary>
			public int CompletionState { get; set; }

			/// <summary>
			/// 执行人code
			/// </summary>
			public string Executor { get; set; }

			/// <summary>
			/// 执行人名称
			/// </summary>
			public string ExecutorName { get; set; }

            /// 优先级
            /// </summary>
            public int Priority { get; set; }
            /// 截止时间
            /// </summary>
            public string Deadline { get; set; }
            /// 是否失效
            /// </summary>
            public bool IsOverdue { get; set; }

        }
		#endregion

		#region 抄送人员列表
		public class CopyPersonDetails
		{
			/// <summary>
			/// 用户编码
			/// </summary>
			public string UserCode { get; set; }

			/// <summary>
			/// 用户姓名
			/// </summary>
			public string UserName { get; set; }

			/// <summary>
			/// 邮箱
			/// </summary>
			public string Email { get; set; }

			/// <summary>
			/// 电话
			/// </summary>
			public string Phone { get; set; }

			/// <summary>
			/// 部门
			/// </summary>
			public string Department { get; set; }
		}

		#endregion
	}
}
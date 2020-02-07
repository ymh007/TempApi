using log4net;
using MCS.Library.Data;
using MCS.Library.OGUPermission;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.Task;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Extensions;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using Seagull2.YuanXin.AppApi.ViewsModel;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using static Seagull2.YuanXin.AppApi.Domain.TaskManage.TaskObserver;
using static Seagull2.YuanXin.AppApi.Domain.TaskManage.TaskSubject;
using static Seagull2.YuanXin.AppApi.ViewsModel.TaskManage.TaskManageViewModel;

namespace Seagull2.YuanXin.AppApi.Domain.TaskManage
{
	
	/// <summary>
	///TaskObserver
	/// </summary>
	public class TaskObserver
	{
		/// <summary>
		/// 新增编辑任务订阅者公共接口
		/// </summary>
		public interface IObserver
		{
			void Add(TaskModel model, Seagull2Identity user, List<IUser> userList);

			void Edit(TaskModel formerModel, TaskModel currentModel, Seagull2Identity user, List<IUser> userList);
		}
		/// <summary>
		/// 取消任务订阅者公共接口
		/// </summary>
		public interface CancelIObserver
		{
			void Cancel(string code, Seagull2Identity user, TaskCollection taskColl);
		}

		public interface ChangeIObserver
		{
			void Change(Seagull2Identity user, TaskModel model, EnumTaskCompletion state);
		}
	}

	#region 系统提醒订阅类
	public class MessageObserver : IObserver
	{
		static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		#region 新增事件
		public void Add(TaskModel model, Seagull2Identity user, List<IUser> userList)
		{
			var domain = new TaskDomain();
			string pushMessage = domain.GetPushContent(model.TitleContent);
			MessageCollection messColl = new MessageCollection();
			if (userList.Count > 0)
			{
				foreach (var items in userList)
				{
					MessageModel mess = new MessageModel()
					{
						Code = Guid.NewGuid().ToString(),
						MeetingCode = model.Code,
						MessageContent = $"“{pushMessage}”抄送提醒",
						MessageStatusCode = EnumMessageStatus.New,
						MessageTypeCode = "2",
						MessageTitleCode = EnumMessageTitle.System,
						ModuleType = EnumMessageModuleType.TaskManage.ToString(),
						Creator = user.Id,
						CreatorName = user.DisplayName,
						ReceivePersonCode = items.ID,
						ReceivePersonName = items.DisplayName,
						ReceivePersonMeetingTypeCode = "",
						OverdueTime = DateTime.Now.AddDays(7),
						ValidStatus = true,
						CreateTime = DateTime.Now
					};
					messColl.Add(mess);
				}
			}
			if (!string.IsNullOrEmpty(model.Executor) && model.Executor != user.Id)
			{
				//执行人员系统提醒
				messColl.Add(new MessageModel
				{
					Code = Guid.NewGuid().ToString(),
					MeetingCode = model.Code,
					MessageContent = $"收到了一条新任务“{pushMessage}”",
					MessageStatusCode = EnumMessageStatus.New,
					MessageTypeCode = "2",
					MessageTitleCode = EnumMessageTitle.System,
					ModuleType = EnumMessageModuleType.TaskManage.ToString(),
					Creator = user.Id,
					CreatorName = user.DisplayName,
					ReceivePersonCode = model.Executor,
					ReceivePersonName = model.ExecutorName,
					ReceivePersonMeetingTypeCode = "",
					OverdueTime = DateTime.Now.AddDays(7),
					ValidStatus = true,
					CreateTime = DateTime.Now
				});
			}
			MessageAdapter.Instance.BatchInsert(messColl);
		}
		#endregion

		#region 编辑事件
		public void Edit(TaskModel formerModel, TaskModel currentModel, Seagull2Identity user, List<IUser> userList)
		{
			var domain = new TaskDomain();
			string pushMessage = domain.GetPushContent(currentModel.TitleContent);
			MessageCollection messColl = new MessageCollection();
			if (userList.Count > 0)
			{
				//抄送人员系统提醒
				foreach (var items in userList)
				{
					MessageModel mess = new MessageModel()
					{
						Code = Guid.NewGuid().ToString(),
						MeetingCode = currentModel.Code,
						MessageContent = $"“{pushMessage}”抄送提醒",
						MessageStatusCode = EnumMessageStatus.New,
						MessageTypeCode = "2",
						MessageTitleCode = EnumMessageTitle.System,
						ModuleType = EnumMessageModuleType.TaskManage.ToString(),
						Creator = user.Id,
						CreatorName = user.DisplayName,
						ReceivePersonCode = items.ID,
						ReceivePersonName = items.DisplayName,
						ReceivePersonMeetingTypeCode = "",
						OverdueTime = DateTime.Now.AddDays(7),
						ValidStatus = true,
						CreateTime = DateTime.Now
					};
					messColl.Add(mess);
				}
			}
			//新的执行人系统提醒
			if (!string.IsNullOrEmpty(currentModel.Executor) && formerModel.Executor != currentModel.Executor && currentModel.Executor != user.Id)
			{
				if (!string.IsNullOrEmpty(formerModel.Executor))
				{
					//原执行人员系统提醒
					messColl.Add(new MessageModel
					{
						Code = Guid.NewGuid().ToString(),
						MeetingCode = string.Empty,
						MessageContent = $"您的任务“{pushMessage}”已更换执行人",
						MessageStatusCode = EnumMessageStatus.New,
						MessageTypeCode = "2",
						MessageTitleCode = EnumMessageTitle.System,
						ModuleType = EnumMessageModuleType.TaskManage.ToString(),
						Creator = user.Id,
						CreatorName = user.DisplayName,
						ReceivePersonCode = formerModel.Executor,
						ReceivePersonName = formerModel.ExecutorName,
						ReceivePersonMeetingTypeCode = "",
						OverdueTime = DateTime.Now.AddDays(7),
						ValidStatus = true,
						CreateTime = DateTime.Now
					});
				}
				//新执行人员系统提醒
				messColl.Add(new MessageModel
				{
					Code = Guid.NewGuid().ToString(),
					MeetingCode = currentModel.Code,
					MessageContent = $"收到了一条新任务“{pushMessage}”",
					MessageStatusCode = EnumMessageStatus.New,
					MessageTypeCode = "2",
					MessageTitleCode = EnumMessageTitle.System,
					ModuleType = EnumMessageModuleType.TaskManage.ToString(),
					Creator = user.Id,
					CreatorName = user.DisplayName,
					ReceivePersonCode = currentModel.Executor,
					ReceivePersonName = currentModel.ExecutorName,
					ReceivePersonMeetingTypeCode = "",
					OverdueTime = DateTime.Now.AddDays(7),
					ValidStatus = true,
					CreateTime = DateTime.Now
				});
			}
			MessageAdapter.Instance.BatchInsert(messColl);
		}
		#endregion
	}
	#endregion

	#region 消息推送订阅类
	public class PushMessageObserver : IObserver
	{
		static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		#region 新增事件
		public void Add(TaskModel model, Seagull2Identity user, List<IUser> userList)
		{
			string pushResult = string.Empty;
			var domain = new TaskDomain();
			string pushMessage = domain.GetPushContent(model.TitleContent);
			//判断抄送人员是否大于0
			if (userList.Count > 0)
			{
				var ids = new List<string>();
				userList.ForEach(m =>
				{
					ids.Add(m.ID);
				});
				if (ids.Count < 1) { throw new Exception("无效的Code"); }
				var pushModelCopy = new PushService.Model()
				{
                    BusinessDesc = "任务管理，来自于移动办公后台的任务提醒",
                    Title = "任务提醒",
					Content = $"“{pushMessage}”抄送提醒",
					SendType = PushService.SendType.Person,
					Ids = string.Join(",", ids.ToArray()),
					Extras = new PushService.ModelExtras()
					{
						action = "TaskDetails",
                        bags = model.Code
					}
				};
				//bool isPushCopy = PushService.Push(pushModelCopy, out pushResult);
				//if (!isPushCopy)
				//{
				//	throw new Exception("推送失败");
				//}
			}
			//执行人推送过滤
			if (!string.IsNullOrEmpty(model.Executor) && model.Executor != user.Id)
			{
				//新执行人推送
				var pushModelNewExecutor = new PushService.Model()
				{
                    BusinessDesc="任务管理，来自于移动办公后台的任务提醒",
					Title = "任务提醒",
					Content = $"收到一个新任务“{pushMessage}”",
					SendType = PushService.SendType.Person,
					Ids = string.Join(",", model.Executor),
					Extras = new PushService.ModelExtras()
					{
						action = "TaskDetails",
                        bags = model.Code
					}
				};
				//bool isPushNewExecutor = PushService.Push(pushModelNewExecutor, out pushResult);

			}
		}
		#endregion

		#region 编辑事件
		public void Edit(TaskModel formerModel, TaskModel currentModel, Seagull2Identity user, List<IUser> userList)
		{
			string pushResult = string.Empty;
			var domain = new TaskDomain();
			string pushMessage = domain.GetPushContent(currentModel.TitleContent);
			//判断抄送人员是否大于0
			if (userList.Count > 0)
			{
				var ids = new List<string>();
				userList.ForEach(m =>
				{
					ids.Add(m.ID);
				});
				if (ids.Count < 1) { throw new Exception("无效的Code"); }
				var pushModelCopy = new PushService.Model()
				{
                    BusinessDesc = "任务管理，来自于移动办公后台的任务提醒",
                    Title = "任务提醒",
					Content = $"“{pushMessage}”抄送提醒",
					SendType = PushService.SendType.Person,
					Ids = string.Join(",", ids.ToArray()),
					Extras = new PushService.ModelExtras()
					{
						action = "TaskDetails",
                        bags = currentModel.Code
					}
				};
				//bool isPushCopy = PushService.Push(pushModelCopy, out pushResult);
				//if (!isPushCopy)
				//{
				//	throw new Exception("推送失败");
				//}
			}
			//执行人推送
			if (!string.IsNullOrEmpty(currentModel.Executor) && currentModel.Executor != user.Id && formerModel.Executor != currentModel.Executor)
			{
				if (string.IsNullOrEmpty(formerModel.Executor))
				{
					//原来执行人推送
					var pushModelOldExecutor = new PushService.Model()
					{
                        BusinessDesc = "任务管理，来自于移动办公后台的任务提醒",
                        Title = "任务提醒",
						Content = $"您的任务“{pushMessage}”已更换执行人",
						SendType = PushService.SendType.Person,
						Ids = string.Join(",", formerModel.Executor),
						Extras = new PushService.ModelExtras()
						{
							action = "TaskDetails",
                            bags = formerModel.Code
						}
					};
					//bool isPushOldExecutor = PushService.Push(pushModelOldExecutor, out pushResult);
				}
				var pushModelExecutor = new PushService.Model()
				{
                    BusinessDesc = "任务管理，来自于移动办公后台的任务提醒",
                    Title = "任务提醒",
					Content = $"收到一个新任务“{pushMessage}”",
					SendType = PushService.SendType.Person,
					Ids = string.Join(",", currentModel.Executor),
					Extras = new PushService.ModelExtras()
					{
						action = "TaskDetails",
                        bags = currentModel.Code
					}
				};
				//bool isPushExecutor = PushService.Push(pushModelExecutor, out pushResult);
			}
		}
		#endregion
	}
	#endregion

	#region 操作记录订阅类
	public class OperationRecordObserver : IObserver
	{
		static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		#region 新增事件
		public void Add(TaskModel model, Seagull2Identity user, List<IUser> userList)
		{
			//初始创建任务不执行任何消息提醒
			if (!string.IsNullOrEmpty(model.ParentCode))
			{
				var parentInfo = TaskApapter.Instance.Load(m => m.AppendItem("Code", model.ParentCode)).SingleOrDefault();
				var domain = new TaskDomain();
				//判断是否新增了子任务
				TaskMessageApapter.Instance.Update(new TaskMessageModel
				{
					Code = Guid.NewGuid().ToString(),
					TaskCode = parentInfo.Code,
					Content = string.Format("{0}创建子任务“{1}”", user.DisplayName, domain.GetPushContent(model.TitleContent)),
					CreatorName = user.DisplayName,
					Type = (int)EnumTaskMessageType.All,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true
				});
			}
		}
		#endregion

		#region 编辑事件
		public void Edit(TaskModel formerModel, TaskModel currentModel, Seagull2Identity user, List<IUser> userList)
		{
			TaskMessageCollection coll = new TaskMessageCollection();
			//判断是否修改了任务名称
			if (formerModel.TitleContent != currentModel.TitleContent)
			{
				coll.Add(new TaskMessageModel
				{
					Code = Guid.NewGuid().ToString(),
					Content = string.Format("{0}修改了任务名称", user.DisplayName),
					TaskCode = currentModel.Code,
					Type = (int)EnumTaskMessageType.All,
					CreatorName = user.DisplayName,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true
				});
			}
			//判断是否修改了执行人
			if (!string.IsNullOrEmpty(currentModel.Executor))
			{
				if (formerModel.Executor != currentModel.Executor)
				{
					if (string.IsNullOrEmpty(formerModel.Executor))
					{
						coll.Add(new TaskMessageModel
						{
							Code = Guid.NewGuid().ToString(),
							Content = string.Format("{0}添加了执行人", user.DisplayName),
							TaskCode = currentModel.Code,
							Type = (int)EnumTaskMessageType.All,
							CreatorName = user.DisplayName,
							Creator = user.Id,
							CreateTime = DateTime.Now,
							ValidStatus = true
						});
					}
					else
					{
						coll.Add(new TaskMessageModel
						{
							Code = Guid.NewGuid().ToString(),
							Content = string.Format("{0}修改了执行人", user.DisplayName),
							TaskCode = currentModel.Code,
							Type = (int)EnumTaskMessageType.All,
							Creator = user.Id,
							CreatorName = user.DisplayName,
							CreateTime = DateTime.Now,
							ValidStatus = true
						});
					}
				}
			}
			//判断是否修改了截止时间
			if (formerModel.Deadline != currentModel.Deadline)
			{
				if (formerModel.Deadline == DateTime.MinValue)
				{
					coll.Add(new TaskMessageModel
					{
						Code = Guid.NewGuid().ToString(),
						Content = $"{user.DisplayName}设置了截止时间",
						TaskCode = currentModel.Code,
						Type = (int)EnumTaskMessageType.All,
						CreatorName = user.DisplayName,
						Creator = user.Id,
						CreateTime = DateTime.Now,
						ValidStatus = true
					});
				}
				else
				{
					coll.Add(new TaskMessageModel
					{
						Code = Guid.NewGuid().ToString(),
						Content = string.Format("{0}修改了截止时间", user.DisplayName),
						TaskCode = currentModel.Code,
						Type = (int)EnumTaskMessageType.All,
						CreatorName = user.DisplayName,
						Creator = user.Id,
						CreateTime = DateTime.Now,
						ValidStatus = true
					});
				}
			}
			//判断是否添加了抄送人
			if (userList.Count > 0)
			{
				coll.Add(new TaskMessageModel
				{
					Code = Guid.NewGuid().ToString(),
					Content = $"{user.DisplayName}添加了抄送人",
					TaskCode = currentModel.Code,
					CreatorName = user.DisplayName,
					Type = (int)EnumTaskMessageType.All,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true
				});
			}
			//批量插入数据
			TaskMessageApapter.Instance.BatchInsert(coll);
		}
		#endregion
	}
	#endregion

	#region Redis操作订阅类
	public class ReidsOperaObserver : IObserver
	{
		/// <summary>
		/// 日志实例化
		/// </summary>
		ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected const string redisKey = "Seagull2.YuanXin.AppApi.TaskManage";

		#region 新增事件
		public void Add(TaskModel model, Seagull2Identity user, List<IUser> userList)
		{
			if (model.Deadline != DateTime.MinValue)
			{
				this.InsertRedis(model);
			}
		}
		#endregion

		#region 编辑事件
		public void Edit(TaskModel formerModel, TaskModel currentModel, Seagull2Identity user, List<IUser> userList)
		{
			RedisManager rm = new Extensions.RedisManager("ClientRedisHost");//配置redis
			if (currentModel.Deadline != DateTime.MinValue)
			{
				if (currentModel.CompletionState == 0)
				{
					if (formerModel.Deadline != currentModel.Deadline && formerModel.Deadline == DateTime.MinValue)
					{
						this.InsertRedis(currentModel);
					}
					else if (formerModel.Deadline != currentModel.Deadline && formerModel.Deadline != DateTime.MinValue)
					{
						var arr = rm.SetCombine(SetOperation.Union, redisKey, redisKey);//查询redis所有集合
						List<RedisViewModel> list = new List<RedisViewModel>();
						if (arr.Length > 0)
						{
							for (int i = 0; i < arr.Length - 1; i++)
							{
								list.Add(JsonConvert.DeserializeObject<RedisViewModel>(arr[i]));
							}
						}
						if (list.Count > 0)
						{
							list.OrderByDescending(o => o.RemindTime).ToList().ForEach(m =>
							{
								if (m.Code == currentModel.Code)
								{
									//从redis删除元素
									rm.SetRemove(redisKey, JsonConvert.SerializeObject(m));
								}
							});
						}
						this.InsertRedis(currentModel);
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// 保存到redis
		/// </summary>
		/// <param name="model"></param>
		public void InsertRedis(TaskModel model)
		{
			RedisManager rm = new Extensions.RedisManager("ClientRedisHost");//配置redis
			var viewModel = new RedisViewModel();
			viewModel.Code = model.Code;
			bool result = false;
			if (DateTime.Now < model.Deadline.AddDays(-1))
			{
				viewModel.RemindStr = "24小时";
				viewModel.RemindTime = model.Deadline.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
				var value = JsonConvert.SerializeObject(viewModel);
				result = rm.SetAdd(redisKey, value);
			}
			if (DateTime.Now < model.Deadline.AddHours(-1))
			{
				viewModel.RemindStr = "1小时";
				viewModel.RemindTime = model.Deadline.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss");
				var value = JsonConvert.SerializeObject(viewModel);
				result = rm.SetAdd(redisKey, value);
			}
			if (DateTime.Now < model.Deadline.AddMinutes(-15))
			{
				viewModel.RemindStr = "15分钟";
				viewModel.RemindTime = model.Deadline.AddMinutes(-15).ToString("yyyy-MM-dd HH:mm:ss");
				var value = JsonConvert.SerializeObject(viewModel);
				result = rm.SetAdd(redisKey, value);
			}
			if (!result)
			{
				log.Error("ReidsOperaObserver保存到redis失败");
			}
		}
	}

	#endregion

	#region 取消任务数据库订阅类
	public class CancelDataBaseObserver : CancelIObserver
	{
		public void Cancel(string code, Seagull2Identity user, TaskCollection taskColl)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				//删除人物表；
				TaskApapter.Instance.Delete(code);
				//删除人员表
				TaskCopyPersonApapter.Instance.Delete(taskColl.Select(m => m.Code).ToList());
				//事务提交
				scope.Complete();
			}
		}
	}
	#endregion

	#region 取消任务系统提醒订阅类
	public class CancelMessageObserver : CancelIObserver
	{
		public void Cancel(string code, Seagull2Identity user, TaskCollection taskColl)
		{
			TaskDomain domain = new TaskDomain(); //定义业务领域
			MessageCollection messColl = new MessageCollection();//定义系统提醒Coll
			var taskCopyPerson = domain.GetPersonsByListCode(taskColl);
			taskColl.ForEach(m =>
			{
				var userList = domain.GetAllPushMessage(taskCopyPerson, m);
				var pushMessage = domain.GetPushContent(m.TitleContent);
				//遍历推送人员list
				userList.ForEach(o =>
				{
					messColl.Add(new MessageModel   //所有任务的系统提醒
					{
						Code = Guid.NewGuid().ToString(),
						MeetingCode = string.Empty,
						MessageContent = string.Format("任务“{0}”已被{1}取消", pushMessage, user.DisplayName),
						MessageStatusCode = EnumMessageStatus.New,
						MessageTypeCode = "2",
						MessageTitleCode = EnumMessageTitle.System,
						ModuleType = EnumMessageModuleType.TaskManage.ToString(),
						Creator = user.Id,
						CreatorName = user.DisplayName,
						ReceivePersonCode = o.Code,
						ReceivePersonName = o.Name,
						ReceivePersonMeetingTypeCode = "",
						OverdueTime = DateTime.Now.AddDays(7),
						ValidStatus = true,
						CreateTime = DateTime.Now
					});
				});
			});
			//系统提醒
			MessageAdapter.Instance.BatchInsert(messColl);
		}
	}
	#endregion

	#region 取消任务推送消息订阅类
	public class CancelPushObserver : CancelIObserver
	{
		public void Cancel(string code, Seagull2Identity user, TaskCollection taskColl)
		{
			TaskDomain domain = new TaskDomain(); //定义业务领域
			MessageCollection messColl = new MessageCollection();//定义系统提醒Coll
			var taskCopyPerson = domain.GetPersonsByListCode(taskColl);
			var pushResult = string.Empty; //定义消息推送返回结果
			taskColl.ForEach(m =>
			{
				var userList = domain.GetAllPushMessage(taskCopyPerson, m);
				var pushMessage = domain.GetPushContent(m.TitleContent);
				//消息推送
				var taskPushPersons = new PushService.Model()
				{
                    BusinessDesc = "任务管理，来自于移动办公后台的任务提醒",
                    Title = string.Format("任务“{0}”已被{1}取消", pushMessage, m.CreatorName),
					Content = m.TitleContent,
					SendType = PushService.SendType.Person,
					Ids = string.Join(",", userList.Select(o => o.Code).ToArray()),
                    Extras = new PushService.ModelExtras()
                    {
                        action = "",
                        bags = ""
                    }
                };
				//PushService.Push(taskPushPersons, out pushResult);
			});
		}
	}
	#endregion

	#region 取消任务Redis订阅类
	public class CancelRedisObserver : CancelIObserver
	{
		protected const string redisKey = "Seagull2.YuanXin.AppApi.TaskManage";
		public void Cancel(string code, Seagull2Identity user, TaskCollection taskColl)
		{
			var rm = new RedisManager("ClientRedisHost");
			var arr = rm.SetCombine(SetOperation.Union, redisKey, redisKey);//查询redis所有集合
			List<RedisViewModel> list = new List<RedisViewModel>();
			if (arr.Length > 0)
			{
				for (int i = 0; i < arr.Length - 1; i++)
				{
					list.Add(JsonConvert.DeserializeObject<RedisViewModel>(arr[i]));
				}
				list.OrderByDescending(o => o.RemindTime).ToList().ForEach(m =>
				{
					if (taskColl.FindAll(t => t.Code == m.Code).Count > 0)
					{
						rm.SetRemove(redisKey, JsonConvert.SerializeObject(m));
					}
				});
			}
		}
	}
	#endregion

	#region 改变任务状态系统提醒订阅类
	public class ChangeTaskMessageObserver : ChangeIObserver
	{
		public void Change(Seagull2Identity user, TaskModel model, EnumTaskCompletion state)
		{
			TaskDomain domain = new TaskDomain();
			string pushContent = string.Empty;
			MessageCollection messColl = new MessageCollection();//定义系统提醒Coll
			if (state == EnumTaskCompletion.Finished)
			{
				pushContent = $"{user.DisplayName}完成了任务“{domain.GetPushContent(model.TitleContent)}”";
			}
			else
			{
				pushContent = $"{user.DisplayName}重新打开了任务“{domain.GetPushContent(model.TitleContent)}”";
			}

			if (model.Creator != model.Executor)
			{
				if (user.Id != model.Executor)
				{
					MessageAdapter.Instance.Update(new MessageModel
					{
						Code = Guid.NewGuid().ToString(),
						MeetingCode = model.Code,
						MessageContent = pushContent,
						MessageStatusCode = EnumMessageStatus.New,
						MessageTypeCode = "2",
						MessageTitleCode = EnumMessageTitle.System,
						ModuleType = EnumMessageModuleType.TaskManage.ToString(),
						Creator = model.Executor,
						CreateTime = DateTime.Now,
						CreatorName = model.ExecutorName,
						ReceivePersonCode = model.Executor,
						ReceivePersonName = model.ExecutorName,
						ReceivePersonMeetingTypeCode = "",
						OverdueTime = DateTime.Now.AddDays(7),
						ValidStatus = true,
					});
				}
				if (user.Id != model.Creator)
				{
					MessageAdapter.Instance.Update(new MessageModel
					{
						Code = Guid.NewGuid().ToString(),
						MeetingCode = model.Code,
						MessageContent = pushContent,
						MessageStatusCode = EnumMessageStatus.New,
						MessageTypeCode = "2",
						CreateTime = DateTime.Now,
						MessageTitleCode = EnumMessageTitle.System,
						ModuleType = EnumMessageModuleType.TaskManage.ToString(),
						Creator = model.Executor,
						CreatorName = model.ExecutorName,
						ReceivePersonCode = model.Creator,
						ReceivePersonName = model.CreatorName,
						ReceivePersonMeetingTypeCode = "",
						OverdueTime = DateTime.Now.AddDays(7),
						ValidStatus = true,
					});
				}
			}
		}
	}
	#endregion

	#region 改变任务状态消息推送订阅类
	public class ChangeTaskPushObserver : ChangeIObserver
	{
		public void Change(Seagull2Identity user, TaskModel model, EnumTaskCompletion state)
		{
			TaskDomain domain = new TaskDomain();
			string pushContent = string.Empty;
			if (state == EnumTaskCompletion.Finished)
			{
				pushContent = $"{user.DisplayName}完成了任务“{domain.GetPushContent(model.TitleContent)}”";
			}
			else
			{
				pushContent = $"{user.DisplayName}重新打开了任务“{domain.GetPushContent(model.TitleContent)}”";
			}
			var ids = new List<string>();

			if (model.Creator != model.Executor)
			{
				if (user.Id != model.Executor)
				{
					ids.Add(model.Executor);
				}
				if (user.Id != model.Creator)
				{
					ids.Add(model.Creator);
				}
			}
			else
			{
				return;
			}
			var pushModel = new PushService.Model()
			{
                BusinessDesc = "任务管理，来自于移动办公后台的任务提醒",
                Title = "任务提醒",
				Content = pushContent,
				SendType = PushService.SendType.Person,
				Ids = string.Join(",", ids),
				Extras = new PushService.ModelExtras()
				{
					action = "TaskDetails",
                    bags = model.Code
				}
			};
			string pushResult;
			//PushService.Push(pushModel, out pushResult);
		}
	}
	#endregion

	#region 改变任务状态操作订阅类
	public class ChangeTaskRecordObserver : ChangeIObserver
	{
		public void Change(Seagull2Identity user, TaskModel model, EnumTaskCompletion state)
		{
			TaskDomain domain = new TaskDomain();
			string pushContent = string.Empty;
			//判断是主任务还是子任务
			if (!string.IsNullOrEmpty(model.ParentCode))
			{
				//如果是子任务
				if (state == EnumTaskCompletion.Finished)
				{
					pushContent = $"{user.DisplayName}完成了子任务“{domain.GetPushContent(model.TitleContent)}”";
				}
				else
				{
					pushContent = $"{user.DisplayName}重新打开了子任务“{domain.GetPushContent(model.TitleContent)}”";
				}
				//给父任务执行记录
				TaskMessageApapter.Instance.Update(new TaskMessageModel
				{
					Code = Guid.NewGuid().ToString(),
					Content = pushContent,
					TaskCode = model.ParentCode,
					Type = (int)EnumTaskMessageType.All,
					Creator = model.Executor,
					CreateTime = DateTime.Now,
					ValidStatus = true
				});
			}
			else
			{
				//如果是父任务
				if (state == EnumTaskCompletion.Finished)
				{
					pushContent = $"{user.DisplayName}将上级任务已标记完成，“{domain.GetPushContent(model.TitleContent)}”子任务不可执行完成操作";
				}
				else
				{
					pushContent = $"{user.DisplayName}将上级任务已标记未完成，“{domain.GetPushContent(model.TitleContent)}”子任务可执行完成操作";
				}
				//给未完成的子任务执行记录
				var childColl = Adapter.Task.TaskApapter.Instance.GetChildCollByCode(model.Code).FindAll(f => f.CompletionState == 0).ToList();
				childColl.ForEach(t =>
				{
					TaskMessageApapter.Instance.Update(new TaskMessageModel
					{
						Code = Guid.NewGuid().ToString(),
						Content = pushContent,
						TaskCode = t.Code,
						Type = (int)EnumTaskMessageType.All,
						Creator = model.Executor,
						CreateTime = DateTime.Now,
						ValidStatus = true
					});
				});
			}

			//当前任务
			if (state == EnumTaskCompletion.Finished)
			{
				pushContent = $"{user.DisplayName}完成了任务“{domain.GetPushContent(model.TitleContent)}”";
			}
			else
			{
				pushContent = $"{user.DisplayName}重新打开了任务“{domain.GetPushContent(model.TitleContent)}”";
			}
			TaskMessageApapter.Instance.Update(new TaskMessageModel
			{
				Code = Guid.NewGuid().ToString(),
				Content = pushContent,
				TaskCode = model.Code,
				Type = (int)EnumTaskMessageType.All,
				Creator = model.Executor,
				CreateTime = DateTime.Now,
				ValidStatus = true
			});

		}
	}
	#endregion
}
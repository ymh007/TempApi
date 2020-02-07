using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.Task;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using static Seagull2.YuanXin.AppApi.Domain.TaskManage.TaskSubject;
using static Seagull2.YuanXin.AppApi.ViewsModel.TaskManage.TaskManageViewModel;

namespace Seagull2.YuanXin.AppApi.Domain.TaskManage
{
    /// <summary>
    /// 任务管理业务领域
    /// </summary>
    public class TaskDomain
    {
        /// <summary>
        /// 过滤无效人员
        /// </summary>
        public List<IUser> FilterInvalid(SaveViewModel model)
        {
            var personColl = TaskCopyPersonApapter.Instance.Select(model.Code);
            var ids = model.CopyPersons.Select(o => o.UserCode).ToList();
            ids.RemoveAll(m => personColl.Select(o => o.UserCode).Contains(m));
            ids = ids.Distinct().ToList();//去重
            if (ids.Count < 1 || ids.FirstOrDefault() == null)
            {
                return new List<IUser>();
            }
            var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ids.ToArray());
            return users.ToList().FindAll(m => m.IsSideline == false);//取出用户主职信息
        }

        /// <summary>
        /// 根据任务编码获取所有抄送人员
        /// </summary>
        /// <param name="code">任务编码</param>
        public List<BaseViewCodeName> GetCopyPerson(string code)
        {
            var list = new List<BaseViewCodeName>();
            var personColl = TaskCopyPersonApapter.Instance.Select(code);
            if (personColl.Count > 0)
            {
                personColl.ForEach(m =>
                {
                    var item = new BaseViewCodeName
                    {
                        Code = m.UserCode,
                        Name = m.UserName,
                    };
                    list.Add(item);
                });
            }
            return list;
        }

        #region 根据任务编码和人员编码删除数据
        /// <summary>
        /// 根据任务编码和人员编码删除数据
        /// </summary>
        /// <param name="taskCode">任务编码</param>
        /// <param name="userCode">人员编码</param>
        public void DletePersonByTaskCodeAndUserCode(string taskCode, string userCode)
        {
            TaskCopyPersonApapter.Instance.Delete(m => m.AppendItem("TaskCode", taskCode).AppendItem("UserCode", userCode));
        }
        #endregion

        /// <summary>
        /// 检验parentCode是否有效
        /// </summary>
        /// <returns></returns>
        public void IsValidOfPanretCode(string parentCode)
        {
            if (!string.IsNullOrEmpty(parentCode))
            {
                var parentInfo = TaskApapter.Instance.GetTaskCollByCode(parentCode);
                if (parentInfo.Count < 1) { throw new Exception("信息不存在"); }
            }
        }

        /// <summary>
        /// 新增任务信息
        /// </summary>
        /// <param name="user">当前登录用户</param>
        /// <param name="model">新增ViewModel</param>
        public void InsertTaskInfo(Seagull2Identity user, SaveViewModel model, out TaskModel taskInfo)
        {
            TaskModel item = new TaskModel
            {
                Code = Guid.NewGuid().ToString(),
                ParentCode = string.IsNullOrEmpty(model.ParentCode) ? "" : model.ParentCode,
                Deadline = model.Deadline,
                TitleContent = model.TitleContent,
                Priority = model.Priority,
                Executor = model.Executor,
                ExecutorName = model.ExecutorName,
                CompletionState = 0,
                Creator = user.Id,
                CreatorName = user.DisplayName,
                CreateTime = DateTime.Now,
                ValidStatus = true
            };
            TaskApapter.Instance.Update(item);
            taskInfo = item;
        }

        /// <summary>
        /// 新增抄送人员
        /// </summary>
        /// <param name="user">当前登录用户</param>
        /// <param name="userList">抄送人员列表</param>
        /// <param name="taskCode">任务编码</param>
        public void InsertCopyPerson(Seagull2Identity user, List<IUser> userList, string taskCode)
        {
            var personsColl = new TaskCopyPersonCollection();//定义任务抄送人员Collection
                                                             //新增任务抄送人员
            userList.ForEach(m =>
            {
                var personItem = new TaskCopyPersonModel
                {
                    Code = Guid.NewGuid().ToString(),
                    UserCode = m.ID,
                    UserName = m.DisplayName,
                    TaskCode = taskCode,
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    Modifier = user.Id,
                    ModifyTime = DateTime.Now,
                    ValidStatus = true
                };
                personsColl.Add(personItem);
            });
            TaskCopyPersonApapter.Instance.TaskCopyPersonInsert(personsColl);
        }

        /// <summary>
        /// 根据任务编码删除隐藏列表
        /// </summary>
        /// <param name="code"></param>
        public void DeleteTaskHide(string code)
        {
            TaskHideApapter.Instance.DeleteHideTaskByTaskCode(code);
        }

        /// <summary>
        /// 过滤发起人和执行人
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        public List<IUser> FilterCreatorAndExecutor(TaskModel taskInfo, List<IUser> userList)
        {
            userList.Remove(userList.Find(m => m.ID == taskInfo.Creator));
            userList.Remove(userList.Find(m => m.ID == taskInfo.Executor));
            return userList;
        }

        /// <summary>
        /// 检验任务Code是否有效
        /// </summary>
        /// <param name="taskCode"></param>
        public TaskModel IsVaildOfTaskCode(Seagull2Identity user, string taskCode)
        {
            var taskInfo = TaskApapter.Instance.GetTaskInfoByCode(taskCode);
            if (taskInfo == null)
            {
                throw new Exception("信息不存在");
            }
            if (taskInfo.Creator != user.Id)
            {
                throw new Exception("无操作权限");
            }
            return taskInfo;
        }

        /// <summary>
        /// 构建newModel
        /// </summary>
        /// <param name="model">新增编辑ViewModel</param>
        /// <param name="formberModel">数据库TaskModel</param>
        /// <returns></returns>
        public TaskModel StructureTaskModel(SaveViewModel model, TaskModel formberModel)
        {
            var currentModel = new TaskModel();
            currentModel.Code = formberModel.Code;
            currentModel.CompletionState = formberModel.CompletionState;
            currentModel.Creator = formberModel.Creator;
            currentModel.CreateTime = formberModel.CreateTime;
            currentModel.CreatorName = formberModel.CreatorName;
            currentModel.ParentCode = formberModel.ParentCode;
            currentModel.ValidStatus = formberModel.ValidStatus;
            currentModel.TitleContent = model.TitleContent;
            currentModel.Priority = model.Priority;
            currentModel.Deadline = model.Deadline;
            currentModel.Executor = string.IsNullOrEmpty(model.Executor) ? string.Empty : model.Executor;
            currentModel.ExecutorName = string.IsNullOrEmpty(model.ExecutorName) ? string.Empty : model.ExecutorName;
            return currentModel;
        }

        /// <summary>
        /// 判断TaskCode是否有效
        /// </summary>
        /// <param name="taskCode">任务编码</param>
        /// <returns></returns>
        public TaskModel IsValidOfTaskCode(string taskCode)
        {
            var taskInfo = TaskApapter.Instance.Load(m => m.AppendItem("Code", taskCode)).FirstOrDefault();
            if (taskInfo == null) { throw new Exception("信息不存在"); }
            return taskInfo;
        }

        /// <summary>
        /// 推送消息超过20字符截取操作
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string GetPushContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            return content.Length > 20 ? content.Substring(0, 20) + "....." : content;
        }

        /// <summary>
        /// 获取redis推送人员
        /// </summary>
        /// <param name="taskCode">任务编码</param>
        /// <returns></returns>
        public TaskCopyPersonCollection GetRedisMessagePush(TaskModel taskInfo)
        {

            var coll = TaskCopyPersonApapter.Instance.Load(m => m.AppendItem("TaskCode", taskInfo.Code));
            //添加执行人
            coll.Add(new TaskCopyPersonModel
            {
                TaskCode = taskInfo.Code,
                UserCode = taskInfo.Executor,
                UserName = taskInfo.ExecutorName
            });
            coll.Remove(m => m == coll.Find(o => o.UserCode == taskInfo.Creator));
            if (coll.Count < 1)
            {
                throw new Exception("无有效的推送人员");
            }
            return coll;
        }

        /// <summary>
        /// windows服务系统提醒
        /// </summary>
        /// <param name="taskInfo">TaskModel</param>
        /// <param name="coll">提醒人员Coll</param>
        /// <param name="pushMessage">推送消息内容</param>
        /// <param name="remindStr">过期字符串</param>
        public void TaskRedisMessagePush(TaskModel taskInfo, string pushMessage, string remindStr)
        {
            Models.Message.MessageCollection messColl = new Models.Message.MessageCollection();
            if (taskInfo.Executor != taskInfo.Creator)
            {
                //执行人
                messColl.Add(new MessageModel
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = taskInfo.Code,
                    MessageContent = $"你的任务“{pushMessage}”将于{remindStr}过期",
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = "2",
                    MessageTitleCode = EnumMessageTitle.System,
                    ModuleType = EnumMessageModuleType.TaskManage.ToString(),
                    Creator = string.Empty,
                    CreatorName = "OfficeService",
                    ReceivePersonCode = taskInfo.Executor,
                    ReceivePersonName = taskInfo.ExecutorName,
                    ReceivePersonMeetingTypeCode = "",
                    OverdueTime = DateTime.Now.AddDays(7),
                    ValidStatus = true,
                    CreateTime = DateTime.Now
                });
                //创建人
                messColl.Add(new MessageModel
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = taskInfo.Code,
                    MessageContent = $"你指派的任务“{pushMessage}”将于{remindStr}过期",
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = "2",
                    MessageTitleCode = EnumMessageTitle.System,
                    ModuleType = EnumMessageModuleType.TaskManage.ToString(),
                    Creator = string.Empty,
                    CreatorName = "OfficeService",
                    ReceivePersonCode = taskInfo.Creator,
                    ReceivePersonName = taskInfo.CreatorName,
                    ReceivePersonMeetingTypeCode = "",
                    OverdueTime = DateTime.Now.AddDays(7),
                    ValidStatus = true,
                    CreateTime = DateTime.Now

                });
            }
            else
            {
                //系统提醒
                messColl.Add(new MessageModel
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = taskInfo.Code,
                    MessageContent = $"你的任务“{pushMessage}”将于{remindStr}过期",
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = "2",
                    MessageTitleCode = EnumMessageTitle.System,
                    ModuleType = EnumMessageModuleType.TaskManage.ToString(),
                    Creator = string.Empty,
                    CreatorName = "OfficeService",
                    ReceivePersonCode = taskInfo.Creator,
                    ReceivePersonName = taskInfo.CreatorName,
                    ReceivePersonMeetingTypeCode = "",
                    OverdueTime = DateTime.Now.AddDays(7),
                    ValidStatus = true,
                    CreateTime = DateTime.Now
                });
            }
            MessageAdapter.Instance.BatchInsert(messColl);
        }

        /// <summary>
        /// Windows服务消息推送
        /// </summary>
        /// <param name="taskInfo">TaskModel</param>
        /// <param name="coll">提醒人员Coll</param>
        /// <param name="pushMessage">推送消息内容</param>
        /// <param name="remindStr">过期字符串</param>
        public void TaskRedisPushService(TaskModel taskInfo, string pushMessage, string remindStr)
        {
            string pushResult = string.Empty;
            if (taskInfo.Executor != taskInfo.Creator)
            {
                //消息推送（执行人）
                var pushCopyPerson = new PushService.Model()
                {
                    BusinessDesc = "任务管理",
                    Title = "任务管理",
                    Content = $"你的任务“{pushMessage}”将于{remindStr}过期",
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", taskInfo.Executor),
                    Extras = new PushService.ModelExtras()
                    {
                        action = "TaskDetails",
                        bags = taskInfo.Code
                    }
                };
                //消息推送（创建人）
                //PushService.Push(pushCopyPerson, out pushResult);
                var pushExecutor = new PushService.Model()
                {
                    BusinessDesc= "任务管理",
                    Title = "任务管理",
                    Content = $"你指派任务“{pushMessage}”将于{remindStr}过期",
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", taskInfo.Creator),
                    Extras = new PushService.ModelExtras()
                    {
                        action = "TaskDetails",
                        bags = taskInfo.Code
                    }
                };
                //PushService.Push(pushCopyPerson, out pushResult);
            }
            else
            {
                //消息推送（创建人）
                var pushCreator = new PushService.Model()
                {
                    BusinessDesc = "任务管理",
                    Title = "任务管理",
                    Content = $"你的任务“{pushMessage}”将于{remindStr}过期",
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", taskInfo.Creator),
                    Extras = new PushService.ModelExtras()
                    {
                        action = "TaskDetails",
                        bags = taskInfo.Code
                    }
                };
                //PushService.Push(pushCreator, out pushResult);
            }
        }

        /// <summary>
        /// 获取所有的任务信息
        /// </summary>
        /// <param name="code">任务编码</param>
        /// <returns></returns>
        public TaskCollection GetAllTaskColl(string code)
        {
            var taskColl = TaskApapter.Instance.GetTaskCollByCode(code);
            if (taskColl.Count < 1) throw new Exception("信息不存在");
            return taskColl;
        }

        /// <summary>
        /// 批量获取所有任务抄送人员
        /// </summary>
        /// <param name="coll">所有任务</param>
        /// <returns></returns>
        public List<TaskCopyPersonModel> GetPersonsByListCode(TaskCollection coll)
        {
            var list = coll.Select(m => m.Code).ToList();
            return TaskCopyPersonApapter.Instance.GetPersonList(list);
        }

        /// <summary>
        /// 获取所有推送消息人员
        /// </summary>
        /// <param name="personList">抄送人员List</param>
        /// <param name="model">任务信息Model</param>
        /// <returns>List<BaseViewCodeName></returns>
        public List<BaseViewCodeName> GetAllPushMessage(List<TaskCopyPersonModel> personList, TaskModel model)
        {
            //定义推送人员集合
            var userList = new List<BaseViewCodeName>();
            var childPersonColl = personList.FindAll(o => o.TaskCode == model.Code);
            var pushCodeList = new List<string>();
            //将执行人员Code，Name添加List
            userList.Add(new BaseViewCodeName
            {
                Code = model.Executor,
                Name = model.ExecutorName
            });
            if (childPersonColl.Count > 0)
            {
                childPersonColl.ForEach(o =>
                {
                    //将抄送人员Code，Name添加List
                    userList.Add(new BaseViewCodeName
                    {
                        Code = o.UserCode,
                        Name = o.UserName
                    });
                });
            }
            return userList;
        }

        /// <summary>
        /// 新增任务评论消息
        /// </summary>
        public void SaveTaskCommentMessage(TaskAddMessageViewModel model, Seagull2Identity user)
        {
            var MessageInfo = new TaskMessageModel
            {
                Code = Guid.NewGuid().ToString(),
                TaskCode = model.TaskCode,
                Content = model.Content,
                Type = (int)EnumTaskMessageType.Comment,
                Creator = user.Id,
                CreatorName = user.DisplayName,
                CreateTime = DateTime.Now,
                ValidStatus = true,
            };
            TaskMessageApapter.Instance.Update(MessageInfo);
        }

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="code">任务编码</param>
        /// <param name="type">消息类型0：全部；1：评论</param>
        /// <returns></returns>
        public TaskMessageList SelectMessageList(int pageIndex, int pageSize, string code, int type)
        {
            var list = TaskMessageApapter.Instance.SelectMessageList(pageIndex, pageSize, code, (int)type);
            var listALl = TaskMessageApapter.Instance.SelectMessageList(code, (int)0);
            var listCommentCount = TaskMessageApapter.Instance.SelectMessageList(code, (int)1);
            var resultList = new TaskMessageList();
            var messageList = new List<TaskMessage>();
            list?.ForEach(m =>
            {
                var item = new TaskMessage
                {
                    Code = m.Code,
                    Content = m.Content,
                    TaskCode = m.TaskCode,
                    Type = m.Type,
                    Creator = m.Creator,
                    CreatorName = m.CreatorName,
                    CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                };
                messageList.Add(item);
            });
            resultList.List = messageList;
            resultList.AllCount = listALl;
            resultList.CommentCount = listCommentCount;
            return resultList;
        }

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        public List<TaskListViewModel> GetTaskList(int pageIndex, int pageSize, int type, Seagull2Identity user)
        {
            var list = new List<TaskListsViewModel>();
            var checkList = new List<TaskListViewModel>();
            list = TaskApapter.Instance.SelectMyTaskList(pageIndex, pageSize, user.Id, type);
            //返回前台格式化后的数据
            list.ForEach(m =>
            {
                var item = new TaskListViewModel
                {
                    Code = m.Code,
                    ParentCode = m.ParentCode,
                    TitleContent = m.TitleContent,
                    Priority = m.Priority,
                    Creator = m.Creator,
                    Executor = m.Executor,
                    ExecutorName = m.ExecutorName,
                    CreatorName = m.CreatorName,
                    CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                    Deadline = m.Deadline != DateTime.MinValue ? m.Deadline.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                    ParentTitle = m.ParentTitle,
                    ParentCompletionState = m.ParentCompletionState,
                    CompletionState = m.CompletionState,
                    IsOverDue = DateTime.Now > m.Deadline ? true : false,
                    ChildTaskCount = m.TaskNumber,
                    CompleteChildTaskCount = m.TaskCompleteNumber
                };
                checkList.Add(item);
            });
            return checkList;
        }

        /// <summary>
        /// 任务搜索（分页）
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="userId">用户id</param>
        /// <param name="searchText">搜索条件</param>
        /// <returns></returns>
        public List<TaskListViewModel> TaskSearch(int pageIndex, int pageSize, string userId, string searchText)
        {
            var searchList = new List<TaskListViewModel>();
            var list = TaskApapter.Instance.TaskSearch(pageIndex, pageSize, userId, searchText);
            //返回前台格式化后的数据
            list.ForEach(m =>
            {
                var item = new TaskListViewModel
                {
                    Code = m.Code,
                    ParentCode = m.ParentCode,
                    TitleContent = m.TitleContent,
                    Priority = m.Priority,
                    Creator = m.Creator,
                    Executor = m.Executor,
                    ExecutorName = m.ExecutorName,
                    CreatorName = m.CreatorName,
                    CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                    Deadline = m.Deadline != DateTime.MinValue ? m.Deadline.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                    ParentTitle = m.ParentTitle,
                    CompletionState = m.CompletionState,
                    ParentCompletionState = m.ParentCompletionState,
                    IsOverDue = DateTime.Now > m.Deadline ? true : false,
                    ChildTaskCount = m.TaskNumber,
                    CompleteChildTaskCount = m.TaskCompleteNumber
                };
                searchList.Add(item);
            });
            return searchList;
        }

        /// <summary>
        /// 任务列表移除任务
        /// </summary>
        /// <param name="code">任务编码</param>
        /// <param name="user">当前登录用户</param>
        public void RemoveTask(string code, Seagull2Identity user)
        {
            var taskHideInfo = new TaskHideModel
            {
                Code = Guid.NewGuid().ToString(),
                TaskCode = code,
                Creator = user.Id,
                CreateTime = DateTime.Now,
                ValidStatus = true
            };
            TaskHideApapter.Instance.Update(taskHideInfo);
        }

        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <param name="code">任务编码</param>
        public TaskDetails GetTaskDetails(string code)
        {
            var currentTaskInfo = this.IsValidOfTaskCode(code);
            var coll = GetAllTaskColl(code);//获取任务及子任务
            var taskInfo = coll.Find(m => m.Code == code);
            var parentInfo = new TaskModel();
            if (!string.IsNullOrEmpty(taskInfo.ParentCode))
            {
                parentInfo = IsValidOfTaskCode(taskInfo.ParentCode);//获取主任务
            }
            var childColl = coll.FindAll(m => m.ParentCode == code);
            var childList = new List<TaskDetailsChildOptins>();
            var details = new TaskDetails
            {
                Code = taskInfo.Code,
                IsMainTask = string.IsNullOrEmpty(taskInfo.ParentCode) ? true : false,
                ParentCode = parentInfo.Code != null ? parentInfo.Code : "",
                ParentTitle = parentInfo.TitleContent != null ? parentInfo.TitleContent : "",
                TitleContent = taskInfo.TitleContent,
                CompletionState = taskInfo.CompletionState,
                ParentCompletionState = parentInfo.CompletionState,
                Priority = taskInfo.Priority,
                Deadline = taskInfo.Deadline != DateTime.MinValue ? taskInfo.Deadline.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                Executor = taskInfo.Executor,
                ExecutorName = taskInfo.ExecutorName,
                CreatorName = taskInfo.CreatorName,
                Creator = taskInfo.Creator,
                CreateTime = taskInfo.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                IsOverdue = DateTime.Now > taskInfo.Deadline ? true : false,
                ChildCount = childColl.Count,
                ChildCompleteCount = childColl.ToList().FindAll(m => m.CompletionState == 1).Count
            };
            //添加抄送人
            details.CopyPersons = this.GetCopyPerson(code);
            //添加子任务信息
            if (childColl.Count > 0)
            {
                childColl.ToList().ForEach(m =>
                {
                    var child = new TaskDetailsChildOptins
                    {
                        Code = m.Code,
                        TitleContent = GetPushContent(m.TitleContent),
                        Executor = m.Executor,
                        ExecutorName = m.ExecutorName,
                        CompletionState = m.CompletionState,
                        Priority = m.Priority,
                        Deadline = m.Deadline != DateTime.MinValue ? m.Deadline.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                        IsOverdue = m.Deadline > DateTime.Now ? true : false

                    };
                    childList.Add(child);
                });

            }
            details.ChildList = childList;
            return details;
        }

        /// <summary>
        /// 完成或重新打开任务
        /// </summary>
        public void ChangeTaskState(Seagull2Identity user, string code, EnumTaskCompletion state)
        {
            var currentTaskInfo = IsValidOfTaskCode(code);
            currentTaskInfo.CompletionState = (int)state;
            TaskApapter.Instance.Update(currentTaskInfo);
            var subject = new TaskSubject();//实例化主题
            subject.ChangeTaskState(user, currentTaskInfo, state);
        }

        /// <summary>
        /// 根据任务编码获取抄送人列表
        /// </summary>
        /// <param name="code"></param>
        public List<CopyPersonDetails> GetCopyPersonList(string code)
        {
            var list = new List<CopyPersonDetails>();
            var personColl = TaskCopyPersonApapter.Instance.Select(code);
            var ids = personColl.Select(o => o.UserCode).ToList();
            var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ids.ToArray());
            var iusers = users.ToList().FindAll(m => m.IsSideline == false);//取出用户主职信息
            if (iusers.Count > 0)
            {
                iusers.ForEach(m =>
                {
                    var arr = m.FullPath.Split('\\');
                    var item = new CopyPersonDetails
                    {
                        UserCode = m.ID,
                        UserName = m.DisplayName,
                        Department = arr[arr.Length - 2],
                        Email = m.Email,
                        Phone = m.Properties["MP"].ToString()
                    };
                    list.Add(item);
                });
            }
            return list;
        }
    }
}
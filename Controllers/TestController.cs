using MCS.Library.Data;
using MCS.Library.Data.Builder;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ContactsLabel;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.ScheduleManage;
using Seagull2.YuanXin.AppApi.Adapter.Sign;
using Seagull2.YuanXin.AppApi.Adapter.Task;
using Seagull2.YuanXin.AppApi.Adapter.Test;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Extensions;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using Seagull2.YuanXin.AppApi.Models.Test;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.TaskManage.TaskManageViewModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// Test-Controller
    /// </summary>
    public class TestController : ApiController
    {
        #region  新增或者更新一条数据
        /// <summary>
        /// 新增一条数据数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult TestSave(TestViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(model.id)) { throw new Exception("参数不能为空"); }
                var checkId = TestAdapter.Instance.Load(m => m.AppendItem("id", model.id));

                if (checkId.Count < 1)
                {

                    //新增保存信息
                    var labelInfo = new TestModel
                    {
                        id = Guid.NewGuid().ToString(),
                        name = model.name,
                        url = model.url,
                        alexa = model.alexa,
                        country = model.country,
                    };
                    TestAdapter.Instance.Update(labelInfo);

                }
                else
                {
                    //更新保存信息

                    var labelInfo = new TestModel
                    {
                        id = model.id,
                        name = model.name,
                        url = model.url,
                        alexa = model.alexa,
                        country = model.country,
                    };
                    TestAdapter.Instance.Update(labelInfo);

                }
            });
            return Ok(result);
        }

        #endregion

        #region 删除一条数据

        /// <summary>
        /// 删除一条数据
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult TestDelete(string id)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(id)) { throw new Exception("参数不能为空"); }
                var deleteId = TestAdapter.Instance.Load(m => m.AppendItem("id", id));
                if (deleteId.Count < 1) { throw new Exception("无效的id"); }

                TestAdapter.Instance.Delete(m => m.AppendItem("id", id));

            });
            return Ok(result);
        }

        #endregion

        #region 获取列表
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public IHttpActionResult TestLIst()
        {
            var result = ControllerService.Run(() =>
            {
                var data = TestAdapter.Instance.Load(o => { });
                return data;
            });
            return Ok(result);
        }

        #endregion

        //任务模块

        #region 取消任务
        /// <summary>
        /// 取消任务
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult TaskCancel(string code)
        {
            var result = ControllerService.Run(() =>
            {
                //参数校验
                if (string.IsNullOrEmpty(code)) { throw new Exception("参数不能为空"); }
                //根据code查询任务
                var taskColl = TaskApapter.Instance.Load(m => m.AppendItem("Code", code));
                if (taskColl.Count < 1) { throw new Exception("无效的code"); }
                //当前任务
                var taskInfo = taskColl.First();
                //获取当前登录用户
                var user = (Seagull2Identity)User.Identity;
                //用户权限校验
                if (taskInfo.Creator != user.Id)
                {
                    throw new Exception("无权限执行此操作");
                }
                //定义系统提醒Coll
                MessageCollection messColl = new MessageCollection();
                //定义消息推送返回结果
                var pushResult = string.Empty;
                //取出所有的任务；
                var tasks = TaskApapter.Instance.GetTaskCollByCode(code);
                //获取所有任务的Code
                var TaskCodeList = tasks.Select(item => item.Code).ToList();
                // 取出所有的抄送人员
                var taskCopyPersonlists = TaskCopyPersonApapter.Instance.GetPersonList(TaskCodeList);
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    //删除人员表；
                    TaskCopyPersonApapter.Instance.Delete(TaskCodeList);
                    //删除任务表
                    TaskApapter.Instance.Delete(code);
                    //事务提交
                    scope.Complete();
                }
                tasks.ForEach(m =>
                {
                    //定义推送人员集合
                    var userList = new List<BaseViewCodeName>();
                    var childPersonColl = taskCopyPersonlists.FindAll(o => o.TaskCode == m.Code);
                    var pushCodeList = new List<string>();
                    //将执行人员Code，Name添加List
                    userList.Add(new BaseViewCodeName
                    {
                        Code = m.Executor,
                        Name = m.ExecutorName
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
                    //遍历推送人员list
                    userList.ForEach(o =>
                     {
                         messColl.Add(new MessageModel   //所有任务的系统提醒
                         {
                             Code = Guid.NewGuid().ToString(),
                             MeetingCode = m.Code,
                             MessageContent = string.Format("任务{0}已被{1}取消", m.TitleContent, user.DisplayName),
                             MessageStatusCode = EnumMessageStatus.New,
                             MessageTypeCode = "2",
                             MessageTitleCode = EnumMessageTitle.System,
                             ModuleType = "Task",
                             Creator = user.Id,
                             CreatorName = user.DisplayName,
                             ReceivePersonCode = o.Code,
                             ReceivePersonName = o.Name,
                             ReceivePersonMeetingTypeCode = "",
                             OverdueTime = DateTime.Now.AddDays(7),
                             ValidStatus = true,
                             CreateTime = DateTime.Now
                         });
                         pushCodeList.Add(o.Code);
                     });
                    //消息推送
                    var taskPushPersons = new PushService.Model()
                    {
                        BusinessDesc = "取消任务",
                        Title = string.Format("任务{0}已被{1}取消", m.TitleContent, m.CreatorName),
                        Content = m.TitleContent,
                        SendType = PushService.SendType.Person,
                        Ids = string.Join(",", pushCodeList.ToArray()),
                        Extras = new PushService.ModelExtras()
                        {
                            action = "",
                            bags = ""
                        }
                    };
                    PushService.Push(taskPushPersons, out pushResult);
                });
                //系统提醒
                MessageAdapter.Instance.BatchInsert(messColl);
            });
            return Ok(result);
        }
        #endregion

        #region 任务评论列表
        /// <summary>
        /// 获取任务评论列表
        /// type 0;全部消息；type 评论消息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult TaskMessageList(int pageIndex, int pageSize, string code, EnumTaskMessageType type)
        {

            var result = ControllerService.Run(() =>
            {
                //参数校验
                var list = TaskMessageApapter.Instance.SelectMessageList(pageIndex, pageSize, code, (int)type);
                return list;

            });
            return Ok(result);
        }
        #endregion

        #region  新增任务评论消息
        /// <summary>
        /// 新增任务评论消息
        /// </summary>
        /// <param name="model">新增任务评论消息ViewModel</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        public IHttpActionResult TaskAddMessage(TaskAddMessageViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(model.TaskCode)) { throw new Exception("任务编码不能为空"); }
                if (string.IsNullOrEmpty(model.Content)) { throw new Exception("消息内容不能为空"); }
                //获取当前登录用户
                var user = (Seagull2Identity)User.Identity;
                //新增保存信息
                var MessageInfo = new TaskMessageModel
                {
                    Code = Guid.NewGuid().ToString(),
                    TaskCode = model.TaskCode,
                    Content = model.Content,
                    Type = 1,
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    ValidStatus = true,
                };
                TaskMessageApapter.Instance.Update(MessageInfo);

            });

            return Ok(result);
        }

        #endregion

        #region 获取任务列表
        /// <summary>
        /// 根据类型获取任务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetTaskList(int pageIndex, int pageSize, int type)
        {
            var result = ControllerService.Run(() =>
            {

                //获取当前用户的信息
                var user = (Seagull2Identity)User.Identity;
                var list = new List<TaskListsViewModel>();
                var checkList = new List<TaskListViewModel>();
                //type=代表未完成,type=代表完成type=2代表我发出的任务type=3代表我执行的任务type=4抄送我的任务
                if (type == 0 || type == 1 || type == 2 || type == 3 || type == 4)
                {
                    list = TaskApapter.Instance.SelectMyTaskList(pageIndex, pageSize, user.Id, type);

                }

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
                        CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                        Deadline = m.Deadline.ToString("yyyy-MM-dd HH:mm"),
                        ParentTitle = m.ParentTitle,
                        CompletionState = m.CompletionState,
                        IsOverDue = DateTime.Now > m.Deadline ? true : false,
                        ChildTaskCount = m.TaskNumber,
                        CompleteChildTaskCount = m.TaskCompleteNumber
                    };
                    checkList.Add(item);
                });

                return checkList;
            });
            return Ok(result);
        }

        #endregion

        #region 获取任务详情
        /// <summary>
        /// 获取任务详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetTaskDetails(string code)
        {
            var result = ControllerService.Run(() =>
            {
                //参数校验
                if (string.IsNullOrEmpty(code)) { throw new Exception("任务编码不能为空"); }
                var currentTask = TaskApapter.Instance.Load(m => m.AppendItem("Code", code));
                if (currentTask.Count < 1) { throw new Exception("无效的code"); }
                var currentTaskInfo = currentTask.First();

                //是主任务
                //获取子任务的集合
                var currentChildTask = TaskApapter.Instance.Load(m => m.AppendItem("ParentCode", code));
                var currentChildTaskinfo = new TaskModel();
                if (currentChildTask.Count > 0)
                {
                    currentChildTaskinfo = currentChildTask.First();
                }

                //是子任务去获取主任务信息
                var currentMainTask = TaskApapter.Instance.Load(m => m.AppendItem("Code", currentTaskInfo.ParentCode));
                var currentMainTaskinfo = new TaskModel();
                if (currentMainTask.Count > 0)
                {
                    currentMainTaskinfo = currentMainTask.First();
                }

                List<TaskDetailsChildOptins> listChildOption = new List<TaskDetailsChildOptins>();
                currentChildTask.ForEach(m =>

                listChildOption.Add(new TaskDetailsChildOptins
                {
                    Code = m.Code,
                    TitleContent = m.TitleContent,
                    Executor = m.Executor,
                    ExecutorName = m.ExecutorName
                })

                );
                var taskDetail = new TaskDetails
                {
                    Code = currentTaskInfo.Code,
                    ParentCode = currentTaskInfo.ParentCode,
                    TitleContent = currentTaskInfo.TitleContent,
                    Creator = currentTaskInfo.Creator,
                    CreatorName = currentTaskInfo.CreatorName,
                    Executor = currentTaskInfo.Executor,
                    ExecutorName = currentTaskInfo.ExecutorName,
                    CreateTime = currentTaskInfo.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                    Deadline = currentTaskInfo.Deadline.ToString("yyyy-MM-dd HH:mm"),
                    ParentTitle = string.IsNullOrEmpty(currentTaskInfo.ParentCode) ? "" : currentMainTaskinfo.TitleContent,
                    IsMainTask = string.IsNullOrEmpty(currentTaskInfo.ParentCode) ? true : false,
                    ChildList = listChildOption
                };
                return taskDetail;
            });
            return Ok(result);
        }

        #endregion

        #region 改变任务状态
        /// <summary>
        /// 改变任务状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ChangeTaskState(string code)
        {
            var result = ControllerService.Run(() =>
            {
                //参数校验
                if (string.IsNullOrEmpty(code)) { throw new Exception("任务编码不能为空"); }
                var currentTask = TaskApapter.Instance.Load(m => m.AppendItem("Code", code));
                if (currentTask.Count < 1) { throw new Exception("无效的code"); }
                var currentTaskInfo = currentTask.First();

                //更新任务信息
                var TaskInfo = new TaskModel
                {
                    Code = currentTaskInfo.Code,
                    ParentCode = currentTaskInfo.ParentCode,
                    Deadline = currentTaskInfo.Deadline,
                    TitleContent = currentTaskInfo.TitleContent,
                    Priority = currentTaskInfo.Priority,
                    Executor = currentTaskInfo.Executor,
                    ExecutorName = currentTaskInfo.ExecutorName,
                    CompletionState = currentTaskInfo.CompletionState == 1 ? 0 : 1,
                    Creator = currentTaskInfo.Creator,
                    CreatorName = currentTaskInfo.CreatorName,
                    CreateTime = currentTaskInfo.CreateTime,
                    ValidStatus = true
                };

                TaskApapter.Instance.Update(TaskInfo);


            });
            return Ok(result);
        }

        #endregion



        [HttpPost]
        public IHttpActionResult SendEmail(emailBody body)
        {
            // 发送邮件
            try
            {
                var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/employee_invite_email_v1.html");
                var streamReader = new StreamReader(fileStream);
                var template = streamReader.ReadToEnd();
                template = template.Replace("{{body}}", body.body);
                var message = new MCS.Library.SOA.DataObjects.EmailMessage(body.email, "远洋移动办公测试邮件", template);
                message.IsBodyHtml = true;
                MCS.Library.SOA.DataObjects.EmailMessageAdapter.Instance.Insert(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return this.Ok(body);
        }

        [HttpGet]
        public IHttpActionResult Redis(string key, string value, DateTime end)
        {
            object result = new object();
            try
            {
                if (DateTime.Now < end)
                {
                    RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
                    TimeSpan span = end.Subtract(DateTime.Now);
                    string valueStr = JsonConvert.SerializeObject(new
                    {
                        id = Guid.NewGuid().ToString(),
                        date = DateTime.Now.ToString(),
                        type = "xxx"
                    });

                    rm.StringSet(key, value, span);
                    result = new { key, value, end,rm };
                };
            }
            catch (Exception ex)
            {
                result = new { error = ex.ToString() };
            }
            return this.Ok(result);
        }
        [HttpGet]
        public IHttpActionResult Redis(string key, bool isInit = false)
        {
            object result = new object();
            try
            {
                RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
           
                if (isInit)
                {
                    string endStr = "";
                    var listData = PunchRemindSettingAdapter.Instance.Load(p => p.AppendItem("IsEnable", 1));
                    listData.ForEach(f =>
                    {
                        if (f.IsEnable)
                        {
                            key = EnumMessageTitle.Signin.ToString() + "_" + f.Code;
                            endStr = PunchRemindSettingAdapter.Instance.CalcEndTime(f);
                            if (endStr.Length > 0)
                            {
                                DateTime endDate = DateTime.Parse(endStr + " " + f.RemindTime);
                                TimeSpan expireSpan = endDate.Subtract(DateTime.Now);
                                rm.StringSet(key, f.Creator, expireSpan);
                            }
                        }
                    });

                    MessagePushRecordAdapter.Instance.Load(p=>p.AppendItem("timingSend",DateTime.Now," >").AppendItem("IsTiming",1)).ForEach(f=> {
                        TimeSpan expireSpan = (TimeSpan)f.TimingSend?.Subtract(DateTime.Now);
                        if (f.TimingSend > DateTime.Now) {
                            rm.StringSet(EnumMessageTitle.SysMsgPush.ToString() + "_" + f.Code, "", expireSpan);
                        }
                    });

                    ScheduleAdapter.Instance.Load(p=>p.AppendItem("ReminderTime",0,">").AppendItem("StartTime",DateTime.Now,">").AppendItem("ScheduleType",2,"<")).ForEach(f=> {
                        key = EnumMessageTitle.ScheduleManage.ToString() + "_" + f.Code;
                        DateTime endDate = DateTime.Now;
                        switch ((int)f.ReminderTime)
                        {
                            case 1:
                                endDate= f.StartTime;
                                break;
                            case 2:
                                endDate =f.StartTime.AddMinutes(-15);
                                break;
                            case 3:
                                endDate =f.StartTime.AddMinutes(-30);
                                break;
                            case 4:
                                endDate =f.StartTime.AddMinutes(-60);
                                break;
                            case 5:
                                endDate =f.StartTime.AddDays(-1);
                                break;
                        }
                        if (endDate > DateTime.Now)
                        {
                            TimeSpan expireSpan = endDate.Subtract(DateTime.Now);
                            rm.StringSet(key, "", expireSpan);
                        }
                    });
                }
                {
                    string getvalue = rm.GetAsync(key);
                    bool isDelSuccess = rm.DeleteKey(key);
                    result = new { key, getvalue, isDelSuccess };
                }
            }
            catch (Exception ex)
            {
                result = new { error = ex.ToString() };
            }
            return this.Ok(result);
        }
    }

    public class emailBody
    {
        public string email { get; set; }
        public string body { get; set; }
    }
}
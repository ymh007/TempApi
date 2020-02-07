using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.UrgeRecordAdapter;
using Seagull2.YuanXin.AppApi.PostSMSForDHSTSoap;
using Seagull2.YuanXin.AppApi.Services;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 代办
    /// </summary>
    public class UserTaskController : ApiController
    {

        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Seagull2 API 访问前缀
        /// </summary>
        string ApiUrl = ConfigurationManager.AppSettings["TaskApiUrl"];

        /// <summary>
        /// 接口对象
        /// </summary>
        private readonly IUserTaskService _userTaskService;

        /// <summary>
        /// 构造
        /// </summary>
        public UserTaskController(IUserTaskService userTaskService)
        {
            _userTaskService = userTaskService;
        }

        #region 获取用户待办数量
        /// <summary>
		/// 获取用户待办数量
		/// </summary>
		public async Task<IHttpActionResult> GetUserTaskCount1()
        {
            try
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var count = await _userTaskService.LoadUserTask(userCode, "task");
                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = count
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// 获取用户待办数量
        /// </summary>
        public async Task<IHttpActionResult> GetUserTaskCount()
        {
            try
            {
                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                var json = await HttpService.Get($"{ApiUrl}Task/GetUserTaskCount", authorization);

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<GetUserTaskCountModel>(json);
                var count = model.UnCompletedTaskNum;

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = count
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// 获取用户待办数量（合并）
        /// </summary>
        public async Task<IHttpActionResult> GetUserTaskCountMerge()
        {
            try
            {
                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                var json = await HttpService.Get($"{ApiUrl}Task/GetUserTaskCount", authorization);

                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<GetUserTaskCountModel>(json);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = model
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 加载待办、流转中、已办结、传阅和通知、收藏列表
        /// <summary>
		/// 加载待办、流转中、已办结、传阅和通知、收藏列表
		/// </summary>
		/// <param name="pageSize">页大小</param>
		/// <param name="pageIndex">页索引，从0开始</param>
		/// <param name="type">task：待办、running：流转中、completed：已办结、notice：传阅和通知、collection：收藏</param>
		/// <param name="keyword">流程标题关键词</param>
		[HttpGet]
        public async Task<IHttpActionResult> GetUserTask1(int pageSize, int pageIndex, string type, string keyword = "")
        {
            try
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var count = await _userTaskService.LoadUserTask(userCode, type, keyword);
                var data = await _userTaskService.LoadUserTask(pageSize, pageIndex, userCode, type, keyword);
                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new ViewsModel.BaseViewPage()
                    {
                        DataCount = count,
                        PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                        PageData = data
                    }
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// 加载待办、流转中、已办结、传阅和通知、收藏列表
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从0开始</param>
        /// <param name="type">task：待办、running：流转中、completed：已办结、notice：传阅和通知、collection：收藏</param>
        /// <param name="keyword">流程标题关键词</param>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserTask(int pageSize, int pageIndex, string type, string keyword = "")
        {
            try
            {
                var json = "";

                pageIndex++;

                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                var post = new GetUserTaskPostModel
                {
                    TaskTitle = string.IsNullOrWhiteSpace(keyword) ? null : keyword,
                    PageData = new PageDataModel
                    {
                        PageSize = pageSize,
                        PageIndex = pageIndex
                    }
                };
                //ServiceUtility.SetCertificatePolicy();
                if (type == "task" || type == "running" || type == "completed" || type == "notice")
                {
                    var query = "";
                    switch (type)
                    {
                        case "task": query = "?taskType=userTask&taskStatus=high"; break;
                        case "running": query = "?taskType=runingTask"; break;
                        case "completed": query = "?taskType=completedTask"; break;
                        case "notice": query = "?taskType=userTask&taskStatus=low"; break;
                    }

                    json = await HttpService.Post($"{ApiUrl}Task/LoadUserTask{query}", Newtonsoft.Json.JsonConvert.SerializeObject(post), authorization);
                }
                if (type == "collection")
                {
                    json = await HttpService.Get($"{ApiUrl}Task/LoadUserTaskCollection?pageSize={pageSize}&pageIndex={pageIndex}", authorization);
                }

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<GetUserTaskModel>(json);

                    var count = data.TotalItems;

                    var view = new ViewsModel.BaseView
                    {
                        State = true,
                        Message = "success.",
                        Data = new ViewsModel.BaseViewPage
                        {
                            DataCount = count,
                            PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                            PageData = data.ToUserTaskList()
                        }
                    };

                    return Ok(view);
                }
                else
                {
                    return Ok(new ViewsModel.BaseView()
                    {
                        State = false,
                        Message = "服务异常！"
                    });
                }
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 流程搜索
        /// <summary>
        /// 流程搜索
        /// </summary>
        /// <param name="count">每个项目显示的数量</param>
        /// <param name="keyword">关键词</param>
        [HttpGet]
        public async Task<IHttpActionResult> Search1(int count, string keyword)
        {
            try
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;

                var taskCount = await _userTaskService.LoadUserTask(userCode, "task", keyword);
                var taskData = await _userTaskService.LoadUserTask(count, 0, userCode, "task", keyword);

                var runningCount = await _userTaskService.LoadUserTask(userCode, "running", keyword);
                var runningData = await _userTaskService.LoadUserTask(count, 0, userCode, "running", keyword);

                var completedCount = await _userTaskService.LoadUserTask(userCode, "completed", keyword);
                var completedData = await _userTaskService.LoadUserTask(count, 0, userCode, "completed", keyword);

                var noticeCount = await _userTaskService.LoadUserTask(userCode, "notice", keyword);
                var noticeData = await _userTaskService.LoadUserTask(count, 0, userCode, "notice", keyword);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new
                    {
                        Task = new SearchData() { Count = taskCount, Data = taskData },
                        Running = new SearchData() { Count = runningCount, Data = runningData },
                        Completed = new SearchData() { Count = completedCount, Data = completedData },
                        Notice = new SearchData() { Count = noticeCount, Data = noticeData }
                    }
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// 流程搜索
        /// </summary>
        /// <param name="count">每个项目显示的数量</param>
        /// <param name="keyword">关键词</param>
        [HttpGet]
        public async Task<IHttpActionResult> Search(int count, string keyword)
        {
            try
            {
                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                var json = await HttpService.Get($"{ApiUrl}Task/Search?count={count}&keyword={keyword}", authorization);


                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchModel>(json);

                var userCode = ((Seagull2Identity)User.Identity).Id;

                var taskCount = data.Data.Task.Data.TotalItems;
                var taskData = data.Data.Task.Data.ToUserTaskList().Take(count);

                var runningCount = data.Data.Running.Data.TotalItems;
                var runningData = data.Data.Running.Data.ToUserTaskList().Take(count);

                var completedCount = data.Data.Completed.Data.TotalItems;
                var completedData = data.Data.Completed.Data.ToUserTaskList().Take(count);

                var noticeCount = data.Data.Notice.Data.TotalItems;
                var noticeData = data.Data.Notice.Data.ToUserTaskList().Take(count);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new
                    {
                        Task = new SearchData() { Count = taskCount, Data = taskData },
                        Running = new SearchData() { Count = runningCount, Data = runningData },
                        Completed = new SearchData() { Count = completedCount, Data = completedData },
                        Notice = new SearchData() { Count = noticeCount, Data = noticeData }
                    }
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// 响应数据结构
        /// </summary>
        class SearchData
        {
            public int Count;
            public object Data;
        }
        #endregion

        #region 收藏/取消收藏
        /// <summary>
        /// 收藏/取消收藏
        /// </summary>
        /// <param name="taskIds">taskId，多个用（,）分割</param>
        [HttpGet]
        public IHttpActionResult Collection(string taskIds)
        {
            var result = ControllerService.Run(() =>
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;

                var taskIdArr = taskIds.Split(',');
                foreach (string taskId in taskIdArr)
                {
                    _userTaskService.Collection(userCode, taskId);
                }
            });
            return Ok(result);
        }
        /// <summary>
        /// 收藏/取消收藏 new
        /// </summary>
        [HttpPost]
        public async Task<IHttpActionResult> CollectionNew(List<CollectionModel> post)
        {
            try
            {
                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                foreach (var item in post)
                {
                    var status = item.Status == 0 ? 1 : 0;
                    await HttpService.Get($"{ApiUrl}Task/UpdateTaskCollection?taskId={item.TaskCode}&collectionStatus={status}&taskType=userTask&isHistory=false", authorization);
                    await HttpService.Get($"{ApiUrl}Task/UpdateTaskCollection?taskId={item.TaskCode}&collectionStatus={status}&taskType=runingTask&isHistory=false", authorization);
                    await HttpService.Get($"{ApiUrl}Task/UpdateTaskCollection?taskId={item.TaskCode}&collectionStatus={status}&taskType=completedTask&isHistory=false", authorization);
                }

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success."
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 催待办
        /// <summary>
        /// 催待办
        /// </summary>
        [HttpGet]
        public IHttpActionResult Urge1(string resourceId, string processId, string type)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                // 获取待办列表
                var tasks = _userTaskService.GetUserTask(resourceId, processId);
                if (tasks.Count < 1)
                {
                    throw new Exception("未找到被催办人信息！");
                }
                // 获取被催办人信息
                var userCodeList = new List<string>();
                tasks.ForEach(item =>
                {
                    userCodeList.Add(item.SendToUser);
                });
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCodeList.ToArray());

                //10分钟之内无法重复催办
                var recordColl = UrgeRecordAdapter.Instance.Load(m =>
                {
                    m.AppendItem("ResourceID", resourceId);
                    m.AppendItem("ProcessID", processId);
                    m.AppendItem("ActivityID", tasks.First().ActivityID);
                    m.AppendItem("Category", type);
                    m.AppendItem("UserCode", user.Id);
                }).OrderByDescending(o => o.CreateTime);
                if (recordColl.Count() > 0)
                {
                    TimeSpan timeSpan = DateTime.Now - recordColl.First().CreateTime;
                    if (timeSpan.TotalMinutes < 10) { throw new Exception("10分钟内无法重复发送"); }
                }

                // 邮件
                if (type == "email")
                {
                    // 邮件内容
                    var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/task_urge_email.html");
                    var streamReader = new StreamReader(fileStream);
                    var template = streamReader.ReadToEnd();

                    //var service = MailService._ExchangeService();
                    var sended = 0;
                    tasks.ForEach(task =>
                    {
                        var _user = users.Find(u => u.ID == task.SendToUser);
                        if (_user != null && !string.IsNullOrWhiteSpace(_user.Email))
                        {
                            var mail = SeagullMailService.GetInstance();
                            var body = template;
                            body = body.Replace("{user-name}", _user.DisplayName);
                            body = body.Replace("{task-title}", task.TaskTitle);
                            body = body.Replace("{task-url}", task.Url);
                            mail.AddSubject("待办催办提醒");
                            mail.AddBody(body, true);
                            mail.AddTo(new Dictionary<string, string>() { { _user.Email, _user.DisplayName } });
                            mail.Send();
                            sended++;
                        }
                    });
                    if (sended < 1)
                    {
                        throw new Exception("没有找到被催办人邮箱！");
                    }
                }
                // 系统提醒
                else if (type == "message")
                {
                    tasks.ForEach(task =>
                    {
                        var _user = users.Find(u => u.ID == task.SendToUser);
                        if (_user != null)
                        {
                            MessageAdapter.Instance.Update(new Models.Message.MessageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                MeetingCode = task.TaskID,
                                MessageContent = $"您有一条标题为“{task.TaskTitle}”的待办未处理，请尽快处理。",
                                MessageStatusCode = EnumMessageStatus.New,
                                MessageTypeCode = "2",
                                MessageTitleCode = EnumMessageTitle.System,
                                ModuleType = "Task",
                                Creator = user.Id,
                                CreatorName = user.DisplayName,
                                ReceivePersonCode = _user.ID,
                                ReceivePersonName = _user.DisplayName,
                                ValidStatus = true,
                                CreateTime = DateTime.Now
                            });
                        }
                    });
                }
                else
                {
                    throw new Exception("消息提醒类型错误！");
                }
                // 保存催办记录
                tasks.ForEach(task =>
                {
                    UrgeRecordAdapter.Instance.Update(new Models.UrgeRecord.UrgeRecordModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        ResourceID = resourceId,
                        ProcessID = processId,
                        ActivityID = task.ActivityID,
                        Category = type,
                        UserCode = user.Id,
                        UserName = user.DisplayName,
                        CreateTime = DateTime.Now,
                        Creator = user.Id,
                        ValidStatus = true
                    });
                });
            });
            return Ok(result);
        }
        /// <summary>
        /// 催待办
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> Urge(string resourceId, string processId, string type)
        {
            try
            {
                //ServiceUtility.SetCertificatePolicy(); GetUserTaskByResourceId
                string authorization = await HttpService.GetToken();
                var user = (Seagull2Identity)User.Identity;
                log.Info("----------获取获取待办列表开始----------");
                Stopwatch sw = new Stopwatch();
                var json = await HttpService.Get($"{ApiUrl}api/UserTask?resourceId={resourceId}", authorization);
                var tasksSeagull = Newtonsoft.Json.JsonConvert.DeserializeObject<List<GetUserTaskInfoModel>>(json);
                var view = new GetUserTaskModel
                {
                    Data = tasksSeagull
                };
                var tasks = view.ToUserTaskList();
                sw.Stop();
                log.Info("----------获取获取待办列表结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");
                if (tasks.Count < 1)
                {
                    throw new Exception("未找到被催办人信息！");
                }
                // 获取被催办人信息
                var userCodeList = new List<string>();
                tasks.ForEach(item =>
                {
                    userCodeList.Add(item.SendToUser);
                });
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCodeList.ToArray());

                //10分钟之内无法重复催办
                var recordColl = UrgeRecordAdapter.Instance.Load(m =>
                {
                    m.AppendItem("ResourceID", resourceId);
                    m.AppendItem("ProcessID", processId);
                    m.AppendItem("ActivityID", tasks.First().ActivityID);
                    m.AppendItem("Category", type);
                    m.AppendItem("UserCode", user.Id);
                }).OrderByDescending(o => o.CreateTime);
                if (recordColl.Count() > 0)
                {
                    TimeSpan timeSpan = DateTime.Now - recordColl.First().CreateTime;
                    if (timeSpan.TotalMinutes < 10) { throw new Exception("10分钟内无法重复发送"); }
                }

                // 发送
                switch (type)
                {
                    // 邮件
                    case "email":
                        {
                            var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/task_urge_email.html");
                            var streamReader = new StreamReader(fileStream);
                            var template = streamReader.ReadToEnd();

                            var sended = 0;
                            tasks.ForEach(task =>
                            {
                                var _user = users.Find(u => u.ID == task.SendToUser);
                                if (_user != null && !string.IsNullOrWhiteSpace(_user.Email))
                                {
                                    var mail = SeagullMailService.GetInstance();
                                    var body = template;
                                    body = body.Replace("{user-name}", _user.DisplayName);
                                    body = body.Replace("{task-title}", task.TaskTitle);
                                    body = body.Replace("{task-url}", task.Url);
                                    mail.AddSubject("待办催办提醒");
                                    mail.AddBody(body, true);
                                    mail.AddTo(new Dictionary<string, string>() { { _user.Email, _user.DisplayName } });
                                    mail.Send();
                                    sended++;
                                }
                            });
                            if (sended < 1)
                            {
                                throw new Exception("没有找到被催办人邮箱！");
                            }

                            break;
                        }
                    // 系统提醒
                    case "message":
                        {
                            tasks.ForEach(task =>
                            {
                                var _user = users.Find(u => u.ID == task.SendToUser);
                                if (_user != null)
                                {
                                    MessageAdapter.Instance.Update(new Models.Message.MessageModel()
                                    {
                                        Code = Guid.NewGuid().ToString(),
                                        MeetingCode = task.TaskID,
                                        MessageContent = $"您有一条标题为“{task.TaskTitle}”的待办未处理，请尽快处理。",
                                        MessageStatusCode = EnumMessageStatus.New,
                                        MessageTypeCode = "2",
                                        MessageTitleCode = EnumMessageTitle.System,
                                        ModuleType = "Task",
                                        Creator = user.Id,
                                        CreatorName = user.DisplayName,
                                        ReceivePersonCode = _user.ID,
                                        ReceivePersonName = _user.DisplayName,
                                        ValidStatus = true,
                                        CreateTime = DateTime.Now
                                    });
                                }
                            });
                            break;
                        }
                    case "sms":
                        {
                            PostSMSForDHSTSoapClient client = new PostSMSForDHSTSoapClient();
                            tasks.ForEach(task =>
                            {
                                var _user = users.Find(u => u.ID == task.SendToUser);
                                if (_user != null && !string.IsNullOrWhiteSpace(_user.Properties["MP"].ToString()))
                                {
                                    string mobile = _user.Properties["MP"].ToString();
                                    string content = "您好，您有一条“" + task.TaskTitle + "”的待办需要处理，请尽快处理。";
                                    string result = client.SendVerificationCode(mobile, content);
                                }
                            });
                            break;
                        }
                    default: throw new Exception("消息提醒类型错误！");
                }

                // 保存催办记录
                tasks.ForEach(task =>
                {
                    UrgeRecordAdapter.Instance.Update(new Models.UrgeRecord.UrgeRecordModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        ResourceID = resourceId,
                        ProcessID = processId,
                        ActivityID = task.ActivityID,
                        Category = type,
                        UserCode = user.Id,
                        UserName = user.DisplayName,
                        CreateTime = DateTime.Now,
                        Creator = user.Id,
                        ValidStatus = true
                    });
                });

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success."
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }


        #endregion

        #region 获取待办详情
        /// <summary>
        /// 获取待办详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDetail1(string taskId)
        {
            var result = ControllerService.Run(() =>
            {
                var model = _userTaskService.GetDetail(taskId);
                if (model == null)
                {
                    throw new Exception("该待办不存在或已办结！");
                }
                return model;
            });
            return Ok(result);
        }
        /// <summary>
        /// 获取待办详情
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> GetDetail(string taskId)
        {
            try
            {

                //ServiceUtility.SetCertificatePolicy();
                string authorization = await HttpService.GetToken();
                var user = (Seagull2Identity)User.Identity;
                var json = await HttpService.Get($"{ApiUrl}api/UserTask?taskId={taskId}&category=1", authorization);
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<GetUserTaskInfoModel>(json);
                var view = new GetUserTaskModel
                {
                    Data = new List<GetUserTaskInfoModel> {
                        data
                    }
                };
                var list = view.ToUserTaskList();

                if (list.Count > 0)
                {
                    return Ok(new ViewsModel.BaseView
                    {
                        State = true,
                        Message = "success.",
                        Data = list.FirstOrDefault()
                    });
                }
                else
                {
                    return Ok(new ViewsModel.BaseView
                    {
                        State = false,
                        Message = "该待办不存在或已办结！"
                    });
                }
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 待办和传阅已读未读
        /// <summary>
        /// 待办和传阅已读未读
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> SetTaskReadFlag1()
        {
            //流程ID
            string taskID = HttpContext.Current.Request["taskID"];
            return Ok(await _userTaskService.SetTaskReadFlag(taskID));
        }
        /// <summary>
        /// 待办和传阅已读未读
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> SetTaskReadFlag()
        {
            try
            {
                string taskID = HttpContext.Current.Request["taskID"];

                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                await HttpService.Get($"{ApiUrl}Task/UpdateTaskReadTime?taskId={taskID}", authorization);

                return Ok("Yes");
            }
            catch
            {
                return Ok("");
            }
        }
        #endregion

        #region 流转中和已办结已读未读
        /// <summary>
        /// 流转中和已办结已读未读
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> SetAccomplihd1()
        {
            //流程ID
            string taskID = HttpContext.Current.Request["taskID"];
            return Ok(await _userTaskService.SetAccomplihd(taskID));
        }
        /// <summary>
        /// 流转中和已办结已读未读
        /// </summary>
        [HttpGet]
        public async Task<IHttpActionResult> SetAccomplihd()
        {
            try
            {
                string taskID = HttpContext.Current.Request["taskID"];

                var authorization = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                await HttpService.Get($"{ApiUrl}Task/UpdateTaskReadTime?taskId={taskID}", authorization);

                return Ok("Yes");
            }
            catch
            {
                return Ok("");
            }
        }
        #endregion
    }
}
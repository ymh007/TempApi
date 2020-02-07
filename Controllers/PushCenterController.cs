using log4net;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Enum;
using SinoOcean.Seagull2.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 推送中心控制器
    /// </summary>
    public class PushCenterController : ApiController
    {
        /// <summary>
        /// 日志存储
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 新闻中心推送
        /// <summary>
        /// 新闻中心推送
        /// </summary>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult PushNews(PushNewsPost news)
        {
            try
            {
                //获取新闻Guid
                string guid = string.Empty;
                string url = System.Web.HttpUtility.UrlDecode(news.url);
                Match match = new Regex("(?:.{0,})sinooceangroup(?:.{0,})?id=(.+)").Match(url);
                if (!match.Success)
                {
                    log.Error("新闻中心推送失败：无法获取到新闻Guid。[" + news.ToString() + "]");
                    return Ok(new ViewsModel.ViewModelBaseNull() { State = false, Message = "无法获取到新闻Guid。" });
                }
                guid = match.Groups[1].Value;

                //推送消息
                var model = new PushService.Model()
                {
                    BusinessDesc = "新闻中心推送",
                    Title = "您收到一条新闻",
                    Content = news.title,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "newsDetailsPage",
                        bags = guid
                    },
                    SendType = PushService.SendType.All
                };
                bool isPush = PushService.Push(model, out string pushResult);
                if (isPush)
                {
                    return Ok(new ViewsModel.ViewModelBaseNull() { State = true, Message = "推送成功。" });
                }
                else
                {
                    return Ok(new ViewsModel.ViewModelBaseNull() { State = false, Message = "推送失败：" + pushResult });
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                return Ok(new ViewsModel.ViewModelBaseNull() { State = false, Message = e.Message });
            }
        }
        /// <summary>
        /// 新闻中心推送POST参数类
        /// </summary>
        public class PushNewsPost
        {
            /// <summary>
            /// 新闻标题
            /// </summary>
            public string title { get; set; }
            /// <summary>
            /// 新闻Url
            /// </summary>
            public string url { get; set; }
            /// <summary>
            /// 将类转换成Json
            /// </summary>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
        #endregion

        #region 预订会议室推送
        /// <summary>
        /// 预订会议室推送
        /// </summary>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult PushMeeting(PushMeetingPost meeting)
        {

            try
            {
                //查询预定信息
                MeetingServiceContract _MeetingServiceContract = MeetingService.GetByCode(meeting.meetingCode);
                if (_MeetingServiceContract == null)
                {
                    return Ok(new ViewsModel.ViewModelBaseNull { State = false, Message = "推送失败，没有获取到会议相关信息！" });
                }

                //推送人员
                var userList = MeetingService.GetPeople(_MeetingServiceContract);

                //推送
                var model = new PushService.Model()
                {
                    BusinessDesc = "预订会议室",
                    Title = _MeetingServiceContract.Subject,
                    Content = _MeetingServiceContract.MeetingTopic,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "meetingReminder",
                        bags = ""
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", userList)
                };
                switch (meeting.meetingState)
                {
                    case MeetingState.Add:
                        {
                            model.Title = "会议通知";
                            model.Content = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                                        _MeetingServiceContract.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                        _MeetingServiceContract.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                        _MeetingServiceContract.MeetingRoomLocation,
                                                        _MeetingServiceContract.Subject);
                            string result;
                            bool isPush = PushService.Push(model, out result);
                            if (isPush)
                            {
                                return Ok(new ViewsModel.ViewModelBaseNull { State = true, Message = "推送成功！" });
                            }
                            else
                            {
                                return Ok(new ViewsModel.ViewModelBaseNull { State = false, Message = "推送失败：" + result });
                            }
                        }
                    case MeetingState.Cancel:
                        {
                            model.BusinessDesc = "会议取消";
                            model.Title = "会议取消通知：" + _MeetingServiceContract.Subject;
                            string result;
                            bool isPush = PushService.Push(model, out result);
                            if (isPush)
                            {
                                return Ok(new ViewsModel.ViewModelBaseNull { State = true, Message = "推送成功！" });
                            }
                            else
                            {
                                return Ok(new ViewsModel.ViewModelBaseNull { State = false, Message = "推送失败：" + result });
                            }
                        }
                    default:
                        {
                            return Ok(new ViewsModel.ViewModelBaseNull { State = false, Message = "推送失败，参数错误！" });
                        }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                return Ok(new ViewsModel.ViewModelBaseNull
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        /// <summary>
        /// 会议室推送类型
        /// </summary>
        public enum MeetingState
        {
            /// <summary>
            /// 预定
            /// </summary>
            Add = 0,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel = 1,
            /// <summary>
            /// 修改
            /// </summary>
            Update = 2
        }
        /// <summary>
        /// 预定 / 取消 会议室推送POST类
        /// </summary>
        public class PushMeetingPost
        {
            /// <summary>
            /// 预定编码
            /// </summary>
            public string meetingCode { get; set; }
            /// <summary>
            /// 推送类型
            /// </summary>
            public MeetingState meetingState { get; set; }
        }
        #endregion

        #region 预订会议室推送 new
        /// <summary>
        /// 预订会议室推送 new
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult PushMeetingNew(PushMeetingNewPost post)
        {
            var result = ControllerService.Run(() =>
            {
                //推送
                var model = new PushService.Model()
                {
                    BusinessDesc = "预定会议室",
                    Title = string.Empty,
                    Content = string.Empty,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "meetingReminder",
                        bags = "",
                        msgType = (int)post.MeetingRoomType == 1 ? "specialMeeting" : "commonMeeting"
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", post.UserCodeList)
                };
                switch (post.MeetingState)
                {
                    case MeetingState.Add:
                        {
                            #region [添加消息提醒]
                            try
                            {
                                MessageForMeetingYuanXin ms = new MessageForMeetingYuanXin();
                                ms.MessageTitleCode = EnumMessageTitle.MeetingRequest;
                                ms.MeetingCode = post.Code;
                                ms.Creator = post.CreatorCode;
                                ms.CreatorName = post.CreatorName;
                                ms.ModuleType = post.MeetingRoomType;
                                var people = new List<MessageForMeetingYuanXinPeople>();
                                foreach (string code in post.UserCodeList)
                                {
                                    people.Add(new MessageForMeetingYuanXinPeople() { UserCode = code });
                                }
                                ms.ReceivePerson = people;
                                ms.OverdueTime = post.StartTime;
                                ms.MessageContent = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                                            post.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                            post.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                            post.MeetingRoomLocation,
                                                            post.Subject);

                                Adapter.Message.MessageAdapter.Instance.AddMessage(ms);
                            }
                            catch (Exception e)
                            {
                                log.Error("发送提醒消息异常：" + e.ToString());
                            }
                            #endregion

                            #region [发推送]
                            model.Title = "会议通知";
                            model.Content = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                                        post.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                        post.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                        post.MeetingRoomLocation,
                                                        post.Subject);
                            model.Extras = new PushService.ModelExtras()
                            {
                                action = "meetingReminder",
                                bags = post.Code,
                                msgType = (int)post.MeetingRoomType == 1 ? "specialMeeting" : "commonMeeting"
                            };
                            bool isPush = PushService.Push(model, out string pushResult);
                            if (!isPush)
                            {
                                throw new Exception("推送失败");
                            }
                            #endregion

                            break;
                        }
                    case MeetingState.Cancel:
                        {
                            #region [添加消息提醒]
                            try
                            {
                                MessageForMeetingYuanXin ms = new MessageForMeetingYuanXin();
                                ms.MessageTitleCode = EnumMessageTitle.CancelMeeting;
                                ms.MeetingCode = post.Code;
                                ms.Creator = post.CreatorCode;
                                ms.CreatorName = post.CreatorName;
                                var people = new List<MessageForMeetingYuanXinPeople>();
                                foreach (string code in post.UserCodeList)
                                {
                                    people.Add(new MessageForMeetingYuanXinPeople() { UserCode = code });
                                }
                                ms.ReceivePerson = people;
                                ms.OverdueTime = post.StartTime;
                                ms.MessageContent = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，已经取消。",
                                                            post.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                            post.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                            post.MeetingRoomLocation,
                                                            post.Subject);

                                Adapter.Message.MessageAdapter.Instance.AddMessage(ms);
                            }
                            catch (Exception e)
                            {
                                log.Error("发送提醒消息异常：" + e.ToString());
                            }
                            #endregion

                            #region [发推送]
                            model.Title = "会议取消通知：" + post.Subject;
                            bool isPush = PushService.Push(model, out string pushResult);
                            if (!isPush)
                            {
                                throw new Exception("推送失败");
                            }
                            #endregion

                            break;
                        }
                    case MeetingState.Update:
                        {
                            try
                            {
                                if (post.AddCodeList.Count > 0)
                                {
                                    PushAddCodeList(post);
                                }
                                if (post.DeleteCodeList.Count > 0)
                                {
                                    PushDeleteCodeList(post);
                                }
                                if (post.InvariableCodeList.Count > 0)
                                {
                                    PushInvariableCodeList(post);
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception("推送失败，错误消息：" + e.ToString());
                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception("推送失败，参数错误！");
                        }
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 对新增的参会人提醒推送
        /// </summary>
        /// <param name="post"></param>
        public void PushAddCodeList(PushMeetingNewPost post)
        {
            try
            {
                #region [添加消息提醒]
                try
                {
                    MessageForMeetingYuanXin ms = new MessageForMeetingYuanXin();
                    ms.MessageTitleCode = EnumMessageTitle.MeetingRequest;
                    ms.MeetingCode = post.Code;
                    ms.Creator = post.CreatorCode;
                    ms.CreatorName = post.CreatorName;
                    ms.ModuleType = post.MeetingRoomType;
                    var people = new List<MessageForMeetingYuanXinPeople>();
                    foreach (string code in post.AddCodeList)
                    {
                        people.Add(new MessageForMeetingYuanXinPeople() { UserCode = code });
                    }
                    ms.ReceivePerson = people;
                    ms.OverdueTime = post.StartTime;
                    ms.MessageContent = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                                post.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                post.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                post.MeetingRoomLocation,
                                                post.Subject);

                    Adapter.Message.MessageAdapter.Instance.AddMessage(ms);
                }
                catch (Exception e)
                {
                    log.Error("发送提醒消息异常：" + e.ToString());
                }
                #endregion

                #region [发推送]
                var model = new PushService.Model()
                {
                    BusinessDesc = "对新增的参会人提醒推送",
                    Title = string.Empty,
                    Content = string.Empty,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "meetingReminder",
                        bags = ""
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", post.AddCodeList)
                };
                model.Title = "会议邀请通知";
                model.Content = string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                            post.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                            post.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                            post.MeetingRoomLocation,
                                            post.Subject);
                model.Extras = new PushService.ModelExtras()
                {
                    action = post.MeetingRoomType == 0 ? "" : post.MeetingRoomType.ToString(),
                    bags = post.Code
                };
                bool isPush = PushService.Push(model, out string pushResult);
                if (!isPush)
                {
                    throw new Exception("推送失败");
                }
                #endregion
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// 对删除的参会人进行提醒推送
        /// </summary>
        /// <param name="post"></param>
        public void PushDeleteCodeList(PushMeetingNewPost post)
        {
            try
            {
                #region [添加消息提醒]
                try
                {
                    MessageForMeetingYuanXin ms = new MessageForMeetingYuanXin();
                    ms.MessageTitleCode = EnumMessageTitle.MeetingUpdate;
                    ms.MeetingCode = post.Code;
                    ms.Creator = post.CreatorCode;
                    ms.CreatorName = post.CreatorName;
                    ms.ModuleType = post.MeetingRoomType;
                    var people = new List<MessageForMeetingYuanXinPeople>();
                    foreach (string code in post.DeleteCodeList)
                    {
                        people.Add(new MessageForMeetingYuanXinPeople() { UserCode = code });
                    }
                    ms.ReceivePerson = people;
                    ms.OverdueTime = post.StartTime;
                    ms.MessageContent = string.Format("主题为“{0}”的会议发生变动，您已不需要参加此次会议。", post.Subject);
                    Adapter.Message.MessageAdapter.Instance.AddMessage(ms);
                }
                catch (Exception e)
                {
                    log.Error("发送提醒消息异常：" + e.ToString());
                }
                #endregion

                #region [发推送]
                var model = new PushService.Model()
                {
                    BusinessDesc = "对删除的参会人进行提醒推送",
                    Title = string.Empty,
                    Content = string.Empty,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "meetingReminder",
                        bags = ""
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", post.UserCodeList)
                };
                model.Title = "会议修改提醒";
                model.Content = string.Format("主题为“{0}”的会议发生变动，您已不需要参加此次会议。", post.Subject);
                model.Extras = new PushService.ModelExtras()
                {
                    action = post.MeetingRoomType == 0 ? "" : post.MeetingRoomType.ToString(),
                    bags = post.Code
                };
                bool isPush = PushService.Push(model, out string pushResult);
                if (!isPush)
                {
                    throw new Exception("推送失败");
                }
                #endregion
            }
            catch (Exception e)
            {

                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// 对不变参会人进行提醒推送
        /// </summary>
        /// <param name="post"></param>
        public void PushInvariableCodeList(PushMeetingNewPost post)
        {
            try
            {
                #region [添加消息提醒]
                try
                {
                    MessageForMeetingYuanXin ms = new MessageForMeetingYuanXin();
                    ms.MessageTitleCode = EnumMessageTitle.MeetingUpdate;
                    ms.MeetingCode = post.Code;
                    ms.Creator = post.CreatorCode;
                    ms.CreatorName = post.CreatorName;
                    ms.ModuleType = post.MeetingRoomType;
                    var people = new List<MessageForMeetingYuanXinPeople>();
                    foreach (string code in post.InvariableCodeList)
                    {
                        people.Add(new MessageForMeetingYuanXinPeople() { UserCode = code });
                    }
                    ms.ReceivePerson = people;
                    ms.OverdueTime = post.StartTime;
                    ms.MessageContent = string.Format("{0}对“{1}”进行了修改。", post.CreatorName, post.Subject);
                    Adapter.Message.MessageAdapter.Instance.AddMessage(ms);
                }
                catch (Exception e)
                {
                    log.Error("发送提醒消息异常：" + e.ToString());
                }
                #endregion

                #region [发推送]
                var model = new PushService.Model()
                {
                    BusinessDesc = "预订会议室",
                    Title = string.Empty,
                    Content = string.Empty,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "meetingReminder",
                        bags = ""
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", post.InvariableCodeList)
                };
                model.Title = "会议修改通知";
                model.Content = string.Format("{0}对“{1}”进行了修改。", post.CreatorName, post.Subject);
                model.Extras = new PushService.ModelExtras()
                {
                    action = post.MeetingRoomType == 0 ? "" : post.MeetingRoomType.ToString(),
                    bags = post.Code
                };
                bool isPush = PushService.Push(model, out string pushResult);
                if (!isPush)
                {
                    throw new Exception("推送失败");
                }
                #endregion
            }
            catch (Exception e)
            {

                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// 预定 / 取消 会议室推送POST类
        /// </summary>
        public class PushMeetingNewPost
        {
            /// <summary>
            /// 推送类型
            /// </summary>
            public MeetingState MeetingState { get; set; }
            /// <summary>
            /// 会议编码
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 是否高管会议
            /// </summary>
            public EnumMessageModuleType MeetingRoomType { get; set; }
            /// <summary>
            /// 会议主题
            /// </summary>
            public string Subject { set; get; }
            /// <summary>
            /// 会议开始时间
            /// </summary>
            public DateTime StartTime { set; get; }
            /// <summary>
            /// 会议结束时间
            /// </summary>
            public DateTime EndTime { set; get; }
            /// <summary>
            /// 会议室地址
            /// </summary>
            public string MeetingRoomLocation { set; get; }
            /// <summary>
            /// 与会议有关的所有用户编码
            /// </summary>
            public List<string> UserCodeList { set; get; }

            /// <summary>
            /// 新增的参会人编码
            /// </summary>
            public List<string> AddCodeList { set; get; }

            /// <summary>
            /// 删除的参会人编码
            /// </summary>
            public List<string> DeleteCodeList { set; get; }

            /// <summary>
            /// 不变的参会人编码
            /// </summary>
            public List<string> InvariableCodeList { get; set; }

            /// <summary>
            /// 创建人编码
            /// </summary>
            public string CreatorCode { get; set; }
            /// <summary>
            /// 创建人名称
            /// </summary>
            public string CreatorName { get; set; }
        }
        #endregion

        #region 注册制推送
        /// <summary>
        /// 注册制推送
        /// </summary>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult PushRegister(PushRegisterPost data)
        {
            try
            {
                var model = new PushService.Model()
                {
                    BusinessDesc = "移动端后台管理",
                    Title = data.Title,
                    Content = data.Content,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "MessageDetail",
                        bags = ""
                    },
                    SendType = PushService.SendType.All,
                    Ids = ""
                };

                string result;
                bool isPush = PushService.Push(model, out result);

                log.Info("注册制推送服务器返回信息：" + result);

                if (isPush)
                {
                    return Ok(new ViewsModel.ViewModelBaseNull { State = true, Message = "推送成功！" });
                }
                else
                {
                    return Ok(new ViewsModel.ViewModelBaseNull { State = false, Message = "推送失败！" });
                }
            }
            catch (Exception e)
            {
                log.Error("注册制推送失败：" + e.ToString());
                return Ok(new ViewsModel.ViewModelBaseNull { State = false, Message = e.Message });
            }
        }
        /// <summary>
        /// 注册制推送POST参数类
        /// </summary>
        public class PushRegisterPost
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { set; get; }
        }
        #endregion

        #region 给指定用户发送推送通用接口
        /// <summary>
        /// 给指定用户发送推送通用接口
        /// </summary>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult PushToUsers(PushToUsersPost post)
        {
            var result = ControllerService.Run(() =>
            {
                // 构建推送消息体
                var model = new PushService.Model()
                {
                    BusinessDesc = "移动端后台管理",
                    Title = post.Title,
                    Content = post.Content,
                    Extras = new PushService.ModelExtras()
                    {
                        action = string.Empty,
                        bags = post.Parameter
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", post.UserCodeList)
                };
                switch (post.Type)
                {
                    case EnumPushType.CustomerManage:
                        {
                            model.Extras.action = "clientManageHome";
                            break;
                        }
                    default:
                        {
                            throw new Exception("推送类别错误！");
                        }
                }

                // 执行推送
                bool isPush = PushService.Push(model, out string pushResult);
                if (!isPush)
                {
                    throw new Exception("推送失败：" + pushResult);
                }
            });
            return Ok(result);
        }
        /// <summary>
        /// 给指定用户发送推送通用接口Post类
        /// </summary>
        public class PushToUsersPost
        {
            /// <summary>
            /// 推送类型
            /// </summary>
            public EnumPushType Type { set; get; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
            /// <summary>
            /// 接收人code列表
            /// </summary>
            public List<string> UserCodeList { set; get; }
            /// <summary>
            /// 自定义参数
            /// </summary>
            public string Parameter { set; get; }
        }
        /// <summary>
        /// 推送类别
        /// </summary>
        public enum EnumPushType
        {
            /// <summary>
            /// 客户管理推送
            /// </summary>
            CustomerManage
        }
        #endregion

        #region 给指定用户发推送和系统消息通用接口
        /// <summary>
        /// 给指定用户发推送和系统消息通用接口
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult PushMessageToUsers(PushMessageToUsersPost post)
        {
            var result = ControllerService.Run(() =>
            {
                if (post.Type != 0 && post.Type != 1 && post.Type != 2)
                {
                    throw new Exception("类型参数错误！");
                }

                var reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                var userCodeList = new List<string>();
                var logonNameList = new List<string>();
                post.UserCodeList.ForEach(item =>
                {
                    if (reg.IsMatch(item))
                    {
                        userCodeList.Add(item);
                    }
                    else
                    {
                        logonNameList.Add(item);
                    }
                });

                var users = new List<Models.AddressBook.ContactsModel>();
                users.AddRange(Adapter.AddressBook.ContactsAdapter.Instance.LoadListByUserCodeList(userCodeList).Where(w => w.SchemaType == "Users").ToList());
                users.AddRange(Adapter.AddressBook.ContactsAdapter.Instance.LoadListByLogonNameList(logonNameList).Where(w => w.SchemaType == "Users").ToList());
                users = users.Where((x, i) => users.FindIndex(z => z.ObjectID == x.ObjectID) == i).ToList();
                if (users.Count < 1)
                {
                    throw new Exception("没有有效的人员！");
                }

                switch (post.Type)
                {
                    case 0:
                        {
                            Message(post.Message, users);
                            Push(post.Push, users);
                            break;
                        }
                    case 1:
                        {
                            Message(post.Message, users);
                            break;
                        }
                    case 2:
                        {
                            Push(post.Push, users);
                            break;
                        }
                }
            });
            return Ok(result);
        }


        /// <summary>
        /// 给指定用户发推送
        /// </summary>
        [HttpPost]
        public IHttpActionResult PushMessage(PushMessageToUsersPost post)
        {
            var result = ControllerService.Run(() =>
            {
                if (post.Type != 0 && post.Type != 1 && post.Type != 2)
                {
                    throw new Exception("类型参数错误！");
                }
                if (post.Users.Count < 1)
                {
                    throw new Exception("推送人员不能为空！");
                }
                List<string> pushIDs = post.Users.Select(s => s.id).ToList();
                switch (post.Type)
                {
                    case 0:
                        {
                            Message(post.Message, post.Users);
                            Push(post.Push, pushIDs);
                            break;
                        }
                    case 1:
                        {
                            Message(post.Message, post.Users);
                            break;
                        }
                    case 2:
                        {
                            Push(post.Push, pushIDs);
                            break;
                        }
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 给指定用户发推送和系统消息通用接口 仅限移动办公内部服务调用不对外开放
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult PushMessageToUsers_Server(PushMessageToUsersPost post)
        {
            var result = ControllerService.Run(() =>
            {
                if (post.Type != 0 && post.Type != 1 && post.Type != 2)
                {
                    throw new Exception("类型参数错误！");
                }

                var reg = new Regex("^[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}$", RegexOptions.IgnoreCase);
                var userCodeList = new List<string>();
                var logonNameList = new List<string>();
                post.UserCodeList.ForEach(item =>
                {
                    if (reg.IsMatch(item))
                    {
                        userCodeList.Add(item);
                    }
                    else
                    {
                        logonNameList.Add(item);
                    }
                });

                var users = new List<Models.AddressBook.ContactsModel>();
                users.AddRange(Adapter.AddressBook.ContactsAdapter.Instance.LoadListByUserCodeList(userCodeList).Where(w => w.SchemaType == "Users").ToList());
                users.AddRange(Adapter.AddressBook.ContactsAdapter.Instance.LoadListByLogonNameList(logonNameList).Where(w => w.SchemaType == "Users").ToList());
                users = users.Where((x, i) => users.FindIndex(z => z.ObjectID == x.ObjectID) == i).ToList();
                if (users.Count < 1)
                {
                    throw new Exception("没有有效的人员！");
                }

                switch (post.Type)
                {
                    case 0:
                        {
                            Message(post.Message, users);
                            Push(post.Push, users);
                            break;
                        }
                    case 1:
                        {
                            Message(post.Message, users);
                            break;
                        }
                    case 2:
                        {
                            Push(post.Push, users);
                            break;
                        }
                }
            });
            return Ok(result);
        }


        /// <summary>
        /// 给指定用户发送推送和系统消息通用接口 - Post类
        /// </summary>
        public class PushMessageToUsersPost
        {
            /// <summary>
            /// 类型（0=系统消息+推送、1=系统消息、2=推送）
            /// </summary>
            public int Type { set; get; }
            /// <summary>
            /// 接收人列表（人员编码或域帐号）
            /// </summary>
            public List<string> UserCodeList { set; get; }
            /// <summary>
            /// 接收人
            /// </summary>
            public List<UserInfo> Users { get; set; }
            /// <summary>
            /// 消息提醒配置
            /// </summary>
            public PushMessageToUsersPostForMessage Message { set; get; }
            /// <summary>
            /// 推送配置
            /// </summary>
            public PushMessageToUsersPostForPush Push { set; get; }
        }
        /// <summary>
        /// 给指定用户发送推送和系统消息通用接口 - Post类 - 消息提醒
        /// </summary>
        public class PushMessageToUsersPostForMessage
        {
            /// <summary>
            /// 消息所属编码（通过该编码能查询到对应的信息）
            /// </summary>
            public string MeetingCode { set; get; }
            /// <summary>
            /// 消息内容
            /// </summary>
            public string MessageContent { set; get; }
            /// <summary>
            /// 消息类型（0签到、1会议、2系统、3会务、4计划管理...）
            /// </summary>
            public string MessageTypeCode { set; get; }
            /// <summary>
            /// 消息标题编码（会议开始提醒、会议邀请通知...）
            /// </summary>
            public EnumMessageTitle MessageTitleCode { get; set; }
            /// <summary>
            /// 模块类型
            /// </summary>
            public string ModuleType { set; get; }
            /// <summary>
            /// 发送人编码
            /// </summary>
            public string Creator { set; get; }
            /// <summary>
            /// 发送人名称
            /// </summary>
            public string CreatorName { set; get; }
        }
        /// <summary>
        /// 给指定用户发送推送和系统消息通用接口 - Post类 - 推送
        /// </summary>
        public class PushMessageToUsersPostForPush
        {
            /// <summary>
            /// 调用方，平台id  默认移动办公平台
            /// </summary>
            public string PlatId { get; set; }
            /// <summary>
            /// 平台业务描述
            /// </summary>
            public string BusinessDesc { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { set; get; }
            /// <summary>
            /// Action
            /// </summary>
            public string Action { set; get; }
            /// <summary>
            /// Paramet
            /// </summary>
            public string Parameter { set; get; }

            /// <summary>
            /// 消息类型跳转的模块标识
            /// </summary>
            public string MsgType { get; set; }
        }
        /// <summary>
        /// 人员信息
        /// </summary>
        public class UserInfo
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        /// <summary>
        /// 批量插入系统提醒数据
        /// </summary>
        void Message(PushMessageToUsersPostForMessage setting, List<Models.AddressBook.ContactsModel> users)
        {
            var models = new Models.Message.MessageCollection();
            users.ForEach(user =>
            {
                models.Add(new Models.Message.MessageModel
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = setting.MeetingCode,
                    MessageContent = setting.MessageContent,
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = setting.MessageTypeCode,
                    MessageTitleCode = setting.MessageTitleCode,
                    ModuleType = setting.ModuleType,
                    Creator = setting.Creator,
                    CreatorName = setting.CreatorName,
                    ReceivePersonCode = user.ObjectID,
                    ReceivePersonName = user.DisplayName,
                    ReceivePersonMeetingTypeCode = string.Empty,
                    OverdueTime = DateTime.Now.AddDays(7),
                    ValidStatus = true,
                    CreateTime = DateTime.Now,
                });
            });
            Adapter.Message.MessageAdapter.Instance.MessageBatchInsert(models);
        }

        void Message(PushMessageToUsersPostForMessage setting, List<UserInfo> users)
        {
            var models = new Models.Message.MessageCollection();
            users.ForEach(user =>
            {
                models.Add(new Models.Message.MessageModel
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = setting.MeetingCode,
                    MessageContent = setting.MessageContent,
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = setting.MessageTypeCode,
                    MessageTitleCode = setting.MessageTitleCode,
                    ModuleType = setting.ModuleType,
                    Creator = setting.Creator,
                    CreatorName = setting.CreatorName,
                    ReceivePersonCode = user.id,
                    ReceivePersonName = user.name,
                    ReceivePersonMeetingTypeCode = string.Empty,
                    OverdueTime = DateTime.Now.AddDays(2),
                    ValidStatus = true,
                    CreateTime = DateTime.Now,
                });
            });
            Adapter.Message.MessageAdapter.Instance.MessageBatchInsert(models);
        }
        /// <summary>
        /// 发推送
        /// </summary>
        void Push(PushMessageToUsersPostForPush setting, List<Models.AddressBook.ContactsModel> users)
        {
            var model = new PushService.Model()
            {
                BusinessDesc = "移动端后台管理",
                Title = setting.Title,
                Content = setting.Content,
                Extras = new PushService.ModelExtras()
                {
                    action = setting.Action,
                    bags = setting.Parameter
                },
                SendType = PushService.SendType.Person,
                Ids = string.Join(",", users.Select(s => s.ObjectID)),
            };
            if (setting.Action == "mobileofficemail")
            {
                model.Title = "你收到一封新的邮件,请及时查收！";
            }
            PushService.Push(model, out string pushResult);
        }
        class bagsMode
        {
            public mailMode bags { get; set; }
        }
        class mailMode
        {
            public string mailId { get; set; }
        }
        void Push(PushMessageToUsersPostForPush setting, List<string> users)
        {
            var model = new PushService.Model()
            {
                PlatId = setting.PlatId,
                BusinessDesc = setting.BusinessDesc,
                Title = setting.Title,
                Content = setting.Content,
                Extras = new PushService.ModelExtras()
                {
                    action = setting.Action,
                    bags = setting.Parameter,
                    msgType = setting.MsgType
                },
                SendType = PushService.SendType.Person,
                Ids = string.Join(",", users),
            };
            PushService.Push(model, out string pushResult);
        }
        #endregion


        #region 推送设置保存 获取

        /// <summary>
        /// 获取某一个人的所有模块设置 包括总开关
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAllModuleSetting()
        {
            var result = ControllerService.Run(() =>
            {
                string userid = ((Seagull2Identity)User.Identity).Id;
                SqlDbHelper dbContext = new SqlDbHelper();
                string modulesStr = System.Configuration.ConfigurationManager.AppSettings["AppModuleFlag"];
                string sql = "SELECT * FROM YuanXinBusiness.push.PushSettings WHERE UserId='" + userid + "'";
                var settings = dbContext.ExecuteDataTable(sql);
                if (settings != null)
                {
                    DataTable addDt = new DataTable();
                    addDt.Columns.Add("Switch", System.Type.GetType("System.Int32"));
                    addDt.Columns.Add("UserId", System.Type.GetType("System.String"));
                    addDt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
                    addDt.Columns.Add("PushModule", System.Type.GetType("System.String"));
                    addDt.Columns.Add("ModuleSwitch", System.Type.GetType("System.Int32"));
                    modulesStr.Split(',').ToList().ForEach(f =>
                    {
                        if (!string.IsNullOrEmpty(f))
                        {
                            var row = settings.Select("[PushModule]='" + f + "'");
                            if (f == "all")
                                row = settings.Select("[PushModule] IS NULL ");
                            if (row.Length < 1)
                            {
                                DataRow add_dr = addDt.NewRow();
                                add_dr["Switch"] = 1;
                                add_dr["UserId"] = userid;
                                add_dr["CreateTime"] = DateTime.Now;
                                add_dr["PushModule"] = f == "all" ? null : f;
                                add_dr["ModuleSwitch"] = f == "meetingReminder" ? 2 : 1;
                                addDt.Rows.Add(add_dr);
                            }
                        }
                    });
                    if (addDt.Rows.Count > 0)
                    {
                        SqlDbHelper.BulkInsertData(addDt, "push.PushSettings", Models.ConnectionNameDefine.YuanXinBusiness);
                    }
                    settings = dbContext.ExecuteDataTable(sql);
                }
                return settings;
            });
            return this.Ok(result);
        }

        /// <summary>
        /// 保存模块推送设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult SavePushModuleSetting(string id, int mSwitch)
        {
            var result = ControllerService.Run(() =>
            {
                SqlDbHelper dbContext = new SqlDbHelper();
                string sql = $"UPDATE YuanXinBusiness.push.PushSettings SET switch={mSwitch}, ModuleSwitch={mSwitch} ,UpdateTime='{DateTime.Now}' where id= " + id;
                int rows = dbContext.ExecuteNonQuery(sql);
                return rows > 0 ? mSwitch == 1 ? "已开启" : "已关闭" : "开启失败";
            });
            return this.Ok(result);
        }

        /// <summary>
        /// 推送设置
        /// </summary>
        public class PushSetting
        {
            public int Id { get; set; }

            public string UserId { get; set; }

            public int Switch { get; set; }
            public string PushModule { get; set; }

            public int ModuleSwitch { get; set; }

        }
        #endregion

    }
}
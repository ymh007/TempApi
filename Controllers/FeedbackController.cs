using MCS.Library.Data;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Feedback;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.Feedback;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.ViewsModel.Feedback;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 意见反馈 Controller
    /// </summary>
    public class FeedbackController : ApiController
    {
        /*
            意见反馈邮件和推送接收人信息配置
        */
        static Dictionary<string, IUser> ReceiveUser = new Dictionary<string, IUser>();
        List<IUser> GetReceiveUser()
        {
            if (ReceiveUser.Count < 1)
            {
                var config = ConfigurationManager.AppSettings["FeedbackReceiveUsers"];
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, config.Split(','));
                foreach (var user in users)
                {
                    if (!ReceiveUser.ContainsKey(user.LogOnName))
                    {
                        ReceiveUser.Add(user.LogOnName, user);
                    }
                }
            }

            return ReceiveUser.Select(s => s.Value).ToList();
        }

        #region 发表意见反馈
        /// <summary>
        /// 发表意见反馈
        /// </summary>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult Add(AddPost post)
        {
            try
            {
                // 根据域帐号获取用户信息
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, post.Account);
                if (users.Count < 1)
                {
                    return Ok(new ViewsModel.BaseView()
                    {
                        State = false,
                        Message = "域帐号错误！"
                    });
                }
                var user = users[0];
                var resourceId = Guid.NewGuid().ToString();
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    // 意见反馈基本信息
                    FeedbackAdapter.Instance.Update(new FeedbackModel()
                    {
                        Code = resourceId,
                        System = post.System,
                        Network = post.Network,
                        Version = post.Version,
                        AppVersion = string.IsNullOrEmpty(post.AppVersion) ? "" : post.AppVersion,
                        Content = post.Content,
                        UserCode = user.ID,
                        UserName = user.DisplayName,
                        IsReply = false,
                        Creator = user.ID,
                        ReplyWay = 1,
                        CreateTime = DateTime.Now,
                        ReplyDateTime = DateTime.Now,
                        ValidStatus = true

                    });
                    // 意见反馈图片
                    post.ImageList.ForEach(imgUrl =>
                    {
                        FeedbackImageAdapter.Instance.Update(new FeedbackImageModel()
                        {
                            Code = Guid.NewGuid().ToString(),
                            FeedbackCode = resourceId,
                            ImageUrl = imgUrl.Split('=').Length > 1 ? imgUrl.Split('=')[1] : imgUrl,
                            Creator = user.ID,
                            CreateTime = DateTime.Now,
                            ValidStatus = true
                        });
                    });
                    //事务提交
                    scope.Complete();
                }
                SendNotify(post, user, resourceId);
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

        /// <summary>
        /// app 提交意见反馈的消息提醒
        /// </summary>
        /// <param name="post"></param>
        /// <param name="user"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public void SendNotify(AddPost post, IUser user, string resourceId)
        {
            // 获取接收人信息
            var receiveUsers = GetReceiveUser();
            if (receiveUsers.Count > 1)
            {
                // 发送邮件提醒
                var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/feedback_email.html");
                var streamReader = new StreamReader(fileStream);
                var template = streamReader.ReadToEnd();

                var mail = SeagullMailService.GetInstance();
                var body = template;
                body = body.Replace("{feedback-user-name}", user.DisplayName);
                body = body.Replace("{feedback-content}", post.Content);
                var imagesHtml = string.Empty;
                post.ImageList.ForEach(item =>
                {
                    imagesHtml += $"<p style=\"margin-bottom:10px;\"><img src=\"{item}\" style=\"max-width:800px;\" /></p>";
                });
                body = body.Replace("{feedback-images}", imagesHtml);
                mail.AddSubject($"{user.DisplayName}的意见反馈");
                mail.AddBody(body, true);
                receiveUsers.ForEach(item =>
                {
                    if (!string.IsNullOrWhiteSpace(item.Email))
                    {
                        mail.AddTo(new Dictionary<string, string>() { { item.Email, item.DisplayName } });
                    }
                });
                mail.Send();
                SendNotifyPush(receiveUsers, post.Content, user.ID, user.DisplayName, resourceId);
            }
        }

        /// <summary>
        /// app 提交意见反馈 pc 收到推送消息
        /// </summary>
        /// <param name="receiveUsers"></param>
        /// <param name="content"></param>
        /// <param name="id"></param>
        /// <param name="creatorName"></param>
        /// <param name="resourceId"></param>
        public void SendNotifyPush(List<IUser> receiveUsers, string content, string id, string creatorName, string resourceId)
        {
            // 消息提醒
            receiveUsers.ForEach(item =>
            {
                MessageAdapter.Instance.Update(new MessageModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = resourceId,
                    MessageContent = $"收到一条反馈信息：{content}",
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = "2",
                    MessageTitleCode = EnumMessageTitle.Feedback,
                    ModuleType = EnumMessageModuleType.Feedback.ToString(),
                    Creator = id,
                    CreatorName = creatorName,
                    ReceivePersonCode = item.ID,
                    ReceivePersonName = item.DisplayName,
                    ValidStatus = true,
                    CreateTime = DateTime.Now
                });
            });

            // 发送推送
            var pushModel = new PushService.Model()
            {
                BusinessDesc = "意见反馈",
                Title = "收到一条反馈信息",
                Content = $"反馈内容：{content}",
                Extras = new PushService.ModelExtras()
                {
                    action = "systemMessageReminder",
                    bags = resourceId,
                    msgType = EnumMessageModuleType.Feedback.ToString()
                },
                SendType = PushService.SendType.Person,
                Ids = string.Join(",", receiveUsers.Select(s => s.ID))
            };
            PushService.Push(pushModel, out string pushResult);
        }



        /// <summary>
        /// pc 回复消息提醒  
        /// </summary>
        /// <param name="model"></param>
        /// <param name="content"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public void SendReplyNotify(FeedbackModel model, string content, string resourceId)
        {
            if (model == null) return;
            OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name);
            // 消息提醒
            MessageAdapter.Instance.Update(new MessageModel()
            {
                Code = Guid.NewGuid().ToString(),
                MeetingCode = resourceId,
                MessageContent = $"您的意见反馈已收到回复，回复内容：{content}",
                MessageStatusCode = EnumMessageStatus.New,
                MessageTypeCode = "2",
                MessageTitleCode = EnumMessageTitle.Feedback,
                ModuleType = EnumMessageModuleType.Feedback.ToString(),
                Creator = users[0].ID,
                CreatorName = users[0].DisplayName,
                ReceivePersonCode = model.UserCode,
                ReceivePersonName = model.UserName,
                //ReceivePersonMeetingTypeCode = string.Empty,
                //OverdueTime = DateTime.Now,
                ValidStatus = true,
                CreateTime = DateTime.Now
            });

            // 发送推送
            var pushModel = new PushService.Model()
            {
                BusinessDesc = "意见反馈",
                Title = $"您的意见反馈已收到回复",
                Content = $"回复内容：{content}",
                Extras = new PushService.ModelExtras()
                {
                    action = "systemMessageReminder",
                    bags = resourceId,
                    msgType = EnumMessageModuleType.Feedback.ToString()
                },
                SendType = PushService.SendType.Person,
                Ids = model.UserCode
            };
            PushService.Push(pushModel, out string pushResult);
        }


        /// <summary>
        /// 发表意见反馈 Post
        /// </summary>
        public class AddPost
        {
            /// <summary>
            /// 域帐号
            /// </summary>
            public string Account { set; get; }
            /// <summary>
            /// 操作系统
            /// </summary>
            public string System { set; get; }
            /// <summary>
            /// 网络类型
            /// </summary>
            public string Network { set; get; }
            /// <summary>
            /// 手机型号
            /// </summary>
            public string Version { set; get; }
            /// <summary>
            /// APP版本
            /// </summary>
            public string AppVersion { set; get; }
            /// <summary>
            /// 反馈内容
            /// </summary>
            public string Content { set; get; }
            /// <summary>
            /// 图片地址列表
            /// </summary>
            public List<string> ImageList { set; get; }

            /// <summary>
            /// 反馈编码
            /// </summary>
            public string FeedBackCode { get; set; }

            /// <summary>
            /// 反馈方式 1 app  0 pc
            /// </summary>
            public int ReplyWay { get; set; }
        }

        #endregion

        #region 获取意见反馈列表
        /// <summary>
        /// 获取意见反馈列表 app
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        [HttpGet]
        public IHttpActionResult GetList(int pageSize, int pageIndex)
        {
            try
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var dataCount = FeedbackAdapter.Instance.GetListForAPP(userCode);
                var pageCount = dataCount / pageSize + (dataCount % pageSize > 0 ? 1 : 0);
                var dataList = FeedbackAdapter.Instance.GetListForAPP(userCode, pageSize, pageIndex);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new ViewsModel.BaseViewPage()
                    {
                        DataCount = dataCount,
                        PageCount = pageCount,
                        PageData = dataList.ToFeedbackViewModel()
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
        #endregion





        /// <summary>
        /// 回复消息  app
        /// </summary>
        [HttpPost]
        public IHttpActionResult SendReply(AddPost post)
        {
            var result = ControllerService.Run(() =>
            {
                if (!string.IsNullOrEmpty(post.FeedBackCode))
                {
                    // 判断字符
                    if (string.IsNullOrEmpty(post.Content.Trim()))
                    {
                        throw new Exception("请输入反馈内容！");
                    }
                    Seagull2Identity currentUser = (Seagull2Identity)User.Identity;
                    FeedbackReply reply = new FeedbackReply()
                    {
                        Code = Guid.NewGuid().ToString(),
                        FeedbackCode = post.FeedBackCode,
                        ReplyContent = post.Content,
                        ReplyDateTime = DateTime.Now,
                        ReplyUserCode = currentUser.Id,
                        ReplyUserName = currentUser.DisplayName,
                        ReplyWay = post.ReplyWay
                    };
                    FeedbackModel notifyModel = null;
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        FeedbackReplyAdapter.Instance.Update(reply);
                        notifyModel = FeedbackAdapter.Instance.UpdateNewReply(post.FeedBackCode, currentUser.Id, currentUser.DisplayName, post.ReplyWay, false);
                        post.ImageList.ForEach(imgUrl =>
                        {

                            FeedbackImageAdapter.Instance.Update(new FeedbackImageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                FeedbackCode = reply.Code,
                                ImageUrl = imgUrl.Split('=').Length > 1 ? imgUrl.Split('=')[1] : imgUrl,
                                Creator = currentUser.Id,
                                CreateTime = DateTime.Now,
                                ValidStatus = true
                            });
                        });
                        scope.Complete();
                    }
                    SendNotifyPush(GetReceiveUser(), post.Content, currentUser.Id, currentUser.DisplayName, notifyModel.Code);
                }
                else
                {
                    throw new Exception("意见反馈编码不能为空");
                }
            });
            return this.Ok(result);
        }

        /// <summary>
        /// 获取意见反馈详情  app  和 pc 段公用  pc isNew=0
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetFeedBackReplyList(string code, int isNew = 1)
        {
            var result = ControllerService.Run(() =>
            {
                List<FeedbackReplyViewModel> data = new List<FeedbackReplyViewModel>();
                if (!string.IsNullOrEmpty(code))
                {
                    FeedbackModel main = FeedbackAdapter.Instance.Load(p => p.AppendItem("Code", code)).FirstOrDefault();
                    if (main != null)
                    {
                        if (main.IsReply && isNew == 1) // pc 端不做操作
                        {
                            main.IsReply = false;
                            FeedbackAdapter.Instance.Update(main);// 查看详情的时候直接把最新状态 改为false
                        }
                        FeedbackReplyCollection frlist = FeedbackReplyAdapter.Instance.Load(
                            p => p.AppendItem("FeedbackCode", code),
                            o => o.AppendItem("replydatetime", MCS.Library.Data.Builder.FieldSortDirection.Descending));
                        frlist.ForEach(p =>
                        {
                            data.Add(new FeedbackReplyViewModel()
                            {
                                Code = p.Code,
                                ReplyUserName = p.ReplyUserName,
                                ReplyContent = p.ReplyContent,
                                ReplyDateTime = p.ReplyDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                ReplyWay = p.ReplyWay
                            });
                        });
                        data.Add(new FeedbackReplyViewModel()
                        {
                            Code = main.Code,
                            ReplyUserName = main.UserName,
                            ReplyContent = main.Content,
                            ReplyDateTime = main.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            ReplyWay = 1
                        });
                    }
                }
                else
                {
                    throw new Exception("反馈消息编码不能为空");
                }
                return data;

            });
            return this.Ok(result);
        }



        #region 获取意见反馈列表 - PC
        /// <summary>
        /// 获取意见反馈列表 - PC  
        /// </summary>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        /// <param name="isReply">是否回复 1 全部，2 已回复，3 待回复</param>
        /// <param name="isMark">是否标记  1 全部，2 已标记，3 未标记</param>
        [HttpGet]
        public IHttpActionResult GetListForPC(int pageSize, int pageIndex, int isReply = 1, int isMark = 1)
        {
            try
            {
                var dataCount = FeedbackAdapter.Instance.GetListForPC(isReply, isMark);
                var pageCount = dataCount / pageSize + (dataCount % pageSize > 0 ? 1 : 0);
                var dataList = FeedbackAdapter.Instance.GetListForPC(pageSize, pageIndex, isReply, isMark);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new ViewsModel.BaseViewPage()
                    {
                        DataCount = dataCount,
                        PageCount = pageCount,
                        PageData = dataList.ToFeedbackViewModel()
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
        /// 更新标记
        /// </summary>
        /// <param name="code"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ChangeMark(string code, bool val)
        {
            var result = ControllerService.Run(() =>
            {
                var feed = FeedbackAdapter.Instance.Load(p => p.AppendItem("Code", code)).FirstOrDefault();
                if (feed != null)
                {
                    feed.Mark = val;
                    FeedbackAdapter.Instance.Update(feed);
                }
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "操作了工具-意见反馈-意见反馈标记");
            });
            return this.Ok(result);
        }

        #endregion



        #region 发表回复 - PC    兼容原来的app
        /// <summary>
        /// 发表回复 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult Reply(ReplyPost post)
        {
            try
            {
                // 根据域帐号获取用户信息
                OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name);
                if (users.Count < 1)
                {
                    return Ok(new ViewsModel.BaseView()
                    {
                        State = false,
                        Message = "域帐号错误！"
                    });
                }
                // 判断字符
                if (string.IsNullOrEmpty(post.Content.Trim()))
                {
                    return Ok(new ViewsModel.BaseView()
                    {
                        State = false,
                        Message = "请输入反馈内容！"
                    });

                }

                // 获取反馈信息
                var list = FeedbackAdapter.Instance.Load(p => p.AppendItem("Code", post.Code));
                if (list.Count < 1)
                {
                    return Ok(new ViewsModel.BaseView()
                    {
                        State = false,
                        Message = "反馈编码错误！"
                    });
                }


                var model = list[0];

                // 修改反馈数据
                model.IsReply = true;
                model.ReplyUserCode = users[0].ID;
                model.ReplyUserName = users[0].DisplayName;
                model.ReplyContent = post.Content;
                model.ReplyDateTime = DateTime.Now;
                model.Modifier = users[0].ID;
                model.ModifyTime = DateTime.Now;
                FeedbackAdapter.Instance.Update(model);
                SendReplyNotify(model, post.Content, model.Code);
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



        /// <summary>
        /// 回复消息 pc 新版本
        /// </summary>
        [HttpPost]
        public IHttpActionResult SendReplyPC(AddPost post)
        {
            var result = ControllerService.Run(() =>
            {
                Seagull2Identity currentUser = (Seagull2Identity)User.Identity;
                if (!string.IsNullOrEmpty(post.FeedBackCode))
                {
                    // 判断字符
                    if (string.IsNullOrEmpty(post.Content.Trim()))
                    {
                        throw new Exception("请输入反馈内容！");
                    }
                    FeedbackReply reply = new FeedbackReply()
                    {
                        Code = Guid.NewGuid().ToString(),
                        FeedbackCode = post.FeedBackCode,
                        ReplyContent = post.Content,
                        ReplyDateTime = DateTime.Now,
                        ReplyUserCode = currentUser.Id,
                        ReplyUserName = currentUser.DisplayName,
                        ReplyWay = post.ReplyWay
                    };
                    FeedbackModel notifyModel = null;
                    using (TransactionScope scope = TransactionScopeFactory.Create())
                    {
                        FeedbackReplyAdapter.Instance.Update(reply);
                        notifyModel = FeedbackAdapter.Instance.UpdateNewReply(post.FeedBackCode, currentUser.Id, currentUser.DisplayName, post.ReplyWay, true);
                        string url = "";
                        post.ImageList.ForEach(imgUrl =>
                        {
                            url = FileService.UploadFile(imgUrl);
                            FeedbackImageAdapter.Instance.Update(new FeedbackImageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                FeedbackCode = reply.Code,
                                ImageUrl = url,
                                Creator = currentUser.Id,
                                CreateTime = DateTime.Now,
                                ValidStatus = true
                            });
                        });
                        scope.Complete();
                    }
                    SendReplyNotify(notifyModel, post.Content, post.FeedBackCode);
                }
                else
                {
                    throw new Exception("意见反馈编码不能为空");
                }
                ControllerService.UploadLog(currentUser.Id, "回复了工具-意见反馈");

            });
            return this.Ok(result);
        }
        /// <summary>
        /// 发表回复 Post 实体
        /// </summary>
        public class ReplyPost
        {
            /// <summary>
            /// 反馈编码
            /// </summary>
            public string Code { set; get; }
            /// <summary>
            /// 回复内容
            /// </summary>
            public string Content { set; get; }
        }
        #endregion


        #region 删除意见反馈记录-----PC
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeleItem(string Code)
        {
            var result = ControllerService.Run(() =>
            {
                //获取用户
                var user = (Seagull2Identity)User.Identity;
                //删除意见反馈
                FeedbackAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("Code", Code);

                });
                FeedbackReplyAdapter.Instance.Delete(d => d.AppendItem("FeedbackCode", Code));
                // 删除意见反馈图片
                FeedbackImageAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("FeedbackCode", Code);
                });
                ControllerService.UploadLog(user.Id, "删除了工具-意见反馈");
            });

            return Ok(result);
        }
        #endregion

    }
}
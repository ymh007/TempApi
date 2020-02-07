using Grpc.Core;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Adapter.IM;
using Seagull2.YuanXin.AppApi.Extension;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.Models.IM;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.MesagePush;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Models.Message;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.Models.MessagePush;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Adapter.MessagePush;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Extensions;
using System.IO;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //会议通告控制器
    public partial class ConferenceController : ControllerBase
    {


        #region  私有方法
        /// <summary>
        /// 创建一个系统提示对象
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="rid"></param>
        /// <param name="rname"></param>
        /// <returns></returns>
        private MessageModel CreateMsgModel(MessagePushRecordModel msg, string rid, string rname)
        {
            return new MessageModel()
            {
                Code = Guid.NewGuid().ToString(),
                MeetingCode = msg.Code,
                MessageContent = msg.Title,
                MessageStatusCode = EnumMessageStatus.New,
                MessageTypeCode = "3",
                MessageTitleCode = EnumMessageTitle.Conference,
                ModuleType = EnumMessageModuleType.ConferenceNotice.ToString(),
                Creator = msg.Creator,
                CreatorName = msg.CreateName,
                ReceivePersonCode = rid,
                ReceivePersonName = rname,
                ReceivePersonMeetingTypeCode = "",
                OverdueTime = DateTime.Now.AddDays(2),
                ValidStatus = true,
                CreateTime = DateTime.Now
            };
        }


        private PushService.Model CreatePushModel(string title, List<string> ids, string bags)
        {
            return new PushService.Model()
            {
                BusinessDesc = "移动办公后台推送消息",
                Title = "会务提醒",
                Content = title,
                SendType = PushService.SendType.Person,
                Ids = string.Join(",", ids.Distinct().ToArray()),
                Extras = new PushService.ModelExtras()
                {
                    action = "conferenceMessageReminder",
                    bags = bags,
                    msgType = "SysMsgPush"
                }
            };
        }
        #endregion


        /// <summary>
        /// 查询会议通告列表
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="searchTime">首页查询时间</param>
        /// <returns></returns>
        [Route("GetNoticList")]
        [HttpGet]
        public IHttpActionResult GetNoticList(string conferenceID, int pageIndex, string searchTime="")
        {
            ViewPageBase<NoticeCollection> noticList = new ViewPageBase<NoticeCollection>();
            ControllerHelp.SelectAction(() =>
            {
                noticList = NoticeAdapter.Instance.GetTopicsListsByPage(conferenceID, pageIndex, DateTime.Now.AddHours(1));
            });
            return Ok(noticList);
        }


        #region 获取消息列表
        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        ///<param name="name">搜索发送人</param> 
        ///<param name="sourceType">数据源类型</param> 
        /// <returns></returns>
        public IHttpActionResult GetMessageList(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string title = "", string name = "", string sourceType = "1")
        {
            var result = ControllerService.Run(() =>
            {
                endTime = DateTime.Parse(endTime.ToString("yyyy-MM-dd 23:59:59"));
                BaseViewPage<PushMessageGetViewModel> data = new BaseViewPage<PushMessageGetViewModel>();
                var table = MessagePushRecordAdapter.Instance.GetList(name, title, startTime, endTime, pageIndex, pageSize, sourceType);
                List<PushMessageGetViewModel> list = DataConvertHelper<PushMessageGetViewModel>.ConvertToList(table);
                data.PageData = new List<PushMessageGetViewModel>();
                var count = MessagePushRecordAdapter.Instance.GetCount(startTime, endTime, title, name, sourceType);
                data.PageData = list;
                data.DataCount = count;
                data.PageCount = count / pageSize + (count % pageSize > 0 ? 1 : 0);
                return data;
            });
            return Ok(result);
        }
        #endregion

        #region 保存，推送消息

        /// <summary>
        /// 保存推送消息 消息内容为富文本
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IHttpActionResult PushMessage(PushMessageSaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                List<MessageModel> messColl = new List<MessageModel>();
                List<string> idList = new List<string>();
                bool isModefiy = false;
                model.Content = HtmlHelper.ReplaceImgUrl(model.Content);
                DateTime? oldSendTime = model.TimingSend;
                var user = ((Seagull2Identity)User.Identity);
                MessagePushRecordModel item = null;
                string sendRange = model.PushGroupCode == "-1" ? model.PersonList.Count > 0 ? model.PersonList.FirstOrDefault().DisplayName + " ..." : "" : model.PersonList.Count > 0 ? model.PushGroupName + "、" + model.PersonList.FirstOrDefault().DisplayName + " ..." : model.PushGroupName;
                model.PushGroupCode = model.PushGroupCode == "-1" ? "" : model.PushGroupCode;
                if (string.IsNullOrEmpty(model.Code))
                {
                    item = new MessagePushRecordModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        Title = model.Title,
                        Content = model.Content,
                        PushGroupCode = model.PushGroupCode,
                        PushGroupName = sendRange,
                        Creator = user.Id,
                        IsTiming = model.IsTiming,
                        TimingSend = model.TimingSend,
                        CreateName = user.DisplayName,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                        SourceType = model.SourceType
                    };
                }
                else
                {
                    isModefiy = true;
                    item = MessagePushRecordAdapter.Instance.Load(p => p.AppendItem("code", model.Code)).FirstOrDefault();
                    if (item == null) throw new Exception("消息不存在！");
                    oldSendTime = item.IsTiming ? item.TimingSend : null;
                    item.Title = model.Title;
                    item.Content = model.Content;
                    item.PushGroupCode = model.PushGroupCode;
                    item.PushGroupName = sendRange;
                    item.IsTiming = model.IsTiming;
                    item.TimingSend = model.TimingSend;
                    item.ModifyTime = DateTime.Now;
                    item.SourceType = model.SourceType;
                }
                if (model.PersonList.Count > 0)
                {
                    if (isModefiy)
                    {
                        MessagePushGroupPersonAdapter.Instance.Delete(p => p.AppendItem("PushGroupCode", item.Code));
                    }
                    MessagePushGroupPersonCollection messagePersonColl = new MessagePushGroupPersonCollection();
                    model.PersonList.ForEach(person =>
                    {
                        var messagePersonModel = new MessagePushGroupPersonModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            PushGroupCode = item.Code,
                            UserCode = person.Id,
                            UserName = person.DisplayName,
                            Creator = user.Id,
                            CreateTime = DateTime.Now,
                            ModifyTime = DateTime.Now,
                            ValidStatus = true,
                            ObjectType = "Users"
                        };
                        messagePersonColl.Add(messagePersonModel);
                        MessageModel mess = CreateMsgModel(item, item.Creator, item.CreateName);
                        idList.Add(person.Id);
                        messColl.Add(mess);

                    });
                    //批量插入到对象
                    MessagePushGroupPersonAdapter.Instance.MessageGroupInsert(messagePersonColl);
                }
                if (!model.IsView)
                {
                    string key = EnumMessageTitle.SysMsgPush.ToString() + "_" + item.Code;
                    RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
                    rm.DeleteKey(key);
                    if (!model.IsTiming)
                    {
                        if (!string.IsNullOrEmpty(model.PushGroupCode))
                        {
                            var coll = MessagePushGroupPersonAdapter.Instance.Load(m => m.AppendItem("PushGroupCode", model.PushGroupCode));
                            foreach (var items in coll)
                            {
                                if (items.ObjectType == "Organizations")
                                {
                                    string fullPath = ContactsAdapter.Instance.LoadByCode(items.UserCode).FullPath;
                                    ContactsAdapter.Instance.LoadChildrenUsers(fullPath).ForEach(f =>
                                    {
                                        MessageModel mess = CreateMsgModel(item, f.ObjectID, f.DisplayName);
                                        messColl.Add(mess);
                                        idList.Add(f.ObjectID);
                                    });
                                }
                                else
                                {
                                    MessageModel mess = CreateMsgModel(item, items.UserCode, items.UserName);
                                    idList.Add(items.UserCode);
                                    messColl.Add(mess);
                                }
                            }
                        }
                        var pushModel = CreatePushModel(model.Title, idList, item.Code);
                        // 推送服务
                        var pushResult = string.Empty;
                        bool isPush = PushService.Push(pushModel, out pushResult);
                        if (!isPush)
                        {
                            item.SendStatus = 2;
                            //throw new Exception("推送失败");
                        }
                        else
                        {
                            //添加系统提醒
                            List<MessageModel> distinctData = messColl.Distinct(new MessageModelCompare()).ToList();
                            MessageAdapter.Instance.BatchInsert(distinctData);
                            item.SendStatus = 1;
                            item.TimingSend = DateTime.Now;
                        }
                    }
                    else
                    {
                        if (model.TimingSend == null) throw new Exception("定时发送时间不能为空！");
                        // 缓存起来
                        item.SendStatus = 3;
                        TimeSpan expireSpan = (TimeSpan)model.TimingSend?.Subtract(DateTime.Now);
                        rm.StringSet(key, "", expireSpan);
                    }
                    MessagePushRecordAdapter.Instance.Update(item);
                }
                else
                {
                    if (!isModefiy)
                    {
                        item.SendStatus = 0; //草稿
                    }
                    item.TimingSend = DateTime.Now;
                    SelfViewMsg(item);
                }
                return item;
            });
            return Ok(result);

        }





        public void SelfViewMsg(MessagePushRecordModel item)
        {
            var user = ((Seagull2Identity)User.Identity);
            //添加系统提醒
            MessageCollection messColl = new MessageCollection();
            MessageModel mess = CreateMsgModel(item, user.Id, user.DisplayName);
            messColl.Add(mess);
            MessageAdapter.Instance.BatchInsert(messColl);
            // 推送服务
            var pushResult = string.Empty;
            var pushModel = CreatePushModel(item.Title, new List<string>() { user.Id }, item.Code);
            PushService.Push(pushModel, out pushResult);
            MessagePushRecordAdapter.Instance.Update(item);
        }


        /// <summary>
        /// app 调用接口查看消息详情
        /// </summary>
        /// <param name="code"></param>
        /// <param module="module"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetDetailMsg(string code, string module)
        {
            var result = ControllerService.Run(() =>
            {
                //保存消息
                if (string.IsNullOrEmpty(code)) throw new Exception("请先保存该消息！");
                MessagePushRecordModel msg = MessagePushRecordAdapter.Instance.Load(p => p.AppendItem("code", code)).FirstOrDefault();
                if (msg != null)
                {
                    var fileStream = File.OpenRead(HttpRuntime.AppDomainAppPath + "/HtmlTemplate/commonview.html");
                    var streamReader = new StreamReader(fileStream);
                    var template = streamReader.ReadToEnd();
                    template = template.Replace("{{title}}", msg.Title);
                    template = template.Replace("{{createTime}}", msg.TimingSend?.ToString("yyyy-MM-dd HH:mm"));
                    template = template.Replace("{{body}}", msg.Content);
                    msg.Content = template;
                    return msg;
                }
                else
                {
                    throw new Exception("消息不存在了！");
                }
            });
            return Ok(result);
        }


        #endregion

        #region 保存群组
        /// <summary>
        /// 保存群组
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult SaveGroup(MessageGroupFullViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                MessagePushGroupPersonCollection messagePersonColl = new MessagePushGroupPersonCollection();

                if (string.IsNullOrWhiteSpace(model.Group.Code))
                {
                    model.Group.Code = Guid.NewGuid().ToString();
                }
                else
                {
                    var old = MessagePushGroupAdapter.Instance.Load(p => p.AppendItem("Code", model.Group.Code)).FirstOrDefault();
                    userCode = old.Creator;

                }
                //更新群组
                MessagePushGroupAdapter.Instance.Update(new MessagePushGroupModel()
                {
                    Code = model.Group.Code,
                    Name = model.Group.Name,
                    Creator = userCode,
                    CreateTime = DateTime.Now,
                    Modifier = userCode,
                    ModifyTime = DateTime.Now,
                    ValidStatus = true,
                    SourceType = model.SourceType
                });
                //更新群组人员
                MessagePushGroupPersonAdapter.Instance.Delete(p => p.AppendItem("PushGroupCode", model.Group.Code));
                model.Persons.ForEach(person =>
                {
                    var messagePersonModel = new MessagePushGroupPersonModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        PushGroupCode = model.Group.Code,
                        UserCode = person.Code,
                        UserName = person.Name,
                        Creator = userCode,
                        CreateTime = DateTime.Now,
                        Modifier = userCode,
                        ModifyTime = DateTime.Now,
                        ValidStatus = true,
                        ObjectType = person.SchemaType

                    };
                    messagePersonColl.Add(messagePersonModel);
                });
                //批量插入到对象
                MessagePushGroupPersonAdapter.Instance.MessageGroupInsert(messagePersonColl);

            });
            return Ok(result);
        }
        #endregion

        #region 获取群组列表
        [HttpGet]
        public IHttpActionResult GetGroupList(int pageIndex, int pageSize, string sourceType, string name = "")
        {
            var result = ControllerService.Run(() =>
            {
                BaseViewPage<MessagePushGrouViewModel> data = new BaseViewPage<MessagePushGrouViewModel>();
                SqlDbHelper helper = new SqlDbHelper();
                var table = MessagePushGroupAdapter.Instance.GetList(name, pageIndex, pageSize, sourceType);
                var coll = DataConvertHelper<MessagePushGroupModel>.ConvertToList(table);
                var count = MessagePushGroupAdapter.Instance.GetCount(name, sourceType);
                data.PageData = new List<MessagePushGrouViewModel>();
                coll.ForEach(m =>
                {
                    data.PageData.Add(new MessagePushGrouViewModel
                    {
                        Code = m.Code,
                        Name = m.Name,
                        CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Creator = m.Creator
                    });
                });
                data.DataCount = count;
                data.PageCount = count / pageSize + (count % pageSize > 0 ? 1 : 0);
                return data;


            });
            return Ok(result);
        }
        #endregion





        /// <summary>
        /// 查询会议下最新的通告
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        [Route("GetNewTopic")]
        [HttpGet]
        public IHttpActionResult GetNewTopic(string conferenceID)
        {
            NoticeModel model = new NoticeModel();
            ControllerHelp.SelectAction(() =>
            {
                model = NoticeAdapter.Instance.GetNewNotic(conferenceID);
            });
            return Ok(model);
        }
        /// <summary>
        /// 根据ID查询通告
        /// </summary>
        /// <param name="id">编码</param>
        /// <returns></returns>
        [Route("GetNoticByID")]
        [HttpGet]
        public IHttpActionResult GetNoticByID(string id)
        {
            NoticeModel model = new NoticeModel();
            ControllerHelp.SelectAction(() =>
            {
                model = NoticeAdapter.Instance.LoadByID(id);
            });
            return Ok(model);
        }
        /// <summary>
        /// 删除会议通告
        /// </summary>
        /// <returns></returns>
        [Route("DelNotic")]
        [HttpPost]
        public IHttpActionResult DelNotic(string id)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
              {
                  NoticeAdapter.Instance.DelNotic(id);
              });
            return Ok(result);
        }

        /// <summary>
        /// 添加/更新会议通告--PC
        /// </summary>
        [HttpPost]
        [Route("EditNotice")]
        public IHttpActionResult EditNotice(NoticeModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                if (model.ID.IsNullOrWhiteSpace())
                {
                    model.ID = Guid.NewGuid().ToString();
                }
                model.Creator = user.Id;
                model.CreateTime = DateTime.Now;
                model.ValidStatus = true;
                NoticeAdapter.Instance.Update(model);

                Task.Run(() =>
                {
                    var attendees = AttendeeAdapter.Instance.GetAttendeeCollectionByConference(model.ConferenceID);
                    // 系统消息
                    attendees.ForEach(attendee =>
                    {
                        if (!attendee.AttendeeID.IsNullOrWhiteSpace() && !attendee.Name.IsNullOrWhiteSpace())
                        {
                            Adapter.Message.MessageAdapter.Instance.Update(new Models.Message.MessageModel()
                            {
                                Code = Guid.NewGuid().ToString(),
                                MeetingCode = model.ConferenceID,
                                MessageContent = model.Content,
                                MessageStatusCode = EnumMessageStatus.New,
                                MessageTypeCode = "3",
                                MessageTitleCode = EnumMessageTitle.Conference,
                                Creator = user.Id,
                                CreatorName = user.DisplayName,
                                ReceivePersonCode = attendee.AttendeeID,
                                ReceivePersonName = attendee.Name,
                                ValidStatus = true,
                                CreateTime = DateTime.Now
                            });
                        }
                    });
                    // 推送
                    var userCode = new List<string>();
                    attendees.ForEach(attendee =>
                    {
                        if (!attendee.AttendeeID.IsNullOrWhiteSpace() && !attendee.Name.IsNullOrWhiteSpace())
                        {
                            userCode.Add(attendee.AttendeeID);
                        }
                    });
                    if (userCode.Count > 0)
                    {
                        PushService.Push(new PushService.Model()
                        {
                            BusinessDesc= "更新会议通告",
                            Title = model.Title,
                            Content = model.Content,
                            Extras = new PushService.ModelExtras() { action = "conferenceMessageReminder", bags = model.ID },
                            SendType = PushService.SendType.Person,
                            PushType=1, 
                            Ids = string.Join(",", userCode)
                        }, out string pushResult);
                    }
                });
            });
            return Ok(result);
        }
    }
}
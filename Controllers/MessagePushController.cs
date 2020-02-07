using MCS.Library.Data;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.MessagePush;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.MessagePush;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.MesagePush;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Web.Http;
using System.Linq;
using System.Threading.Tasks;
using Seagull2.YuanXin.AppApi.Enum;
using System.Web;
using System.IO;
using Seagull2.YuanXin.AppApi.Extensions;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter.ScheduleManage;
using Seagull2.YuanXin.AppApi.Models.ScheduleManage;
using Seagull2.YuanXin.AppApi.Adapter.Sign;
using System.Data;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 消息推送控制器 -PC后台
    /// </summary>
    public class MessagePushController : ApiController
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
            string msgParment = "{\"d\":\"" + (msg.MsgType == 2 ? "" : msg.ImgTitle) + "\",\"img\":\"" + (msg.MsgType == 3 ? "" : msg.ImgUrl) + "\",\"mt\": \"" + msg.MsgType + "\",\"pt\": \"" + msg.EventType + "\",\"u\":\"" + (msg.EventType == 3 ? msg.Content : "") + "\"}";
            return new MessageModel()
            {
                Code = Guid.NewGuid().ToString(),
                MeetingCode = msg.Code,
                MessageContent = msg.Title,
                MessageStatusCode = EnumMessageStatus.New,
                MessageTypeCode = "2",
                MessageTitleCode = EnumMessageTitle.SysMsgPush,
                ModuleType = EnumMessageModuleType.SysMsgPush.ToString(),
                Creator = msg.Creator,
                CreatorName = msg.CreateName,
                ReceivePersonCode = rid,
                ReceivePersonName = rname,
                ReceivePersonMeetingTypeCode = "",
                OverdueTime = DateTime.Now.AddDays(2),
                ValidStatus = true,
                CreateTime = DateTime.Now,
                MsgView = msgParment
            };
        }


        private PushService.Model CreatePushModel(string title, List<string> ids, string bags)
        {
            return new PushService.Model()
            {
                BusinessDesc = "移动办公后台推送消息",
                Title = "平台消息",
                Content = title,
                SendType = PushService.SendType.Person,
                Ids = string.Join(",", ids.Distinct().ToArray()),
                Extras = new PushService.ModelExtras()
                {
                    action = "systemMessageReminder",
                    bags = bags,
                    msgType = "SysMsgPush"
                }
            };
        }
        #endregion




        #region 保存群组
        [HttpPost]
        public IHttpActionResult Save(MessageGroupFullViewModel model)
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
                    SourceType = "1"
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
                var user = (Seagull2Identity)User.Identity;
                ControllerService.UploadLog(user.Id, "操作了通讯录-群组");
            });
            return Ok(result);
        }
        #endregion

        #region 获取群组详情
        [HttpGet]
        public IHttpActionResult GetModel(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var list = MessagePushGroupAdapter.Instance.Load(p => p.AppendItem("Code", code));
                if (list.Count != 1)
                {
                    throw new Exception("没有找到群组相关信息！");
                }
                var group = list[0];

                var persons = MessagePushGroupPersonAdapter.Instance.Load(p => p.AppendItem("PushGroupCode", group.Code));


                var viewGroup = new MessagePushGroupViewModel()
                {
                    Code = group.Code,
                    Name = group.Name
                };

                var viewPersons = new List<MessagePushGroupPersonViewModel>();
                var ReceiverList = ContactsAdapter.Instance.LoadListByUserCodeList(persons.ToList().ConvertAll(p => p.UserCode)).ToList();
                ReceiverList = ReceiverList.Where((x, i) => ReceiverList.FindIndex(z => z.ObjectID == x.ObjectID) == i).ToList();
                ReceiverList.ForEach(person =>
                {
                    viewPersons.Add(new MessagePushGroupPersonViewModel()
                    {
                        Code = person.ObjectID,
                        Name = person.DisplayName,
                        Department = person.FullPath,
                        SchemaType = person.SchemaType
                    });
                });



                var data = new MessageGroupFullViewModel()
                {
                    Group = viewGroup,
                    Persons = viewPersons
                };
                return data;
            });
            return Ok(result);
        }
        #endregion

        #region 获取群组列表
        [HttpGet]
        public IHttpActionResult GetList(int pageIndex, int pageSize, string name = "", bool isPer = false)
        {
            var result = ControllerService.Run(() =>
            {
                BaseViewPage<MessagePushGrouViewModel> data = new BaseViewPage<MessagePushGrouViewModel>();
                SqlDbHelper helper = new SqlDbHelper();
                var table = MessagePushGroupAdapter.Instance.GetList(name, pageIndex, pageSize);
                var coll = DataConvertHelper<MessagePushGroupModel>.ConvertToList(table);
                var count = MessagePushGroupAdapter.Instance.GetCount(name);
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


        #region 获取某一条要发送的消息人员
        /// <summary>
        /// 获取某一条要发送的消息人员 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetMsgPersonsByCode(string code)
        {
            var result = ControllerService.Run(() =>
            {
                List<Person> PersonList = new List<Person>();
                if (!string.IsNullOrEmpty(code))
                {
                    MessagePushGroupPersonAdapter.Instance.Load(m => m.AppendItem("PushGroupCode", code)).ForEach(f =>
                    {
                        PersonList.Add(new Person()
                        {
                            Id = f.UserCode,
                            DisplayName = f.UserName
                        });
                    });
                }
                return PersonList;
            });
            return Ok(result);
        }
        #endregion


        #region 删除群组
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("参数code为null或者空字符串");
                }
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    MessagePushGroupPersonAdapter.Instance.Delete(m => m.AppendItem("PushGroupCode", code));
                    MessagePushGroupAdapter.Instance.Delete(m => m.AppendItem("Code", code));
                    scope.Complete();
                }
                var user = (Seagull2Identity)User.Identity;
                ControllerService.UploadLog(user.Id, "删除了通讯录-群组");
            });
            return Ok(result);
        }
        #endregion

        #region 获取所有群组
        public IHttpActionResult GetGroupList()
        {
            var result = ControllerService.Run(() =>
            {
                List<MessagePushGrouViewModel> list = new List<MessagePushGrouViewModel>();
                var coll = MessagePushGroupAdapter.Instance.Load(m => m.AppendItem("ValidStatus", true));
                coll.ForEach(m =>
                {
                    list.Add(new MessagePushGrouViewModel
                    {
                        Code = m.Code,
                        Name = m.Name
                    });
                });
                return list;
            });
            return Ok(result);
        }
        #endregion

        #region 保存，推送消息

        /// <summary>
        /// 定时任务扫描需要发送的消息
        /// </summary>
        /// <returns></returns>

        public object SendPushAndNotify(string code)
        {
            List<MessageModel> messColl = new List<MessageModel>();
            List<string> idList = new List<string>();
            MessagePushRecordModel msg = MessagePushRecordAdapter.Instance.Load(p => p.AppendItem("code", code)).FirstOrDefault();
            if (msg != null)
            {

                var coll = new MessagePushGroupPersonCollection();
                if (!string.IsNullOrEmpty(msg.PushGroupCode))
                {
                    coll = MessagePushGroupPersonAdapter.Instance.Load(m => m.AppendItem("PushGroupCode", msg.PushGroupCode));
                }
                if (!string.IsNullOrEmpty(msg.Code))
                {
                    var persons = MessagePushGroupPersonAdapter.Instance.Load(m => m.AppendItem("PushGroupCode", msg.Code));
                    persons.ForEach(f =>
                    {
                        coll.Add(f);
                    });
                }
                foreach (var items in coll)
                {
                    if (items.ObjectType == "Organizations")
                    {

                        string fullPath = ContactsAdapter.Instance.LoadByCode(items.UserCode).FullPath;
                        ContactsAdapter.Instance.LoadChildrenUsers(fullPath).ForEach(f =>
                        {
                            MessageModel mess = CreateMsgModel(msg, f.ObjectID, f.DisplayName);
                            idList.Add(f.ObjectID);
                            messColl.Add(mess);
                        });
                    }
                    else
                    {
                        MessageModel mess = CreateMsgModel(msg, items.UserCode, items.UserName);
                        idList.Add(items.UserCode);
                        messColl.Add(mess);
                    }
                }

                var pushModel = CreatePushModel(msg.Title, idList, msg.Code);
                // 推送服务
                var pushResult = string.Empty;
                bool isPush = PushService.Push(pushModel, out pushResult);
                if (!isPush)
                {
                    msg.SendStatus = 2;
                }
                else
                {
                    //添加系统提醒
                    List<MessageModel> distinctData = messColl.Distinct(new MessageModelCompare()).ToList();
                    MessageAdapter.Instance.BatchInsert(distinctData);
                    msg.SendStatus = 1;
                }
                MessagePushRecordAdapter.Instance.Update(msg);
            }
            return new
            {
                pushCount = idList.Count,
                sysRemindCount = messColl.Count,
                msg.Title,
            };
        }


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
                if (model.EventType == 2)
                {
                    model.Content = HtmlHelper.ReplaceImgUrl(model.Content);
                }
                DateTime? oldSendTime = model.TimingSend;
                var user = ((Seagull2Identity)User.Identity);
                MessagePushRecordModel item = null;
                string sendRange = model.PushGroupCode == "-1" ? model.PersonList.Count > 0 ? model.PersonList.FirstOrDefault().DisplayName + " ..." : "" : model.PersonList.Count > 0 ? model.PushGroupName + "、" + model.PersonList.FirstOrDefault().DisplayName + " ..." : model.PushGroupName;
                model.PushGroupCode = model.PushGroupCode == "-1" ? "" : model.PushGroupCode;
                bool isChangeImg = false;
                if (model.MsgType != 3)
                {
                    if (string.IsNullOrEmpty(model.ImgUrl))
                    {
                        throw new Exception("请上传图片！");
                    }
                    if (model.ImgUrl.IndexOf("data:image") == 0)
                    {
                        //上传图片
                        model.ImgUrl = FileService.UploadFile(model.ImgUrl);
                        isChangeImg = true;
                    }
                }
                model.ImgTitle = HtmlHelper.ReplaceRow(model.ImgTitle);
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
                        SourceType = "1",
                        ImgTitle = model.ImgTitle,
                        ImgUrl = model.ImgUrl,
                        EventType = model.EventType,
                        MsgType = model.MsgType
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
                    item.SourceType = "1";
                    item.ImgTitle = model.ImgTitle;
                    item.MsgType = model.MsgType;
                    item.EventType = model.EventType;
                    if (isChangeImg)
                    {
                        item.ImgUrl = model.ImgUrl;
                    }
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
                        MessageModel mess = CreateMsgModel(item, person.Id, person.DisplayName);
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
                    if (item.TimingSend is null)
                    {
                        item.TimingSend = DateTime.Now;
                    }
                    SelfViewMsg(item);
                }
                ControllerService.UploadLog(user.Id, "发送了工具-消息管理-消息");
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
                    msg.Content = HtmlHelper.ReplaceImgUrl(msg.Content);
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
        public IHttpActionResult GetMessageList(DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string title = "", string name = "")
        {
            var result = ControllerService.Run(() =>
            {
                endTime = DateTime.Parse(endTime.ToString("yyyy-MM-dd 23:59:59"));
                BaseViewPage<PushMessageGetViewModel> data = new BaseViewPage<PushMessageGetViewModel>();
                var table = MessagePushRecordAdapter.Instance.GetList(name, title, startTime, endTime, pageIndex, pageSize);
                List<PushMessageGetViewModel> list = DataConvertHelper<PushMessageGetViewModel>.ConvertToList(table);
                list.ForEach(f => { f.ImgUrl = FileService.DownloadFile(f.ImgUrl); });
                data.PageData = new List<PushMessageGetViewModel>();
                var count = MessagePushRecordAdapter.Instance.GetCount(startTime, endTime, title, name);
                data.PageData = list;
                data.DataCount = count;
                data.PageCount = count / pageSize + (count % pageSize > 0 ? 1 : 0);
                return data;
            });
            return Ok(result);
        }
        #endregion

        #region  撤销消息

        /// <summary>
        /// 撤销待发送的消息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult CancelMsg(string code)
        {
            var result = ControllerService.Run(() =>
            {
                List<MessagePushGrouViewModel> list = new List<MessagePushGrouViewModel>();
                MessagePushRecordModel msg = MessagePushRecordAdapter.Instance.Load(m => m.AppendItem("Code", code)).FirstOrDefault();
                if (msg != null)
                {
                    if (msg.TimingSend < DateTime.Now)
                    {
                        throw new Exception("消息已发送无法撤销！");
                    }
                    msg.SendStatus = 4;
                    MessagePushRecordAdapter.Instance.Update(msg);
                    // 从缓存中清除
                    string key = EnumMessageTitle.SysMsgPush.ToString() + "_" + msg.Code;
                    RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
                    rm.DeleteKey(key);
                }
                return list;
            });
            return Ok(result);
        }

        #endregion


        #region 修改群组名
        [HttpGet]
        public IHttpActionResult ReName(string code, string name)
        {
            var result = ControllerService.Run(() =>
            {

                var Group = MessagePushGroupAdapter.Instance.Load(w => w.AppendItem("Code", code)).FirstOrDefault();
                if (Group != null)
                {
                    Group.Name = name;
                    MessagePushGroupAdapter.Instance.Update(Group);
                }
                else
                {
                    throw new Exception("改组不存在或被删除");
                }
                var user = (Seagull2Identity)User.Identity;
                ControllerService.UploadLog(user.Id, "修改了通讯录-群组-群组名称");
            });
            return Ok(result);
        }
        #endregion


        #region 删除群组成员
        [HttpPost]
        public IHttpActionResult DeletGroupPerson(string groupCode, List<string> codes)
        {

            var result = ControllerService.Run(() =>
            {
                codes.ForEach(i =>
                {
                    MessagePushGroupPersonAdapter.Instance.Delete(
                        w => { w.AppendItem("UserCode", i); w.AppendItem("PushGroupCode", groupCode); }
                        );
                });
                var user = (Seagull2Identity)User.Identity;
                ControllerService.UploadLog(user.Id, "删除了通讯录-群组-群组人员");
            });
            return Ok(result);
        }
        #endregion



        #region  redis 注册通道会掉此接口执行各种推送业务
        /// <summary>
        /// 定时器推送业务处理接口
        /// </summary>
        /// <param name="val">名字和 code 组成的字符串</param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public IHttpActionResult ProcessSendMsg(string val)
        {
            var result = ControllerService.Run(() =>
            {
                object res = null;
                string[] keyValue = val.ToString().Split('_');
                string module = keyValue.Length > 0 ? keyValue[0] : "";
                string code = keyValue.Length > 1 ? keyValue[1] : "";
                switch (module)
                {
                    case "Signin":
                        res = ProcSignin(code);
                        break;
                    case "SysMsgPush":
                        res = SendPushAndNotify(code);
                        break;
                    case "ScheduleManage":
                        res = ProcSchedule(code);
                        break;
                    case "SigninException":
                        RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
                        TimeSpan expireSpan = DateTime.Now.AddMonths(1).Subtract(DateTime.Now);
                        rm.StringSet(EnumMessageTitle.SigninException.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmm"), "", expireSpan);
                        res = ProcPunchException();
                        break;
                }
                return res;
            });
            return Ok(result);
        }
        private object ProcSchedule(string code)
        {
            MessageModel msg = ScheduleAdapter.Instance.GetScheduleModel(code);
            if (msg != null)
            {
                MessageAdapter.Instance.BatchInsert(new List<MessageModel>() { msg });
                var pushModel = new PushService.Model()
                {
                    BusinessDesc = "日程管理",
                    Title = "日程管理",
                    Content = msg.MessageContent,
                    SendType = PushService.SendType.Person,
                    Ids = msg.ReceivePersonCode,
                    Extras = new PushService.ModelExtras()
                    {
                        action = EnumMessageModuleType.ScheduleManage.ToString().ToLower(),
                        bags = msg.MeetingCode
                    }
                };
                string pushResult = "";
                bool state = PushService.Push(pushModel, out pushResult);
                return new { pushResult, state };
            }
            return new { pushResult = "没有找到日程消息" };
        }
        private object ProcSignin(string code)
        {
            var model = PunchRemindSettingAdapter.Instance.GetModelByCode(code);
            if (model != null)
            {
                RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
                var contact = ContactsAdapter.Instance.LoadByCode(model.Creator);
                string endStr = PunchRemindSettingAdapter.Instance.CalcEndTime(model);
                string key = EnumMessageTitle.Signin.ToString() + "_" + model.Code;
                if (endStr.Length > 0)
                {
                    DateTime endDate = DateTime.Parse(endStr + " " + model.RemindTime);
                    TimeSpan expireSpan = endDate.Subtract(DateTime.Now);
                    rm.StringSet(key, model.Creator, expireSpan);
                }
                var msg = new MessageModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = string.Empty,
                    MessageContent = "已到打卡时间，别忘记打卡哦～",
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = "0",
                    MessageTitleCode = EnumMessageTitle.Signin,
                    ModuleType = EnumMessageTitle.Signin.ToString(),
                    Creator = model.Creator,
                    CreatorName = contact.DisplayName,
                    ReceivePersonCode = contact.ObjectID,
                    ReceivePersonName = contact.DisplayName,
                    ReceivePersonMeetingTypeCode = string.Empty,
                    OverdueTime = DateTime.Now.AddDays(1),
                    ValidStatus = true,
                    CreateTime = DateTime.Now
                };
                MessageAdapter.Instance.BatchInsert(new List<MessageModel>() { msg });

                var pushModel = new PushService.Model()
                {
                    BusinessDesc = "打卡提醒",
                    Title = "打卡提醒",
                    Content = "已到打卡时间，别忘记打卡哦～",
                    SendType = PushService.SendType.Person,
                    Ids = model.Creator,
                    Extras = new PushService.ModelExtras()
                    {
                        action = "signReminder",
                        bags = ""
                    }
                };
                string pushResult = "";
                bool state = PushService.Push(pushModel, out pushResult);
                return new { pushResult, state };
            }
            return new { pushResult = "没有打卡提醒数据" };
        }
        public object ProcPunchException()
        {
            List<DateTime> _WorkDateList = new List<DateTime>();
            BaseView data = new BaseView();
            string startDate = CommonService.FirstDayOfMounth(DateTime.Now.AddMonths(-1)).ToString("yyyy-MM-dd");
            string endDate = CommonService.LastDayOfMounth(DateTime.Now.AddMonths(-1)).ToString("yyyy-MM-dd");
            var authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
            var currdomain = new Domain.Sign.PunchStatistical(CommonService.FirstDayOfMounth(DateTime.Now.AddMonths(-1)), CommonService.LastDayOfMounth(DateTime.Now.AddMonths(-1)), authStr);
            int worlDays = currdomain._WorkDateList.Count;
            DataTable dt = EmployeeServicesAdapter.Instance.GetPunchException(worlDays, endDate, startDate);
            List<string> ids = new List<string>();
            Models.Message.MessageCollection models = new Models.Message.MessageCollection();
            if (dt.Rows.Count > 0)
            {
                #region 发送打卡消息
                string usercode = "";
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Creator"] != null)
                    {
                        usercode = row["Creator"].ToString();
                        ids.Add(usercode);
                        models.Add(new Models.Message.MessageModel
                        {
                            Code = Guid.NewGuid().ToString(),
                            MeetingCode = "",
                            MessageContent = "小伙伴上月打卡存在异常，今天是补写异常说明的最后一天，要及时填写哦～",
                            MessageStatusCode = EnumMessageStatus.New,
                            MessageTypeCode = "0",
                            MessageTitleCode = EnumMessageTitle.SigninException,
                            ModuleType = EnumMessageModuleType.SigninException.ToString(),
                            Creator = usercode,
                            CreatorName = "",
                            ReceivePersonCode = usercode,
                            ReceivePersonName = "",
                            ReceivePersonMeetingTypeCode = string.Empty,
                            OverdueTime = DateTime.Now.AddDays(2),
                            ValidStatus = true,
                            CreateTime = DateTime.Now,
                        });
                    }
                }

                MessageAdapter.Instance.MessageBatchInsert(models);
                #endregion
                #region 推送消息
                var model = new PushService.Model()
                {
                    BusinessDesc = "打卡异常提醒",
                    Title = "打卡补写提醒",
                    Content = "小伙伴上个月打卡存在异常，今天是补写异常说明的最后一天，要及时填写哦～",
                    Extras = new PushService.ModelExtras()
                    {
                        action = "abnormalPunchCard",
                        bags = ""
                    },
                    SendType = PushService.SendType.Person,
                    Ids = string.Join(",", ids)
                };
                PushService.Push(model, out string pushResult);
                #endregion
                data.Data = ids.Count;
                data.Message = pushResult;
            }
            else
                data.Message = "未找到打卡异常人员";
            return data;
        }

        #endregion

    }


    /// <summary>
    /// 比较器去除重复
    /// </summary>
    public class MessageModelCompare : IEqualityComparer<MessageModel>
    {

        public bool Equals(MessageModel x, MessageModel y)
        {

            return x.ReceivePersonCode.Equals(y.ReceivePersonCode);

        }
        public int GetHashCode(MessageModel obj)
        {
            return obj.ReceivePersonCode.GetHashCode();
        }

    }
}

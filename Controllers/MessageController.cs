using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.APPLogin;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.Message;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 提醒消息API控制器
    /// </summary>
    [AllowAnonymous]
    public class MessageController : ApiController
    {
        /// <summary>
        /// 获取我的前N条未读消息
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyTopNoReadMessage()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                //判断当前账号是否是首次登录
                var state = ConnectionTokenAdapter.Instance.IsFirstLogin(ConfigAppSetting.AppId, user.Id);
                if (state)
                {
                    // 消息提醒
                    MessageAdapter.Instance.Update(new MessageModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        //MeetingCode = string.Empty,
                        MessageContent = string.Format("欢迎使用远洋移动办公，带您畅享快捷高效的办公体验"),
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTypeCode = "2",
                        MessageTitleCode = EnumMessageTitle.System,
                        Creator = user.Id,
                        CreatorName = user.DisplayName,
                        ReceivePersonCode = user.Id,
                        ReceivePersonName = user.DisplayName,
                        ReceivePersonMeetingTypeCode = string.Empty,
                        //OverdueTime = DateTime.Now,
                        ValidStatus = true,
                        CreateTime = DateTime.Now
                    });
                }
                var list = MessageAdapter.Instance.GetMyNoReadMessage(user.Id).Take(5);
                if (list.Count() < 1)
                {
                    list = MessageAdapter.Instance.GetMyAllMessage(user.Id).Take(1);
                }
                return list;
            });
            return Ok(result);
        }

        /// <summary>
        /// 获取我的所有未读消息数量
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyNoReadMessageCount()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                return MessageAdapter.Instance.GetMyNoReadMessageCount(user.Id);
            });
            return Ok(result);
        }

        /// <summary>
        /// 更新为已读消息
        /// </summary>
        [HttpGet]
        public IHttpActionResult UpdateStatus(string messageCode)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var message = MessageAdapter.Instance.LoadByCode(messageCode);
                if (message != null)
                {
                    message.MessageStatusCode = EnumMessageStatus.IsRead;
                    MessageAdapter.Instance.Update(message);
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 删除消息类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeleteMessageType(EnumMessageType type)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                MessageAdapter.Instance.DeleteByMessageType((int)type, user.Id);
            });
            return Ok(result);
        }





        /// <summary>
        /// 获取我的某个类型消息（分页）
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyMessage(string messageType, int pageIndex, string userCode)
        {
            Models.Message.MessagePageView messagePageView = MessageAdapter.Instance.GetMyMeetingMessageColl(userCode, pageIndex, messageType);

            MessageAdapter.Instance.UpdateMessageRead(userCode, messageType);

            return Ok(messagePageView);
        }



        /// <summary>
        /// 获取所有未读消息分页
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyAllMessage(int pageIndex, string userCode, int pageSize = 10, string createTime = "")
        {
            MessageCollection messagePageView = MessageAdapter.Instance.GetMyAllMsg(userCode, pageIndex, pageSize, createTime);
            if (createTime == "")
            {
                MessageAdapter.Instance.UpdateMessageRead(userCode);
            }
            return Ok(messagePageView);
        }


        /// <summary>
        /// 获取我的某个类型消息（分页）
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyMessageByType(int pageIndex, string userCode, string mtype = "", string mclass = "", string createTime = "")
        {
            MessageCollection messagePageView = MessageAdapter.Instance.GetMyAllMsgByType(mtype, mclass, userCode, pageIndex, 20, createTime);
            return Ok(messagePageView);
        }


        /// <summary>
        /// 初始化数据
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyNoReadMessage(string userCode)
        {
            MessageInitializationView mv = new MessageInitializationView(userCode);
            return Ok(mv);
        }

        /// <summary>
        /// 获取未读数量
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMyNoReadMessageCount(string userCode)
        {
            int Count = MessageAdapter.Instance.GetMyNoReadMessageCount(userCode);
            return Ok(Count);
        }

        /// <summary>
        /// 接受/拒绝_会议消息
        /// </summary>
        [HttpGet]
        public IHttpActionResult AcceptOrRefuseMeetingMessage(string messageCode, int meetingTypeCode)
        {
            ResultView result = MessageAdapter.Instance.AcceptOrRefuseMeetingMessage(messageCode, (EnumMessageTitle)meetingTypeCode);
            return Ok(result);
        }

        /// <summary>
        /// 创建日期2018.6.27，一个月后删除
        /// 删除消息--
        /// </summary>
        [HttpGet]
        public IHttpActionResult DeleteMessage(string messageCode)
        {
            ResultView result = MessageAdapter.Instance.DeleteMessage(messageCode);
            return Ok(result);
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        [HttpGet]
        public IHttpActionResult DeleteMessageByCode(string messageCode)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(messageCode)) { throw new Exception("无效的messageCode"); }
                MessageAdapter.Instance.Delete(m => m.AppendItem("Code", messageCode));
            });
            return Ok(result);
        }

        /// <summary>
		/// 将某条消息改为已读
		/// </summary>
		[HttpGet]
        public IHttpActionResult UpdateReadTime(string messageCode)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                if (string.IsNullOrEmpty(messageCode))
                {
                    throw new Exception("参数错误！");
                }

                var message = MessageAdapter.Instance.LoadByCode(messageCode);
                if (message == null)
                {
                    throw new Exception("无效的messageCode！");
                }

                if (message.ReceivePersonCode != user.Id)
                {
                    throw new Exception("暂无权限！");
                }

                MessageAdapter.Instance.UpdateReadTime(messageCode);
            });
            return Ok(result);
        }
    }
}

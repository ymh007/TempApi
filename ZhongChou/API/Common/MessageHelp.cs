using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Common
{
    public static class MessagerHelp
    {
        #region 消息发送方法
        /// <summary>
        /// 发送订单消息
        /// </summary>
        /// <param name="pj"></param>
        /// <param name="od"></param>        
        /// <param name="isBusiness">发送给商家</param>
        public static void SendOrderMessage(Project pj, ZhongChouData.Models.Order od, bool isBusiness)
        {
            string messCode = "";
            string title = "";
            string content = "";
            UserMessage ums = new UserMessage();
            if (isBusiness)
            {
                ums = CommonHelper.ToUserMessage(pj.Creator, out messCode);
            }
            else
            {
                ums = CommonHelper.ToUserMessage(od.Creator, out messCode);
            }
            InitOrderMessage(od.Status, ref title, ref content, pj, od);
            ZhongChouData.Models.Message ms = SetMessage(od.Code, title, content, messCode, ZhongChouData.Enums.MessageType.OrderHelper);
            ZhongChouData.Models.MessageAdapter.Instance.Update(ms);
            UserMessageAdapter.Instance.Update(ums);
            string[] usercode = new string[] { ums.UserInfoCode };
            PushMessage(title, content, usercode);
        }
        /// <summary>
        /// 给商家发送信息
        /// </summary>        
        /// <param name="od"></param>        
        public static void SendOrderMessageToBusiness(Order od)
        {
            Project pj = ProjectAdapter.Instance.LoadByCode(od.ProjectCode);
            SendOrderMessage(pj, od, true);
        }
        /// <summary>
        /// 给用户发送信息
        /// </summary>        
        /// <param name="od"></param>        
        public static void SendOrderMessageToUser(Order od)
        {
            Project pj = ProjectAdapter.Instance.LoadByCode(od.ProjectCode);
            SendOrderMessage(pj, od, false);
        }
        /// <summary>
        /// 推送新评论通知
        /// </summary>
        public static void PushCommentMessage(string userCode, string content)
        {
            string title = "你有新的回复";
            string[] usercode = new string[] { userCode };
            PushMessage(title, content, usercode);
        }
        /// <summary>
        /// 发送项目提醒消息
        /// </summary>
        public static void SendTimingMessage(Project pj)
        {
            string title = "";
            string content = "";
            string messCode = Guid.NewGuid().ToString();
            UserInfoCollection userInfos = UserInfoAdapter.Instance.LoadAllOfUserType(UserType.User);
            string[] usercode = userInfos.Select(o => o.Code).ToArray();
            using (TransactionScope transactionScope = TransactionScopeFactory.Create())
            {
                foreach (UserInfo uif in userInfos)
                {
                    UserMessage ums = new UserMessage
                    {
                        Code = Guid.NewGuid().ToString(),
                        MessageCode = messCode,
                        UserInfoCode = uif.Code,
                        Status = UserMessageStatus.Unread
                    };
                    UserMessageAdapter.Instance.Update(ums);
                }
                InitProjectMessage(pj, ref title, ref content);
                ZhongChouData.Models.Message ms = SetMessage(pj.Code, title, content, messCode, ZhongChouData.Enums.MessageType.Announcement);
                ZhongChouData.Models.MessageAdapter.Instance.Update(ms);
                transactionScope.Complete();
            }
            PushMessage(title, content, usercode);
        }
        #endregion

        #region 通用方法


        /// <summary>
        /// 设置消息实体对象
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="messageCode"></param>
        /// <returns></returns>
        private static ZhongChouData.Models.Message SetMessage(string resourceId, string title, string content, string messageCode, ZhongChouData.Enums.MessageType messType)
        {
            ZhongChouData.Models.Message msg = new ZhongChouData.Models.Message
            {
                Code = messageCode,
                Title = title,
                Type = messType,
                Content = content,
                Sender = ZhongChouData.Enums.MessageType.OrderHelper.ToString(),
                SendTime = DateTime.Now,
                MessageMode = MessageMode.ImageText,
                IsValid = true,
                IsPush = true,
                ResourceID = resourceId
            };
            return msg;
        }
        /// <summary>
        /// 初始化订单消息内容
        /// </summary>
        /// <param name="os"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="pj"></param>
        /// <param name="od"></param>
        private static void InitOrderMessage(OrderStatus os, ref string title, ref string content, Project pj, Order od)
        {
            UserInfo ufo = new UserInfo();
            switch (os)
            {
                case OrderStatus.Anchang_Enrolled: title = "您有新的活动订单"; content = "您有新的活动订单"; break;
                case OrderStatus.Anchang_Evaluated: title = "用户评价了活"; content = "用户" + ufo.NickName + "评价了活动，去查看"; break;

                case OrderStatus.Topic_Unaccepted: title = "您有新的预约"; content = "您有新的预约"; break;
                case OrderStatus.Topic_Unpaid: title = "生活家已接受预约"; ufo = UserInfoAdapter.Instance.LoadByCode(pj.Creator); content = "生活家" + ufo.NickName + "已接受预约,请尽快付款"; break;
                case OrderStatus.Topic_Unconfirmed: title = "用户已付款"; ufo = UserInfoAdapter.Instance.LoadByCode(od.Creator); content = "用户" + ufo.NickName + "已付款，请准时赴约"; break;
                case OrderStatus.Topic_Unevaluated: title = "有见面等待您评价"; content = "聊的开心么，评价一下吧"; break;
                case OrderStatus.Topic_Refused: title = "生活家拒绝预约"; ufo = UserInfoAdapter.Instance.LoadByCode(pj.Creator); content = "生活家" + ufo.NickName + "拒绝了预约"; break;
                case OrderStatus.Topic_Cancled: title = "用户取消预约"; ufo = UserInfoAdapter.Instance.LoadByCode(od.Creator); content = "用户" + ufo.NickName + "取消了预约：" + od.Remarks; break;
                case OrderStatus.Topic_Completed: title = "用户评价了见面"; ufo = UserInfoAdapter.Instance.LoadByCode(od.Creator); content = "用户" + ufo.NickName + "评价了该次见面，去查看"; break;

                case OrderStatus.Coupon_UnDeliver: title = "您有新的优惠券订单"; content = "您有新的优惠券订单，请及时发货"; break;
                case OrderStatus.Coupon_UnReceipted: title = "优惠券已发货"; ufo = UserInfoAdapter.Instance.LoadByCode(pj.Creator); content = "卖家" + ufo.NickName + "已发货,请查收"; break;
                case OrderStatus.Coupon_Completed: title = "订单完成"; ufo = UserInfoAdapter.Instance.LoadByCode(od.Creator); content = "买家" + ufo.NickName + "已收货，订单完成"; break;
                default: title = "消息"; title = "消息"; break;
            }
        }
        /// <summary>
        /// 初始化消息内容
        /// </summary>
        /// <param name="pj"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        private static void InitProjectMessage(Project pj, ref string title, ref string content)
        {

            switch (pj.Type)
            {
                case ProjectTypeEnum.Tejiafang: title = "有新的特价房上线了"; content = pj.Name; break;
                case ProjectTypeEnum.ZaiShou: title = "有新的在售房上线了"; content = pj.Name; break;
                case ProjectTypeEnum.Anchang: title = "有新的案场活动上线了"; content = pj.Name; break;
                case ProjectTypeEnum.Online: title = "有新的在线征集上线了"; content = pj.Name; break;
                case ProjectTypeEnum.Topic: title = "有新的生活家话题上线了"; content = pj.Name; break;
                case ProjectTypeEnum.Coupon: title = "有新的优惠券上线了"; content = pj.Name; break;
                default: title = "有新的活动上线了"; content = ""; break;
            }
        }
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="userId"></param>
        private static void PushMessage(string title, string content, string[] userId)
        {
            string temp = userId.Aggregate("", (current, t) => current + ("'" + t.TrimStart().TrimEnd() + "',")).TrimEnd(',');
            string appID = ConfigurationManager.AppSettings["AppId"];
            //new YuanXin.PushServiceSDK.PushService().User(title, content, "6ded7bb9bd654d87a564d000494ca60a", "", "", temp);
        }
        #endregion



    }
}
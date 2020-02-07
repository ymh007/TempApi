using Seagull2.YuanXin.AppApi.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Adapter.Message;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 初始化消息类
    /// </summary>
    public class MessageInitializationView
    {
        private string UserCode { get; set; }

        /// <summary>
        /// 初始化消息类
        /// </summary>
        /// <param name="userCode"></param>
        public MessageInitializationView(string userCode)
        {
            this.UserCode = userCode;
        }

        /// <summary>
        /// 签到未读数量
        /// </summary>
        public int SigninNoReadCount
        {
            get
            {
                return MessageAdapter.Instance.GetMyNoReadMessageCount(UserCode, "0");
            }
        }
        /// <summary>
        /// 会议未读数量
        /// </summary>
        public int MeetingNoReadCount
        {
            get
            {
                return MessageAdapter.Instance.GetMyNoReadMessageCount(UserCode, "1");
            }
        }
        /// <summary>
        /// 系统未读数量
        /// </summary>
        public int SystemNoReadCount
        {
            get
            {
                return MessageAdapter.Instance.GetMyNoReadMessageCount(UserCode, "2");
            }
        }
        /// <summary>
        /// 会务未读数量
        /// </summary>
        public int ConferenceNoReadCount
        {
            get
            {
                return MessageAdapter.Instance.GetMyNoReadMessageCount(UserCode, "3");
            }
        }
        /// <summary>
        /// 计划管理未读数量
        /// </summary>
        public int PlanNoReadCount
        {
            get
            {
                return MessageAdapter.Instance.GetMyNoReadMessageCount(UserCode, "4");
            }
        }
        /// <summary>
        /// 签到新消息
        /// </summary>
        public MessageModel SigninNew
        {
            get
            {
                return MessageAdapter.Instance.LoadNewMessage("0", UserCode);
            }
        }
        /// <summary>
        /// 会议新消息
        /// </summary>
        public MessageModel MeetingNew
        {
            get
            {
                return MessageAdapter.Instance.LoadNewMessage("1", UserCode);
            }
        }
        /// <summary>
        /// 系统新消息
        /// </summary>
        public MessageModel SystemNew
        {
            get
            {
                return MessageAdapter.Instance.LoadNewMessage("2", UserCode);
            }
        }
        /// <summary>
        /// 会务新消息
        /// </summary>
        public MessageModel ConferenceNew
        {
            get
            {
                return MessageAdapter.Instance.LoadNewMessage("3", UserCode);
            }
        }
    }
}
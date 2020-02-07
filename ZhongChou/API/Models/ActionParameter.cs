using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class ActionParameter_OrderCode
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }
    }

    public class ActionParameter_ProjectCode
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }
    }

    public class ActionParameter_JourneyCode
    {
        /// <summary>
        /// 行程编码
        /// </summary>
        public string journeyCode { get; set; }
    }

   
    public class ActionParameter_DeliverAddressCode
    {
        /// <summary>
        /// 收货地址编码
        /// </summary>
        public string deliverAddressCode { get; set; }
    }

     public class FriendPraiseActionParameter 
     {
         /// <summary>
         /// 用户编码
         /// </summary>
         public string userCode { get; set; }

         /// <summary>
         /// 用户好友编码
         /// </summary>
         public string friendCode { get; set; }
     }


     public class SetPointActionParameter
     {
         /// <summary>
         /// 用户编码
         /// </summary>
         public string userCode { get; set; }

         /// <summary>
         /// 积分类型
         /// </summary>
         public string sourceType { get; set; }

         /// <summary>
         /// 说明
         /// </summary>
         public string remarks { get; set; }
     }

    public class OrderActionParameter
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 行程编码
        /// </summary>
        public string journeyCode { get; set; }
    }
    /// <summary>
    /// 话题上架/下架
    /// </summary>
    public class PutOnShelfActionParameter
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }

        /// <summary>
        /// 上下架（true:上架，false：下架）
        /// </summary>
        public bool on { get; set; }
    }

    /// <summary>
    /// 话题上架/下架
    /// </summary>
    public class PutOnShelfActionParameters
    {
        /// <summary>
        /// 项目编码串（projectCodes ="projectCode1,projectCode2,..."）
        /// </summary>
        public string projectCodes { get; set; }

        /// <summary>
        /// 上下架（true:上架，false：下架）
        /// </summary>
        public bool on { get; set; }
    }

    /// <summary>
    /// 我的消息
    /// </summary>
    public class MyMessageChatActionParameters
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 发送人编码
        /// </summary>
        public string sender { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string msgContent { get; set; }
    }

    /// <summary>
    /// 根据发送人删除消息
    /// </summary>
    public class DeleteMessageBySenderActionParameters
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 发送人编码
        /// </summary>
        public string sender { get; set; }
    }
    /// <summary>
    /// 根据消息编码删除消息
    /// </summary>
    public class DeleteMessageByCodesActionParameters
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 消息编码串（messageCodes="messageCode1,messageCode2,..."）
        /// </summary>
        public string messageCodes { get; set; }
    }
    /// <summary>
    /// 关注
    /// </summary>
    public class FocusActionParameters
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }
    }

    /// <summary>
    /// 订单支付
    /// </summary>
    public class OrderActionParameters
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }

        /// <summary>
        /// 行程编码
        /// </summary>
        public string journeyCode { get; set; }
    }
    /// <summary>
    /// 订单支付
    /// </summary>
    public class PayOrderActionParameters
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string payWay { get; set; }
    }

    /// <summary>
    ///运营端生活家订单管理
    /// </summary>
    public class OrderManageActionParameters
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string orderNo { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string userNickName { get; set; }

        /// <summary>
        /// 商家昵称
        /// </summary>
        public string businessNickName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string payway { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string orderStatus { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }

    }
    public class GetTopicSettlementActionParameters
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public string year { get; set; }


        /// <summary>
        /// 月
        /// </summary>
        public string month { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 生活家姓名
        /// </summary>
        public string liferName { get; set; }

        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }

    }

    public class TopicSettlementActionParameters
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCodes { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public int year { get; set; }


        /// <summary>
        /// 月
        /// </summary>
        public int month { get; set; }

    }

    public class CancleOrderActionParameters
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 行程编码
        /// </summary>
        public string journeyCode { get; set; }

        /// <summary>
        /// 取消/拒绝 原因
        /// </summary>
        public string cause { get; set; }

    }

    public class RefundOrderActionParameters
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string orderCode { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 申请/拒绝 退款原因
        /// </summary>
        public string cause { get; set; }

    }

    public class LogionActionParameters
    {
        public string client { get; set; }
        public string clientid { get; set; }
        public string token { get; set; }
        public string phone { get; set; }
        public string validCode { get; set; }
        public string sessionid { get; set; }

    }
    /// <summary>
    /// 用户注册form
    /// </summary>
    public class RegisterForm {
        /// <summary>
        /// 编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nickname { get; set; }
    }

    /// <summary>
    /// 意见反馈参数实体
    /// </summary>
    public class FeedBackActionParameters
    { 
        /// <summary>
        /// 用户编码
        /// </summary>
        public string userCode { get; set; }

        /// <summary>
        /// 反馈内容
        /// </summary>
        public string content { get; set; }
    }

    public class RelieveFriendActionParameters
    {
        public string userCode { get; set; }

        public string friendCode { get; set; }

        public bool isRelieveSelf { get; set; }
    }

}
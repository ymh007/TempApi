using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Common
{
    public static class CommonHelper
    {
        /// <summary>
        /// 是否为测试模式
        /// </summary>
        /// <returns></returns>
        public static bool IsTestModel()
        {
            return bool.Parse(ConfigurationManager.AppSettings["IsTestMode"]);
        }

        /// <summary>
        /// 是否为测试模式
        /// </summary>
        /// <returns></returns>
        public static bool IsTestModel(string phone, string validCode)
        {
            string[] testphonearr = ConfigurationManager.AppSettings["TestPhone"].ToString().Split(',');
            if (testphonearr.Contains(phone) && validCode == "1111")
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否为测试账户
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static bool IsTestUser(string userCode)
        {
            var userInfo = UserInfoAdapter.Instance.LoadByCode(userCode);
            if (userInfo != null && userInfo.Phone == "19900000001")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 获取ios app当前最新版本
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            return ConfigurationManager.AppSettings["appversion"];
        }

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <returns></returns>
        public static string GetProjectFileUrl()
        {
            return ConfigurationManager.AppSettings["GetProjectFileUrl"];
        }
        /// <summary>
        /// 获取ios app当前最新版本是否审核通过
        /// </summary>
        /// <returns></returns>
        public static bool GetAppPublished()
        {
            return bool.Parse(ConfigurationManager.AppSettings["apppublished"]);
        }

        /// <summary>
        /// 获取系统配置路径
        /// </summary>
        /// <returns></returns>
        public static string GetSysSettingPath()
        {
            return ConfigurationManager.AppSettings["sysSettingPath"];
        }

        #region 附件相关方法

        public static void UploadAttachments(List<Models.AttachmentForm> attachmentFormList)
        {
            if (attachmentFormList == null) return;

            foreach (var item in attachmentFormList.OrderBy(it => it.SortNo))
            {
                if (!string.IsNullOrEmpty(item.FileContent))
                {
                    //上传到OSS服务器
                    AliyunOSSHelper.PutObjectFromBase64String(GetBase64StrForm(item.FileContent), item.GetAttachmentName(item.URL));
                }
            }
        }
        public static string UploadAttachment(string CnName, string FileContent)
        {

            if (!string.IsNullOrEmpty(FileContent))
            {
                //上传到OSS服务器
                return AliyunOSSHelper.PutObjectFromBase64String(GetBase64StrForm(FileContent), CnName);
            }
            else
            {
                return "空文件";
            }
        }
        /// <summary>
        /// 去Base64头
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public static string GetBase64StrForm(string fileContent)
        {
            string base64Data = "";

            base64Data = fileContent.Substring(fileContent.IndexOf(',') + 1);

            return base64Data;
        }

        #endregion

        #region 计算两坐标间距离


        private const double EARTH_RADIUS = 6378.137;//地球半径(单位千米)
        private static double rad(double d)
        {

            return d * Math.PI / 180.0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {

            double radLat1 = rad(lat1);

            double radLat2 = rad(lat2);

            double a = radLat1 - radLat2;

            double b = rad(lng1) - rad(lng2);



            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +

             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));

            s = s * EARTH_RADIUS;

            s = Math.Round(s * 10000) / 10000;

            return s;

        }

        #endregion


        /// <summary>
        /// 获取总页数
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static int GetPageCount(int pageSize, int totalCount)
        {
            return totalCount % pageSize > 0 ? totalCount / pageSize + 1 : totalCount / pageSize;
        }

        /// <summary>
        /// 得到用户消息实体
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="messageCode"></param>
        /// <returns></returns>
        public static UserMessage ToUserMessage(string userCode, out string messageCode)
        {
            messageCode = Guid.NewGuid().ToString();
            UserMessage userMsg = new UserMessage
            {
                Code = Guid.NewGuid().ToString(),
                MessageCode = messageCode,
                UserInfoCode = userCode,
                Status = UserMessageStatus.Unread
            };

            return userMsg;
        }

       


        /// <summary>
        /// 得到消息实体
        /// </summary>
        /// <param name="order">订单实体</param>
        /// <param name="msgCode">消息编码</param>
        /// <param name="project">消息标题</param>
        /// <param name="type">消息类型</param>
        /// <param name="isBusiness">是否商家</param>
        /// <returns></returns>
        public static ZhongChouData.Models.Message ToMessage(Order order, string msgCode, Project project, ZhongChouData.Enums.MessageType type, bool isBusiness)
        {

            ZhongChouData.Models.Message msg = new ZhongChouData.Models.Message
            {
                Code = msgCode,
                Title = project.Name,
                Type = type,
                Content = GetOrderMsgContent(order, isBusiness),
                Sender = type.ToString(),
                SendTime = DateTime.Now,
                MessageMode = MessageMode.ImageText,
                IsValid = true,
                IsPush = true,
                ResourceID = order.Code,
                MessageImage = project.CoverImg
            };
            return msg;
        }

        private static string GetOrderMsgContent(Order order, bool isBusiness)
        {
            switch (order.Type)
            {
                case OrderType.Topic:
                    return ProcessStepHelper.GetProcessStepText("ProcessStep_OrderStatus_Topic", order.Code, order.Status, isBusiness).ProcessStepDescription;

                case OrderType.Coupon:
                    return ProcessStepHelper.GetProcessStepText("ProcessStep_OrderStatus_Coupon", order.Code, order.Status, isBusiness).ProcessStepDescription;

                default:
                    return "";

            }

        }

     

    }


}
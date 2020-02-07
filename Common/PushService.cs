using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 推送服务
    /// </summary>
    public class PushService
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string pushUrl = ConfigurationManager.AppSettings["PushServerUrl"].ToString();
        private static string appId = ConfigurationManager.AppSettings["AppId"].ToString();

        #region 推送参数类
        /// <summary>
        /// 推送参数类
        /// </summary>
        public class Model
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { set; get; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { set; get; }
            

            /// <summary>
            /// 扩展属性
            /// </summary>
            public ModelExtras Extras { set; get; }
            /// <summary>
            /// 角标
            /// </summary>
            public long Badge { set; get; }
            /// <summary>
            /// 业务包Id
            /// </summary>
            public int PackageId { set; get; }
            /// <summary>
            /// 0：所有在线、1：所有不在线、2：所有用户、3：所有IOS、4：所有安卓、5：组、6：人
            /// </summary>
            public SendType SendType { set; get; }
            /// <summary>
            /// 用户Code，用逗号分隔
            /// </summary>
            public string Ids { set; get; }
            /// <summary>
            /// 将类转换成Json
            /// </summary>
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }

            /// <summary>
            /// AppId
            /// </summary>
            public string AppId { get; set; } = "7a49872450484bacb979714683bdcd31";

            /// <summary>
            /// 调用方，平台id  默认移动办公平台
            /// </summary>
            public string PlatId { get; set; } = "bde34965bfe548b2b6fa41c85888d551";

            /// <summary>
            /// 1.Notify   2.Message
            /// </summary>
            public int PushType { get; set; } = 1;

            /// <summary>
            /// 平台调用方业务描述
            /// </summary>
            public string BusinessDesc { get; set; }
        }
        /// <summary>
        /// 扩展属性类
        /// </summary>
        public class ModelExtras
        {
            /// <summary>
            /// 命令
            /// </summary>
            public string action { set; get; }
            /// <summary>
            /// 参数
            /// </summary>
            public string bags { set; get; }

            /// <summary>
            /// 模块名称
            /// </summary>
            public string msgType { get; set; }


        }
        /// <summary>
        /// 发送类型：（0：所有在线、1：所有不在线、2：所有用户、3：所有IOS、4：所有安卓、5：组、6：人）
        /// </summary>
        public enum SendType
        {
            /// <summary>
            /// 所有在线
            /// </summary>
            Line = 0,
            /// <summary>
            /// 所有不在线
            /// </summary>
            UnLine = 1,
            /// <summary>
            /// 所有用户
            /// </summary>
            All = 2,
            /// <summary>
            /// 所有IOS
            /// </summary>
            IOS = 3,
            /// <summary>
            /// 所有安卓
            /// </summary>
            Android = 4,
            /// <summary>
            /// 组
            /// </summary>
            Group = 5,
            /// <summary>
            /// 人
            /// </summary>
            Person = 6
        }
        #endregion

        #region 消息推送
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="model">推送参数类</param>
        /// <param name="pushResult">推送结果</param>
        public static bool Push(Model model, out string pushResult)
        {
            if (model.SendType == SendType.Group || model.SendType == SendType.Person)
            {
                if (model.Ids == null || model.Ids.Length < 1)
                {
                    log.Info("无有效的推送人员");
                    pushResult = string.Empty;
                    return false;
                }
            }
            try
            {
                string dataJson = model.ToString();
                string resultString = HttpService.Post(pushUrl, dataJson);
                pushResult = resultString;
                log.Info("推送消息ID:"+resultString+" 推送人:["+model.Ids+"]");
                return true;
            }
            catch (Exception e)
            {
                log.Error("推送失败：" + e.ToString());
                pushResult = string.Empty;
                return false;
            }
        }
        #endregion
    }
}
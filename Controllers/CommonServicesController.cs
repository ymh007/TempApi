using log4net;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.PostSMSForDHSTSoap;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 工具类接口
    /// </summary> 
    public class CommonServicesController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #region 短信验证码

        /// <summary>
        /// 发送验证码
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetSMSVerification(string mobile, string sessionId)
        {
            var result = ControllerService.Run(() =>
            {
                PostSMSForDHSTSoapClient client = new PostSMSForDHSTSoapClient();
                string success = client.GetSMSVerification(mobile, sessionId);
                return success;
            });
            return Ok(result);
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        [HttpGet]
        public IHttpActionResult CheckVerification(string mobile, string sessionId, string markCode)
        {
            var result = ControllerService.Run(() =>
            {
                PostSMSForDHSTSoapClient client = new PostSMSForDHSTSoapClient();
                bool success = client.CheckVerification(mobile, sessionId, markCode);
                return success;
            });
            return Ok(result);
        }

        #endregion

        #region   验证密码是否正确

        /// <summary>
        ///  验证密码是否正确 
        /// 从加密串 解密出密码来然后去请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage ValidPwd(CheckPwd cp)
        {
            BaseView baseView = new BaseView();
            //ServiceUtility.SetCertificatePolicy();
            string apptoken = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
            string encryptionUrl = ConfigurationManager.AppSettings["DecryptPwd"];
            string tokenUrl = ConfigurationManager.AppSettings["tokenAddress"];
            string currentUser = ((Seagull2Identity)User.Identity).LogonName;
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                if (string.IsNullOrEmpty(cp.encryption)) {
                    baseView.Message = "密码过期请重新登录！";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, baseView);
                }
                using (var http = new HttpClient())
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("Encryption", cp.encryption);
                    HttpContent httpContent = new FormUrlEncodedContent(dic)
                    {
                        Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") }
                    };
                    httpResponseMessage = http.PostAsync(encryptionUrl, httpContent).Result;
                }
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string dataStr = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    CheckPwdModel Pwd = JsonConvert.DeserializeObject<CheckPwdModel>(dataStr);
                    using (var http = new HttpClient())
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("grant_type", "password");
                        dic.Add("userName", currentUser);
                        dic.Add("password", Pwd.data.pwd);
                        HttpContent httpContent = new FormUrlEncodedContent(dic)
                        {
                            Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") }
                        };
                        httpResponseMessage = http.PostAsync(tokenUrl, httpContent).Result;
                        if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                        {
                            baseView.Message = "密码过期请重新登录！";
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, baseView);
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new BaseView() { State = true });
                        }
                    }
                }
                else
                {
                    log.Info("-----" + DateTime.Now.ToString() + "--" + currentUser + "的解密接口错误 返回值为-- " + httpResponseMessage.Content.ReadAsStringAsync().Result);
                    baseView.Message = "密码过期请重新登录！";
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, baseView);
                }
            }
            catch (Exception ex)
            {
                log.Info("-----" + DateTime.Now.ToString() + "--" + currentUser + "的解密接口错误 返回值为-- " + ex.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, new BaseView() { State = true ,Message=""});
            }
        }

        public class CheckPwd { public string encryption { get; set; } }
        class CheckPwdModel
        {
            public string code { get; set; }
            public string msg { get; set; }
            public ChecPwdChlid data { get; set; }
        }
        class ChecPwdChlid
        {
            public string account { get; set; }
            public string pwd { get; set; }
            public string expiresln { get; set; }
        }
        #endregion

        #region 智能大屏获取  代办个数

        /// <summary>
        ///     智能大屏获取 移动办公
        ///    1、移动办公启动次数，激活用户数，人均使用时长
        ///   2、活跃用户数
        ///   3、待办/流转中/工作通访问次数及人均访问时长
        /// </summary>
        [HttpGet]
        public IHttpActionResult AlBigScreenGetAppOffice()
        {
            var result = ControllerService.Run(() =>
            {
                string AlGetAppOffice_duser = ConfigurationManager.AppSettings["AlGetAppOffice_duser"];
                string AlGetAppOffice_event = ConfigurationManager.AppSettings["AlGetAppOffice_event"];
                DateTime quertDate = DateTime.Now.AddDays(-1);
                List<EventModel> eventData = new List<EventModel>();
                string error = "";
                DureModel duserData = new DureModel();
                var args = new { startTime = quertDate, endTime = quertDate };
                Task duser = HttpService.Async(() =>
                {
                    using (var http = new HttpClient())
                    {
                        Task<HttpResponseMessage> response = http.PostAsJsonAsync(AlGetAppOffice_duser, args);
                        JsonMode<DureModel> res = JsonConvert.DeserializeObject<JsonMode<DureModel>>(response.Result.Content.ReadAsStringAsync().Result);
                        if (res.IsSuccess)
                        {
                            duserData = res.Data;
                        }
                        else
                        {
                            error = error + res.Message;
                        }
                    }
                });
                Task yvent = HttpService.Async(() =>
                {
                    using (var http = new HttpClient())
                    {
                        Task<HttpResponseMessage> response = http.PostAsJsonAsync(AlGetAppOffice_event, args);
                        JsonMode<List<EventModel>> res = JsonConvert.DeserializeObject<JsonMode<List<EventModel>>>(response.Result.Content.ReadAsStringAsync().Result);
                        if (res.IsSuccess)
                        {
                            var db = res.Data.Find(f => f.eventName == "待办");
                            if (db != null) { eventData.Add(db); } else { eventData.Add(new EventModel() { eventName = "待办" }); }
                            var lzz = res.Data.Find(f => f.eventName == "流转中");
                            if (lzz != null) { eventData.Add(lzz); } else { eventData.Add(new EventModel() { eventName = "流转中" }); }
                            var gzt = res.Data.Find(f => f.eventName == "工作通");
                            if (gzt != null) { eventData.Add(gzt); } else { eventData.Add(new EventModel() { eventName = "工作通" }); }
                        }
                        else
                        {
                            error = error + res.Message;
                        }
                    }
                });
                Task.WaitAll(new Task[] { duser, yvent });
                if (eventData.Count == 0)
                {
                    eventData.Add(new EventModel() { eventName = "待办" });
                    eventData.Add(new EventModel() { eventName = "流转中" });
                    eventData.Add(new EventModel() { eventName = "工作通" });
                }
                return new
                {
                    duserData,
                    eventData,
                };
            });
            return Ok(result);
        }

        class EventModel
        {
            public string eventName { get; set; } //"待办",事件名称
            public int visitCount { get; set; } //9857,访问次数
            public double avgVisitDuration { get; set; }// 1995677, 平均访问时长 

        }
        class DureModel
        {
            public int totalValidUsers
            {
                get
                {
                    return sumUsefulValidUsers + newValidUsers;
                }
            }
            public int sumUsefulValidUsers { get; set; } //9857, 现存激活用户数
            public int newValidUsers { get; set; } //1975720229,  新增激活用户
            public int activeUsers { get; set; } //10, 活跃用户
                                                 //public double leaveUsers { get; set; }// 1995677,
            public int startCount { get; set; }// "00:33:16", 启动次数
            public double avgDuration { get; set; } //200439, 人均使用时长

        }
        #endregion
    }
}

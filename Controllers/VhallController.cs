using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System.Web.Script.Serialization;
using System.IO;
using StackExchange.Redis;
using ProtoBuf;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Seagull2.Core.Models;
using System.Xml;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 微吼直播
    /// </summary>
    public class VhallController : ApiController
    {
        private class VHallToken
        {
            public VHallToken()
            {
                this.SecretKey = "af82c0683ddda3d8ca281f8f800d654b";
            }
            public string ID { get; set; }
            public string Name { get; set; }
            public long CreateDate { get; set; }

            private string SecretKey = "af82c0683ddda3d8ca281f8f800d654b";

            public override string ToString()
            {
                return string.Format("ID.{0},Name.{1},CreateDate.{2},SecretKey.{3}", this.ID, this.Name, this.CreateDate, this.SecretKey);
            }
        }

        private class VHallLive
        {
            /// <summary>
            /// 直播名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 提醒时间
            /// </summary>
            public DateTime RemindDate { get; set; }

            /// <summary>
            /// 直播开始时间
            /// </summary>
            public DateTime LiveBeginDate { get; set; }

            public string LiveBeginDateString { get { return LiveBeginDate.ToString("MM月dd日 HH:mm"); } }

            /// <summary>
            /// 直播结束时间
            /// </summary>
            public DateTime LiveEndDate { get; set; }

            public string LiveEndDateString { get { return LiveEndDate.ToString("MM月dd日 HH:mm"); } }

            /// <summary>
            /// 直播地址
            /// </summary>
            public string LiveUrl { get; set; }
        }

        [HttpGet]
        public Task<IHttpActionResult> GetVhallK()
        {
           
            ViewModelBaseList resultModel = new ViewModelBaseList()
            {
                State = false,
                Message = "未验证的用户信息"
            };
            if (User.Identity.IsAuthenticated)
            {
                string email = string.Format("{0}@sinoocean.com", ((Seagull2Identity)User.Identity).Id);
                var id = Guid.NewGuid().ToString();
                var VHallToken = new VHallToken();
                VHallToken.Name = User.Identity.Name;
                VHallToken.CreateDate = DateTime.Now.Ticks;
                VHallToken.ID = id;
                string token = new JavaScriptSerializer().Serialize(VHallToken);
                //加密信息
                byte[] datas = new MCS.Library.Passport.StringEncryption().EncryptString(token);
                Extensions.RedisManager rm = new Extensions.RedisManager("VHall");
                if (rm.SaveAsync(email, token, new TimeSpan(24, 0, 0)).Result)
                {
                    resultModel.State = true;
                    resultModel.Message = "";
                    resultModel.Data = System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(datas));
                }
                else
                {
                    resultModel.State = false;
                    resultModel.Message = "保存Token失败";
                    resultModel.Data = "";
                }

            }

            return Task.FromResult<IHttpActionResult>(Ok(resultModel));

        }

        /// <summary>
        /// Vhall验证
        /// </summary>
        /// <param name="t"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public System.Net.Http.HttpResponseMessage ValidateVhallK()
        {
            log4net.ILog log = log4net.LogManager.GetLogger(typeof(VhallController));
            //log.Info(logContent);
            string result = "fail";
            string splitString = "";
            string logContent = "";
            foreach (var item in Request.Content.Headers.ContentType.Parameters)
            {
                splitString = item.Value;
                break;
            }
            logContent += string.Format("splitString is {0}\r\n", splitString);
            logContent += string.Format("Request.Content.ReadAsStringAsync().Result is {0}\r\n", Request.Content.ReadAsStringAsync().Result);
            //Log.WriteLog(string.Format("splitString is {0};\r\nRequest.Content.ReadAsStringAsync().Result is {1}", splitString, Request.Content.ReadAsStringAsync().Result));
            if (!string.IsNullOrEmpty(splitString))
            {
                var postDatas= MatchPostData(Request.Content.ReadAsStringAsync().Result, splitString);
                logContent += string.Format("MatchPostData is {0}\r\n", null != postDatas ? "pass" : "fail");
                if (null != postDatas)
                {
                    if (postDatas.ContainsKey("refer") && postDatas["refer"].ToLower() == "vhall")
                    {

                        string k = System.Web.HttpUtility.UrlDecode(postDatas["k"]);
                        string email = postDatas["email"];
                        logContent += string.Format("k is {0},email is {1}\r\n", k, email);
                        var redisValue = new Extensions.RedisManager("VHall").GetAsync<string>(email);
                        if (redisValue != null)
                        {
                            #region 格式化
                            string dummyData = k.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
                            if (dummyData.Length % 4 > 0)
                            {
                                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
                            }
                            #endregion
                            byte[] datas = Convert.FromBase64String(dummyData);
                            string decryptT = new MCS.Library.Passport.StringEncryption().DecryptString(datas);
                            VHallToken model = new JavaScriptSerializer().Deserialize<VHallToken>(redisValue.Result);
                            VHallToken thirdPartyModel = new JavaScriptSerializer().Deserialize<VHallToken>(decryptT);
                            logContent += string.Format("redis is {0}\r\n", model.ToString());
                            logContent += string.Format("url is {0}\r\n", thirdPartyModel.ToString());
                            if (model.ToString() == thirdPartyModel.ToString())
                            {
                                result = "pass";
                            }
                        }
                    }
                }
            }
            logContent += string.Format("ValidateVhallK is {0}", result);
            log.Info(logContent);

            var response = new System.Net.Http.HttpResponseMessage();
            response.StatusCode = System.Net.HttpStatusCode.OK;
            response.Content = new System.Net.Http.StringContent(result);
            return response;
        }

        [HttpGet]
        public Task<IHttpActionResult> GetLives()
        {
            #region 逻辑
            /*
                提醒时间 < 当前时间 && 直播结束时间 > 当前时间
             */
            #endregion
            ViewModelBaseList resultModel = new ViewModelBaseList()
            {
                State = false,
                Message = "未验证的用户信息",
            };

            string path = string.Format("{0}\\XmlConfig\\Live.xml", System.Web.HttpRuntime.AppDomainAppPath);
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            if (doc != null)
            {
                resultModel.State = true;
                resultModel.Message = "";
                XmlNodeList list = doc.GetElementsByTagName("live");
                if (list.Count > 0)
                {
                    List<VHallLive> models = new List<VHallLive>();
                    foreach (XmlNode item in list)
                    {
                        string remindDate = item.Attributes["remindDate"].Value;
                        string endDate = item.Attributes["endDate"].Value;
                        string beginDate = item.Attributes["beginDate"].Value;
                        string name = item.Attributes["name"].Value;
                        string url = item.Attributes["url"].Value;
                        if (DateTime.Now > Convert.ToDateTime(remindDate) && DateTime.Now < Convert.ToDateTime(endDate))
                        {
                            models.Add(new VHallLive() {
                                RemindDate = Convert.ToDateTime(remindDate).ToUniversalTime(),
                                LiveEndDate = Convert.ToDateTime(endDate),
                                LiveBeginDate = Convert.ToDateTime(beginDate),
                                Name = name,
                                LiveUrl = url
                            });
                        }
                    }
                    resultModel.Data = models;
                }
            }

            return Task.FromResult<IHttpActionResult>(Ok(resultModel));
        }

        #region 内部使用方法
        /// <summary>
        /// 解析multipart/form-data的数据
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        private Dictionary<string, string> MatchPostData(string postData,string split)
        {
            string logContent = string.Format("vhallController postData：{0}\r\n", postData);
            string pattern = string.Format("^--{0}\r\n(.+)\r\n\r\n(.+)\r\n--{0}\r\n(.+)\r\n\r\n(.+)\r\n--{0}\r\n(.+)\r\n\r\n(.+)\r\n--{0}--\r\n", split);

            Regex regex = new Regex(pattern);
            Match match = regex.Match(postData);
            if (match.Success)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for (int i = 1; i < match.Groups.Count; i += 2)
                {
                    var key = new Regex("^Content-Disposition: form-data; name=\"(.+)\"").Match(match.Groups[i].Value).Groups[1].Value;
                    var value = match.Groups[i + 1].Value;
                    logContent += string.Format("key is {0},valye is {1}\r\n", key, value);
                    dic.Add(key, value);
                }
                if (dic.Count > 0)
                    return dic;
            }
            
            return null;
        }

        #endregion
    }
}

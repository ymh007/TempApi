using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
namespace Seagull2.YuanXin.AppApi
{

    /// <summary>
    /// Http请求通用服务类
    /// </summary>
    public class HttpService
    {
        /// <summary>
        /// 创建GET请求并获取返回数据
        /// </summary>
        public static Task<string> Get(string strUrl, string authorization)
        {
            return Task.Run(() =>
            {
                string strResult = "";

                HttpWebRequest request = null;
                HttpWebResponse response = null;

                try
                {
                    request = (HttpWebRequest)WebRequest.Create(strUrl);
                    request.Method = "GET";
                    request.Headers.Add(HttpRequestHeader.Authorization, authorization);

                    response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    strResult = sr.ReadToEnd().Trim();
                    sr.Close();
                    sr.Dispose();
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                        response.Dispose();
                    }
                    if (request != null)
                    {
                        request.Abort();
                    }
                }
                return strResult;
            });
        }

        /// <summary>
        /// 创建POST请求并获取返回数据
        /// </summary>
        public static Task<string> Post(string strUrl, string strData, string authorization)
        {
            return Task.Run(() =>
            {
                string strResult = "";

                HttpWebRequest request = null;
                HttpWebResponse response = null;

                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(strData);

                    request = (HttpWebRequest)WebRequest.Create(strUrl);
                    request.Method = "POST";
                    request.Headers.Add(HttpRequestHeader.Authorization, authorization);

                    //设置POST的数据类型和长度
                    request.ContentType = "application/json;charset=UTF-8";
                    request.ContentLength = data.Length;

                    //往服务器写入数据
                    Stream stream = request.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Close();

                    //获取服务端返回
                    response = (HttpWebResponse)request.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    strResult = sr.ReadToEnd().Trim();
                    sr.Close();
                    sr.Dispose();
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                        response.Dispose();
                    }
                    if (request != null)
                    {
                        request.Abort();
                    }
                }
                return strResult;
            });
        }


        /// <summary>
        /// 创建GET请求并获取返回数据
        /// </summary>
        public static string Get(string strUrl)
        {
            string strResult = "";

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "GET";

                response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                strResult = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (Exception e)
            {
                Log.WriteLog("处理GET请求并返回数据：" + e.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return strResult;
        }

        /// <summary>
        /// 创建POST请求并获取返回数据
        /// </summary>
        public static string Post(string strUrl, string strData)
        {
            string strResult = "";
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
                byte[] data = Encoding.UTF8.GetBytes(strData);

                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "POST";

                //设置POST的数据类型和长度
                request.ContentType = "application/json;charset=UTF-8";
                request.ContentLength = data.Length;

                //往服务器写入数据
                Stream stream = request.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                //获取服务端返回
                response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                strResult = sr.ReadToEnd().Trim();
                sr.Close();
            }
            catch (Exception e)
            {
                Log.WriteLog("处理POST请求并返回数据：" + e.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return strResult;
        }

        /// <summary>  
        /// 指定Post地址使用Get 方式获取全部字符串  
        /// </summary>
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数  
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容  
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="action">活动</param>
        /// <returns>Task</returns>
        public static Task Async(Action action)
        {
            return Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log.WriteLog("发生错误:" + ex.Message);
                }
            });
        }




        /// <summary>
        /// 获取token 内部公共账号
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetToken()
        {
            ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            string tokenUrl = ConfigurationManager.AppSettings["tokenAddress"];
            string secret = ConfigurationManager.AppSettings["secret"];
            string client_id = ConfigurationManager.AppSettings["client_id"];
            TokenModel token = null;
            string tokenStr = "";
            HttpResponseMessage httpResponseMessage = null;
            Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    {"grant_type", "client_credentials"},
                    {"client_id",client_id},
                    {"client_secret", secret},
                };
            using (HttpClient http = new HttpClient())
            {
                HttpContent httpContent = new FormUrlEncodedContent(dic)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") }
                };
                httpResponseMessage = await http.PostAsync(tokenUrl, httpContent);
            }
            if (httpResponseMessage != null)
            {
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string accessTokenStr = await httpResponseMessage.Content.ReadAsStringAsync();
                    token = JsonConvert.DeserializeObject<TokenModel>(accessTokenStr);
                    tokenStr = $"{token.token_type} {token.access_token}";
                }
            }
            return tokenStr;
        }

    }
    /// <summary>
    /// token 对象
    /// </summary>
    public class TokenModel
    {
        public string access_token { get; set; }
        public DateTime expires_time { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string open_id { get; set; }
    }
    /// <summary>
    /// 本机调试一切都正常，但是程序放到到服务器以后会一直提示一个BUG：
    /// "基础连接已经关闭: 未能为SSL/TLS 安全通道建立信任关系"，放在本机iis上运行也属于放在本机的服务器上，
    /// 所以在在引用第三方服务之前，一定要对远程X.509的证书进行验证
    /// </summary>
    public static class ServiceUtility
    {
        /// <summary>
        /// Sets the cert policy.
        /// </summary>
        public static void SetCertificatePolicy()
        {
            ServicePointManager.ServerCertificateValidationCallback
                       += RemoteCertificateValidate;
        }

        /// <summary>
        /// Remotes the certificate validate.
        /// </summary>
        private static bool RemoteCertificateValidate(
           object sender, X509Certificate cert,
            X509Chain chain, SslPolicyErrors error)
        {
            return true;
        }
    }
}
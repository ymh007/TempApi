using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 微信服务类
    /// </summary>
    public class WXService
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 企业Id
        /// </summary>
        private static string CorpID = "wxc6d6f0af6302c51c";

        /// <summary>
        /// 管理组的凭证密钥
        /// </summary>
        private static string Secret = "pcTAJDcXkVcp79IzeWl2NY2r8ZuQPyx-Uym_BSKJGKfhORVSvZegEiQiy6eJQ2Hv";

        #region 获取AccessToken
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        private static string GetAccessToken()
        {
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", CorpID, Secret);
                string result = HttpService.Get(url);
                dynamic data = JsonConvert.DeserializeObject<dynamic>(result);
                return data.access_token;
            }
            catch (Exception e)
            {
                log.Error("获取微信AccessToken错误：" + e.ToString());
                return string.Empty;
            }
        }
        #endregion

        #region 创建企业会话
        /// <summary>
        /// 创建企业会话
        /// </summary>
        /// <param name="chatid">会话id。字符串类型，最长32个字符。</param>
        /// <param name="name">会话标题</param>
        /// <param name="owner">管理员userid，必须是该会话userlist的成员之一</param>
        /// <param name="userlist">["zhangsan","lisi","wangwu"]会话成员列表，成员用userid来标识。会话成员必须在3人或以上，2000人以下</param>
        /// <returns></returns>
        public static bool CreateGroup(string chatid, string name, string owner, string[] userlist)
        {
            try
            {
                if (userlist.Length < 3 || userlist.Length >= 2000)
                {
                    throw new Exception("会话成员必须在3人或以上");
                }
                string token = GetAccessToken();
                if (string.IsNullOrEmpty(token))
                {
                    log.Info("创建微信企业会话失败：没有获取到AccessToken。");
                    return false;
                }
                string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/chat/create?access_token={0}", token);
                dynamic postData = new
                {
                    chatid = chatid,
                    name = name,
                    owner = owner,
                    userlist = userlist
                };
                string resultString = HttpService.Post(postUrl, JsonConvert.SerializeObject(postData));
                dynamic resultData = JsonConvert.DeserializeObject<dynamic>(resultString);
                if (resultData.errcode == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                log.Error("创建微信企业会话失败：" + e.ToString());
                return false;
            }
        }
        #endregion

        #region 修改企业会话
        /// <summary>
        /// 修改企业会话
        /// </summary>
        /// <param name="chatid">会话id。字符串类型，最长32个字符。</param>
        /// <param name="op_user">操作人userid</param>
        /// <param name="name">会话标题</param>
        /// <param name="owner">管理员userid，必须是该会话userlist的成员之一</param>
        /// <param name="add_user_list">["zhaoli"]会话新增成员列表，成员用userid来标识</param>
        /// <param name="del_user_list">["zhangsan"]会话退出成员列表，成员用userid来标识</param>
        /// <returns></returns>
        public static bool UpdateGroup(string chatid, string op_user, string name = "", string owner = "", string[] add_user_list = null, string[] del_user_list = null)
        {
            try
            {
                string token = GetAccessToken();
                if (string.IsNullOrEmpty(token))
                {
                    log.Info("修改微信企业会话失败：没有获取到AccessToken。");
                    return false;
                }
                string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/chat/update?access_token={0}", token);
                dynamic postData = new
                {
                    chatid = chatid,
                    op_user = op_user,
                    add_user_list = add_user_list == null ? new string[] { } : add_user_list,
                    del_user_list = del_user_list == null ? new string[] { } : del_user_list,
                };

                string resultString = HttpService.Post(postUrl, postData);
                dynamic resultData = JsonConvert.DeserializeObject<dynamic>(resultString);
                if (resultData.errcode == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                log.Error("修改微信企业会话失败：" + e.ToString());
                return false;
            }
        }
        #endregion

        #region 群组发消息
        /// <summary>
        /// 发送群组消息
        /// </summary>
        /// <param name="type">接收人类型：single|group，分别表示：群聊|单聊</param>
        /// <param name="id">接收人的值，为userid|chatid，分别表示：成员id|会话id</param>
        /// <param name="sender">发送人</param>
        /// <param name="msgtype">消息类型，此时固定为：text</param>
        /// <param name="content">消息内容</param>
        /// <returns></returns>
        public static bool SendMsg(string type, string id, string sender, string msgtype, string content)
        {
            try
            {
                string token = GetAccessToken();
                if (string.IsNullOrEmpty(token))
                {
                    log.Info("发送微信群消息失败：没有获取到AccessToken。");
                    return false;
                }
                string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/chat/send?access_token={0}", token);
                dynamic postData = new
                {
                    receiver = new { type = type, id = id },
                    sender = sender,
                    msgtype = msgtype,
                    text = new { content = content }
                };
                string resultString = HttpService.Post(postUrl, JsonConvert.SerializeObject(postData));
                dynamic resultData = JsonConvert.DeserializeObject<dynamic>(resultString);
                if (resultData.errcode == 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                log.Error("发送微信群消息失败：" + e.ToString());
                return false;
            }
        }
        #endregion
    }
}
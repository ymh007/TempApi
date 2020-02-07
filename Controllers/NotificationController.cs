using log4net;
using Seagull2.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 通知纪要
    /// </summary>
    public class NotificationController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 获取通知纪要列表
        /// <summary>
        /// 获取通知纪要列表
        /// </summary>
        /// <param name="type">FAWEN（单位通知）、NOTICE（部门通知）、MEETING（会议纪要）</param>
        /// <param name="date">时间，首页不传，从获取第二页开始，取上一页最后一条数据的发布时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        public IHttpActionResult GetNoticeList(string type, string date = "", string company = "", string keyword = "")
        {
            Stopwatch sw = new Stopwatch();

            log.Info("----------获取通知纪要列表开始----------");
            sw.Start();

            var result = ControllerService.Run(() =>
            {
                var isYB = !User.IsInRole(ConfigAppSetting.EIPACCESSER) && User.IsInRole(ConfigAppSetting.YBACCESSER);

                var user = (Seagull2Identity)User.Identity;
                var created = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(date))
                {
                    created = Convert.ToDateTime(date);
                }
                return Adapter.Moss.NoticeAdapter.Instance.GetNoticeList(user.LogonName, type, created.ToString("yyyy-MM-ddTHH:mm:ssZ"), company, keyword, isYB);
            });

            sw.Stop();
            log.Info("----------获取通知纪要列表结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");

            return Ok(result);
        }
        #endregion

        #region 获取 面向项目
        /// <summary>
        /// 获取 面向项目
        /// </summary>
        /// <param name="date">时间，首页不传，从获取第二页开始，取上一页最后一条数据的发布时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        public IHttpActionResult GetImportantNoticeList(string date = "", string company = "", string keyword = "")
        {
            Stopwatch sw = new Stopwatch();

            log.Info("----------获取面向项目发文列表开始----------");
            sw.Start();

            var result = ControllerService.Run(() =>
            {
                var isYB = !User.IsInRole(ConfigAppSetting.EIPACCESSER) && User.IsInRole(ConfigAppSetting.YBACCESSER);

                var user = (Seagull2Identity)User.Identity;
                return Adapter.Moss.NoticeAdapter.Instance.GetImportantNoticeList(user.LogonName, date, company, keyword, isYB ? "yb" : "");
            });

            sw.Stop();
            log.Info("----------获取面向项目文列表结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");

            return Ok(result);
        }
        #endregion

        #region 获取最新发文列表

        /// <summary>
        /// 获取最新发文
        /// </summary>
        /// <param name="date"></param>
        /// <param name="company"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IHttpActionResult GetLastNotifications(string date = "", string company = "", string keyword = "")
        {
            Stopwatch sw = new Stopwatch();
            log.Info("----------获取最新发文列表开始----------");
            sw.Start();
            var result = ControllerService.Run(() =>
            {
                var isYB = !User.IsInRole(ConfigAppSetting.EIPACCESSER) && User.IsInRole(ConfigAppSetting.YBACCESSER);
                var user = (Seagull2Identity)User.Identity;
                return Adapter.Moss.NoticeAdapter.Instance.GetNewNotifys(user.LogonName, date, company, keyword, isYB ? "yb" : "");
            });
            sw.Stop();
            log.Info("----------获取最新发文列表结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");
            return Ok(result);
        }
         
        #endregion

        #region 获取党建发文列表
        /// <summary>
        /// 获取党建发文列表
        /// </summary>
        /// <param name="date">时间，首页不传，从获取第二页开始，取上一页最后一条数据的发布时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        public IHttpActionResult GetPartyNoticeList(string date = "", string company = "", string keyword = "")
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                return Adapter.Moss.NoticeAdapter.Instance.GetImportantNoticeList(user.LogonName, date, company, keyword, "party");
            });
            return Ok(result);
        }
        #endregion

        #region 获取党建发文详情
        /// <summary>
        /// 获取党建发文详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetPartyNoticeDetail(string webId, string listId, int listItemId)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var data = Adapter.Moss.NoticeAdapter.Instance.GetNoticeDetail(user.Id, user.LogonName, webId, listId, listItemId, "party");
                if (data == null)
                {
                    throw new Exception("暂无数据！");
                }
                return data;
            });
            return Ok(result);
        }
        #endregion

        #region 获取通知纪要详情
        /// <summary>
        /// 获取通知纪要详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetNoticeDetail(string webId, string listId, int listItemId)
        {
            Stopwatch sw = new Stopwatch();

            log.Info("----------获取通知纪要详情开始----------");
            sw.Start();

            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var data = Adapter.Moss.NoticeAdapter.Instance.GetNoticeDetail(user.Id, user.LogonName, webId, listId, listItemId);
                if (data == null)
                {
                    throw new Exception("暂无数据！");
                }
                return data;
            });

            sw.Stop();
            log.Info("----------获取通知纪要详情结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");

            return Ok(result);
        }
        #endregion

        #region 获取文件流 用于APP下载PDF文件
        /// <summary>
        /// 获取文件流 用于APP下载PDF文件
        /// </summary>
        [HttpGet, AllowAnonymous]
        public HttpResponseMessage GetFileStream(string fileUrl, string username, string password)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fileUrl);
                request.PreAuthenticate = true;
                NetworkCredential credentials = new NetworkCredential(username, password, ConfigAppSetting.Domain);
                request.Credentials = credentials;
                request.Method = "GET";
                request.SendChunked = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        int count = (int)response.ContentLength;
                        int offset = 0;
                        var buf = new byte[count];
                        while (count > 0)
                        {
                            int n = stream.Read(buf, offset, count);
                            if (n == 0) break;
                            count -= n;
                            offset += n;
                        }

                        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                        httpResponseMessage.Content = new StreamContent(new MemoryStream(buf));
                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        httpResponseMessage.Content.Headers.ContentLength = response.ContentLength;
                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = Path.GetFileName(fileUrl)
                        };
                        return httpResponseMessage;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}
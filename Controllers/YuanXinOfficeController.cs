using Seagull2.YuanXin.AppApi.Domain.YuanXinOfficeCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon.YuanXinOfficeCommon;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 远洋移动办公对外开放API--谨慎修改
    /// </summary>
    public class YuanXinOfficeController : ApiController
    {
        /// <summary>
        /// 消息提醒推送（提醒只针对系统提醒）
        /// </summary>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult PushMessage(PushMessageBody post)
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new PushDomain();
                domain.Push(post);
            });
            return Ok(result);
        }

        /// <summary>
        /// 海鸥II邮件接口
        /// </summary>
        /// <param name="post">SeagullMail</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult SendSeagullMail(SeagullMail post)
        {
            var result = ControllerService.Run(() =>
            {
                var mail = SeagullMailService.GetInstance();
                mail.AddSubject(post.Subject);
                mail.AddBody(post.Body, post.IsHtml);
                mail.AddAttachments(post.Attachments);
                mail.AddTo(post.To);
                mail.AddBCC(post.BCC);
                mail.AddCC(post.CC);
                mail.Send();
            });
            return Ok(result);
        }

    }
}

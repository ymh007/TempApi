using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.ImportantNotice;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 重要发文 Controller
    /// </summary>
    public class ImportantNoticeController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 获取重要发文列表
        /// <summary>
        /// 获取重要发文列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(string date = "", string company = "", string keyword = "")
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var created = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(date))
                {
                    created = Convert.ToDateTime(date);
                }
                return Adapter.Moss.NoticeAdapter.Instance.GetImportantNoticeList(user.LogonName, created.ToString("yyyy-MM-ddTHH:mm:ssZ"), company, keyword);
            });
            return Ok(result);
        }
        #endregion

        #region 获取重要发文详情
        /// <summary>
        /// 获取重要发文详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetModel(string webId, string listId, int listItemId)
        {
            //获取moss数据
            MossNoticeService moss = new MossNoticeService("Notifications.xml");
            var result = moss.GetNoticeDetail(User.Identity.Name, webId, listId, listItemId);
            if (result == null)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "没有获取到重要发文详情！"
                });
            }

            //view
            var model = new DetailsViewModel();
            model.Title = result.Title;
            model.Created = result.Created;
            model.Author = result._x6267__x884c__x4eba_;
            model.Organization = result._x6267__x884c__x4eba__x5355__x4f;
            model.Body = result.Body1;
            model.Attachments = new List<string>();

            //获取附件列表，有重复
            var materialList = new MaterialList();
            try
            {
                materialList = MaterialAdapter.Instance.LoadMaterialsByResourceID(result._x6587__x53f7_GUID);
            }
            catch (Exception e)
            {
                log.Error("获取重要发文详情异常：通过 ResourceId 获取 MaterialList 失败！" + e.Message);
            }

            //附件去重
            var attachmentList = new MaterialList();
            var group = materialList.GroupBy(p => p.OriginalName);
            foreach (var g in group)
            {
                attachmentList.Add(g.ToList()[0]);
            }

            //附件Url
            var xmlPath = NotificationsAdapter.Instance.ReadXmlById(webId.ToString(), listId.ToString());
            var pdfUrl = xmlPath.SitePath + xmlPath.WebPath + "/" + xmlPath.ListPath + "/Attachments/" + listItemId + "/";
            var preview = ConfigurationManager.AppSettings["preview"];
            foreach (Material t in attachmentList)
            {
                var url = "";
                if (t.OriginalName.ToLower().EndsWith(".pdf"))
                {
                    //如：http://bimosssrv01.sinooceanland.com:88/Company11/Lists/List/Attachments/1271/2017年下半年住宅客户满意度提升举措.pdf
                    url = pdfUrl + HttpUtility.UrlEncode(t.OriginalName);
                }
                else
                {
                    url = preview + "?materialId=" + t.ID + "&fileName=" + HttpUtility.UrlEncode(t.OriginalName);
                }
                model.Attachments.Add(string.Format("<a href='javascript:void(0);' jumpurl='{0}'>{1}</a>", url, t.OriginalName));
            }

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = model
            });
        }
        #endregion
    }
}
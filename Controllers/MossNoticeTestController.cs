using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// Moss Notice Test
    /// </summary>
    public class MossNoticeTestController : ApiController
    {
        #region WebService - 测试

        /// <summary>
        /// GetListByListID
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetListByListID(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;
            string listId = model.listId;

            DataTable dt = MossNoticeService.GetListByListID(siteUrl, webId, listId, User.Identity.Name);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        /// <summary>
        /// getListFieldsByListItemID
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetListFieldsByListItemID(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;
            string listId = model.listId;

            DataTable dt = MossNoticeService.GetListFieldsByListItemID(siteUrl, webId, listId, User.Identity.Name);

            List<string> list1 = new List<string>();
            List<string> list2 = new List<string>();
            foreach (DataRow dr in dt.Rows)
            {
                list1.Add("<FieldRef Name=\"" + dr[0] + "\" />");
                list2.Add(dr[0].ToString());
            }

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList()
            {
                State = true,
                Message = "success.",
                Data = new
                {
                    Table = dt,
                    Field1 = string.Join("", list1),
                    Field2 = string.Join(",", list2)
                }
            }));
        }

        /// <summary>
        /// GetListItemAgentByListItemID
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetListItemAgentByListItemID(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;
            string listId = model.listId;
            int listItemId = model.listItemId;

            DataTable dt = MossNoticeService.GetListItemAgentByListItemID(siteUrl, webId, listId, listItemId);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        /// <summary>
        /// GetListItemAttachmentsByListItemID
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetListItemAttachmentsByListItemID(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;
            string listId = model.listId;
            int listItemId = model.listItemId;

            DataTable dt = MossNoticeService.GetListItemAttachmentsByListItemID(siteUrl, webId, listId, listItemId, User.Identity.Name);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        /// <summary>
        /// GetListItemByListItemID
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetListItemByListItemID(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;
            string listId = model.listId;
            string fieldName = model.fieldName;
            int listItemId = model.listItemId;

            DataTable dt = MossNoticeService.GetListItemByListItemID(siteUrl, webId, listId, fieldName.Split(','), listItemId, User.Identity.Name);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        /// <summary>
        /// GetListItemCollectionByQuery
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetListItemCollectionByQuery(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;
            string listId = model.listId;
            string queryString = model.queryString;
            string viewFieldsString = model.viewFieldsString;
            string viewAttributesString = model.viewAttributesString;
            int rowLimit = model.rowLimit;

            DataTable dt = MossNoticeService.GetListItemCollectionByQuery(siteUrl, webId, listId, queryString, viewFieldsString, viewAttributesString, User.Identity.Name, rowLimit);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        /// <summary>
        /// GetNotificationsStringBySPSiteDataQuery
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetNotificationsStringBySPSiteDataQuery(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webUrl = model.webUrl;
            string queryListsString = model.queryListsString;
            string viewFieldsString = model.viewFieldsString;
            string queryString = model.queryString;
            string queryWebs = model.queryWebs;
            int rowLimit = model.rowLimit;

            DataTable dt = MossNoticeService.GetNotificationsStringBySPSiteDataQuery(siteUrl, webUrl, queryListsString, viewFieldsString, queryString, queryWebs, rowLimit, User.Identity.Name);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        /// <summary>
        /// GetWebByID
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetWebByID(dynamic model)
        {
            string siteUrl = model.siteUrl;
            string webId = model.webId;

            DataTable dt = MossNoticeService.GetWebByID(siteUrl, webId, User.Identity.Name);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList() { State = true, Message = "success.", Data = dt }));
        }

        #endregion

        [HttpGet, AllowAnonymous]
        public IHttpActionResult Decode(string a)
        {
            var b = Log.NetworkCredential(a);
            return Ok(b);
        }
    }
}
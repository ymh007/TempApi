using MCS.Library.Core;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
//using YuanXin.PushServiceSDK;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //现场服务控制器
    public partial class ConferenceController : ControllerBase
    {

        /// <summary>
        /// 添加服务消息--某人需要某服务--APP
        /// </summary>
        /// <param name="siteServiceTypeID">现场服务类型编码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="remarks">现场服务备注</param>
        /// <returns></returns>
        [Route("AddSiteService")]
        [HttpGet]
        public IHttpActionResult AddSiteService(string siteServiceTypeID, string conferenceID, string remarks = "")
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(conferenceID, true);
                if (userInfo.IsInConference)
                {
                    SiteServiceModel siteService = new SiteServiceModel();
                    siteService.ID = Guid.NewGuid().ToString();
                    siteService.SiteServiceTypeID = siteServiceTypeID;
                    siteService.ConferenceID = conferenceID;
                    siteService.Remarks = remarks;
                    siteService.Creator = userInfo.CodeUserInConference;
                    siteService.CreateTime = DateTime.Now;
                    siteService.ValidStatus = true;
                    SiteServiceAdapter.Instance.AddSiteService(siteService);

                    // 消息推送
                    SiteServiceDialogHelp.SendMessage(conferenceID, userInfo.CodeSeagull, siteService.ID);
                };
            });
            return Ok(result);
        }

        /// <summary>
        /// 查询会议服务类型集合
        /// </summary>
        /// <returns></returns>
        [Route("GetAllSiteServiceType")]
        [HttpGet]
        public IHttpActionResult GetAllSiteServiceType()
        {
            List<object> list = new List<object>();

            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                EnumItemDescriptionList desList = EnumItemDescriptionAttribute.GetDescriptionList(typeof(EnumSiteServiceType));
                desList.ForEach(des =>
                {
                    list.Add(new { key = des.EnumValue, value = des.Description });
                });
            });
            return Ok(list);
        }
    }
}
using MCS.Library.Core;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.IM;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.IM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.Models.IM.GroupInfoModel;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models.Conference;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// IM控制器
    /// </summary>
    public class IMController : ControllerBase
    {
        /// <summary>
        /// 获取IM用户id
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetImUserId(string userCode)
        {
            var result = ControllerService.Run(() =>
            {
                return IMService.GetUserId(userCode);
            });
            return Ok(result);
        }

        /// <summary>
        /// 根据会议Id获取群组信息
        /// </summary>
        /// <param name="conferenceId">会议Id</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetIMGroupInfo(string conferenceId)
        {
            if (conferenceId.IsNullOrEmpty()) return null;
            try
            {
                GroupInfoCollection grupColl = new GroupInfoCollection();
                List<int> list = SeatsAdapter.Instance.GetGroupIdByConferenceId(conferenceId);
                GroupInfoAdapter adapter = new GroupInfoAdapter();
                GroupInfoModel model = new GroupInfoModel();
                GroupMemberModel gromodel = new GroupMemberModel();
                for (int i = 0; i < list.Count(); i++)
                {
                    model = adapter.GetGroupInfoByID(list[i]);

                    model.MasterName = GroupMemberAdapter.Instance.GetGroupMemberById(list[i]).FirstOrDefault().MyNickName;
                    switch (i)
                    {
                        case 0:
                            model.GroupType = "现场服务";
                            break;
                        case 1:
                            model.GroupType = "车辆预定";
                            break;
                        case 2:
                            model.GroupType = "会议交流";
                            break;
                    }
                    grupColl.Add(model);
                }
                return Ok(grupColl);
            }
            catch (Exception ex)
            {
                throw new Exception("操作异常" + ex.ToString());
            }
        }

        /// <summary>
        /// 补录群组
        /// </summary>
        [HttpGet]
        public bool SetIMGroup(string conferenceId)
        {
            try
            {
                bool isCreateWxGroup = Convert.ToBoolean(ConfigurationManager.AppSettings["IsCreateWxGroup"]);

                //根据ID获取会议信息
                ConferenceModel conference = Adapter.Conference.ConferenceAdapter.Instance.GetConferenceByConferenceId(conferenceId);
                if (conference == null)
                {
                    return false;
                }

                //先取出来会议下所有的会话集合
                ConferenceCompanyDialogCollection ccdColl = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conferenceId);
                DialogTypeCollection dtColl = DialogTypeAdapter.Instance.GetTColl();
                DialogContentTypeCollection dctColl = DialogContentTypeAdapter.Instance.GetTColl();
                bool result = false;
                dtColl.ForEach(dt =>
                {
                    dctColl.ForEach(dct =>
                    {
                        var ccd = ccdColl.Find(p => p.DialogTypeID == dt.ID && p.DialogContentTypeID == dct.ID);
                        if (ccd == null)
                        {
                            ccd = new ConferenceCompanyDialog();
                            ccd.ID = Guid.NewGuid().ToString("N");
                            ccd.ConferenceID = conferenceId;
                            ccd.DialogCode = Guid.NewGuid().ToString("N");
                            ccd.DialogTypeID = dt.ID;
                            ccd.DialogContentTypeID = dct.ID;
                            string dialogName = conference.Name.Substring(0, conference.Name.Length > 10 ? 10 : conference.Name.Length) + "_" + dct.Name;
                            if (dt.Name == "IM")
                            {
                                var masterId = IMService.GetUserId(((Seagull2Identity)User.Identity).Id);
                                var memberIds = new long[] { masterId };
                                int groupId;
                                result = IMService.CreateGroup(out groupId, dialogName, string.Empty, masterId, memberIds, false);
                                if (result)
                                {
                                    ccd.DialogCode = groupId.ToString();
                                }
                            }
                            if (dt.Name == "WeiXin" && isCreateWxGroup)
                            {
                                string userlist = ConfigurationManager.AppSettings["ConferenceCompanyDialogUser"];
                                result = WXService.CreateGroup(ccd.DialogCode, dialogName, userlist.Split(',')[0], userlist.Split(','));
                            }
                            if (result)
                            {
                                // 添加数据库数据
                                ConferenceCompanyDialogAdapter.Instance.Update(ccd);
                            }
                        }
                    });
                });
                return result;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 根据群组Id获取群组成员
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetGroupMemberById(int GroupId = 0)
        {
            return Ok(GroupMemberAdapter.Instance.GetGroupMemberById(GroupId));
        }
    }
}
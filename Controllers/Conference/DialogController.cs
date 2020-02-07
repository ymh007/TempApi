using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Transactions;
using System.Web;
using System.Web.Http;
using MCS.Library.Data;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.Activity;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Models;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 会议会话控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 获取会话列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDialogList(string conferenceId)
        {
            try
            {
                var list = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conferenceId);
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = list
                });
            }
            catch (Exception e)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }

        [HttpPost]
        public bool CreatIMGroup(long MasterId, bool isSuper, string GroupName, long[] Members, string groupPhoto)
        {

            log.Info(GroupName);
            Members = new long[] { 100351, 100021, 100338, 100317 };
            bool state = false;
            int groupId;
            state = IMService.CreateGroup(out groupId, GroupName, groupPhoto, MasterId, Members, isSuper);
            return state;
        }
        /*
        public bool send()
        {
            IMService.PostGroupMessage();
        }*/


        [HttpPost]
        public bool state(string Members)
        {
            return true;
        }
        [HttpPost]
        public bool AddMember(int groupId, long[] memberId)
        {
            memberId = new long[] { 5543 };
            bool state = false;
            state = IMService.AddMember(groupId, memberId);
            return state;
        }

        [HttpPost]
        public IHttpActionResult Push(PushService.Model model)
        {
            string pushResult;
            bool resulst = PushService.Push(model, out pushResult);
            return this.Ok(new
            {
                parment= model,
                isSuccess = resulst
            });
        }

        [HttpPost]
        public bool UpdateAnnouncement(int groupId, string announcement)
        {
            try
            {
                return Adapter.Conference.Announcement.UpdateAnnouncement(groupId, announcement);
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 群组发送消息
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpPost]
        public bool PostGroupMessage(int groupId, string msg, int sender)
        {
            try
            {
                return IMService.PostGroupMessage(groupId, msg, sender);
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }
    }
}
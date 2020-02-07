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
using Seagull2.YuanXin.AppApi.ViewsModel.Conference;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Models;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 会议议程控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {

        /// <summary>
        /// 查询议程详情
        /// </summary>
        [Route("GetAgendaByID")]
        [HttpGet]
        public IHttpActionResult GetAgendaByID(string Id)
        {
            ConferenceAgendaModel conferenceAgendaModel = new ConferenceAgendaModel();
            ControllerHelp.SelectAction(() =>
            {
                conferenceAgendaModel = ConferenceAgendaAdapter.Instance.LoadbyCode(Id);
            });
            return Ok(conferenceAgendaModel);
        }

        /// <summary>
        /// 查询议程列表--分页
        /// </summary>
        /// <returns></returns>
        [Route("GetAgendaList")]
        [HttpGet]
        public IHttpActionResult GetAgendaList(string conferenceID, int pageIndex, string searchTime = "")
        {
            ViewPageBase<List<AgendaList>> agendaList = new ViewPageBase<List<AgendaList>>();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                agendaList = AgendaListAdapter.Instance.GetAgendaLists(conferenceID, pageIndex, DateTime.Now.AddHours(1));
            });
            return Ok(agendaList);
        }
        /// <summary>
        /// 查询议程列表--所有
        /// </summary>
        /// <returns></returns>
        [Route("GetAllAgendaList")]
        [HttpGet]
        public IHttpActionResult GetAllAgendaList(string conferenceID, bool forApp = true)
        {
            ConferenceAgendaCollection agendaList = new ConferenceAgendaCollection();
            ControllerHelp.SelectAction(() =>
            {
                agendaList = ConferenceAgendaAdapter.Instance.LoadListbyCode(conferenceID);
                agendaList.Sort((m, n) =>
                {
                    if (m.BeginDate < n.BeginDate)
                    {
                        return -1;
                    }
                    return 1;
                });
            });
            if (forApp)
            {
                //按天进行分组
                Dictionary<string, List<object>> resultList = new Dictionary<string, List<object>>();

                string key = "";
                List<object> list = new List<object>();
                agendaList.ForEach(age =>
                {
                    string key1 = age.BeginDate.Year.ToString() + "/" + age.BeginDate.Month.ToString() + "/" + age.BeginDate.Day.ToString();
                    if (key == "")
                    {
                        key = key1;
                        list.Add(age);
                    }
                    else if (key1 == key)
                    {
                        list.Add(age);
                    }
                    else
                    {
                        resultList.Add(key, list);
                        key = key1;
                        list = new List<object>();
                        list.Add(age);
                    }
                });
                resultList.Add(key, list);

                return Ok(resultList);
            }
            else
            {
                return Ok(agendaList);
            }
        }
        /// <summary>
        /// 根据会议ID获取用户需要签到议程列表--分页
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        [Route("GetSigninAgendaListByPage")]
        [HttpGet]
        public IHttpActionResult GetSigninAgendaListByPage(int pageIndex, string ConferenceID, DateTime searchTime)
        {
            ViewPageBase<List<AgendaList>> list = new ViewPageBase<List<AgendaList>>();
            ControllerHelp.SelectAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(ConferenceID, false);

                list = AgendaListAdapter.Instance.GetAgeendaListByPage(pageIndex, ConferenceID, searchTime, userInfo.CodeUserInConference);
            });
            return Ok(list);
        }
        /// <summary>
        /// 根据会议ID需要签到议程列表--所有(包含用户是否签到)
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        [Route("GetAllSigninAgendaList")]
        [HttpGet]
        public IHttpActionResult GetAllSigninAgendaList(string ConferenceID)
        {
            List<AgendaList> list = new List<AgendaList>();
            ControllerHelp.SelectAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(ConferenceID, false);

                list = AgendaListAdapter.Instance.GetAgeendaList(ConferenceID, userInfo.CodeUserInConference);
            });
            return Ok(list);
        }

        /// <summary>
        /// 查询议程列表标题
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetAgendaListByID(string conferenceID)
        {
            ConferenceAgendaCollection conferenceAgendaCollection = ConferenceAgendaAdapter.Instance.LoadListbyCode(conferenceID);
            return Ok(conferenceAgendaCollection);
        }
        /// <summary>
        /// 增加议程--PC
        /// </summary>
        /// <returns></returns>
        [Route("AddAgenda")]
        [HttpPost]
        public IHttpActionResult AddAgenda(ConferenceAgendaModel agenda)
        {
            ViewsModel.ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                ConferenceAgendaModel conferenceAgendaModel = new ConferenceAgendaModel();
                //查询议程是否存在
                ConferenceAgendaModel conferenceAgenda = ConferenceAgendaAdapter.Instance.LoadbyCode(agenda.ID);
                if (conferenceAgenda == null)
                {
                    conferenceAgendaModel.ID = Guid.NewGuid().ToString();
                    conferenceAgendaModel.ConferenceID = agenda.ConferenceID;

                    conferenceAgendaModel.BeginDate = agenda.BeginDate;
                    conferenceAgendaModel.EndDate = agenda.EndDate;
                    conferenceAgendaModel.Title = agenda.Title;
                    conferenceAgendaModel.PeopleRemark = agenda.PeopleRemark;
                    conferenceAgendaModel.AddreddRemark = agenda.AddreddRemark;
                    conferenceAgendaModel.NeedSign = agenda.NeedSign;
                    if (agenda.NeedSign)
                    {
                        conferenceAgendaModel.SignBeginDate = agenda.SignBeginDate;
                        conferenceAgendaModel.SingEndDate = agenda.SingEndDate;
                    }
                    conferenceAgendaModel.Creator = CurrentUserCode;
                    conferenceAgendaModel.CreateTime = DateTime.Now;
                    conferenceAgendaModel.ValidStatus = true;
                    ConferenceAgendaAdapter.Instance.AddConferenceAgends(conferenceAgendaModel);
                }
                else
                {
                    conferenceAgenda.ConferenceID = agenda.ConferenceID;
                    conferenceAgenda.BeginDate = agenda.BeginDate;
                    conferenceAgenda.EndDate = agenda.EndDate;
                    conferenceAgenda.Title = agenda.Title;
                    conferenceAgenda.PeopleRemark = agenda.PeopleRemark;
                    conferenceAgenda.AddreddRemark = agenda.AddreddRemark;
                    conferenceAgenda.NeedSign = agenda.NeedSign;
                    if (agenda.NeedSign)
                    {
                        conferenceAgenda.SignBeginDate = agenda.SignBeginDate;
                        conferenceAgenda.SingEndDate = agenda.SingEndDate;
                    }
                    conferenceAgenda.Creator = CurrentUserCode;
                    conferenceAgenda.CreateTime = DateTime.Now;
                    conferenceAgenda.ValidStatus = true;
                    ConferenceAgendaAdapter.Instance.AddConferenceAgends(conferenceAgenda);
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 修改议程--PC
        /// </summary>
        /// <returns></returns>
        [Route("UpdateAgenda")]
        [HttpPost]
        public IHttpActionResult UpdateAgenda(ConferenceAgendaModel conferenceAgendaModel)
        {
            ViewsModel.ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                ConferenceAgendaAdapter.Instance.AddConferenceAgends(conferenceAgendaModel);
            });
            return Ok(result);
        }

        /// <summary>
        /// 删除议程--PC
        /// </summary>
        /// <param name="Id">编码</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DelAgenda(string Id)
        {
            ViewsModel.ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                ConferenceAgendaAdapter.Instance.Delete(m => { m.AppendItem("ID", Id); });
            });
            return Ok(result);
        }
    }
}
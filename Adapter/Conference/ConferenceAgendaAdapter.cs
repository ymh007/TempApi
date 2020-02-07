using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 会议议程
    /// </summary>
    public class ConferenceAgendaAdapter : UpdatableAndLoadableAdapterBase<ConferenceAgendaModel, ConferenceAgendaCollection>
    {
        public static readonly ConferenceAgendaAdapter Instance = new ConferenceAgendaAdapter();


        public ConferenceAgendaAdapter()
        {

        }
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 增加会议议程
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public void AddConferenceAgends(ConferenceAgendaModel conferenceAgendaModel)
        {
            Update(conferenceAgendaModel);
        }

        /// <summary>
        /// 查询议程详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ConferenceAgendaModel LoadbyCode(string Id)
        {
            return this.Load(p =>
            {
                p.AppendItem("ID", Id);
                p.AppendItem("ValidStatus", true);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 查询议程详情列表
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        public ConferenceAgendaCollection LoadListbyCode(string conferenceID)
        {
            return this.Load(p =>
            {
                p.AppendItem("ConferenceID", conferenceID);
            });
        }
    }
}
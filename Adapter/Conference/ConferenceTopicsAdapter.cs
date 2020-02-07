using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 会议议题
    /// </summary>
    public class ConferenceTopicsAdapter : BaseAdapter<ConferenceTopicsModel, ConferenceTopicsCollection>
    {
        public static readonly ConferenceTopicsAdapter Instance = new ConferenceTopicsAdapter();
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public ConferenceTopicsAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        /// <summary>
        /// 查询议题列表
        /// </summary>
        /// <param name="agendaID"></param>
        /// <returns></returns>
        public ConferenceTopicsCollection LoadByConferenceTopicsCollection(string agendaID)
        {
            return this.Load(p =>
            {
                p.AppendItem("AgendaID", agendaID);
            });
        }

        /// <summary>
        /// 查询单个议题
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ConferenceTopicsModel LoadByConferenceTopics(string id)
        {
            return this.Load(p =>
            {
                p.AppendItem("ID", id);
            }).FirstOrDefault();
        }

    }
}
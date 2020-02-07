using MCS.Library.SOA.DataObjects; 
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 
    /// </summary>
    public class TopicPersonAdapter : UpdatableAndLoadableAdapterBase<TopicPersonModels, TopicPersonCooection>
    {
        public static readonly TopicPersonAdapter Instance = new TopicPersonAdapter();

        public TopicPersonAdapter()
        {

        }
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }
        /// <summary>
        /// 根据ID删除议题讨论人数据
        /// </summary>
        /// <param name="conferenceTopicID"></param>
        public void DelTopicPersionByconferenceTopicID(string conferenceTopicID)
        {
            Delete(p => {
                p.AppendItem("ConferenceTopicID", conferenceTopicID);
            });
        }

        /// <summary>
        /// 根据ID删除议题讨论人数据
        /// </summary>
        /// <param name="conferenceTopicID"></param>
        public TopicPersonCooection GetTopicPersionByconferenceTopicID(string conferenceTopicID)
        {
            return this.Load(p=> {
                p.AppendItem("ConferenceTopicID", conferenceTopicID);
            });
        }
    }
}
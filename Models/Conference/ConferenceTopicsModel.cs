using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{

    /// <summary>
    /// 会议议题
    /// </summary>
    [ORTableMapping("office.ConferenceTopic")]
    public class ConferenceTopicsModel : ConferenceBase
    {
        /// <summary>
        /// 会议议程的ID
        /// </summary>
        [ORFieldMapping("AgendaID")]
        public string AgendaID { get; set; }

        /// <summary>
        /// 议题名称
        /// </summary>
        [ORFieldMapping("TopicName")]
        public string TopicName { get; set; }

        /// <summary>
        /// 主持人
        /// </summary>
        [ORFieldMapping("Presenter")]
        public string Presenter { get; set; }

        /// <summary>
        /// 引导师
        /// </summary>
        [ORFieldMapping("Guide")]
        public string Guide { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        [ORFieldMapping("Address")]
        public string Address { get; set; }

        /// <summary>
        /// 热议编码
        /// </summary>
        [ORFieldMapping("TopicCode")]
        public string TopicCode { get; set; }

        /// <summary>
        /// 议题讨论人
        /// </summary>
        [NoMapping]
        public TopicPersonCooection topicPersonColl = new TopicPersonCooection();

        [NoMapping]
        public string TopicImagePath
        {
            get
            {
                TopicModel topic = TopicModelAdapter.Instance.LoadByCode(this.TopicCode);
                return topic == null ? "" : topic.Image;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConferenceTopicsCollection : EditableDataObjectCollectionBase<ConferenceTopicsModel>
    {

    }
}
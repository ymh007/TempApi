using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    [ORTableMapping("office.TopicPerson")]
    public class TopicPersonModels : ConferenceBase
    {
        /// <summary>
        /// 会议议题的ID
        /// </summary>
        public string ConferenceTopicID { get; set; }

        /// <summary>
        /// 参会人ID
        /// </summary>
        public string AttendeeID { get; set; }
    }

    [Serializable]
    public class TopicPersonCooection : EditableDataObjectCollectionBase<TopicPersonModels> {

    }
}
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{

    /// <summary>
    /// 会议议程
    /// </summary>
    [ORTableMapping("office.Agenda")]
    public class ConferenceAgendaModel: ConferenceBase
    {

        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("BeginDate")]
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("EndDate")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 议程标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 人员备注
        /// </summary>
        [ORFieldMapping("PeopleRemark")]
        public string PeopleRemark { get; set; }

        /// <summary>
        /// 地点备注
        /// </summary>
        [ORFieldMapping("AddreddRemark")]
        public string AddreddRemark { get; set; }

        /// <summary>
        /// 是否签到
        /// </summary>
        [ORFieldMapping("NeedSign")]
        public bool NeedSign { get; set; }

        /// <summary>
        /// 签到开始时间
        /// </summary>
        [ORFieldMapping("SignBeginDate")]
        public DateTime SignBeginDate { get; set; }


        /// <summary>
        /// 签到结束时间
        /// </summary>
        [ORFieldMapping("SingEndDate")]
        public DateTime SingEndDate { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ConferenceAgendaCollection : EditableDataObjectCollectionBase<ConferenceAgendaModel>
    {
    }
}
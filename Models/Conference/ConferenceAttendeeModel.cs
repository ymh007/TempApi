using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    /// <summary>
    /// 会议参会人
    /// </summary>
    [ORTableMapping("office.Attendee")]
    public class ConferenceAttendeeModel
    {
        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }

        /// <summary>
        /// 参会人姓名
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 参会人邮箱
        /// </summary>
        [ORFieldMapping("Email")]
        public string Email { get; set; }

        /// <summary>
        /// 参会人电话
        /// </summary>
        [ORFieldMapping("MobilePhone")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// 参会人头像
        /// </summary>
        [ORFieldMapping("PhotoAddress")]
        public string PhotoAddress { get; set; }

        /// <summary>
        /// 参会人组织机构
        /// </summary>
        [ORFieldMapping("OrganizationStructure")]
        public string OrganizationStructure { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConferenceAttendeeCollection : EditableDataObjectCollectionBase<ConferenceAttendeeModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Adapter.Conference;

namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    /// <summary>
    /// 参会人
    /// </summary>
    [ORTableMapping("office.Attendee")]
    public class AttendeeModel : ConferenceBase
    {

        /// <summary>
        /// 参会人ID（UserCode）
        /// </summary>
        [ORFieldMapping("AttendeeID")]
        public string AttendeeID { get; set; }

        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ORFieldMapping("Email")]
        public string Email { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [NoMapping]
        public string LoginName
        {
            get
            {
                if (Email.IndexOf('@') > -1)
                {
                    return this.Email.Substring(0, this.Email.IndexOf('@'));
                }
                return Email;
            }
        }

        /// <summary>
        /// 电话
        /// </summary>
        [ORFieldMapping("MobilePhone")]
        public string MobilePhone { get; set; }

        /// <summary>
        /// 参会人头像
        /// </summary>
        [ORFieldMapping("PhotoAddress")]
        public string PhotoAddress { get; set; }

        /// <summary>
        /// 组织结构
        /// </summary>
        [ORFieldMapping("OrganizationStructure")]
        public string OrganizationStructure { get; set; }

        /// <summary>
        /// 组织机构ID
        /// </summary>
        [ORFieldMapping("OrganizationID")]
        public string OrganizationID { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [ORFieldMapping("CityName")]
        public string CityName { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [NoMapping]
        public string SeatAddress
        {
            get
            {
                SeatsModel seat = SeatsAdapter.Instance.GetUserSeatModel(this.ID, this.ConferenceID, this.AttendeeType);
                return seat == null ? "" : seat.SeatAddress;
            }
          

        }
        //为了解决会议批量排座位，与上面字段相同时候会被置空的问题
        [NoMapping]
        public string SeatAddress1 { get; set; }

        /// <summary>
        /// 是否参与抽奖
        /// </summary>
        [ORFieldMapping("IsJoinPrize")]
        public bool IsJoinPrize { get; set; }

        /// <summary>
        /// 参会人类型（1=开会、2=宴请）
        /// </summary>
        [ORFieldMapping("AttendeeType")]
        public int AttendeeType { get; set; }
     
    }

    /// <summary>
    /// 
    /// </summary>
    public class AttendeeCollection : EditableDataObjectCollectionBase<AttendeeModel>
    {

    }
}
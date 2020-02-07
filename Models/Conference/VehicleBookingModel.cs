using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 车辆预定
    /// </summary>
    [Serializable]
    [XElementSerializable]
    [ORTableMapping("Office.VehicleBooking")]
    public class VehicleBookingModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 预定时间
        /// </summary>
        [ORFieldMapping("ReserveTime")]
        public DateTime ReserveTime { get; set; }

        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }

        /// <summary>
        /// 出发地
        /// </summary>
        [ORFieldMapping("BeginPlace")]
        public string BeginPlace { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [ORFieldMapping("EndPlace")]
        public string EndPlace { get; set; }

        /// <summary>
        /// 预定者联系电话
        /// </summary>
        [ORFieldMapping("Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 预定者参会人ID
        /// </summary>
        [ORFieldMapping("AttendeeID")]
        public string AttendeeID { get; set; }
        /// <summary>
        /// 对接人姓名
        /// </summary>
        [ORFieldMapping("ContactName")]
        public string ContactName { get; set; }
        /// <summary>
        /// 对接人电话
        /// </summary>
        [ORFieldMapping("ContactPhone")]
        public string ContactPhone { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }
    [Serializable]
    public class VehicleBookingModelCollection : EditableDataObjectCollectionBase<VehicleBookingModel>
    {
    }
}
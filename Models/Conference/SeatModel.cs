using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{

    /// <summary>
    /// 参会人座位 Model
    /// </summary>
    [ORTableMapping("Office.Seats")]
    public class SeatsModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }
        /// <summary>
        /// 参会人ID
        /// </summary>
        [ORFieldMapping("AttendeeID")]
        public string AttendeeID { get; set; }
        /// <summary>
        /// 座位号
        /// </summary>
        [ORFieldMapping("SeatAddress")]
        public string SeatAddress { get; set; }
        /// <summary>
        /// 座位位置坐标编号
        /// </summary>
        [ORFieldMapping("SeatCoordinate")]
        public string SeatCoordinate { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
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

        /// <summary>
        /// 座位类型
        /// </summary>
        [ORFieldMapping("SeatType")]
        public int SeatType { get; set; }
    }

    /// <summary>
    /// 参会人座位 Collection
    /// </summary>
    public class SeatsCollection : EditableDataObjectCollectionBase<SeatsModel>
    {

    }
}
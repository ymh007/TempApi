using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;
using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 禁用座位记录表
    /// </summary>
    [ORTableMapping("Office.DisableSeats")]
    public class DisableSeatsModel : ConferenceBase
    {

        /// <summary>
        /// 会议ID
        /// </summary>
        /// 
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }


        /// <summary>
        /// 座位实际对应的座位分布图坐标编号
        /// </summary>
        /// 
        [ORFieldMapping("SeatAddress")]
        public string SeatAddress { get; set; }
     
    }

    /// <summary>
    /// 禁用座位记录集合
    /// </summary>
    public class DisableSeatsModelCollection : EditableDataObjectCollectionBase<DisableSeatsModel>
    {
    }
}
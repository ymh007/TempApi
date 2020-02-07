using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 业绩报表用户关注菜单表
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    [ORTableMapping("office.PerformanceReportUserFocus")]
    public class PerformanceReportUserFocusModel : BaseModel
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        [ORFieldMapping("ReportCode")]
        public string ReportCode { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }

    /// <summary>
    /// 业绩报表用户关注菜单集合
    /// </summary>
    public class PerformanceReportUserFocusCollection : EditableDataObjectCollectionBase<PerformanceReportUserFocusModel>
    {

    }
}
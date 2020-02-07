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
    /// 业绩报表菜单表
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    [ORTableMapping("office.PerformanceReportMenu")]
    public class PerformanceReportMenuModel : BaseModel
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        [ORFieldMapping("MenuId")]
        public string MenuId { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [ORFieldMapping("Href")]
        public string Href { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 状态标识
        /// </summary>
        [ORFieldMapping("Status")]
        public int Status { get; set; }

        /// <summary>
        /// IconUrl
        /// </summary>
        [ORFieldMapping("IconSrc")]
        public string IconSrc { get; set; }

        /// <summary>
        /// IconBase64
        /// </summary>
        [ORFieldMapping("IconResourceSrc")]
        public string IconResourceSrc { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        [ORFieldMapping("ModuleType")]
        public string ModuleType { get; set; }
    }

    /// <summary>
    /// 业绩报表菜单集合
    /// </summary>
    public class PerformanceReportMenuCollection : EditableDataObjectCollectionBase<PerformanceReportMenuModel>
    {

    }
}
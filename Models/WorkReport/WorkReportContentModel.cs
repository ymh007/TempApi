using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.WorkReport
{
    /// <summary>
    /// 汇报内容
    /// </summary>
    [ORTableMapping("office.WorkReportContent")]
    public class WorkReportContentModel : BaseModel
    {
        /// <summary>
        /// 汇报编码
        /// </summary>
        [ORFieldMapping("ReportCode")]
        public string ReportCode { get; set; }
        /// <summary>
        /// 模板标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WorkReportContentCollection : EditableDataObjectCollectionBase<WorkReportContentModel>
    {


    }
}
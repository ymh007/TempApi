using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.WorkReport
{
    /// <summary>
    /// 模板
    /// </summary>
      [ORTableMapping("office.WorkReportTemplate")]
    public class WorkReportTemplateModel : BaseModel
    {
        /// <summary>
        /// 模板标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }
        /// <summary>
        /// 是否是系统
        /// </summary>
        [ORFieldMapping("IsSystem")]
        public bool IsSystem { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class WorkReportTemplateCollection : EditableDataObjectCollectionBase<WorkReportTemplateModel>
    {

    }
}
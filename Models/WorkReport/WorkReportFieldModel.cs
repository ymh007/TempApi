using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.WorkReport
{
    /// <summary>
    /// 模板字符
    /// </summary>
    [ORTableMapping("office.WorkReportField")]
    public class WorkReportFieldModel : BaseModel
    {


        /// <summary>
        /// 模板编码
        /// </summary>
        [ORFieldMapping("TemplateCode")]
        public string TemplateCode { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class WorkReportFieldCollection : EditableDataObjectCollectionBase<WorkReportFieldModel>
    {


    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.WorkReport
{
    /// <summary>
    /// 
    /// </summary>
    [ORTableMapping("office.WorkReportDetails")]
    public class WorkReportDetailsModel : BaseModel
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
        /// 备注
        /// </summary>
        [ORFieldMapping("Mark")]
        public string Mark { get; set; }
        /// <summary>
        /// 接收人详情
        /// </summary>
        [ORFieldMapping("ReceiverDetails")]
        public string ReceiverDetails { get; set; }

        /// <summary>
        /// 抄送人详情
        /// </summary>
        [ORFieldMapping("CopyDetails")]
        public string CopyDetails { get; set; }

        /// <summary>
        /// 转发人详情
        /// </summary>
        [ORFieldMapping("ForwardDetails")]
        public string ForwardDetails { get; set; }
        /// <summary>
        /// 是否阅读
        /// </summary>
        [ORFieldMapping("IsRead")]
        public bool IsRead { get; set; }
        /// <summary>
        /// 是否原件
        /// </summary>
        [ORFieldMapping("IsOriginal")]
        public bool IsOriginal { get; set; }
        /// <summary>
        /// 是否是发出者
        /// </summary>
        [ORFieldMapping("IsSender")]
        public bool IsSender { get; set; }
        /// <summary>
        /// 转发标题
        /// </summary>
        [ORFieldMapping("CopyDetails")]
        public string ForwardTitle { get; set; }
        /// <summary>
        /// 发送人名
        /// </summary>
        [ORFieldMapping("CreateName")]
        public string CreateName { get; set; }
        /// <summary>
        /// 接收人名
        /// </summary>
        [ORFieldMapping("ReceiveName")]
        public string ReceiveName { get; set; }
        /// <summary>
        /// 接收人编码
        /// </summary>
        [ORFieldMapping("ReceiveCode")]
        public string ReceiveCode { get; set; }
       
    }
    /// <summary>
    /// 
    /// </summary>
    public class WorkReportDetailsCollection : EditableDataObjectCollectionBase<WorkReportDetailsModel>
    {


    }
}
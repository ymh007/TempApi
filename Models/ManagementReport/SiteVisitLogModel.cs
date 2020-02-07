using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.ManagementReport
{
    /// <summary>
    /// 访问记录实体
    /// </summary>
    [ORTableMapping("dbo.SITE_VISIT_LOG")]
    public class SiteVisitLogModel
    {
        /// <summary>
        /// Url
        /// </summary>
        [ORFieldMapping("URL")]
        public string Url { set; get; }
        /// <summary>
        /// 用户名（如：liumh）
        /// </summary>
        [ORFieldMapping("USER_NAME")]
        public string UserName { set; get; }
        /// <summary>
        /// 访问时间
        /// </summary>
        [ORFieldMapping("DROP_TIME")]
        public DateTime DropTime { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("TITLE")]
        public string Title { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        [ORFieldMapping("SOURCE")]
        public string Source { set; get; }
    }

    /// <summary>
    /// 访问记录列表
    /// </summary>
    public class SiteVisitLogCollection : EditableDataObjectCollectionBase<SiteVisitLogModel>
    {

    }
}
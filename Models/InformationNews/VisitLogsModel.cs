using System;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 访问日志Model实体
    /// </summary>
    [ORTableMapping("dbo.SITE_VISIT_LOG")]
    public class VisitLogsModel
    {
        /// <summary>
        /// 文章URL
        /// </summary>
        [ORFieldMapping("URL")]
        public string URL { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ORFieldMapping("USER_NAME")]
        public string USER_NAME { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        [ORFieldMapping("DROP_TIME")]
        public DateTime DROP_TIME { get; set; }

        /// <summary>
        /// 文章标题
        /// </summary>
        [ORFieldMapping("TITLE")]
        public string TITLE { get; set; }

        /// <summary>
        /// 访问来源
        /// </summary>
        [ORFieldMapping("SOURCE")]
        public string SOURCE { get; set; }
    }

    /// <summary>
    /// 访问日志集合
    /// </summary>
    public class VisitLogsCollection : EditableDataObjectCollectionBase<VisitLogsModel>
    {

    }
}
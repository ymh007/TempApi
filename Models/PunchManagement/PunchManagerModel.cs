using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.PunchManagement
{
    /// <summary>
    /// 打卡管理人员
    /// </summary>
    [ORTableMapping("dbo.PunchManager")]
    public class PunchManagerModel :BaseModel
    {
        /// <summary>
        /// 打卡管理code
        /// </summary>
        [ORFieldMapping("PunchCode")]
        public string PunchCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
        /// <summary>
        /// 联系人code
        /// </summary>
        [ORFieldMapping("ConcatCode")]
        public string ConcatCode { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PunchManagerCollection : EditableDataObjectCollectionBase<PunchManagerModel>
    {

    }
}
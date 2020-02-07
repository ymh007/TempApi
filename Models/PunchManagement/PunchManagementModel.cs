using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.PunchManagement
{
    /// <summary>
    /// 打卡管理
    /// </summary>
    [ORTableMapping("dbo.PunchManagement")]
    public class PunchManagementModel : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }

        /// <summary>
        /// 创建者名字
        /// </summary>
        [ORFieldMapping("CreatorName")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 上班时间
        /// </summary>
        [ORFieldMapping("OnTime")]
        public string OnTime { get; set; }

        /// <summary>
        /// 下班时间
        /// </summary>
        [ORFieldMapping("OffTime")]
        public string OffTime { get; set; }

        /// <summary>
        /// 打卡范围
        /// </summary>
        [ORFieldMapping("PunchArea")]
        public int PunchArea { get; set; }

        /// <summary>
        /// 打卡部门/人员是否改变
        /// </summary>
        [ORFieldMapping("IsChange")]
        public bool IsChange { set; get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PunchManagementCollection : EditableDataObjectCollectionBase<PunchManagementModel>
    {

    }
}
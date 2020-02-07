using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.PunchManagement
{
    /// <summary>
    /// 打卡部门
    /// </summary>
    [ORTableMapping("dbo.PunchDepartment")]
    public class PunchDepartmentModel : BaseModel
    {
        /// <summary>
        /// 打卡管理code
        /// </summary>
        [ORFieldMapping("PunchCode")]
        public string PunchCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public string Type { get; set; }
        /// <summary>
        /// 联系人或部门code
        /// </summary>
        [ORFieldMapping("ConcatCode")]
        public string ConcatCode { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class PunchDepartmentCollection : EditableDataObjectCollectionBase<PunchDepartmentModel>
    {

    }
     

}
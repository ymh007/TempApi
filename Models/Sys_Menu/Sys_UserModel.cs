using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Sys_Menu
{
    /// <summary>
    /// 用户
    /// </summary>
    [ORTableMapping("office.Sys_User")]
    public class Sys_UserModel : BaseModel
    {
        /// <summary>
        /// 用户code
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户code
        /// </summary>
        [ORFieldMapping("Account")]
        public string Account { get; set; }

        /// <summary>
        /// 是否为超管
        /// </summary>
        [ORFieldMapping("Super")]
        public bool Super { get; set; }

        /// <summary>
        /// 是否为员工考勤超管
        /// </summary>
        [ORFieldMapping("IsPunchSuper")]
        public bool IsPunchSuper { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnabled")]
        public bool IsEnabled { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Sys_UserCollection : EditableDataObjectCollectionBase<Sys_UserModel>
    {

    }
}
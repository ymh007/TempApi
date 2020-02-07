using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Sys_Menu
{
    /// <summary>
    /// 
    /// </summary>
    /// <summary>
    /// 菜单
    /// </summary>
    [ORTableMapping("office.Sys_UserMenu")]
    public class Sys_UserMenuModel : BaseModel
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
        /// 菜单code
        /// </summary>
        [ORFieldMapping("MenuCode")]
        public string MenuCode { get; set; }

        /// <summary>
        /// 组织架构
        /// </summary>
        [ORFieldMapping("FullPath")]
        public string FullPath { get; set; }

       


    }

    /// <summary>
    /// 
    /// </summary>
    public class Sys_UserMenuCollection : EditableDataObjectCollectionBase<Sys_UserMenuModel>
    {

    }
}
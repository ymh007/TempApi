using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Sys_Menu
{
    /// <summary>
    /// 菜单
    /// </summary>
    [ORTableMapping("office.Sys_Menu")]
    public class Sys_MenuModel : BaseModel
    {

        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 父菜单code
        /// </summary>
        [ORFieldMapping("ParentCode")]
        public string ParentCode { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [ORFieldMapping("Icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 键值Key
        /// </summary>
        [ORFieldMapping("Key")]
        public string Key { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }
   /// <summary>
   /// 
   /// </summary>
    public class Sys_MenuCollection : EditableDataObjectCollectionBase<Sys_MenuModel>
    {

    }
}
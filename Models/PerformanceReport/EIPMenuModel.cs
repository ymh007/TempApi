using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 海鸥Ⅱ菜单实体类
    /// </summary>
    public class EIPMenuModel
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 父级ID
        /// </summary>
        public string ParentID { set; get; }
    }

    /// <summary>
    /// 海鸥Ⅱ菜单集合
    /// </summary>
    public class EIPMenuCollection : EditableDataObjectCollectionBase<EIPMenuModel>
    {

    }
}
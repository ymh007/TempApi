using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Seagull2.YuanXin.AppApi.Models.Share
{
    /// <summary>
    /// 菜单
    /// </summary>
    [ORTableMapping("office.S_Menu")]
    public class MenuModel : BaseModel
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        [ORFieldMapping("ParentCode")]
        public string ParentCode { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }

    /// <summary>
    /// 菜单集
    /// </summary>
    public class MenuCollection : EditableDataObjectCollectionBase<MenuModel>
    {

    }
}
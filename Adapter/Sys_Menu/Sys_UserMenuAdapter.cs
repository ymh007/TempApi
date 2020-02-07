using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Sys_Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Sys_Menu
{
    /// <summary>
    /// 用户菜单
    /// </summary>
    public class Sys_UserMenuAdapter : UpdatableAndLoadableAdapterBase<Sys_UserMenuModel, Sys_UserMenuCollection>
    {

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly Sys_UserMenuAdapter Instance = new Sys_UserMenuAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;
    }
}
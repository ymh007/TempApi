using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Sys_Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Sys_Menu
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    public class Sys_MenuAdapater : UpdatableAndLoadableAdapterBase<Sys_MenuModel, Sys_MenuCollection>
    {
        /// <summary>
		/// 实例
		/// </summary>
		public static readonly Sys_MenuAdapater Instance = new Sys_MenuAdapater();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;
    }
}
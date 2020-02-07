using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Share
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuViewModel : MenuBaseViewModel
    {
        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuBaseViewModel> SubMenu { set; get; }
    }

    /// <summary>
    /// 菜单基本信息
    /// </summary>
    public class MenuBaseViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
    }
}
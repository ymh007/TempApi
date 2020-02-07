using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu
{
    /// <summary>
    /// 用户列表
    /// </summary>
    public class SysUserAuthorityViewModel
    {
        /// <summary>
        ///  编码code
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        ///用户编码
        /// </summary>
        public string UserCode { set; get; }

        /// <summary>
        ///用户名
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        ///账号
        /// </summary>
        public string AccountNumber { set; get; }

        /// <summary>
        ///是否启用
        /// </summary>
        public bool IsEnabled { set; get; }

        /// <summary>
        ///超级管理员
        /// </summary>
        public bool Super { set; get; }

        /// <summary>
        /// 是否为员工考勤超管
        /// </summary>
        public bool IsPunchSuper { set; get; }

        /// <summary>
        ///CreateTime
        /// </summary>
        public DateTime CreateTime { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public List<MenuList> UserMenuList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ValidStatus { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public class MenuList
        {

            /// <summary>
            ///  功能code
            /// </summary>
            public string MenuCode { get; set; }
        }

    }
}
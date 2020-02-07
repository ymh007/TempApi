using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu
{
    /// <summary>
    /// 
    /// </summary>
    public class Sys_UserViewModel
    {
        /// <summary>
        /// 权限编码
        /// </summary>
        public string UserAuthorityCode { get; set; }
        /// <summary>
        /// 用户域帐号
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// 是否超级管理员
        /// </summary>
        public bool Super { get; set; }
        /// <summary>
        /// 是否为员工考勤超管
        /// </summary>
        public bool IsPunchSuper { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 权限列表
        /// </summary>
        public List<RoleList> UserFuncList { get; set; }


        /// <summary>
        /// 权限
        /// </summary>
        public class RoleList
        {
            /// <summary>
            ///  功能code
            /// </summary>
            public string SystemCode { get; set; }
        }
    }
}
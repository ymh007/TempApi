using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Employee
{
    /// <summary>
    /// 人员信息 ViewModel
    /// </summary>
    public class EmployeeViewModel
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string EnName { set; get; }
        /// <summary>
        /// 域帐号
        /// </summary>
        public string CnName { set; get; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phone { set; get; }
    }

    /// <summary>
    /// 人员信息 ViewModel PC
    /// </summary>
    public class EmployeeListViewModel
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 域帐号
        /// </summary>
        public string CnName { set; get; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string EnName { set; get; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 组织机构路径
        /// </summary>
        public string FullPath { set; get; }
        /// <summary>
        /// 邀请次数
        /// </summary>
        public int InviteCount { set; get; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsValid { set; get; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// 版本
        /// </summary>
        public string AppVersion { set; get; }


        ///// <summary>
        ///// 内部排序
        ///// </summary>
        //public int InnerSort { set; get; }
    }

    /// <summary>
    /// 人员信息 ViewModel PC
    /// </summary>
    public class EmailViewModel
    {
        /// <summary>
        /// 邮件名
        /// </summary>
        public string EmailTitle { set; get; }
        /// <summary>
        /// 邮件主题
        /// </summary>
        public string EmailBody { set; get; }
        /// <summary>
        /// 邮件主题
        /// </summary>
        public string EmailTheme { set; get; }
        /// <summary>
        /// 要发送的用户
        /// </summary>
        public string User { set; get; }
    }

}
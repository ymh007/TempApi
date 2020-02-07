using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 定义连接名称
    /// </summary>
    public class ConnectionNameDefine
    {
        /// <summary>
        /// 
        /// </summary>
        public const string EmployeeServiceConnectionName = "EmployeeService";
        /// <summary>
        /// 员工考勤
        /// </summary>
        public const string EmployeeAttendance = "EmployeeAttendance";
        /// <summary>
        /// 移动办公
        /// </summary>
        public const string YuanXinBusiness = "YuanXinBusiness";
        /// <summary>
        /// 
        /// </summary>
        public const string YuanXinForDBHelp = "yuanxin";
        /// <summary>
        /// SinooceanLandAddressList
        /// </summary>
        public const string SinooceanLandAddressList = "SinooceanLandAddressList";
        /// <summary>
        /// 流程中心
        /// </summary>
        public const string MCS_WORKFLOW = "MCS_WORKFLOW";

        /// <summary>
        /// 通讯录
        /// </summary>
        public const string MCS_ReportDB = "MCS_ReportDB";
        /// <summary>
        /// 
        /// </summary>
        public const string HB2008 = "HB2008";
        /// <summary>
        /// IM
        /// </summary>
        public const string IMConnectionNam = "YuanXinIM";
        /// <summary>
        /// 海鸥二发票
        /// </summary>
        public const string SubjectDB_InvoiceHeader = "SubjectDB_InvoiceHeader";

        /// <summary>
        /// 计划管理II期
        /// </summary>
        public const string SubjectDB_PlanManage = "SubjectDB_PlanManage";

        /// <summary>
        /// 员工服务
        /// </summary>
        public const string SubjectDB_EmpService = "SubjectDB_EmpService";

    }
}
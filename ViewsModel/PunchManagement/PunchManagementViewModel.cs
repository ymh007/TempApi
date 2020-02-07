using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.PunchManagement
{
    /// <summary>
    /// 打卡管理viewModel
    /// </summary>
    public class PunchManagementViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 创建者编码
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string OnTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public string OffTime { get; set; }
        /// <summary>
        /// 打卡范围
        /// </summary>
        public int PunchArea { get; set; }
        /// <summary>
        /// 打卡部门/人员是否改变
        /// </summary>
        public bool IsChange { set; get; }
        /// <summary>
        /// 有效状态
        /// </summary>
        public bool ValidStatus { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public List<PunchDepartmentListItem> PunchDepartmentList { get; set; }
        /// <summary>
        /// 管理人员
        /// </summary>
        public List<PunchManagerListItem> PunchManagerList { get; set; }
    }

    /// <summary>
    /// 考勤对象
    /// </summary>
    public class PunchDepartmentListItem
    {
        /// <summary>
        /// 考勤对象编码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 类型 Users 或 Organizations
        /// </summary>
        public string ObjectType { get; set; }
    }

    /// <summary>
    /// 打卡管理员
    /// </summary>
    public class PunchManagerListItem
    {
        /// <summary>
        /// 管理员编码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 管理员名称
        /// </summary>
        public string DisplayName { get; set; }
    }
}
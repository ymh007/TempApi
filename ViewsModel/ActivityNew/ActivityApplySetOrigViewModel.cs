using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew
{
    /// <summary>
    /// 活动报名范围设置保存
    /// </summary>
    public class ActivityApplySetOrigSaveViewModel
    {
        /// <summary>
        /// 用户/部门编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 类型（Users 或 Organizations）
        /// </summary>
        public Type Type { get; set; }
    }

    /// <summary>
    /// 活动报名范围设置显示
    /// </summary>
    public class ActivityApplySetOrigShowViewModel
    {
        /// <summary>
        /// 用户/部门编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 类型（Users 或 Organizations）
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// 选择类型（用户/组织）
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// 用户
        /// </summary>
        Users,
        /// <summary>
        /// 组织
        /// </summary>
        Organizations
    }
}
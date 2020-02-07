using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 工作通菜单分类
    /// </summary>
    public enum EnumWorkRegionMenuCategory
    {
        /// <summary>
        /// 高效办公
        /// </summary>
        [EnumItemDescription("高效办公", SortId = 1)]
        EfficientOffice = 1,

        /// <summary>
        /// 专业应用
        /// </summary>
        [EnumItemDescription("专业应用", SortId = 2)]
        ProfessionalApplication = 2,


        /// <summary>
        /// 员工服务
        /// </summary>
        [EnumItemDescription("员工服务", SortId = 3)]
        EmployeeService = 3,


        /// <summary>
        /// 数据与信息
        /// </summary>
        [EnumItemDescription("数据与信息", SortId = 4)]
        DataInformation = 4,


        /// <summary>
        /// 学习中心
        /// </summary>
        [EnumItemDescription("学习中心", SortId = 5)]
        LearningCenter = 5


    }
    /// <summary>
    /// 权限模块分类
    /// </summary>
    public enum EnumPermissionCategory
    {
        /// <summary>
        /// 工作通菜单
        /// </summary>
        WorkRegionMenu = 1,

        /// <summary>
        /// 广告
        /// </summary>
        Advertisement = 2,

        
    }
}
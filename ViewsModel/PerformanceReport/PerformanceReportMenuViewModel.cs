using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Seagull2.YuanXin.AppApi.Models.ModelExtention;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// 业绩报表视图实体类 For PC
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    public class PerformanceReportMenuViewModelPC
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MenuId { set; get; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { set; get; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Href { set; get; }
        /// <summary>
        /// 排序数字
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        /// 状态标识
        /// </summary>
        public int Status { set; get; }
        /// <summary>
        /// 状态描述
        /// </summary>
        public string StatusName
        {
            get
            {
                return GetEnumDescriptionByValue<Enum.EnumStatus>(Status);
            }
        }
        /// <summary>
        /// 图标URL
        /// </summary>
        public string IconSrc { set; get; }
        /// <summary>
        /// 图标Base64
        /// </summary>
        public string IconResourceSrc { set; get; }
        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleType { set; get; }
    }

    /// <summary>
    /// 业绩报表视图实体类 For APP
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    public class PerformanceReportMenuViewModelAPP
    {
        /// <summary>
        /// 构造
        /// </summary>
        public PerformanceReportMenuViewModelAPP()
        {
            ChildNodes = new List<ViewsModel.PerformanceReportMenuViewModelAPP>();
        }

        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MenuId { set; get; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { set; get; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Href { set; get; }
        /// <summary>
        /// 图标URL
        /// </summary>
        public string IconSrc { set; get; }
        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsFocus { set; get; }
        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<PerformanceReportMenuViewModelAPP> ChildNodes { set; get; }
    }

    /// <summary>
    /// 用户菜单返回两个List，分别是用户全部菜单、用户关注菜单
    /// </summary>
    public class PerformanceReportMenuViewModelAPPMenuList
    {
        /// <summary>
        /// 全部菜单
        /// </summary>
        public object ListAll { set; get; }
        /// <summary>
        /// 关注菜单
        /// </summary>
        public object ListFocus { set; get; }
    }

    /// <summary>
    /// 关注、取消Post请求接收对象
    /// </summary>
    [Serializable]
    public class PerformanceReportMenuViewModelAPPPost
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        public string MenuId { set; get; }
    }
}
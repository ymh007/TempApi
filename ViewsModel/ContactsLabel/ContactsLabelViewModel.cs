using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ContactsLabel
{
    #region 标签保存-ViewModel
    /// <summary>
    /// 通讯录标签保存-ViewModel
    /// </summary>
    public class ContactsLabelSaveViewModel
    {
        /// <summary>
        /// 标签Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标签人员
        /// </summary>
        public List<ContactsLabelSavePersonViewModel> Persons { get; set; }
    }
    /// <summary>
    /// 标签人员
    /// </summary>
    public class ContactsLabelSavePersonViewModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }
    }
    #endregion

    #region 标签列表-ViewModel
    /// <summary>
    /// 标签列表-ViewModel
    /// </summary>
    public class ContactsLabelListViewModel
    {
        /// <summary>
        /// 标签编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 人员数量
        /// </summary>
        public int Count { get { return List.Count; } }
        /// <summary>
        /// 标签列表人员集合
        /// </summary>
        public List<ContactsLabelListPersonViewModel> List { get; set; }
    }

    /// <summary>
    /// 标签列表人员-ViewModel
    /// </summary>
    public class ContactsLabelListPersonViewModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                var arr = FullPath.Split('\\');
                if (arr.Length >= 2)
                {
                    return arr[arr.Length - 2];
                }
                return string.Empty;
            }
        }
    }
    #endregion

    #region 人员详情标签信息-ViewModel
    public class LabelInfo
    {
        /// <summary>
        /// 标签编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// 标签详情
    /// </summary>
    public class LabelDetails
    {
        /// <summary>
        /// 已经选择的
        /// </summary>
        public List<LabelInfo> IsChoice { get; set; }
        /// <summary>
        /// 没有选择的
        /// </summary>
        public List<LabelInfo> NoChoice { get; set; }
    }

    #endregion

    #region 设置人员标签
    public class SeetingLabelInfo
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 标签编码
        /// </summary>
        public List<LabelInfo> Choice { get; set; }
    }
    #endregion
}
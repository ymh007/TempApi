using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.AddressBook
{
    #region 通讯录通用用户信息类 ViewModel
    /// <summary>
    /// 通讯录通用用户信息类 ViewModel
    /// </summary>
    public class UserInfoViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 所属组织机构
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 帐号
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { set; get; }
        /// <summary>
        /// 组织机构路径
        /// </summary>
        public string FullPath { set; get; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
                try
                {
                    var array = FullPath.Split('\\');
                    return array[array.Length - 2];
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// 所有子级人员数量
        /// </summary>
        public int UserCount
        {
            get
            {
                if (ObjectType == "Organizations")
                {
                    return Adapter.AddressBook.ContactsAdapter.Instance.GetUserCountByPName(FullPath);
                }
                return 0;
            }
        }
        /// <summary>
        /// 子部门数量
        /// </summary>
        public int ChildOrgCount
        {
            get
            {
                if (ObjectType == "Organizations")
                {
                    return Adapter.AddressBook.ContactsAdapter.Instance.LoadOrganizations(ID).Count;
                }
                return 0;
            }
        }
        /// <summary>
        ///用户头像
        /// </summary>
        public string UserHeadUrl
        {
            get
            {
                if (ObjectType == "Users")
                {
                    return UserHeadPhotoService.GetUserHeadPhoto(ID);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static List<UserInfoViewModel> fromTable(System.Data.DataTable table)
        {
            List<UserInfoViewModel> list = new List<UserInfoViewModel>();
            try
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    list.Add(new UserInfoViewModel()
                    {
                        ID = table.Rows[i]["ID"].ToString(),
                        DisplayName = table.Rows[i]["DISPLAY_NAME"].ToString(),
                        Name = "",
                        ObjectType = table.Rows[i]["ObjectType"].ToString(),
                        Phone = table.Rows[i]["MOBILE"].ToString(),
                        Email = table.Rows[i]["INTERNET_EMAIL"].ToString()
                    });
                }
            }
            catch
            {

            }
            return list;
        }
    }
    #endregion

    #region 我的部门列表 ViewModel
    /// <summary>
    /// 我的部门列表 ViewModel
    /// </summary>
    public class MyDepartmentViewModel
    {
        /// <summary>
        /// 部门编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name
        {
            get
            {
                try
                {
                    var array = FullPath.Split('\\');
                    return array[array.Length - 2];
                }
                catch
                {
                    return "未知";
                }
            }
        }
        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get { return "Organizations"; } }
    }
    #endregion

    #region 将组织机构、标签转换成人员 ViewModel
    /// <summary>
    /// 将组织机构、标签转换成人员 ViewModel
    /// </summary>
    public class ConvertToUserViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public DataType Type { set; get; }
        /// <summary>
        /// 类型枚举
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// 组织机构
            /// </summary>
            Orgization = 0,
            /// <summary>
            /// 用户
            /// </summary>
            User = 1,
            /// <summary>
            /// 标签
            /// </summary>
            Label = 2
        }
    }
    /// <summary>
    /// 将组织机构、标签转换成人员 结果 ViewModel
    /// </summary>
    public class ConvertToUserResultViewModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LogonName { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { set; get; }
        /// <summary>
        /// 标签，固定格式
        /// </summary>
        public string Tag { get { return "Convert"; } }
    }
    #endregion

    /// <summary>
    /// 通讯录
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// 对象的ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 对象的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对象的类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { set; get; }
    }

    /// <summary>
    /// 联系人列表
    /// </summary>
    public class ContactsViewModel
    {
        public List<Contact> DataList { get; set; }
    }

    /// <summary>
    /// 组织机构信息
    /// </summary>
    public class Organizations
    {
        public string ObjectID { get; set; }

        public string DisPlayName { get; set; }
    }

    /// <summary>
    /// 组织机构信息和人员信息
    /// </summary>
    public class OrganizationsInfo
    {
        public List<Organizations> OrganList { get; set; }

        public List<UserInfoViewModel> UserList { get; set; }
    }
}
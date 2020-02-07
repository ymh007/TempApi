using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 找朋友个人信息
    /// </summary>
    public class UsersInfoExtend
    {
        /// <summary>
        /// 获取或设置 
        /// </summary>

        public string ID { get; set; }

        /// <summary>
        /// 获取或设置 性别
        /// </summary>

        public string GENDER { get; set; }

        /// <summary>
        /// 获取或设置 民族
        /// </summary>

        public string NATION { get; set; }

        /// <summary>
        /// 获取或设置 出生日期
        /// </summary>

        public DateTime BIRTHDAY { get; set; }

        /// <summary>
        /// 获取或设置 入职日期
        /// </summary>

        public DateTime StartWorkTime { get; set; }

        /// <summary>
        /// 获取或设置 手机
        /// </summary>

        public string MOBILE { get; set; }

        /// <summary>
        /// 获取或设置 手机
        /// </summary>

        public string MOBILE2 { get; set; }

        /// <summary>
        /// 获取或设置 办公室电话
        /// </summary>

        public string OfficeTel { get; set; }

        /// <summary>
        /// 获取或设置 外网Email
        /// </summary>

        public string InternetEmail { get; set; }

        /// <summary>
        /// 获取或设置 即时通讯地址
        /// </summary>

        public string ImAddress { get; set; }

        /// <summary>
        /// 获取或设置 
        /// </summary>

        public string MEMO { get; set; }

        /// <summary>
        /// 获取或设置 个人签名图片路径
        /// </summary>

        public string SignImagePath { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>

        public string CodeName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>

        public string DisplayName { get; set; }

        /// <summary>
        /// 全部路径
        /// </summary>

        public string FullPath { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName
        {
            get
            {
				if (string.IsNullOrEmpty(this.FullPath))
				{
					return "";
				}
                var arr = this.FullPath.Split('\\');
                if (arr.Length > 1)
                {
                    if (this.DisplayName.Contains("已离职"))
                    {
                        return arr[arr.Length - 2];
                    }
                    return arr[arr.Length - 1];
                }
                else
                {
                    return "";
                }

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UsersInfoExtendCollection : EditableDataObjectCollectionBase<UsersInfoExtend>
    {

    }
}
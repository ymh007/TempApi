using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class UserDetailModel
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
        /// 对象的显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LogonName { get; set; }

        /// <summary>
        /// 全路径
        /// </summary>
        public string FullPath { get; set; }

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
        public string BIRTHDAY { get; set; }

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
        /// 获取或设置 入职日期
        /// </summary>
        public string StartWorkTime { get; set; }

        /// <summary>
        /// 内网Email
        /// </summary>
        public string InnerEmail { get; set; }

        /// <summary>
        /// 外网Email
        /// </summary>
        public string ExtendEmail { get; set; }

        /// <summary>
        /// 是否为常用联系人
        /// </summary>
        public bool  IsContact { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 使用版本
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// 邀请次数
        /// </summary>
        public int InviteCount { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHeadUrl { get; set; }
    }
}
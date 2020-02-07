using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Contacts
{
    /// <summary>
    /// 客服人员ViewModel
    /// </summary>
    public class ContactsViewModel
    {
        /// <summary>
        /// 人员Code
        /// </summary>
        public string ObjectID { get; set; }

        /// <summary>
        /// 所属组织机构
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string LOGIN_Name { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 组织机构路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mail { get; set; }


        /// <summary>
        /// 住宅电话
        /// </summary>
        public string WP { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string MP { get; set; }

        /// <summary>
        /// 组织机构路径编码
        /// </summary>
        public string GlobalSort { get; set; }

        /// <summary>
        /// 客服人员头像
        /// </summary>
        public string UserHead
        {
            get
            {
                return UserHeadPhotoService.GetUserHeadPhoto(ObjectID);
            }
        }
    }
}
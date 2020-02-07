using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook
{
    /// <summary>
    /// 通讯录 Model
    /// </summary>
    [ORTableMapping("dbo.Contacts")]
    public class ContactsModel
    {
        /// <summary>
        /// 主键编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { set; get; }

        /// <summary>
        /// 人员Code
        /// </summary>
        [ORFieldMapping("ObjectID")]
        public string ObjectID { get; set; }

        /// <summary>
        /// 所属组织机构
        /// </summary>
        [ORFieldMapping("ParentID")]
        public string ParentID { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        [ORFieldMapping("LOGON_NAME")]
        public string Logon_Name { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// 组织机构路径
        /// </summary>
        [ORFieldMapping("FullPath")]
        public string FullPath { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ORFieldMapping("Mail")]
        public string Mail { get; set; }

        /// <summary>
        /// 住宅电话
        /// </summary>
        [ORFieldMapping("WP")]
        public string WP { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [ORFieldMapping("MP")]
        public string MP { get; set; }

        /// <summary>
        /// 全局排序
        /// </summary>
        [ORFieldMapping("GlobalSort")]
        public string GlobalSort { get; set; }

        /// <summary>
        /// 内部排序
        /// </summary>
        [ORFieldMapping("InnerSort")]
        public int InnerSort { set; get; }

        /// <summary>
        /// 类型（Organizations、Users）
        /// </summary>
        [ORFieldMapping("SchemaType")]
        public string SchemaType { set; get; }

        /// <summary>
        /// 职位名称
        /// </summary>
        [ORFieldMapping("EmploymentName")]
        public string EmploymentName { set; get; }

        /// <summary>
        /// 职能类别
        /// </summary>
        [ORFieldMapping("StationCategory")]
        public string StationCategory { set; get; }

        /// <summary>
        /// 职能类别全路径
        /// </summary>
        [ORFieldMapping("FullPathStationCategory")]
        public string FullPathStationCategory { set; get; }

        /// <summary>
        /// 是否为主职
        /// </summary>
        [ORFieldMapping("IsDefault")]
        public int IsDefault { get; set; }
    }

    /// <summary>
    /// 通讯录 Collection
    /// </summary>
    public class ContactsCollection : EditableDataObjectCollectionBase<ContactsModel>
    {
        /// <summary>
        /// 将 ContactsCollection 转换为 ViewsModel.AddressBook.UserInfoViewModel 集合
        /// </summary>
        /// <returns></returns>
        public List<ViewsModel.AddressBook.UserInfoViewModel> ToUserInfoViewModelList()
        {
            var list = new List<ViewsModel.AddressBook.UserInfoViewModel>();
            ForEach(item =>
            {
                list.Add(new ViewsModel.AddressBook.UserInfoViewModel()
                {
                    ID = item.ObjectID,
                    DisplayName = item.DisplayName,
                    ParentID=item.ParentID,
                    Name = item.Logon_Name,
                    ObjectType = item.SchemaType,
                    Email = item.Mail,
                    Phone = item.MP,
                    FullPath = item.FullPath
                });
            });
            return list;
        }
    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.ContactsLabel
{
    /// <summary>
    /// 标签人员Model
    /// </summary>
    [ORTableMapping("office.ContactsLabelPerson")]
    public class ContactsLabelPersonModel
    {
		/// <summary>
		/// 主键编码
		/// </summary>
		[ORFieldMapping("Code", PrimaryKey = true)]
		public string Code { get; set; }

		/// <summary>
		/// 标签编码
		/// </summary>
		[ORFieldMapping("LabelCode")]
        public string LabelCode { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 组织路径
        /// </summary>
        [ORFieldMapping("FullPath")]
        public string FullPath { get; set; }

		/// <summary>
		/// 创建人
		/// </summary>
		[ORFieldMapping("Creator")]
		public string Creator { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CreateTime")]
		public DateTime CreateTime { get; set; }
	}

    /// <summary>
    /// 通讯录标签人员coleection
    /// </summary>

    public class ContactsLabelPersonCollection : EditableDataObjectCollectionBase<ContactsLabelPersonModel>
    {

    }


    /// <summary>
    /// 外部联系人
    /// </summary>
    [ORTableMapping("office.ExternalContact")]
    public class ExternalContact
    {
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }
        public string Name { get; set; }
        [NoMapping]
        public string PinYin { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string Mark { get; set; }
        public string Label { get; set; }
        public string Photo { get; set; }
        public string Card { get; set; }
        public string Creator { get; set; }

        public string[] Phones { get; set; }
        public string[] Emails { get; set; }
    }

    /// <summary>
    /// 外部联系人 集合
    /// </summary>
    public class ExternalContactCollection : EditableDataObjectCollectionBase<ExternalContact>
    {

    }
}
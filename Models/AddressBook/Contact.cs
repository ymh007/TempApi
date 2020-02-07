using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook
{
    /// <summary>
    /// 常用联系人 Model
    /// </summary>
    [ORTableMapping("office.Contact")]
    public class ContactModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 联系人编码
        /// </summary>
        [ORFieldMapping("ContactId")]
        public string ContactID { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 常用联系人 Collection
    /// </summary>
    public class ContactCollection : EditableDataObjectCollectionBase<ContactModel>
    {

    }
}
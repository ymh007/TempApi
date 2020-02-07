using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.ContactsLabel
{
    /// <summary>
    /// 通讯录标签Model
    /// </summary>

    [ORTableMapping("office.ContactsLabel")]
    public class ContactsLabelModel : BaseModel
    {
        /// <summary>
        /// 通讯录标签名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
        /// <summary>
        /// 标签类型 0 内部联系人 1 外部联系人
        /// </summary>
        [ORFieldMapping("LabelType")]
        public int LabelType { get; set; }

    }
    /// <summary>
    /// 通讯录标签coleection
    /// </summary>

    public class ContactsLabelCollection : EditableDataObjectCollectionBase<ContactsLabelModel>
    {

    }
     

}
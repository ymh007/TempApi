using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
    /// <summary>
    /// 常用联系人 Adapter
    /// </summary>
    public class ContactAdapter : UpdatableAndLoadableAdapterBase<ContactModel, ContactCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ContactAdapter Instance = new ContactAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取我的常用联系人列表
        /// </summary>
        public ContactCollection LoadByCreator(string creator)
        {
            return Load(p =>
            {
                p.AppendItem("Creator", creator);
                p.AppendItem("ValidStatus", true);
            });
        }

        /// <summary>
        /// 获取我的某个常用联系人
        /// </summary>
        public ContactModel LoadByContactID(string contactId, string creator)
        {
            return Load(p =>
            {
                p.AppendItem("ContactID", contactId);
                p.AppendItem("Creator", creator);
                p.AppendItem("ValidStatus", true);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 获取我的某个常用联系人
        /// </summary>
        public void Delete(string contactId, string creator)
        {
            Delete(w =>
            {
                w.AppendItem("ContactID", contactId);
                w.AppendItem("Creator", creator);
            });
        }
    }
}
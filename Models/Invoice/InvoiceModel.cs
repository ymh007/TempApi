using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Invoice
{
    /// <summary>
    /// 发票 Model
    /// </summary>
    [ORTableMapping("office.Invoice")]
    public class InvoiceModel : BaseModel
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [ORFieldMapping("CompanyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        [ORFieldMapping("TaxpayerId")]
        public string TaxpayerId { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        [ORFieldMapping("OpeningBank")]
        public string OpeningBank { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        [ORFieldMapping("BankAccount")]
        public string BankAccount { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        [ORFieldMapping("Address")]
        public string Address { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [ORFieldMapping("PhoneNumber")]
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// 发票 Collection
    /// </summary>
    public class InvoiceCollection : EditableDataObjectCollectionBase<InvoiceModel>
    {

    }
}
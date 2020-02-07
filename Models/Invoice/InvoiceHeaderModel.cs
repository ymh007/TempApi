using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Invoice
{
    /// <summary>
    /// 海鸥二发票信息 Model
    /// </summary>
    [ORTableMapping("Finance.InvoiceHeader")]
    public class InvoiceHeaderModel
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        [ORFieldMapping("CompanyName")]
        public string CompanyName;
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        [ORFieldMapping("TaxpayerID")]
        public string TaxpayerID;
        /// <summary>
        /// 开户银行
        /// </summary>
        [ORFieldMapping("OpeningBank")]
        public string OpeningBank;
        /// <summary>
        /// 银行账号
        /// </summary>
        [ORFieldMapping("BankAccount")]
        public string BankAccount;
        /// <summary>
        /// 公司地址
        /// </summary>
        [ORFieldMapping("Address ")]
        public string Address;
        /// <summary>
        /// 电话号码
        /// </summary>
        [ORFieldMapping("PhoneNumber")]
        public string PhoneNumber;
    }

    /// <summary>
    /// 海鸥二发票信息 Collection
    /// </summary>
    public class InvoiceHeaderCollection : EditableDataObjectCollectionBase<InvoiceHeaderModel>
    {

    }
}
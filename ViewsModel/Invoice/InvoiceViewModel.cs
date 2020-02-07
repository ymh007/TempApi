using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Invoice
{
    /// <summary>
    /// 发票信息（基类） ViewModel
    /// </summary>
    public class InvoiceBaseViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxpayerId { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string OpeningBank { set; get; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { set; get; }
        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { set; get; }
    }

    /// <summary>
    /// 发票添加/修改 ViewModel
    /// </summary>
    public class InvoiceSaveViewModel : InvoiceBaseViewModel
    {

    }

    /// <summary>
    /// 获取发票列表 ViewModel
    /// </summary>
    public class InvoiceListViewModel : InvoiceBaseViewModel
    {
        /// <summary>
        /// 分享路径
        /// </summary>
        public string ShareUrl
        {
            get
            {
                return ConfigAppSetting.OfficePath + "invoice.html?code=" + base.Code;
            }
        }
    }

    /// <summary>
    /// 获取发票详情 ViewModel
    /// </summary>
    public class InvoiceModelViewModel : InvoiceBaseViewModel
    {

    }

    /// <summary>
    /// 删除发票 ViewModel
    /// </summary>
    public class InvoiceDeleteViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
    }
}
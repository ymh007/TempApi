using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Invoice
{
    /// <summary>
    /// 海鸥二发票搜索 ViewModel
    /// </summary>
    public class InvoiceHeaderSelectViewModel
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { set; get; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { set; get; }
    }
}
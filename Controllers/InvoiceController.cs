using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Invoice;
using Seagull2.YuanXin.AppApi.ViewsModel.Invoice;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 发票
    /// </summary>
    public class InvoiceController : ApiController
    {
        #region 搜索海鸥二发票信息
        /// <summary>
        /// 搜索海鸥二发票信息
        /// </summary>
        [HttpPost]
        public IHttpActionResult Select(InvoiceHeaderSelectViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                var dataCount = InvoiceHeaderAdapter.Instance.GetList(post.CompanyName);
                var pageCount = dataCount / post.PageSize + (dataCount % post.PageSize > 0 ? 1 : 0);
                var dataList = InvoiceHeaderAdapter.Instance.GetList(post.CompanyName, post.PageSize, post.PageIndex);

                return new ViewsModel.BaseViewPage()
                {
                    DataCount = dataCount,
                    PageCount = pageCount,
                    PageData = dataList
                };
            });
            return Ok(result);
        }
        #endregion

        #region 添加/修改发票信息
        /// <summary>
        /// 添加/修改发票信息
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(InvoiceSaveViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                if (string.IsNullOrWhiteSpace(post.Code))
                {
                    InvoiceAdapter.Instance.Update(new Models.Invoice.InvoiceModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        CompanyName = post.CompanyName,
                        TaxpayerId = post.TaxpayerId,
                        OpeningBank = post.OpeningBank,
                        BankAccount = post.BankAccount,
                        Address = post.Address,
                        PhoneNumber = post.PhoneNumber,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true
                    });
                }
                else
                {
                    var model = InvoiceAdapter.Instance.Load(w => w.AppendItem("Code", post.Code)).SingleOrDefault();
                    if (model == null)
                    {
                        throw new Exception("编码错误！");
                    }
                    if (model.Creator != user.Id)
                    {
                        throw new Exception("系统错误！");
                    }
                    model.CompanyName = post.CompanyName;
                    model.TaxpayerId = post.TaxpayerId;
                    model.OpeningBank = post.OpeningBank;
                    model.BankAccount = post.BankAccount;
                    model.Address = post.Address;
                    model.PhoneNumber = post.PhoneNumber;
                    model.Modifier = user.Id;
                    model.ModifyTime = DateTime.Now;
                    InvoiceAdapter.Instance.Update(model);
                }
            });
            return Ok(result);
        }
        #endregion

        #region 获取发票列表
        /// <summary>
        /// 获取发票列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(int pageSize, int pageIndex)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var dataCount = InvoiceAdapter.Instance.GetList(user.Id);
                var pageCount = dataCount / pageSize + (dataCount % pageSize > 0 ? 1 : 0);
                var dataList = InvoiceAdapter.Instance.GetList(user.Id, pageSize, pageIndex);

                // view
                var view = new List<InvoiceListViewModel>();
                dataList.ForEach(item =>
                {
                    view.Add(new InvoiceListViewModel()
                    {
                        Code = item.Code,
                        CompanyName = item.CompanyName,
                        TaxpayerId = item.TaxpayerId,
                        OpeningBank = item.OpeningBank,
                        BankAccount = item.BankAccount,
                        Address = item.Address,
                        PhoneNumber = item.PhoneNumber
                    });
                });

                return new ViewsModel.BaseViewPage()
                {
                    DataCount = dataCount,
                    PageCount = pageCount,
                    PageData = view
                };
            });
            return Ok(result);
        }
        #endregion

        #region 获取发票详情
        /// <summary>
        /// 获取发票详情
        /// </summary>
        [HttpGet, AllowAnonymous]
        public IHttpActionResult GetModel(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var model = InvoiceAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
                if (model == null)
                {
                    throw new Exception("发票已被删除！");
                }
                return new InvoiceModelViewModel()
                {
                    Code = model.Code,
                    CompanyName = model.CompanyName,
                    TaxpayerId = model.TaxpayerId,
                    OpeningBank = model.OpeningBank,
                    BankAccount = model.BankAccount,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber
                };
            });
            return Ok(result);
        }
        #endregion

        #region 删除发票
        /// <summary>
        /// 删除发票
        /// </summary>
        [HttpPost]
        public IHttpActionResult Delete(InvoiceDeleteViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var model = InvoiceAdapter.Instance.Load(w => w.AppendItem("Code", post.Code)).SingleOrDefault();
                if (model == null)
                {
                    throw new Exception("编码错误！");
                }
                if (model.Creator != user.Id)
                {
                    throw new Exception("系统错误！");
                }
                InvoiceAdapter.Instance.Delete(w => w.AppendItem("Code", post.Code));
            });
            return Ok(result);
        }
        #endregion
    }
}
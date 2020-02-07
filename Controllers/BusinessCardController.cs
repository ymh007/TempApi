using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using QRCoder;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.BusinessCard;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models.BusinessCard;
using Seagull2.YuanXin.AppApi.ViewsModel.BusinessCard;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 个人名片
    /// </summary>
    public class BusinessCardController : ApiController
    {
        #region 保存名片
        /// <summary>
        /// 保存名片
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(BusinessCardSaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var list = BusinessCardAdapter.Instance.Load(w => w.AppendItem("Creator", user.Id));

                if (list.Count < 1)
                {
                    //新增
                    var card = new BusinessCardModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        LogoKey = model.LogoKey,
                        Name = model.Name,
                        Position = model.Position,
                        Company = model.Company,
                        Email = model.Email,
                        Mobile = model.Mobile,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true
                    };
                    BusinessCardAdapter.Instance.Update(card);
                    BusinessCardPropertyAdapter.Instance.Delete(w => w.AppendItem("BusinessCardCode", card.Code));
                    BusinessCardPropertyAdapter.Instance.SavePropertyList(model.Address, (int)EnumBusinessCardType.Address, card.Code, user.Id);
                    BusinessCardPropertyAdapter.Instance.SavePropertyList(model.Phone, (int)EnumBusinessCardType.Phone, card.Code, user.Id);
                }
                else
                {   //编辑
                    var card = list[0];
                    card.LogoKey = model.LogoKey;
                    card.Name = model.Name;
                    card.Position = model.Position;
                    card.Company = model.Company;
                    card.Email = model.Email;
                    card.Mobile = model.Mobile;
                    card.Modifier = user.Id;
                    card.ModifyTime = DateTime.Now;
                    BusinessCardAdapter.Instance.Update(card);
                    BusinessCardPropertyAdapter.Instance.Delete(w => w.AppendItem("BusinessCardCode", card.Code));
                    BusinessCardPropertyAdapter.Instance.SavePropertyList(model.Address, (int)EnumBusinessCardType.Address, card.Code, user.Id);
                    BusinessCardPropertyAdapter.Instance.SavePropertyList(model.Phone, (int)EnumBusinessCardType.Phone, card.Code, user.Id);
                }
            });
            return Ok(result);
        }
        #endregion

        #region 获取名片列表
        /// <summary>
        /// 获取名片列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var list = new List<BusinessCardListViewModel>();
                BusinessCardAdapter.Instance.Load(m => m.AppendItem("Creator", user.Id)).OrderByDescending(o => o.CreateTime).ToList().ForEach(item =>
                {
                    list.Add(new BusinessCardListViewModel()
                    {
                        Code = item.Code,
                        LogoKey = item.LogoKey,
                        Name = item.Name,
                        Position = item.Position,
                        Company = item.Company,
                        Mobile = item.Mobile,
                        Email = item.Email
                    });
                });
                return list;
            });
            return Ok(result);
        }
        #endregion

        #region 获取名片详情
        /// <summary>
        /// 获取名片详情
        /// </summary>
        [HttpGet, AllowAnonymous]
        public IHttpActionResult GetModel(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var model = BusinessCardAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
                if (model == null)
                {
                    throw new Exception("名片已被删除！");
                }
                var address = new List<string>();
                var phone = new List<string>();
                var propertys = BusinessCardPropertyAdapter.Instance.Load(w => w.AppendItem("BusinessCardCode", model.Code)).OrderBy(o => o.CreateTime).ToList();
                propertys.FindAll(f => f.Type == (int)EnumBusinessCardType.Address)
                    .ForEach(item =>
                    {
                        address.Add(item.TypeValue);
                    });
                propertys.FindAll(f => f.Type == (int)EnumBusinessCardType.Phone)
                    .ForEach(item =>
                    {
                        phone.Add(item.TypeValue);
                    });
                return new BusinessCardModelViewModel()
                {
                    Code = model.Code,
                    LogoKey = model.LogoKey,
                    Name = model.Name,
                    Position = model.Position,
                    Company = model.Company,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    Address = address,
                    Phone = phone
                };
            });
            return Ok(result);
        }
        #endregion

        #region 获取名片二维码
        /// <summary>
        /// 获取名片二维码
        /// </summary>
        [HttpGet, AllowAnonymous]
        public HttpResponseMessage GetQrCode(string code)
        {
            var model = BusinessCardAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
            if (model == null)
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Seagull2.YuanXin.AppApi.Resources.404.jpg");
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

                return response;
            }

            var propertys = BusinessCardPropertyAdapter.Instance.Load(w => w.AppendItem("BusinessCardCode", model.Code)).OrderBy(o => o.CreateTime).ToList();
            var phone = propertys.FindAll(f => f.Type == (int)EnumBusinessCardType.Phone);
            var address = propertys.FindAll(f => f.Type == (int)EnumBusinessCardType.Address);

            var newName = "";
            var flag = false;
            var regex = new System.Text.RegularExpressions.Regex(@"[\u4e00-\u9fa5]");
            foreach (var item in model.Name)
            {
                if (regex.IsMatch(item.ToString()) && flag == false)
                {
                    newName += item + ";";
                    flag = true;
                }
                else
                {
                    newName += item;
                }
            }

            var vCard = $@"BEGIN:VCARD
                VERSION:3.0
                N:{ newName }
                TITLE:{ model.Position }
                ORG:{ model.Company }
                TEL;TYPE=CELL:{ model.Mobile }
                EMAIL:{ model.Email }";
            string[] phoneType = { "WORK", "PCS" };
            foreach (var item in phone)
            {
                var index = phone.IndexOf(item);
                if (index > phoneType.Length - 1)
                {
                    break;
                }
                vCard += $@"
                TEL;TYPE={ phoneType[index] }:{ item.TypeValue }";
            }
            string[] addressType = { "WORK", "INTL" };
            foreach (var item in address)
            {
                var index = address.IndexOf(item);
                if (index > addressType.Length - 1)
                {
                    break;
                }
                vCard += $@"
                ADR;TYPE={ addressType[index] }:{ item.TypeValue }";
            }
            vCard += $@"
                END:VCARD";
            vCard = vCard.Replace("\r\n                ", "\r\n");

            using (var qrCodeGenerator = new QRCodeGenerator())
            {
                using (var qrCodeData = qrCodeGenerator.CreateQrCode(vCard, QRCodeGenerator.ECCLevel.Q))
                {
                    using (var qrCode = new QRCode(qrCodeData))
                    {
                        var bitmap = qrCode.GetGraphic(2);
                        var memoryStream = new MemoryStream();
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);

                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(memoryStream.GetBuffer())
                        };
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");

                        bitmap.Dispose();
                        memoryStream.Dispose();

                        return response;
                    }
                }
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Advertisement;
using Seagull2.YuanXin.AppApi.Models.Adavertisement;
using Seagull2.YuanXin.AppApi.ViewsModel.Advertisement;
using Seagull2.YuanXin.AppApi.Extension;
using Seagull2.YuanXin.AppApi.Adapter.Common;
using Seagull2.YuanXin.AppApi.Models.Common;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Enum;
using Microsoft.Exchange.WebServices.Data;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using log4net;
using System.Reflection;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 广告图Controller
    /// </summary>
    public class AdvertisementController : ApiController
    {

        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // PC

        #region 获取广告类型
        /// <summary>
        /// 获取广告类型
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetCategoryList()
        {
            var result = ControllerService.Run(() =>
            {
                return Models.ModelExtention.GetEnumTYpeDesList<Enum.EnumAdvertisementType>();
            });
            return Ok(result);
        }
        #endregion

        #region 保存

        [HttpPost, AllowAnonymous]
        public IHttpActionResult TS(SaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                string icoUrl = FileService.UploadFile(model.Images);
                return icoUrl;
            });
            return this.Ok(result);
        }


        /// <summary>
        /// 保存
        /// </summary>
        [HttpPost, AllowAnonymous]
        public IHttpActionResult Save(SaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                if (model.StartTime >= model.EndTime)
                {
                    throw new Exception("结束时间不能小于开始时间！");
                }
                if (model.Images.IsNullOrWhiteSpace() && !model.IsVideo)
                {
                    throw new Exception("请上传图片！");
                }
                string relationCode = "";
                PersonUnitModelCollection pumList = new PersonUnitModelCollection();
                //上传图片
                var isChangeIco = false;
                var isChangeBgImg = false;
                var user = (Seagull2Identity)User.Identity;
                if (model.Images.IndexOf("data:image") == 0)
                {
                    //上传图片
                    string icoUrl = FileService.UploadFile(model.Images);
                    model.Images = icoUrl;
                    isChangeIco = true;
                }
                if (model.BackGroundImg.IndexOf("data:image") == 0 && model.Type == EnumAdvertisementType.WorkRegionBagOper)
                {
                    //上传图片 背景色图片
                    model.Link = FileService.UploadFile(model.BackGroundImg);
                    isChangeBgImg = true;
                }
                if (string.IsNullOrEmpty(model.Code))
                {
                    var advertModel = new AdvertisementModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        Type = (int)model.Type,
                        Title = model.Title,
                        Link = model.Link,
                        Images = model.IsVideo ? model.VideoUrl : model.Images,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        Modifier = user.Id,
                        ModifyTime = DateTime.Now,
                        ValidStatus = true
                    };
                    relationCode = advertModel.Code;
                    AdvertisementAdapter.Instance.Update(advertModel);
                }
                else
                {
                    //编辑
                    var deaModel = AdvertisementAdapter.Instance.Load(m => m.AppendItem("Code", model.Code)).SingleOrDefault();
                    deaModel.Type = (int)model.Type;
                    deaModel.Title = model.Title;
                    deaModel.Link = model.Link;
                    deaModel.StartTime = model.StartTime;
                    deaModel.EndTime = model.EndTime;
                    deaModel.Modifier = user.Id;
                    deaModel.ModifyTime = DateTime.Now;
                    if (isChangeIco)
                    {
                        deaModel.Images = model.Images;
                    }
                    if (model.IsVideo)
                    {
                        deaModel.Images = model.VideoUrl;
                    }
                    if (isChangeBgImg)
                    {
                        deaModel.Link = model.Link;
                    }
                    else
                    {
                        if (model.Type != EnumAdvertisementType.WorkRegionBagOper && model.ClickType != 4)
                        {
                            deaModel.Link = model.Link;
                        }
                    }
                    relationCode = deaModel.Code;
                    AdvertisementAdapter.Instance.Update(deaModel);
                }
                //是否保存前台判断 model.PermissionData长度大于0并且编辑过
                if (model.IsSavePermission)
                {
                    PersonUnitAdapter.Instance.UpdatePersionUnit(model.PermissionData, relationCode, user.Id, (int)EnumPermissionCategory.Advertisement);

                }
                ControllerService.UploadLog(user.Id, "操作了工具-广告管理-广告");
            });
            return Ok(result);
        }
        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetListForPC(string searText = "")
        {
            var result = ControllerService.Run(() =>
            {
                var coll = AdvertisementAdapter.Instance.Load(m => m.AppendItem("ValidStatus", true).AppendItem("Title", $"%{searText}%", "LIKE")).OrderByDescending(o => o.CreateTime).ToList();
                var list = new List<GetListForPC>();
                if (coll.Count > 0)
                {
                    coll.ForEach(m =>
                    {
                        var model = new GetListForPC
                        {
                            Code = m.Code,
                            Type = (Enum.EnumAdvertisementType)m.Type,
                            Title = m.Title,
                            Images = FileService.DownloadFile(m.Images),
                            VideoUrl = m.Images,
                            Link = m.Type != 3 ? m.Link : FileService.DownloadFile(m.Link),
                            StartTime = m.StartTime.ToString("yyyy-MM-dd HH:mm"),
                            EndTime = m.EndTime.ToString("yyyy-MM-dd HH:mm")
                        };
                        model.IsVideo = IsVideo(model.VideoUrl);
                        list.Add(model);
                    });
                }
                else
                {
                    throw new Exception("未获取到数据");
                }
                return list;
            });
            return Ok(result);
        }
        #endregion

        private bool IsVideo(string fileName)
        {
            string imgTypes = ".png.jpeg.bmp.jpg";
            string allFile = ".mp4.avi.flv.rmvb";
            string extenFile = fileName.Substring(fileName.LastIndexOf('.')).ToLower();
            return allFile.Contains(extenFile);
        }


        #region 获取详情
        /// <summary>
        /// 获取详情
        /// </summary>
        public IHttpActionResult GetDeatilByCode(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var deaModel = AdvertisementAdapter.Instance.Load(m => m.AppendItem("Code", code)).SingleOrDefault();
                var model = new SaveViewModel
                {
                    Code = deaModel.Code,
                    Type = (Enum.EnumAdvertisementType)deaModel.Type,
                    Title = deaModel.Title,
                    Images = FileService.DownloadFile(deaModel.Images),
                    Link = deaModel.Link,
                    StartTime = deaModel.StartTime,
                    EndTime = deaModel.EndTime
                };
                return model;
            });
            return Ok(result);
        }
        #endregion

        #region 删除
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    throw new Exception("code不能为空");
                }
                AdvertisementAdapter.Instance.Delete(m => m.AppendItem("Code", code));
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了工具-广告管理-广告");
            });
            return Ok(result);
        }
        #endregion

        // APP

        #region 获取开屏广告
        /// <summary>
        /// 获取开屏广告
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForAPPv1()
        {
            try
            {
                string userid = "";
                var user = (Seagull2Identity)User.Identity;
                userid = user.Id;
                var date = DateTime.Now;
                var model = AdvertisementAdapter.Instance.Load(
                    m =>
                    {
                        m.AppendItem("Type", (int)EnumAdvertisementType.AppStart);
                        m.AppendItem("StartTime", date, "<");
                        m.AppendItem("EndTime", date, ">");
                    },
                    o =>
                    {
                        o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Descending);
                    }
                );

                if (model.Count == 0)
                {
                    return Ok(new ViewsModel.BaseView
                    {
                        State = false,
                        Message = "暂无开屏广告！"
                    });
                }
                //查找有权限得就按权限数据走如果没有就用没有分权限的
                GetListForAPP getModel = null;
                GetListForAPP common = null;
                if (!string.IsNullOrEmpty(userid))
                {
                    AppCommonPermissionModelCollection currentUserPermission = AppCommonPermissionAdapter.Instance.Load(w => w.AppendItem("Module", (int)EnumPermissionCategory.Advertisement).AppendItem("UserCode", userid));
                    List<string> allModule = AppCommonPermissionAdapter.Instance.GetAllModule((int)EnumPermissionCategory.Advertisement);
                    model.ForEach(f =>
                    {
                        if (currentUserPermission.Exists(e => e.RelationCode == f.Code))
                        {
                            if (getModel == null)
                            {
                                getModel = new GetListForAPP()
                                {
                                    Title = f.Title,
                                    Images = f.Images,
                                    Link = f.Link,
                                    CreateTime = f.CreateTime
                                };
                            }
                        }
                        if (!allModule.Exists(e => e == f.Code))
                        {
                            if (common == null)
                            {
                                common = new GetListForAPP()
                                {
                                    Title = f.Title,
                                    Images = f.Images,
                                    Link = f.Link,
                                    CreateTime = f.CreateTime
                                };
                            }
                        }
                    });
                }
                GetListForAPP returnValue = null;
                if (getModel != null && common == null)
                {
                    returnValue = getModel;
                }
                else if (getModel != null && common != null)
                {
                    returnValue = getModel.CreateTime > common.CreateTime ? getModel : common;
                }
                else if (getModel == null && common != null)
                {
                    returnValue = common;
                }
                if (returnValue != null)
                {
                    if (IsVideo(returnValue.Images))
                    {
                        returnValue.Video = returnValue.Images;
                        returnValue.Images = "";
                    }
                    else
                    {
                        returnValue.Images = FileService.DownloadFile(returnValue.Images);
                        returnValue.Video = "";
                    }
                    return Ok(new ViewsModel.BaseView
                    {
                        State = true,
                        Message = "success.",
                        Data = returnValue
                    });
                }
                else
                {

                    return Ok(new ViewsModel.BaseView
                    {
                        State = false,
                        Message = "暂无开屏广告！"
                    });
                }
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView
                {
                    State = false,
                    Message = e.Message
                });
            }
        }


        /// <summary>
        /// 获取开屏广告old
        /// </summary>
        [HttpGet, AllowAnonymous]
        public IHttpActionResult GetListForAPP()
        {
            try
            {
                var date = DateTime.Now;
                var model = AdvertisementAdapter.Instance.Load(
                    m =>
                    {
                        m.AppendItem("Type", (int)Enum.EnumAdvertisementType.AppStart);
                        m.AppendItem("StartTime", date, "<");
                        m.AppendItem("EndTime", date, ">");
                    },
                    o =>
                    {
                        o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Descending);
                    }
                ).FirstOrDefault();

                if (model == null)
                {
                    return Ok(new ViewsModel.BaseView
                    {
                        State = false,
                        Message = "暂无开屏广告！"
                    });
                }

                var getModel = new GetListForAPP
                {
                    Title = model.Title,
                    Images = FileService.DownloadFile(model.Images),
                    Link = model.Link,
                    Video = ""
                };

                return Ok(new ViewsModel.BaseView
                {
                    State = true,
                    Message = "success.",
                    Data = getModel
                });
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView
                {
                    State = false,
                    Message = e.Message
                });
            }
        }

        #endregion

        #region 获取工作通banner
        /// <summary>
        /// 获取工作通banner
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForWorkRegionBanner()
        {
            try
            {
                var user = (Seagull2Identity)User.Identity;
                var date = DateTime.Now;
                var model = AdvertisementAdapter.Instance.Load(
                    m =>
                    {
                        m.AppendItem("Type", (int)Enum.EnumAdvertisementType.WorkRegionBannerNew);
                        m.AppendItem("StartTime", date, "<");
                        m.AppendItem("EndTime", date, ">");
                    },
                    o =>
                    {
                        o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Descending);
                    }
                );

                if (model.Count == 0)
                {
                    return Ok(new ViewsModel.BaseView
                    {
                        State = false,
                        Message = "暂无工作通banner广告！"
                    });
                }

                string link = "";
                GetListForAPP getModel = null;
                GetListForAPP common = null;
                AppCommonPermissionModelCollection currentUserPermission = AppCommonPermissionAdapter.Instance.Load(w => w.AppendItem("Module", (int)EnumPermissionCategory.Advertisement).AppendItem("UserCode", user.Id));
                if (currentUserPermission.Count > 0)
                {
                    model.ForEach(f =>
                    {
                        if (currentUserPermission.Exists(e => e.RelationCode == f.Code))
                        {
                            getModel = new GetListForAPP()
                            {
                                Title = f.Title,
                                Images = FileService.DownloadFile(f.Images),
                                Link = f.Link,
                                Video = ""
                            };
                            return;
                        }
                        else
                        {
                            if (common == null)
                            {
                                common = new GetListForAPP()
                                {
                                    Title = f.Title,
                                    Images = FileService.DownloadFile(f.Images),
                                    Link = f.Link,
                                    Video = ""
                                };
                            }
                        }
                    });
                }
                if (getModel != null)
                {
                    if (getModel.Link == "" || getModel.Link.StartsWith("http"))
                    {
                        link = getModel.Link;
                    }
                    return Ok(new ViewsModel.BaseView
                    {
                        State = true,
                        Message = "success.",
                        Data = getModel
                    });
                }
                else
                {

                    if (common != null)
                    {
                        if (IsVideo(common.Images))
                        {
                            common.Video = common.Images;
                        }
                        return Ok(new ViewsModel.BaseView
                        {
                            State = true,
                            Message = "success.",
                            Data = common
                        });
                    }
                    else
                    {
                        return Ok(new ViewsModel.BaseView
                        {
                            State = false,
                            Message = "暂无工作通banner广告！"
                        });
                    }
                }
            }
            catch (Exception e)
            {
                return Ok(new ViewsModel.BaseView
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion



        [HttpGet, AllowAnonymous]
        public HttpResponseMessage video()
        {
            System.IO.Stream stream = null;
            using (var http = new HttpClient())
            {
                string url = "http://10.23.74.101/MobileBusiness/FileDownService/down/file?parment=YuanXin-File://2019/10/001.mp4";
                stream = http.GetStreamAsync(url).Result;
            }
            if (stream != null)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(stream);
                httpResponseMessage.Content.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                httpResponseMessage.Content.Headers.Add("Access-Control-Allow-Origin", "*");
                httpResponseMessage.Content.Headers.Add("Access-Control-Allow-Methods", "GET");
                httpResponseMessage.Content.Headers.Add("Allow", "GET");
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
                return httpResponseMessage;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }

        }
    }
}
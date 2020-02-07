using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AppConfig;
using Seagull2.YuanXin.AppApi.Models.AppConfig;
using Seagull2.YuanXin.AppApi.ViewsModel.AppConfig;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// APP首页相关配置接口
    /// </summary>
    public class AppConfigController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 首页快捷入口
        /// <summary>
        /// 首页快捷入口
        /// </summary>
        [HttpGet, AllowAnonymous]
        public IHttpActionResult AppHomeShowBtn()
        {
            try
            {
                string json = ConfigurationManager.AppSettings["AppHomeShowBtn"];
                var config = JsonConvert.DeserializeObject<dynamic>(json);
                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = config
                });
            }
            catch (Exception e)
            {
                log.Error(JsonConvert.SerializeObject(e));
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 首页轮播图
        /// <summary>
        /// 首页轮播图
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAppHomeSlideList()
        {
            var list = AppHomeSlideAdapter.Instance.GetList();

            var view = new List<AppHomeSlideViewModel>();

            list.ForEach(model =>
            {
                if (model.IsEnable)
                {
                    view.Add(new AppHomeSlideViewModel()
                    {
                        Code = model.Code,
                        Title = model.Title,
                        ImageUrl = model.ImageUrl,
                        Type = model.Type,
                        Event = model.Event,
                        IsEnable = model.IsEnable,
                        Sort = model.Sort
                    });
                }
            });

            return Ok(new ViewsModel.ViewModelBaseList
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }
        #endregion

        #region 获取轮播图类型 - PC
        /// <summary>
        /// 获取轮播图类型 -PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAppHomeSlideType()
        {
            var list = Models.ModelExtention.GetEnumTYpeDesList<Enum.EnumAppHomeSlideType>();

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = list
            });
        }
        #endregion

        #region 获取轮播图实体 - PC
        /// <summary>
        /// 获取轮播图实体 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAppHomeSlideModel(string code)
        {
            var list = AppHomeSlideAdapter.Instance.Load(p => { p.AppendItem("Code", code); });
            if (list.Count <= 0)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = "轮播图编号错误！"
                });
            }
            var model = list[0];

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = new AppHomeSlideViewModel()
                {
                    Code = model.Code,
                    Title = model.Title,
                    ImageUrl = model.ImageUrl,
                    Type = model.Type,
                    Event = model.Event,
                    IsEnable = model.IsEnable,
                    Sort = model.Sort
                }
            });
        }
        #endregion

        #region 获取轮播图列表 - PC
        /// <summary>
        /// 获取轮播图列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAppHomeSlideListForPC()
        {
            var list = AppHomeSlideAdapter.Instance.GetList();

            var view = new List<AppHomeSlideViewModel>();
            foreach (var model in list)
            {
                view.Add(new AppHomeSlideViewModel()
                {
                    Code = model.Code,
                    Title = model.Title,
                    ImageUrl = model.ImageUrl,
                    Type = model.Type,
                    Event = model.Event,
                    IsEnable = model.IsEnable,
                    Sort = model.Sort
                });
            }

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }
        #endregion

        #region 编辑轮播图信息 - PC
        /// <summary>
        /// 编辑轮播图信息 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult EditAppHomeSlide(AppHomeSlideModel model)
        {
            //上传图片
            var isChangeImage = false;
            if (model.ImageUrl.IndexOf("data:image") == 0)
            {
                //上传图片
                string imageUrl = FileService.UploadFile(model.ImageUrl);
                model.ImageUrl = imageUrl;
                isChangeImage = true;
            }

            //添加
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                model.Code = Guid.NewGuid().ToString();
                model.Creator = ((Seagull2Identity)User.Identity).Id;
                model.CreateTime = DateTime.Now;
                model.ValidStatus = true;

                AppHomeSlideAdapter.Instance.Update(model);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success."
                });
            }
            //修改
            else
            {
                var list = AppHomeSlideAdapter.Instance.Load(p => { p.AppendItem("Code", model.Code); });
                if (list.Count <= 0)
                {
                    return Ok(new ViewsModel.BaseView()
                    {
                        State = false,
                        Message = "轮播图编号错误！"
                    });
                }
                var item = list[0];
                item.Title = model.Title;
                if (isChangeImage)
                {
                    item.ImageUrl = model.ImageUrl;
                }
                item.Type = model.Type;
                item.Event = model.Event;
                item.IsEnable = model.IsEnable;
                item.Sort = model.Sort;
                item.Modifier = ((Seagull2Identity)User.Identity).Id;
                item.ModifyTime = DateTime.Now;

                AppHomeSlideAdapter.Instance.Update(item);

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success."
                });
            }
        }
        #endregion

        #region 直播热词
        /// <summary>
        /// 直播热词
        /// </summary>
        [HttpGet, AllowAnonymous]
        public IHttpActionResult LiveHotWord()
        {
            try
            {
                string words = ConfigurationManager.AppSettings["LiveHotWord"];
                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = words.Split(',')
                });
            }
            catch (Exception e)
            {
                log.Error(JsonConvert.SerializeObject(e));
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 数字远洋
        /// <summary>
        /// 数字远洋
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetOceanData()
        {
            try
            {
                var showHrNames = ConfigAppSetting.OceanDataShowHr.Split(',');
                var data = OceanDataAdapter.Instance.GetList();
                var list = new List<OceanDataViewModel>();
                foreach (var item in data)
                {
                    list.Add(new OceanDataViewModel()
                    {
                        Name = item.Name,
                        Total = item.TotalAmount + item.AddedAmount,
                        Today = showHrNames.Contains(item.Name) ? -9999 : item.AddedAmount
                    });
                }

                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = list
                });
            }
            catch (Exception e)
            {
                log.Error(JsonConvert.SerializeObject(e));
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion
    }
}
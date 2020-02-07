using System;
using System.Collections.Generic;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Models.Common;
using Seagull2.YuanXin.AppApi.Adapter.Common;
using Seagull2.YuanXin.AppApi.Enum;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 工作通 Controller
    /// </summary>
    public class WorkRegionController : ApiController
    {
        #region 获取工作通首页相关信息
        /// <summary>
        /// 获取工作通首页相关信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult Home(string location = "北京")
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                // 广告 带权限范围的
                var adView = new List<ViewsModel.Advertisement.GetListForAPP>();
                AppCommonPermissionModelCollection currentUserPermission = AppCommonPermissionAdapter.Instance.Load(w => w.AppendItem("Module", (int)EnumPermissionCategory.Advertisement).AppendItem("UserCode", user.Id));
                List<string> allModule = AppCommonPermissionAdapter.Instance.GetAllModule((int)EnumPermissionCategory.Advertisement);
                Task task_advertisment = Task.Run(() => {
                    var allData = Adapter.Advertisement.AdvertisementAdapter.Instance.GetDataByType((int)EnumAdvertisementType.WorkRegionBannerNew);
                    allData.ForEach(item =>
                    {
                        if (currentUserPermission.Exists(e => e.RelationCode == item.Code))
                        {
                            var link = "";
                            if (item.Link == "" || item.Link.StartsWith("http"))
                            {
                                link = item.Link;
                            }
                            adView.Add(new ViewsModel.Advertisement.GetListForAPP
                            {
                                Title = item.Title,
                                Images = FileService.DownloadFile(item.Images),
                                Link = link,
                                NewLink = item.Link,
                            });
                        }
                    });
                    if (adView.Count == 0)
                    {
                        allData.ForEach(f =>
                        {
                            if (!allModule.Exists(e => e == f.Code))
                            {
                                var link = "";
                                if (f.Link == "" || f.Link.StartsWith("http"))
                                {
                                    link = f.Link;
                                }
                                adView.Add(new ViewsModel.Advertisement.GetListForAPP
                                {
                                    Title = f.Title,
                                    Images = FileService.DownloadFile(f.Images),
                                    Link = link,
                                    NewLink = f.Link,
                                });
                            }
                        });
                    }
                });
                // 股票
                ViewsModel.Stock.StockViewModel stockViewModel = new ViewsModel.Stock.StockViewModel { Last = "0", Change = "0", };
                Task task_stock = Task.Run(() => {
                    var stockModel = Adapter.Stock.StockAdapter.Instance.GetSingle();
                    if (stockModel != null && !string.IsNullOrWhiteSpace(stockModel.JsonDataString))
                    {
                        stockViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewsModel.Stock.StockViewModel>(stockModel.JsonDataString);
                    }
                    var enable = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["StockEnable"]);
                    if (!enable)
                    {
                        stockViewModel = null;
                    }
                });
                // 天气
                var weatherViewModel = new ViewsModel.Weather.ResultsItem();
                Task task_weather = Task.Run(() => {
                    var weatherModel = Adapter.Weather.WeatherAdapter.Instance.GetSingle(location);
                    if (weatherModel != null)
                    {
                        weatherViewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewsModel.Weather.ResultsItem>(weatherModel.JsonDataString);
                    }
                });
                // 问候语
                var greetView = "Hi，见到你真好！";
                Task task_greet = Task.Run(() => {
                    var greet = Adapter.Greetings.GreetingsAdapter.Instance.GetSignle();
                    if (greet != null)
                    {
                        greetView = greet["Content"].ToString();
                    }
                });

                // 查询工作通 背景图和前景图
                string backgroundImgUrl = "", foregroundImgUrl = "";
                Task task_bf = Task.Run(() => {

                    var bf_data = Adapter.Advertisement.AdvertisementAdapter.Instance.GetDataByType((int)EnumAdvertisementType.WorkRegionBagOper);
                    bf_data.ForEach(f => {
                        if (currentUserPermission.Exists(e => e.RelationCode == f.Code))
                        {
                            foregroundImgUrl = FileService.DownloadFile(f.Images);
                            backgroundImgUrl = FileService.DownloadFile(f.Link);
                        }
                    });
                    if (string.IsNullOrEmpty(backgroundImgUrl))
                    {
                        bf_data.ForEach(f => {
                            if (!allModule.Exists(e => e == f.Code))
                            {
                                if (string.IsNullOrEmpty(backgroundImgUrl))
                                {
                                    foregroundImgUrl = FileService.DownloadFile(f.Images);
                                    backgroundImgUrl = FileService.DownloadFile(f.Link);
                                }
                            }
                        });
                    }
                });
                Task.WaitAll(task_stock, task_weather, task_greet, task_advertisment, task_bf);
                return new
                {
                    Stock = stockViewModel,
                    Weather = weatherViewModel,
                    Greet = greetView,
                    Advertisement = adView,
                    bgAndfg = new
                    {
                        backgroundImgUrl,
                        foregroundImgUrl
                    }
                };
            });
            return Ok(result);
        }
        #endregion
    }
}
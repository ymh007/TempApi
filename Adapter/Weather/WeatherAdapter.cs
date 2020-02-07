using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Weather
{
    /// <summary>
    /// 天气 Adapter
    /// </summary>
    public class WeatherAdapter : UpdatableAndLoadableAdapterBase<WeatherModel, WeatherCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly WeatherAdapter Instance = new WeatherAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取天气信息
        /// </summary>
        public WeatherModel GetSingle(string location)
        {
            try
            {
                var model = Load(w => w.AppendItem("Location", location)).SingleOrDefault();
                if (model == null)
                {
                    return RequestData(location);
                }
                else
                {
                    if (model.ExpiredTime <= DateTime.Now)
                    {
                        return RequestData(location, model);
                    }
                    else
                    {
                        return model;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        WeatherModel RequestData(string location, WeatherModel model = null)
        {
            try
            {
                var url = string.Format(string.Format(System.Configuration.ConfigurationManager.AppSettings["WeatherApiUrl"], location));
                var json = HttpService.Get(url);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ViewsModel.Weather.Root>(json);

                if (model == null)
                {
                    model = new WeatherModel();
                    model.Code = Guid.NewGuid().ToString();
                    model.Creator = "Auto Refresh";
                    model.CreateTime = DateTime.Now;
                    model.ValidStatus = true;
                }
                else
                {
                    model.Modifier = "Auto Refresh";
                    model.ModifyTime = DateTime.Now;
                }
                model.Location = location;
                model.JsonDataString = Newtonsoft.Json.JsonConvert.SerializeObject(result.Results[0]);
                model.RefreshTime = DateTime.Now;
                model.ExpiredTime = DateTime.Now.AddHours(2);

                Update(model);

                return model;
            }
            catch
            {
                return null;
            }
        }


    }
}
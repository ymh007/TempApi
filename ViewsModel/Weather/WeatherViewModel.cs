using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Weather
{
    /// <summary>
    /// 地址信息
    /// </summary>
    public class Location
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// 天气信息
    /// </summary>
    public class Now
    {
        /// <summary>
        /// 天气现象文字
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 天气现象代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 温度，单位为：c=摄氏度或f=华氏度
        /// </summary>
        public string Temperature { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get
            {
                return $"{ConfigAppSetting.ApiPath}Images/weather/{Code}.png";
            }
        }
    }

    /// <summary>
    /// 天气信息结果
    /// </summary>
    public class ResultsItem
    {
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public Location Location { get; set; }
        /// <summary>
        /// 天气信息
        /// </summary>
        public Now Now { get; set; }
    }

    /// <summary>
    /// 天气信息结果根节点
    /// </summary>
    public class Root
    {
        /// <summary>
        /// 结果集
        /// </summary>
        public List<ResultsItem> Results { get; set; }
    }
}
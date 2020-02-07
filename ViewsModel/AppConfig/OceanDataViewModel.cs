using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.AppConfig
{
    /// <summary>
    /// 数字远洋ViewModel
    /// </summary>
    public class OceanDataViewModel
    {
        /// <summary>
        /// 图标Url
        /// </summary>
        public string Ico
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ConferenceImageDownLoadRootPath"] + "/OceanDataIco/" + Name + ".png";
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { set; get; }
        /// <summary>
        /// 今日新增
        /// </summary>
        public int Today { set; get; }
    }
}
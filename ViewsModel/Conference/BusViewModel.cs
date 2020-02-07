using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Conference
{
    /// <summary>
    /// 手机端班车 ViewModel
    /// </summary>
    public class ConferenceBusAppViewModel
    {
        /// <summary>
        /// 线路名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 发车日期
        /// </summary>
        public string Date { set; get; }
        /// <summary>
        /// 发车时间
        /// </summary>
        public string Time { set; get; }
        /// <summary>
        /// 发车地点
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { set; get; }
    }
}
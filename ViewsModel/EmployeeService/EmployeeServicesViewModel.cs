using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.EmployeeService
{
    /// <summary>
    /// 员工打卡 ViewModel
    /// </summary>
    public class SaveViewModel
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public string Lng { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Lat { get; set; }
        /// <summary>
        /// 打卡地点
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// 符合系统规定打卡的集合
    /// </summary>
    public class EmployHelpViewModel
    {
        /// <summary>
        /// 距离
        /// </summary>
        public double Distances { get; set; }

        /// <summary>
        /// 打卡地点编码
        /// </summary>
        public string StanderCode { get; set; }
        /// <summary>
        /// 签到时间
        /// </summary>
        public string OnTime { get; set; }
        /// <summary>
        /// 签退时间
        /// </summary>
        public string OffTime { get; set; }
    }
}
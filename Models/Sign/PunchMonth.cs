using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 我的考勤实体（当月考勤）
    /// </summary>
    public class PunchMonthView
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string Date { set; get; }
        /// <summary>
        /// 签到状态
        /// </summary>
        public bool IsNormal { set; get; }
        /// <summary>
        /// 签到详情
        /// </summary>
        public Info AM { set; get; }
        /// <summary>
        /// 签退详情
        /// </summary>
        public Info PM { set; get; }
    }

    /// <summary>
    /// 打卡详情
    /// </summary>
    public class Info
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { set; get; }
        /// <summary>
        /// 是否正常
        /// </summary>
        public bool IsNormal { set; get; }
        /// <summary>
        /// 签到地点
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string MapUrl { set; get; }
    }

    /// <summary>
    /// 我的考勤实体（当月考勤）
    /// </summary>
    public class PunchMonth
    {
        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string MapUrl { set; get; }
        /// <summary>
        /// 签到地点编号
        /// </summary>
        public string StandardPunchCode { set; get; }
        /// <summary>
        /// 签到地点
        /// </summary>
        public string Address { set; get; }
    }

    /// <summary>
    /// 我的考勤集合
    /// </summary>
    public class PunchMonthCollection : EditableDataObjectCollectionBase<PunchMonth>
    {

    }
}
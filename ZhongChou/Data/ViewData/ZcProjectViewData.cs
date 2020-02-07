using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class ZcProjectViewData : Project
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }

        /// <summary>
        /// 开始时间格式化字符串
        /// </summary>
        [NoMapping]
        public string StartTimeFormat { get { return this.StartTime.ToString("M/d"); } }

        /// <summary>
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeFormat { get { return this.EndTime.ToString("M/d"); } }
    }
    public class ZcProjectViewDataCollection : EditableDataObjectCollectionBase<ZcProjectViewData>
    {
    }
}

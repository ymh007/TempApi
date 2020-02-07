using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class ActivityFocusViewData : UserFocus
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }

        /// <summary>
        /// 开始时间格式化字符串
        /// </summary>
        [NoMapping]
        public string StartTimeFormat 
        { 
            get 
            {
                return this.Project == null ? "" : this.Project.StartTime.ToString("M/d");
            } 
        }

        /// <summary>
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeFormat
        {
            get
            {
                return this.Project == null ? "" : this.Project.EndTime.ToString("M/d");
            }
        }

    }
    public class ActivityFocusViewDataCollection : EditableDataObjectCollectionBase<ActivityFocusViewData>
    {
        /// <summary>
        /// 转化为ListDataView
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalCount">总行数</param>
        /// <returns></returns>
        public ListDataView ToListDataView(int pageIndex, int pageSize, int totalCount)
        {
            var result = new ListDataView
            {
                PageIndex = pageIndex,
                PageCount = totalCount % pageSize > 0 ? totalCount / pageSize + 1 : totalCount / pageSize,
                TotalCount = totalCount,
                ListData = this
            };

            return result;
        }
    }
}

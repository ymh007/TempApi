using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 
    /// </summary>
    public class TopProjectViewData : Project
    {
       
        /// <summary>
        /// 置顶开始时间
        /// </summary>
        public string TopStartTime { get; set; }

        /// <summary>
        /// 置顶结束时间
        /// </summary>
        public string TopEndTime { get; set; }

        /// <summary>
        /// 置顶天数
        /// </summary>
        public int TopTotalDays
        {
            get
            {
                TimeSpan ts = DateTime.Parse(TopEndTime) - DateTime.Parse(TopStartTime);
                return (int)ts.TotalDays + 1;
            }

        }

    }

    public class TopProjectViewDataCollection : EditableDataObjectCollectionBase<TopProjectViewData>
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

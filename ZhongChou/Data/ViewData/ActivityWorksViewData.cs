using MCS.Library.Core;
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
    public class ActivityWorksViewData : ActivityWorks
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }

        public Project Project
        {
            get
            {
                return  OnlineProjectAdapter.Instance.LoadByCode(this.ProjectCode);
            }
        }

    }

    public class ActivityWorksViewDataCollection : EditableDataObjectCollectionBase<ActivityWorksViewData>
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

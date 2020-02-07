using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// 分页查询基类
    /// </summary>
    public class ViewPageBase<TCollection> : ViewModelBase
    {
        /// <summary>
        /// 每页大小
        /// </summary>
        internal static readonly int PageSize = 10;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 是否是最后一页
        /// </summary>
        public bool IsLastPage { get; set; }

        /// <summary>
        /// 首页查询时的时间（每次查询都加上CreateTime小于FirstPageTime,以保证每页数据不出现重复情况)
        /// </summary>
        public string FirstPageSearchTime { get; set; }

        /// <summary>
        /// 每页数据集合
        /// </summary>
        public TCollection dataList { get; set; }

        /// <summary>
        /// 总行数
        /// </summary>
        public int TotalRowsCount { get; set; }
    }
}
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class ListView
    {
        public ListView() { }
        public ListView(int pageSize, int totalCount)
        {
            this.TotalCount = totalCount;

            this.PageCount = CommonHelper.GetPageCount(pageSize, totalCount);
        }
        public ListView(int pageSize, int totalCount, int pageIndex)
        {
            this.TotalCount = totalCount;
            this.PageCount = CommonHelper.GetPageCount(pageSize, totalCount);
            this.IsLastPage = pageIndex * pageSize >= TotalCount ? true : false;
        }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// 每页数据
        /// </summary>
        public object ListData { get; set; }

        #region 最新扩展属性
        /// <summary>
        /// 是否是最后一页
        /// </summary>
        public bool IsLastPage { get; set; }
        #endregion
    }
}
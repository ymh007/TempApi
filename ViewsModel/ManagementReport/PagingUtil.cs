using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ManagementReport
{
    /// <summary>
    /// 分页工具类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingUtil<T> : List<T>
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int DataCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNo { get; set; }
        /// <summary>
        /// 每页显示记录数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage
        {
            get { return PageNo > 1; }
        }
        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage
        {
            get { return PageNo < this.PageCount; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        public PagingUtil(List<T> dataList, int pageSize, int pageNo)
        {
            this.PageSize = pageSize;
            this.PageNo = pageNo;
            this.DataCount = dataList.Count;
            this.PageCount = (int)Math.Ceiling((decimal)this.DataCount / pageSize);
            this.AddRange(dataList.Skip((pageNo - 1) * pageSize).Take(pageSize));
        }
    }
}
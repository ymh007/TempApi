using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.ManagementReport
{
    /// <summary>
    /// 管理报告文章浏览次数或点赞次数实体类
    /// </summary>
    public class ViewOrPraiseCountModel
    {
        /// <summary>
        /// Url
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 访问次数
        /// </summary>
        public int Count { set; get; }
    }

    /// <summary>
    /// 管理报告点击量集合
    /// </summary>
    public class ViewOrPraiseCountCollection : EditableDataObjectCollectionBase<ViewOrPraiseCountModel>
    {

    }
}
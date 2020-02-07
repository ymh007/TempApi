using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ImportantNotice
{
    /// <summary>
    /// 重要发文列表
    /// </summary>
    public class ListItemViewModel
    {
        /// <summary>
        /// WebId
        /// </summary>
        public string WebId { set; get; }
        /// <summary>
        /// ListId
        /// </summary>
        public string ListId { set; get; }
        /// <summary>
        /// Id
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string Created { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
    }
}
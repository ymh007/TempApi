using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Message
{
    /// <summary>
    /// 消息分页展示类
    /// </summary>
    public class MessagePageView
    {
        /// <summary>
        /// 是否最后一页
        /// </summary>
        public bool IsLastPage { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        public MessageCollection MessageColl { get; set; }
    }
}
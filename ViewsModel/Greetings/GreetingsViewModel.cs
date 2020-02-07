using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Greetings
{
    /// <summary>
    /// 
    /// </summary>
    public class GreetingsViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public List<ContentListItem> ContentList { get; set; }

        /// <summary>
        /// 内容项
        /// </summary>
        public class ContentListItem
        {
           

            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
        }
    }
}
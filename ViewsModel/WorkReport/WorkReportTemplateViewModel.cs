using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.WorkReport
{
    /// <summary>
    /// 添加模板
    /// </summary>
    public class WorkReportTemplateViewModel
    {

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 测试
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 是否是系统
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ChildListItem> ChildList { get; set; }

        /// <summary>
        /// 模板字符
        /// </summary>
        public class ChildListItem
        {
            /// <summary>
            /// 字符编码
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 字符标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            public int Sort { get; set; }
        }

    }
}
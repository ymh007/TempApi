using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.WorkReport
{
    /// <summary>
    /// 工作汇报详情
    /// </summary>
    public class WorkReportDetailViewModel
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Receiver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> CopyPerSon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ChildListItem> ChildList { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ImageList { get; set; }

        /// <summary>
        /// 字符内容
        /// </summary>
        public class ChildListItem
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }

            /// <summary>
            /// 排序
            /// </summary>
            public int Sort { get; set; }
        }
       
        }
    }

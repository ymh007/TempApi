using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.WorkReport
{
    /// <summary>
    /// 工作汇报详情
    /// </summary>
    public class WorkReportListDetailViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 发送人名称
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 汇报时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ChildListItems> ChildList { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 接收人详情
        /// </summary>
        public string ReceiverDetails { get; set; }
        /// <summary>
        /// 抄送人详情
        /// </summary>
        public string CopyDetails { get; set; }
        /// <summary>
        /// 转发详情
        /// </summary>
        public string ForwardDetails { get; set; }
        /// <summary>
        /// 转发标题
        /// </summary>
        public string ForwardTitle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ImageList { get; set; }
        /// <summary>
        /// 是否原件
        /// </summary>
        public bool IsOriginal { get; set; }
        /// <summary>
        /// 是否原件
        /// </summary>
        public bool IsSender{ get; set; }
        /// <summary>
        /// 是否阅读
        /// </summary>
        public bool IsRead { get; set; }

        public class ChildListItems
        {
            /// <summary>
            /// 字符
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
        }



    }

    /// <summary>
    /// 转发人
    /// </summary>
    public class ForwardModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Receiver { get; set; }
    }
}
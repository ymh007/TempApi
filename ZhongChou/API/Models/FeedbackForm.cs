using Seagull2.YuanXin.AppApi.Controllers;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// 反馈内容表单
    /// </summary>
    public class FeedbackForm
    {
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 图片地址列表
        /// </summary>
        public List<string> imgUrlList { get; set; }


    }
}
using Seagull2.YuanXin.AppApi.ZhongChouData.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class AppIndexView
    {
        /// <summary>
        /// 置顶项目
        /// </summary>
        public TopProjectViewDataCollection TopProjectCollection { get; set; }
       
        /// <summary>
        /// 活动
        /// </summary>
        public CaseProjectViewDataCollection ActivityCollection { get; set; }

        /// <summary>
        /// 生活家话题
        /// </summary>
        public TopicsViewDataCollection TopicsCollection { get; set; }

        /// <summary>
        /// 最新众筹
        /// </summary>
        public ZcProjectViewDataCollection NewestZcProjectCollection { get; set; }


    }
}
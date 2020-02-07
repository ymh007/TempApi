using MCS.Library.Core;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class OrderDetailView
    {
      
        /// <summary>
        /// 订单信息
        /// </summary>
        public Order Order { get;  set; }

        /// <summary>
        /// 项目信息
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// 用户评价信息
        /// </summary>
        public UserEvaluation UserEvaluation { get; set; }


        /// <summary>
        /// 订单状态流程步骤
        /// </summary>s
        public ProcessStepText ProcessStepText { get; set; }



    }
   
}
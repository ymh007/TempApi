using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// 案场线下活动详情Model
    /// </summary>
    public class OrderView_Case : OrderDetailView
    {
        public OrderView_Case(string orderCode){
        
            var order = OrderAdapter.Instance.LoadByCode(orderCode);

            this.Order = order;

            this.Project = ProjectAdapter.Instance.LoadByCode(order.ProjectCode);
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        public string StatusStr
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(Order.Status);
            }
        }
    }
        
}
using MCS.Library.Core;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderForm
    {
        /// <summary>
        /// 
        /// </summary>
        public OrderForm()
        {
            OrderCode = Guid.NewGuid().ToString();
        }
        private string OrderCode { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 商品总价
        /// </summary>
        public float GoodsPrice { get; set; }
        /// <summary>
        /// 是否需要支付
        /// </summary>
        public Boolean NeedPay { get; set; }
        /// <summary>
        /// 下单人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 子编码(场次编码)
        /// </summary>
        public string SubProjectCode { get; set; }

        /// <summary>
        /// 商品分数
        /// </summary>
        public int GoodsCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceiverCellPhone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReceiverName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReciverProvince { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReciverCity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DetailAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Order ToOrder(OrderType type)
        {
            return new Order
            {
                Code = OrderCode,
                Type = type,
                CreateTime = DateTime.Now,
                Creator = this.Creator,
                GoodsPrice = this.GoodsPrice,
                ProjectCode = this.ProjectCode,
                GoodsCount = this.GoodsCount == 0 ? 1 : this.GoodsCount,
                IsValid = true,
                OrderNo = this.GetOrderNo(type),
                PayWay = PayWayType.None,
                Remarks = "",
                Status = this.GetOrderStatus(type),
                Total = this.GoodsPrice * (this.GoodsCount == 0 ? 1 : this.GoodsCount),
                TradeNo = "",
                VoucherPrice = 0,
                SubProjectCode = this.SubProjectCode
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OrderAddress ToOrderAddress()
        {
            return new OrderAddress
            {
                Code = Guid.NewGuid().ToString(),
                Receiver = ReceiverName,
                DetailAddress = DetailAddress,
                City = ReciverCity,
                Province = ReciverProvince,
                OrderCode = OrderCode,
                Phone = ReceiverCellPhone
            };
        }

        private string GetOrderNo(OrderType type)
        {
            if (type == OrderType.Topic)
            {
                return SystemNumAdapter.Instance.Create(BusinessCodeDefine.Topic);
            }
            else if (type == OrderType.Coupon)
            {
                return SystemNumAdapter.Instance.Create(BusinessCodeDefine.Coupon);
            }
            else if (type == OrderType.Tejiafang)
            {
                return SystemNumAdapter.Instance.Create(BusinessCodeDefine.TejiafangOrder);
            }
            else if (type == OrderType.ZaiShou)
            {
                return SystemNumAdapter.Instance.Create(BusinessCodeDefine.ZaiShou);
            }
            else if (type == OrderType.Anchang)
            {
                return SystemNumAdapter.Instance.Create(BusinessCodeDefine.Anchang);
            }
            return string.Empty;
        }

        private OrderStatus GetOrderStatus(OrderType type)
        {
            if (type == OrderType.Topic)
                return OrderStatus.Topic_Unaccepted;
            else if (type == OrderType.Coupon) {
                if (GoodsPrice > 0)
                {
                    return OrderStatus.Coupon_Unpaid;
                }
                else {
                    return OrderStatus.Coupon_UnDeliver;
                }
            }
            else if (type == OrderType.ZaiShou)
            {
                if (GoodsPrice > 0)
                {
                    return OrderStatus.ZaiShou_None;
                }
                return OrderStatus.ZaiShou_Paid;
            }
            else if (type == OrderType.Tejiafang)
            {
                if (GoodsPrice > 0)
                {
                    return OrderStatus.Tejiafang_None;
                }
                return OrderStatus.Tejiafang_Shared;
            }
            else if (type == OrderType.Anchang)
            {
                if (NeedPay)
                {
                    return OrderStatus.Anchang_NotEnroll;
                }
                else
                {
                    return OrderStatus.Anchang_Enrolled;
                }
            }
            return OrderStatus.ZaiShou_None;
        }

    }
}
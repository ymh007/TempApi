using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChou.Data.Models
{
    /// <summary>
    /// 活动--支付信息表
    /// </summary>
    [ORTableMapping("AppPay.TransactionInfo")]
    public class TransactionInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [ORFieldMapping("TradeNo")]
        public string TradeNo { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        [ORFieldMapping("AppId")]
        public string AppId { get; set; }
        /// <summary>
        /// AliPay，WxPay
        /// </summary>
        [ORFieldMapping("TradeType")]
        public int TradeType { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        [ORFieldMapping("TotalFee")]
        public double TotalFee { get; set; }
        /// <summary>
        /// 未付款，已付款，付款失败
        /// </summary>
        [ORFieldMapping("TradeState")]
        public int TradeState { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        [ORFieldMapping("UserId")]
        public string UserId { get; set; }
        /// <summary>
        /// 操作id
        /// </summary>
        [ORFieldMapping("ObjectId")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 支付宝订单编号
        /// </summary>
        [ORFieldMapping("AliTradeNo")]
        public string AliTradeNo { get; set; }
        /// <summary>
        /// 微信统一下单编号
        /// </summary>
        [ORFieldMapping("WxTradeNo")]
        public string WxTradeNo { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
    }
    public class TransactionInfoCollection : EditableDataObjectCollectionBase<TransactionInfo> { }
    public class TransactionInfoAdapter : BaseAdapter<TransactionInfo, TransactionInfoCollection>
    {
        public static TransactionInfoAdapter Instance = new TransactionInfoAdapter();
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public TransactionInfoAdapter()
        {
            BaseConnectionStr = ConnectionString;
        }

        public TransactionInfo GetByObjectID(string objectID)
        {
            TransactionInfoCollection tiColl = Load(m => m.AppendItem("ObjectId", objectID));
            if (tiColl.Count == 0)
            {
                return null;
            }
            TransactionInfo ti = tiColl.Find(t => t.TradeState == 1);
            if (ti != null)
            {
                return ti;
            }
            return tiColl[0];
        }
    }
}
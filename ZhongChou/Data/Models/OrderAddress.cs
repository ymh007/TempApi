using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 订单地址
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.OrderAddress")]
    public class OrderAddress
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 订单编码
        /// </summary>
        [ORFieldMapping("OrderCode")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 接收人
        /// </summary>
        [ORFieldMapping("Receiver")]
        public string Receiver { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [ORFieldMapping("Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [ORFieldMapping("Province")]
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [ORFieldMapping("City")]
        public string City { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [ORFieldMapping("DetailAddress")]
        public string DetailAddress { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [ORFieldMapping("Post")]
        public string Post { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [ORFieldMapping("IDNumber")]
        public string IDNumber { get; set; }

    }

    /// <summary>
    /// 订单地址??
    /// </summary>
    [Serializable]
    public class OrderAddressCollection : EditableDataObjectCollectionBase<OrderAddress>
    {
    }

    /// <summary>
    /// 订单地址???
    /// </summary>
    public class OrderAddressAdapter : UpdatableAndLoadableAdapterBase<OrderAddress, OrderAddressCollection>
    {
        public static readonly OrderAddressAdapter Instance = new OrderAddressAdapter();

        private OrderAddressAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public OrderAddress LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }

        public void DeleteByOrderCode(string orderCode)
        {

            this.Delete(where => where.AppendItem("OrderCode", orderCode));

        }

        public OrderAddressCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public OrderAddressCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        public OrderAddress LoadByOrderCode(string orderCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("OrderCode", orderCode);
            }).FirstOrDefault();
        }
    }


}


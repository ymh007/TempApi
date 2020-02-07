using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 交易失败原因
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.FailedReason")]
    public class FailedReason
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
        /// 原因
        /// </summary>
        [ORFieldMapping("Cause")]
        public string Cause { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

      
        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 原因类型
        /// </summary>
        [ORFieldMapping("ReasonTypeCode")]
        public ReasonType ReasonTypeCode { get; set; }


    }

    /// <summary>
    /// ??
    /// </summary>
    [Serializable]
    public class FailedReasonCollection : EditableDataObjectCollectionBase<FailedReason>
    {
    }

    /// <summary>
    /// ???
    /// </summary>
    public class FailedReasonAdapter : UpdatableAndLoadableAdapterBase<FailedReason, FailedReasonCollection>
    {
        public static readonly FailedReasonAdapter Instance = new FailedReasonAdapter();

        private FailedReasonAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName(); ;
        }

        public FailedReason LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        public FailedReason LoadByOrderCode(string orderCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("OrderCode", orderCode);
            }).FirstOrDefault();
        }
        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //默认逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }

        public FailedReasonCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public FailedReasonCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
    }


}


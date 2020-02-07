using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.AppConfig
{
    /// <summary>
    /// 数字远洋实体
    /// </summary>
    public class OceanDataModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 今日新增
        /// </summary>
        public int AddedAmount { set; get; }
        /// <summary>
        /// 总计
        /// </summary>
        public int TotalAmount { set; get; }
    }

    /// <summary>
    /// 数字远洋集合
    /// </summary>
    public class OceanDataCollection : EditableDataObjectCollectionBase<OceanDataModel>
    {

    }
}
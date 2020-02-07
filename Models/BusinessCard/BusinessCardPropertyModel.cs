using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.BusinessCard
{
    /// <summary>
    /// 名片属性Model
    /// </summary>
    [ORTableMapping("office.BusinessCardProperty")]
    public class BusinessCardPropertyModel : BaseModel
    {
        /// <summary>
        /// 名片编码
        /// </summary>
        [ORFieldMapping("BusinessCardCode")]
        public string BusinessCardCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }

        /// <summary>
        /// 类型值
        /// </summary>
        [ORFieldMapping("TypeValue")]
        public string TypeValue { get; set; }

    }
    /// <summary>
    /// 名片属性集合
    /// </summary>
    public class BusinessCardPropertyCollection : EditableDataObjectCollectionBase<BusinessCardPropertyModel>
    {

    }
}
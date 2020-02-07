using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
namespace Seagull2.YuanXin.AppApi.Models.BusinessCard
{
    /// <summary>
    /// 名片Model
    /// </summary>
    [ORTableMapping("office.BusinessCard")]
    public class BusinessCardModel : BaseModel
    {
        /// <summary>
        /// Logo标识
        /// </summary>
        [ORFieldMapping("LogoKey")]
        public string LogoKey { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        [ORFieldMapping("Position")]
        public string Position { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [ORFieldMapping("Company")]
        public string Company { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ORFieldMapping("Email")]
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [ORFieldMapping("Mobile")]
        public string Mobile { get; set; }
    }

    /// <summary>
    /// 名片集合
    /// </summary>
    public class BusinessCardCollection : EditableDataObjectCollectionBase<BusinessCardModel>
    {

    }
}
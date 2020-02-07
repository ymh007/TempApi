using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.UserHeadPhoto
{

    /// <summary>
    /// 用户头像实体
    /// </summary>
    [ORTableMapping("office.UserHeadPhoto")]
    public class UserHeadPhotoModel : BaseModel
    {

        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [ORFieldMapping("Url")]
        public string Url { get; set; }

        /// <summary>
        /// 是否操作
        /// </summary>
        [ORFieldMapping("IsOperate")]
        public bool IsOperate { get; set; }

        /// <summary>
        /// 是否审核
        /// </summary>
        [ORFieldMapping("IsAudit")]
        public bool IsAudit { get; set; }

        /// <summary>
        /// 操作人编码
        /// </summary>
        [ORFieldMapping("Operator")]
        public string Operator { get; set; }

        /// <summary>
        /// 操作人名称
        /// </summary>
        [ORFieldMapping("OperatorName")]
        public string OperatorName { get; set; }

        /// <summary>
        /// 操作人时间
        /// </summary>
        [ORFieldMapping("OperateTime")]
        public DateTime OperateTime { get; set; }
    }

    /// <summary>
    /// 用户头像集合
    /// </summary>
    public class UserHeadPhotoCollection : EditableDataObjectCollectionBase<UserHeadPhotoModel>
    {

    }
}
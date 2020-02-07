using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Share
{
    /// <summary>
    /// 记录
    /// </summary>
    [ORTableMapping("office.S_Record")]
    public class RecordModel : BaseModel
    {
        /// <summary>
        /// 目标编码
        /// </summary>
        [ORFieldMapping("TargetCode")]
        public string TargetCode { get; set; }

        /// <summary>
        /// 类型（0：阅读；1：点赞）
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
    }

    /// <summary>
    /// 记录集合
    /// </summary>
    public class RecordCollection : EditableDataObjectCollectionBase<RecordModel>
    {

    }
}
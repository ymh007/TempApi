using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.UserOperationRecord
{
    /// <summary>
    /// 用户操作记录 Model
    /// </summary>
    [ORTableMapping("office.UserOperationRecord")]
    public class UserOperationRecordModel : BaseModel
    {
        /// <summary>
        /// 模块标识
        /// </summary>
        [ORFieldMapping("Module")]
        public string Module { get; set; }
    }

    /// <summary>
    /// 用户操作记录 Collection
    /// </summary>
    public class UserOperationRecordCollection : EditableDataObjectCollectionBase<UserOperationRecordModel>
    {

    }
}
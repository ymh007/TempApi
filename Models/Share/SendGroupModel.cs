using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Share
{
    /// <summary>
    /// 发送范围群组
    /// </summary>
    [ORTableMapping("office.S_SendGroup")]
    public class SendGroupModel : BaseModel
    {
        /// <summary>
        /// 群组名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 发送范围群组集
    /// </summary>
    public class SendGroupCollection : EditableDataObjectCollectionBase<SendGroupModel>
    {

    }
}
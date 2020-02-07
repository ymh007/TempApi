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
    /// 发送范围群组人员
    /// </summary>
    [ORTableMapping("office.S_SendGroupPerson")]
    public class SendGroupPersonModel : BaseModel
    {
        /// <summary>
        /// 群组编码
        /// </summary>
        [ORFieldMapping("SendGroupCode")]
        public string SendGroupCode { get; set; }

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
    }

    /// <summary>
    /// 发送范围群组人员集合
    /// </summary>
    public class SendGroupPersonCollection : EditableDataObjectCollectionBase<SendGroupPersonModel>
    {

    }
}
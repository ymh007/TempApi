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
    /// 图文消息组
    /// </summary>
    [ORTableMapping("office.S_Group")]
    public class GroupModel : BaseModel
    {
        /// <summary>
        /// 全部发送
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 群组编码
        /// </summary>
        [ORFieldMapping("SendGroupCode")]
        public string SendGroupCode { get; set; }

        /// <summary>
        /// 菜单编码
        /// </summary>
        [ORFieldMapping("MenuCode")]
        public string MenuCode { get; set; }
    }

    /// <summary>
    /// 图文消息集
    /// </summary>
    public class GroupCollection : EditableDataObjectCollectionBase<GroupModel>
    {

    }
}
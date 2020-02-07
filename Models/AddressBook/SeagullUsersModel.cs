using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook
{
    /// <summary>
    /// 海鸥二用户 Model
    /// </summary>
    [ORTableMapping("Business.SeagullUsers")]
    public class SeagullUsersModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserId", PrimaryKey = true)]
        public string UserId { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }
        /// <summary>
        /// 邀请次数
        /// </summary>
        [ORFieldMapping("InviteCount")]
        public int InviteCount { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [ORFieldMapping("AppVersion")]
        public string AppVersion { get; set; }
    }

    /// <summary>
    /// 海鸥二用户 Collection
    /// </summary>
    public class SeagullUsersCollection : EditableDataObjectCollectionBase<SeagullUsersModel>
    {

    }
}
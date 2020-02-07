using MCS.Library.Data.Mapping;
using System;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.IM
{
    /// <summary>
    /// 群组实体
    /// </summary>
    [ORTableMapping("dbo.group_member")]
    public class GroupMemberModel
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        [ORFieldMapping("Group_Id")]
        public int Group_Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [ORFieldMapping("Uid")]
        public long Uid { get; set; }

        /// <summary>
        /// 是否免消息提醒
        /// </summary>
        [ORFieldMapping("isDisturb")]
        public short IsDisturb { get; set; }
        /// <summary>
        /// 是否有联络
        /// </summary>
        [ORFieldMapping("isContacts")]
        public short IsContacts { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [ORFieldMapping("myNickName")]
        public string MyNickName { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        [ORFieldMapping("isScreenNames")]
        public short isScreenNames { get; set; }

        
    }

    /// <summary>
    /// 
    /// </summary>
    public class GroupMemberCollection : EditableDataObjectCollectionBase<GroupMemberModel>
    {

    }
}
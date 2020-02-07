using MCS.Library.Data.Mapping;
using System;
using MCS.Library.Core;
using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.IM
{
    /// <summary>
    /// 群组实体
    /// </summary>
    [ORTableMapping("IM.GroupInfo")]
    public class GroupInfoModel
    {
        /// <summary>
        /// 群组ID
        /// </summary>
        [ORFieldMapping("GroupId")]
        public int GroupId { get; set; }

        /// <summary>
        /// APP项目ID（移动办公）
        /// </summary>
        [ORFieldMapping("AppId")]
        public int AppId { get; set; }

        /// <summary>
        /// 群组管理员
        /// </summary>
        [ORFieldMapping("Master")]
        public int Master { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [ORFieldMapping("Status")]
        public int Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("VersionStartTime")]
        public DateTime VersionStartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("VersionEndTime")]
        public DateTime VersionEndTime { get; set; }

        /// <summary>
        /// 群组类型
        /// </summary>
        public string GroupType { get; set; }

        /// <summary>
        /// 管理员姓名
        /// </summary>
        public string MasterName { get; set; }

        /// <summary>
        ///IM群组还是微信群组
        /// </summary>
        public string IsIMorWenXin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public class GroupInfoCollection : EditableDataObjectCollectionBase<GroupInfoModel>
        {

        }
    }
}
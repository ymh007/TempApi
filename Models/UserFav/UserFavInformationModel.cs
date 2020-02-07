using System;
using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.UserFav
{
    /// <summary>
    /// 用户资讯收藏实体
    /// </summary>
    [ORTableMapping("office.UserFavInformation")]
    public class UserFavInformationModel : BaseModel
    {
        /// <summary>
        /// WebId
        /// </summary>
        [ORFieldMapping("WebId")]
        public string WebId { get; set; }

        /// <summary>
        /// ListId
        /// </summary>
        [ORFieldMapping("ListId")]
        public string ListId { get; set; }

        /// <summary>
        /// ListItemId
        /// </summary>
        [ORFieldMapping("ListItemId")]
        public int ListItemId { get; set; }

        /// <summary>
        /// 类型（new，notice）
        /// </summary>
        [ORFieldMapping("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 资讯标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [ORFieldMapping("Source")]
        public string Source { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [ORFieldMapping("ReleaseTime")]
        public DateTime ReleaseTime { get; set; }
    }

    /// <summary>
    /// 用户资讯收藏集合
    /// </summary>
    public class UserFavInformationCollection : EditableDataObjectCollectionBase<UserFavInformationModel>
    {

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using System.Runtime.Serialization;

namespace Seagull2.YuanXin.AppApi.Models.Channel
{
    /// <summary>
    /// 我的频道
    /// </summary>
    [ORTableMapping("office.ChannelFav")]
    public class ChannelFavModel : BaseModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 频道编码
        /// </summary>
        [ORFieldMapping("ChannelCode")]
        public string ChannelCode { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }

    /// <summary>
    /// 推荐频道集合
    /// </summary>
    public class ChannelFavCollection : EditableDataObjectCollectionBase<ChannelFavModel>
    {

    }
}
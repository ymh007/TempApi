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
    /// 推荐频道
    /// </summary>
    [ORTableMapping("office.Channel")]
    public class ChannelModel : BaseModel
    {
        /// <summary>
        /// 频道名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 频道标识
        /// </summary>
        [ORFieldMapping("Keys")]
        public string Keys { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否默认配置
        /// </summary>
        [ORFieldMapping("IsDefault")]
        public bool IsDefault { get; set; }

    }

    /// <summary>
    /// 频道集合
    /// </summary>
    public class ChannelCollection : EditableDataObjectCollectionBase<ChannelModel>
    {
        
    }
}
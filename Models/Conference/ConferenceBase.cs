using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ConferenceBase
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }
}
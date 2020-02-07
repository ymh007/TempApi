using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 公共实体类（表的公共字段）
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 主键编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [ORFieldMapping("Modifier")]
        public string Modifier { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 有效状态
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }
}
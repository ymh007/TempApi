using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 会议圈类型（企业会议圈，年会会议圈）
    /// </summary>
    public enum EnumMomentType
    {
        /// <summary>
        /// 企业会议圈
        /// </summary>
        [EnumItemDescription("企业会议圈")]
        Company = 0,
        /// <summary>
        /// 年会会议圈
        /// </summary>
        [EnumItemDescription("年会会议圈")]
        Conference = 1
    }
}
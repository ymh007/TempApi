using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 作品评选方式
    /// </summary>
    public enum WorksSelectedTypeEnum
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumItemDescription("无")]
        None = 0,

        /// <summary>
        /// 用户投票
        /// </summary>
        [EnumItemDescription("用户投票")]
        UserVote = 1,

        /// <summary>
        /// 商家评选
        /// </summary>
        [EnumItemDescription("商家评选")]
        BusinessSelection = 2

    }
}

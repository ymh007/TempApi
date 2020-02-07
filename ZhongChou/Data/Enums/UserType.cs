using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumItemDescription("草稿箱")]
        None = 0,
        /// <summary>
        /// 普通用户
        /// </summary>
        [EnumItemDescription("普通用户")]
        User = 1,
        /// <summary>
        /// 企业用户
        /// </summary>
        [EnumItemDescription("企业用户")]
        Company = 2,
        /// <summary>
        /// 平台用户
        /// </summary>
        [EnumItemDescription("平台用户")]
        Platform = 3
    }
}

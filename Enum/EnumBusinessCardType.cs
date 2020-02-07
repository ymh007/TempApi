using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 个人名片属性类型
    /// </summary>
    public enum EnumBusinessCardType
    {
        /// <summary>
        /// 地址
        /// </summary>
        [EnumItemDescription("地址")]
        Address = 0,
        /// <summary>
        /// 座机
        /// </summary>
        [EnumItemDescription("座机")]
        Phone = 1,
    }
}
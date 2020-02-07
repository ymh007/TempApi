using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 状态描述
    /// </summary>
    public enum EnumStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        [EnumItemDescription("禁用")]
        Disable = 0,

        /// <summary>
        /// 启用
        /// </summary>
        [EnumItemDescription("启用")]
        Enable = 1
    }
}
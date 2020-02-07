using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 服务器端返回状态
    /// </summary>
    public enum EnumServerResStatus
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [EnumItemDescription("操作成功!")]
        ResponseSuccess = 0,
        /// <summary>
        /// 操作失败
        /// </summary>
        [EnumItemDescription("操作失败!")]
        ResponseFaild = -1,
        /// <summary>
        /// 所提交的Menu已经在存在,请重新选择!
        /// </summary>
        [EnumItemDescription("所提交的菜单已经在存在,请重新选择!")]
        ResponseMenuExists = -2,
        /// <summary>
        /// 所提交的菜单不存在,请重新选择!
        /// </summary>
        [EnumItemDescription("所提交的菜单不存在,请重新选择!")]
        ResponseMenuNotExists = -3,
        /// <summary>
        /// 提交的关注已经存在!
        /// </summary>
        [EnumItemDescription("提交的关注已经存在!")]
        ResponseFavoriateExists = -4,
        /// <summary>
        /// 父级菜单创建失败
        /// </summary>
        [EnumItemDescription("父级菜单创建失败!")]
        ResponseParentMenuNotCreatFaild = -5
    }
}
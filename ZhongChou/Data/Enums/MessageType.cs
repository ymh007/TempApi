using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [EnumItemDescription("未知")]
        None = 0,
        /// <summary>
        /// 远洋通告
        /// </summary>
        [EnumItemDescription("远洋通告")]
        Announcement = 1,

        /// <summary>
        /// 订单助手
        /// </summary>
        [EnumItemDescription("订单消息")]
        OrderHelper = 2,

        /// <summary>
        /// 私信
        /// </summary>
        [EnumItemDescription("私信")]
        Chat = 3,

        /// <summary>
        /// 群聊
        /// </summary>
        [EnumItemDescription("群聊")]
        GropuChat = 4
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageMode
    {
        /// <summary>
        /// 文本
        /// </summary>
        [EnumItemDescription("文本")]
        Text = 0,
        /// <summary>
        /// 链接
        /// </summary>
        [EnumItemDescription("链接")]
        Link = 1,
        /// <summary>
        /// 图文
        /// </summary>
        [EnumItemDescription("图文")]
        ImageText = 2

    }
}

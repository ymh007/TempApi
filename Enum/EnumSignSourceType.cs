using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 签到来源类型
    /// </summary>
    public enum EnumSignSourceType
    {
        /// <summary>
        /// 微信号
        /// </summary>
        [EnumItemDescription("微信号")]
        WinXin = 0,  
        /// <summary>
        /// 移动办公APP
        /// </summary>
        [EnumItemDescription("移动办公APP")]
        App = 1
    }
}
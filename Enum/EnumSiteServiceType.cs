using MCS.Library.Core; 
namespace Seagull2.YuanXin.AppApi.Enum
{
    /// <summary>
    /// 会议服务（饮用水，文具，IT服务，麦克，其他）
    /// </summary>
    public enum EnumSiteServiceType
    {
        /// <summary>
        /// 饮用水
        /// </summary>
        [EnumItemDescription("饮用水")]
        Water = 0,
        /// <summary>
        /// 文具
        /// </summary>
        [EnumItemDescription("文具")]
        Stationery = 1,
        /// <summary>
        /// IT服务
        /// </summary>
        [EnumItemDescription("IT服务")]
        ITService = 2,
        /// <summary>
        /// 麦克
        /// </summary>
        [EnumItemDescription("麦克")]
        Mike = 3,
        /// <summary>
        /// 其他
        /// </summary>
        [EnumItemDescription("其他")]
        Others = 4,
    }
}
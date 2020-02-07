using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity
{
    /// <summary>
    /// 附件类型枚举
    /// </summary>
    public enum AttachmentTypeEnum
    {
        /// <summary>
        /// 生活家话题
        /// </summary>
        [EnumItemDescription("项目详情图片")]
        zc_Project_Detail = 13,

        /// <summary>
        /// 生活家话题
        /// </summary>
        [EnumItemDescription("生活家话题")]
        zc_Topic = 1000,

        /// <summary>
        /// 生活家见面凭证
        /// </summary>
        [EnumItemDescription("生活家见面凭证")]
        zc_Topic_Meeting = 1001,

        /// <summary>
        /// 生活家评价
        /// </summary>
        [EnumItemDescription("生活家评价")]
        zc_Topic_Evaluation = 1002,

        /// <summary>
        /// 生活家头像
        /// </summary>
        [EnumItemDescription("生活家头像")]
        zc_TopicHeadImage = 1003,

        /// <summary>
        /// 生活家名片
        /// </summary>
        [EnumItemDescription("生活家名片")]
        zc_TopicBusinessCard = 1004,

        /// <summary>
        /// 优惠券封面轮播图
        /// </summary>
        [EnumItemDescription("优惠券封面轮播图")]
        zc_CouponCoverImg = 1005,

        [EnumItemDescription("用户头像")]
        zc_UserHeadImage = 999,

        /// <summary>
        /// 活动图片
        /// </summary>
        [EnumItemDescription("活动图片")]
        ActivityImg = 12,

        /// <summary>
        /// 活动详情图片
        /// </summary>
        [EnumItemDescription("活动详情图片")]
        ActivityDetailImg = 13,

        /// <summary>
        /// 奖项图片
        /// </summary>
        [EnumItemDescription("奖项图片")]
        AwardsImg = 14,

        /// <summary>
        /// 活动作品
        /// </summary>
        [EnumItemDescription("活动作品")]
        ActivityWorks = 1010,


        /// <summary>
        /// 用户评论
        /// </summary>
        [EnumItemDescription("项目评论")]
        UserComment = 1011,
        /// <summary>
        /// 活动(订单)评论
        /// </summary>
        [EnumItemDescription("活动(订单)评价")]
        UserEvaluation = 1012,

        /// <summary>
        /// 微分享图片
        /// </summary>
        [EnumItemDescription("微分享图片")]
        WeSharePicture = 20,

        /// <summary>
        /// 微分享视频
        /// </summary>
        [EnumItemDescription("微分享视频")]
        WeShareVideo = 21,
        /// <summary>
        /// 微分享话题图片
        /// </summary>
        [EnumItemDescription("微分享话题图片")]
        WeShareTagPicture = 22,
        /// <summary>
        /// APP意见反馈
        /// </summary>
        [EnumItemDescription("微分享话题图片")]
        Feedback = 99999
    }
}

using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Enums
{
    /// <summary>
    /// 积分来源类型
    /// </summary>
    public enum SourceType
    {
        /// <summary>
        /// 签到
        /// </summary>
        [EnumItemDescription("签到")]
        Sign = 0,
        /// <summary>
        /// 分享
        /// </summary>
        [EnumItemDescription("分享")]
        Share = 1,        
        /// <summary>
        /// 讨论
        /// </summary>
        [EnumItemDescription("讨论")]
        Discuss=2,       
        /// <summary>
        /// 参与活动
        /// </summary>
        [EnumItemDescription("参与活动")]
        AvtivityIn = 3,
        /// <summary>
        /// 分享
        /// </summary>
        [EnumItemDescription("参与征集")]
        CollectIn = 4,
        /// <summary>
        /// 讨论
        /// </summary>
        [EnumItemDescription("参与试用")]
        Trial = 5,
        /// <summary>
        /// 完善资料
        /// </summary>
        [EnumItemDescription("参与收益共享")]
        RevenueShare = 6,
        /// <summary>
        /// 签到
        /// </summary>
        [EnumItemDescription("推荐入驻")]
        RecommendIn = 7,
        /// <summary>
        /// 分享
        /// </summary>
        [EnumItemDescription("评价活动")]
        AssessAvtivity = 8,
        /// <summary>
        /// 讨论
        /// </summary>
        [EnumItemDescription("评价试用")]
        AssessTrial = 9,
        /// <summary>
        /// 完善资料
        /// </summary>
        [EnumItemDescription("邀请好友注册")]
        InviteFriend = 10,
        /// <summary>
        /// 分享
        /// </summary>
        [EnumItemDescription("补充个人资料")]
        Perfect = 11,
        /// <summary>
        /// 讨论
        /// </summary>
        [EnumItemDescription("上传头像")]
        Gravatar = 12,
        /// <summary>
        /// 完善资料
        /// </summary>
        [EnumItemDescription("修改昵称")]
        Nickname = 13,
        /// <summary>
        /// 分享
        /// </summary>
        [EnumItemDescription("给活动点赞")]
        Praise = 14,
        /// <summary>
        /// 关注一个众筹
        /// </summary>
        [EnumItemDescription("关注一个众筹")]
        FollowCrowdfund = 15

    }
}

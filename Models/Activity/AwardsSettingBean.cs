using System;

namespace Seagull2.YuanXin.AppApi.Models
{
    //奖项设置 - 线上活动
    public class AwardsSettingBean
    {
        public string Code { get; set; } //奖项设置的唯一标识 - 线上活动
        public string AwardsName { get; set; } //奖品名称 - 线上活动（用户投票）
        public int AwardsCount { get; set; } //中奖人数 - 线上活动（用户投票）
        public int StartRanking { get; set; } //中奖起始人数 - 线上活动（用户投票）
        public int StopRanking { get; set; }   //中奖截止人数 - 线上活动（用户投票）
        public string AwardsContent { get; set; } //奖品描述 - 线上活动（用户投票）
        public int SortNo { get; set; } //奖项序号 - 线上活动（用户投票）
        public string ProjectCode { get; set; }//关联的项目ID - 线上活动


        public DateTime VoteEndTime { get; set; } //投票截止时间 - 线上活动（用户投票）
               

    }
}
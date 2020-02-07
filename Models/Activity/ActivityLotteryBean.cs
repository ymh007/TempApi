
namespace Seagull2.YuanXin.AppApi.Models
{
    //活动抽奖信息 - 众筹（特价）
    public class ActivityLotteryBean
    {
        public string Code { get; set; } //唯一ID - 特价
        public string OrderCode { get; set; } //对应订单的唯一ID - 特价
        public string BallCode { get; set; } // 双色球号码 - 特价
        public string LotteryTime { get; set; }//开奖时间 - 特价
        public string LotteryResult { get; set; }//开奖结果 - 特价
        public string ProjectCode { get; set; }//关联的项目ID - 特价
        public string LotteryNo { get; set; }//双色球期号 - 特价
        public string PlanLotteryTime { get; set; }//计划开奖时间 - 特价
    }
}
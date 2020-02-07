
namespace Seagull2.YuanXin.AppApi.Models
{
    public class OnlineActivityDetail
    {
        public string name { get; set; } //活动名称
        public string coverImg { get; set; } //活动图片
        public string enrollDeadline { get; set; } //投票截止时间
        public string endTime { get; set; } //作品提交截止时间
        public int worksSelectedType { get; set; } //评选方式
        public string auditStatus { get; set; } //审核状态
        public string opinion { get; set; } //审核意见
        public string auditDate { get; set; } //审核时间
        public int supportNo { get; set; } //支持人数
        public bool isOpenPrize { get; set; } //是否已开奖
        public string systemTime { get; set; } //系统当前时间
    }
}
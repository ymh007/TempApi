
namespace Seagull2.YuanXin.AppApi.Models
{
    public class OfflineActivityDetail
    {
        public string name { get; set; } //活动名称
        public string coverImg { get; set; } //活动图片
        public string startTime { get; set; } //开始时间
        public string endTime { get; set; } //结束时间
        public string address { get; set; } //活动地点
        public string cityName { get; set; } //活动所在城市
        public int supportNo { get; set; } //报名人数
        public string auditStatus { get; set; } //审核状态
        public string opinion { get; set; } //审核意见
        public string auditDate { get; set; } //审核时间
        public string systemTime { get; set; } //系统当前时间
    }
}
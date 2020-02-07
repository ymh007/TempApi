
namespace Seagull2.YuanXin.AppApi.Models
{
    public class ActivityEventInfo
    {
        public string startTime { get; set; }//活动场次开始时间 - 线下活动
        public string endTime { get; set; }//活动场次开始时间 - 线下活动
        public float hours { get; set; }//活动场次预计时长 - 线下活动
        public float price { get; set; }//活动报名价格 - 线下活动
        public int limitNo { get; set; }//活动限制人数 - 线下活动
    }
}
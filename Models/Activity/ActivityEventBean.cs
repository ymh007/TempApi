
namespace Seagull2.YuanXin.AppApi.Models
{
    //活动场次 - 线下活动
    public class ActivityEventBean
    {
        public string Code { get; set; }//活动场次的唯一标识 - 线下活动
        public string StartTime { get; set; }//活动场次开始时间 - 线下活动
        public float Hours { get; set; }//活动场次预计时长 - 线下活动
        public float Price{ get; set; }//活动报名价格 - 线下活动
        public int LimitNo{ get; set; }//活动限制人数 - 线下活动
        public string ProjectCode { get; set; }//关联的项目ID - 线下活动
        public int SortNo { get; set; }//排序顺序

    }
}

namespace Seagull2.YuanXin.AppApi.Models
{
    public class ActivityBroadcastBean : AbstractBean
    {
        public string Name { get; set; }//直播名称 
        public string StartTime { get; set; }//直播开始时间 
        public string CoverImg { get; set; }//直播预报封面图片
        public int SortNo { get; set; } //直播排序号
        public string Detail { get; set; }//直播介绍 
        public string ProjectCode { get; set; }//关联的项目ID 
        public bool IsVaild { get; set; }//直播有效性，0：无效，1：有效 
    }
}
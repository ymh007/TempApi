
namespace Seagull2.YuanXin.AppApi.Models
{
    //项目标签 - 活动
    public class ProjectTagBean
    {
        public string Code { get; set; }//活动标签的唯一标识 - 线下活动、线上活动
        public string TagCode { get; set; } //活动标签的编码 - 线下活动、线上活动
        public string TagName { get; set; }//活动标签的名称 - 线下活动、线上活动
        public int SortNo { get; set; } //活动选择顺序- 线下活动、线上活动
        public string ProjectCode { get; set; }//关联的项目ID - 线上活动、线下活动
    }
}
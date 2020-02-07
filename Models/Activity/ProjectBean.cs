
namespace Seagull2.YuanXin.AppApi.Models
{
    //众筹、活动项目表
    public class ProjectBean:AbstractBean
    {
        
        public string Type { get; set; }//项目类型，包括在售房众筹、特价房众筹、线上活动、线下活动 - 在售、特价、线上活动、线下活动
        public string Name { get; set; }//项目名称 - 在售、线下活动、线上活动
        public string ProvinceCode { get; set; } //省份编码 - 在售
        public string Province { get; set; } //省份名称 - 在售
        public string CityCode { get; set; } //城市编码 - 在售、线下活动
        public string City { get; set; } //城市名称 - 在售、线下活动
        public string Address { get; set; } //详细地址 - 在售、线下活动
        public string CoverImg { get; set; } //封面图片 - 在售、特价
        public int TargetNo { get; set; } //众筹信息 - 目标人数 - 在售、特价
        public int SupportNo { get; set; } //众筹信息 - 支持人数 - 在售、特价、线上、线下活动
        public string StartTime { get; set; } //众筹信息 - 开始时间 - 在售、特价、线上活动
        public string EndTime { get; set; } //众筹信息 - 结束时间 - 在售、特价、线上活动
        public string BuildingCode { get; set; } //关联的楼盘信息的ID - 在售、特价
        public int AuditStatus { get; set; } //审核状态 - 在售、特价、线上、线下
        public string EnrollDeadline { get; set; } //报名截止时间 - 线下活动
        public string Detail { get; set; } //活动详情，需要存储活动照片 - 线下活动、线上活动
        public int WorksSelectedType { get; set; }  //作品评选方式，1：用户投票，2：自行评估 - 线上活动
        public string CompanyCode { get; set; } //创建项目的公司ID
        public int IsValid { get; set; } //项目是否有效 
        public string Summary { get; set; } //项目介绍
        public int SubItemJoinLimit { get; set; } //是否可报名多个场次活动 - 线下活动
        public int PraiseNo { get; set; } //点赞数量 - 线上、线下活动
        public int ShareNo { get; set; } //分享数量 - 线上、线下活动
        public int CommentNo { get; set; } //话题数量- 线上、线下活动
        public int SubItemNo { get; set; } //场次数量- 线下活动
        public int IssueRange { get; set; } //发布范围- 线下活动


    }
}
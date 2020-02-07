
namespace Seagull2.YuanXin.AppApi.Models
{
    //用于APP调用时返回的活动列表信息
    public class ActivityInfo
    {
        public string code { get; set; }    //项目的唯一标识 -列表查询
        public string name { get; set; }      //项目名称 - 列表查询
        public string auditStatus { get; set; }//审核状态 - 列表查询
        public string activityImg { get; set; }  //项目封面图片地址 - 列表查询
        public string cityName { get; set; }  //项目所在城市名称 - 列表查询（线下）
        public int praiseNo { get; set; }  //活动点赞数量 - 列表查询
        public int shareNo { get; set; }  //活动分享数量 - 列表查询
        public int supportNo { get; set; }  //报名人数 - 列表查询
        public int subItemNo { get; set; }  //话题数量 - 列表查询
    }
}
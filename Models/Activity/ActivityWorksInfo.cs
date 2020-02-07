
using System.Collections.Generic;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class ActivityWorksInfo
    {
        public string worksCode { get; set; } //作品的编号
        public string worksContent { get; set; } //作品描述
        public string createTime { get; set; } //提交时间
        public string creator { get; set; }//参加人姓名
        public string phone { get; set; }//参加人手机号码
        public int voteCount { get; set; }//累计票数
        public string worksImg { get; set; }//作品图片

        public List<string> worksImgList = new List<string>();//作品图片列表
    }
}
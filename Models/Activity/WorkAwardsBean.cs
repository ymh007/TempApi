
namespace Seagull2.YuanXin.AppApi.Models
{
    //中奖作品信息表
    public class WorkAwardsBean
    {
       public string Code { get; set; } //中奖作品信息表唯一标识
       public string ActivityWorksCode { get; set; } //作品唯一标识
       public string AwardsSettingCode { get; set; } //奖项唯一标识
       public string ProjectCode { get; set; } //项目唯一标识
    }
}
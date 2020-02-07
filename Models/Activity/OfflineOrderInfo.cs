
namespace Seagull2.YuanXin.AppApi.Models
{
    public class OfflineOrderInfo
    {
        public string createTime { get; set; }//订单时间
        public string status { get; set; } //订单状态
        public string orderNo { get; set; } //订单号
        public string receiver { get; set; }//购房人姓名
        public string phone { get; set; } //手机号
        public string payWay { get; set; } //支付方式
    }
}
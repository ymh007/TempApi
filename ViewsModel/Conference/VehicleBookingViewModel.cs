using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    public class VehicleBookingViewModel : ViewModelBase
    {

    }
    public class VehicleBookingListModel
    {
        /// <summary>
        /// 预定时间
        /// </summary>
        public DateTime ReserveTime { get; set; }

        /// <summary>
        /// 预定时间Str
        /// </summary>
        public string ReserveTimeStr
        {
            get
            {
                return this.ReserveTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 出发地
        /// </summary>
        public string BeginPlace { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        public string EndPlace { get; set; }

        /// <summary>
        /// 预订人姓名
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 预订人电话
        /// </summary>
        public string ContactPhone { get; set; }
        /// <summary>
        /// 预订人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 预订人联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 预定车辆ID
        /// </summary>
        public string ID { get; set; }

    }
    public class VehicleBookingListModelAdapter : ViewBaseAdapter<VehicleBookingListModel, List<VehicleBookingListModel>>
    {
        private static string ConnectionString = "yuanxin";
        public static VehicleBookingListModelAdapter Instance = new VehicleBookingListModelAdapter();
        public VehicleBookingListModelAdapter() : base(ConnectionString)
        {

        }
        /// <summary>
        /// 根据条件查询预定列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="ConferenceID"></param>
        /// <param name="BeginPlace"></param>
        /// <param name="EndPlace"></param>
        /// <param name="ReserveTime"></param>
        /// <returns></returns>
        public ViewPageBase<List<VehicleBookingListModel>> GetVehicleBookingListByPage(int pageIndex, string ConferenceID, string BeginPlace, string EndPlace, DateTime ReserveTime, DateTime searchTime, string userId)
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            StringBuilder fromAndWhereSQL = new StringBuilder();
            ViewPageBase<List<VehicleBookingListModel>> result = new ViewPageBase<List<VehicleBookingListModel>>();
            string selectSQL = "SELECT A.[ID],A.[BeginPlace],A.[EndPlace],A.[ReserveTime],B.Name ContactName,B.MobilePhone ContactPhone,B.Name, B.MobilePhone,A.Phone,A.Remark";

            fromAndWhereSQL.Append(string.Format(@"FROM [office].[VehicleBooking] A  
                                                INNER JOIN office.Conference C ON A.ConferenceID=C.ID
                                                INNER join [office].[Attendee] B ON A.[AttendeeID]=B.ID AND B.ConferenceID=C.ID  
                                                where B.ValidStatus=1 AND A.ConferenceID='{0}' AND A.CreateTime<='{1}'", ConferenceID, searchTime));
            string orderSQL = "order by A.CreateTime DESC";

            if (!string.IsNullOrEmpty(BeginPlace) && BeginPlace != "null")
            {
                fromAndWhereSQL.Append(string.Format(" and A.BeginPlace LIKE '%{0}%'", BeginPlace));
            }
            if (!string.IsNullOrEmpty(EndPlace) && BeginPlace != "null")
            {
                fromAndWhereSQL.Append(string.Format(" and A.EndPlace LIKE '%{0}%'", EndPlace));
            }
            if (ReserveTime != DateTime.MinValue)
            {
                fromAndWhereSQL.Append(string.Format(" and A.ReserveTime ='{0}'", ReserveTime));
            }
            if (!userId.IsEmptyOrNull())
            {
                fromAndWhereSQL.Append(string.Format(" and A.AttendeeID ='{0}'", userId));
            }
            result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL.ToString(), orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return result;
        }
    }
}
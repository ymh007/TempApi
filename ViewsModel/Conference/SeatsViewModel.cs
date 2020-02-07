using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    public class SeatsViewModel : ViewModelBase
    {
        /// <summary>
        /// 座位号
        /// </summary>
        public string SeatAddress { get; set; }
    }

    public class SeatsViewUpdateModel
    {
        /// <summary>
        /// 座位集合
        /// </summary>
        public SeatsCollection SeatColl { get; set; }

        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceID { get; set; }
    }
    /// <summary>
    /// 座位列表集合
    /// </summary>
    public class SeatsViewListModel
    {
        /// <summary>
        /// 参会人编码
        /// </summary>
        public string AttendeeID { get; set; }

        /// <summary>
        /// 参会人部门
        /// </summary>
        public string OrganizationStructure { get; set; }

        /// <summary>
        /// 参会人姓名
        /// </summary>
        public string AttendeeName { get; set; }
        /// <summary>
        /// 座位号
        /// </summary>
        public string SeatAddress { get; set; }
    }

    /// <summary>
    /// 禁用座位信息
    /// </summary>
    public class DisableSeatsViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceID { get; set; }

        /// <summary>
        /// 座位实际对应的坐标编号
        /// </summary>
        public string SeatAddress { get; set; }
    }

    /// <summary>
    /// 禁用整行或者整列座位信息视图类
    /// </summary>
    public class DisableRowSeatsViewModel
    {
        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceID { get; set; }

        /// <summary>
        /// 座位总行数
        /// </summary>
        public string SeatRows { get; set; }

        /// <summary>
        /// 座位总列数
        /// </summary>
        public string SeatColumns { get; set; }

        /// <summary>
        /// 禁用行或列的标记（1表示禁用行，2表示禁用列）
        /// </summary>
        public string flag  { get; set; }

        /// <summary>
        /// 当前要禁用的行或列编号
        /// </summary>
        public string NowSeatRow { get; set; }
    }

    public class SeatsViewListModelAdapter : ViewBaseAdapter<SeatsViewListModel, List<SeatsViewListModel>>
    {
        private static string ConnectionString = "yuanxin";
        public static SeatsViewListModelAdapter Instance = new SeatsViewListModelAdapter();
        public SeatsViewListModelAdapter() : base(ConnectionString)
        {

        }

        public ViewPageBase<List<SeatsViewListModel>> GetUserSeatListByPage(int pageIndex, string ConferenceID, DateTime searchTime)
        {
            if (pageIndex==1)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "SELECT A.[ID],A.AttendeeID ,A.[ConferenceID],A.[Name] as AttendeeName,A.[OrganizationStructure], B.SeatAddress";
            string fromAndWhereSQL = string.Format(@"FROM [office].[Attendee] A  
                                                    LEFT JOIN [office].[Seats] B ON A.[ConferenceID]=B.[ConferenceID] AND A.AttendeeID=B.AttendeeID   
                                                    where A.[ValidStatus]= 1 and A.[ConferenceID]='{0}' and A.CreateTime<='{1}'", ConferenceID, searchTime);
            string orderSQL = "order by A.CreateTime DESC";

            ViewPageBase<List<SeatsViewListModel>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
          
            return result;
        }

        
    }

    /// <summary>
    /// 禁用座位视图操作类
    /// </summary>
    public class DisableSeatsViewListModelAdapter : ViewBaseAdapter<DisableSeatsViewModel, List<DisableSeatsViewModel>>
    {
        private static string ConnectionString = "yuanxin";
        public static DisableSeatsViewListModelAdapter Instance = new DisableSeatsViewListModelAdapter();
        public DisableSeatsViewListModelAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 根据会议编码获取被禁用的座位信息
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        public List<DisableSeatsViewModel> GetDisableSeatsModels(string conferenceID)
        {
            string sql = string.Format(@"SELECT SeatAddress,ConferenceID FROM [office].[DisableSeats] WHERE ConferenceID='{0}' AND ValidStatus=1", conferenceID);
            return LoadTColl(sql);
        }
    }
}
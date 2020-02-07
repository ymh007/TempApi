using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Conference
{
    /// <summary>
    /// 手机端会议议程 ViewMod
    /// </summary>
    public class ConferenceAgendaAppViewModel
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
    }


    public class AgendaList
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 我是否已经签到
        /// </summary>
        public bool IsSigned { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// 开始时间Str
        /// </summary>
        public string BeginDateStr
        {
            get
            {
                return this.BeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 结束时间Str
        /// </summary>
        public string EndDateStr
        {
            get
            {
                return this.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 签到开始时间
        /// </summary>
        public DateTime SignBeginDate { get; set; }
        /// <summary>
        /// 签到开始时间Str
        /// </summary>
        public string SignBeginDateStr
        {
            get
            {
                return this.SignBeginDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 签到结束时间
        /// </summary>
        public DateTime SingEndDate { get; set; }
        /// <summary>
        /// 签到结束时间Str
        /// </summary>
        public string SingEndDateStr
        {
            get
            {
                return this.SingEndDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 人员备注
        /// </summary>
        public string PeopleRemark { get; set; }

        /// <summary>
        /// 地点备注
        /// </summary>
        public string AddreddRemark { get; set; }
    }

    public class AgendaListAdapter : ViewBaseAdapter<AgendaList, List<AgendaList>>
    {
        private static string ConnectionString = "yuanxin";
        public static AgendaListAdapter Instance = new AgendaListAdapter();
        public AgendaListAdapter() : base(ConnectionString)
        {

        }
        /// <summary>
        /// 分页查询方法
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ViewPageBase<List<AgendaList>> GetAgendaLists(string conferenceID, int pageIndex, DateTime searchTime)
        {
            ViewPageBase<List<AgendaList>> result = new ViewPageBase<List<AgendaList>>();

            string selectSQL = "SELECT ID,Title,BeginDate,EndDate,PeopleRemark,AddreddRemark";
            string fromAndWhereSQL = string.Format(@"FROM  office.Agenda
                                            WHERE ConferenceID='{0}' AND CreateTime<='{1}'", conferenceID, searchTime);
            string orderSQL = "ORDER BY BeginDate";
            result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>
        /// 获取某个会议下需要签到的议程列表--分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="ConferenceID">会议编码</param>
        /// <param name="searchTime">首页查询时间</param>
        /// <param name="AttendeeID">参会人编码</param>
        /// <returns></returns>
        public ViewPageBase<List<AgendaList>> GetAgeendaListByPage(int pageIndex, string ConferenceID, DateTime searchTime, string AttendeeID)
        {
            if (searchTime == DateTime.MinValue || searchTime == null)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = string.Format(@"select A.[ID],A.Title,A.BeginDate,A.EndDate,
                            CASE WHEN (SELECT SignDate FROM office.SignDetail WHERE AgendaID=A.ID AND AttendeeID='{0}') IS NULL THEN 0 ELSE 1 END AS IsSigned", AttendeeID);
            string fromAndWhereSQL = string.Format(@"from [office].[Agenda] A 
                                                    where A.[ConferenceID]='{0}' AND A.CreateTime<'{1}' AND A.NeedSign=1", ConferenceID, searchTime);
            string orderSQL = "order by A.CreateTime DESC";
            ViewPageBase<List<AgendaList>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return result;
        }
        /// <summary>
        /// 获取某个会议下需要签到的议程列表--所有
        /// </summary>
        /// <param name="ConferenceID">会议编码</param>
        /// <param name="AttendeeID">参会人编码</param>
        /// <returns></returns>
        public List<AgendaList> GetAgeendaList(string ConferenceID, string AttendeeID = "")
        {

            string sql = string.Format(@"select A.[ID],A.Title,A.BeginDate,A.EndDate,A.SignBeginDate,A.SingEndDate,
                            CASE WHEN (SELECT top(1) SignDate FROM office.SignDetail WHERE AgendaID=A.ID AND AttendeeID='{0}') IS NULL THEN 0 ELSE 1 END AS IsSigned
                            from [office].[Agenda] A 
                            where A.[ConferenceID]='{1}' AND A.NeedSign=1", AttendeeID, ConferenceID);
            List<AgendaList> result = LoadTColl(sql);

            return result;
        }
    }
}
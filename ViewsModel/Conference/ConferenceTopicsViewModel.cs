using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    public class ConferenceTopicsListViewModel
    {
        /// <summary>
        /// 议题ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 议题标题
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 会议议题列表
        /// </summary>
        public List<ConferenceTopicsList> conferenceTopicsList = new List<ConferenceTopicsList>();
    }

    public class ConferenceTopicsList
    {
        /// <summary>
        /// 议题ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 议题标题
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 会议主持人
        /// </summary>
        public string Presenter { get; set; }

        /// <summary>
        /// 会议引导人
        /// </summary>
        public string Guide { get; set; }

        /// <summary>
        /// 议题开始时间(默认是议程的开始时间)
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 议题结束时间(默认是议程的结束时间)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 议题地点
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 议题参与人数
        /// </summary>
        public int ConferenceTopicCount { get; set; }
        /// <summary>
        /// 热议编码
        /// </summary>
        public string TopicCode { get; set; }

        public bool TopicsExist { get; set; }
    }


    public class TopicsListViewModelAdapter : ViewBaseAdapter<ConferenceTopicsListViewModel, List<ConferenceTopicsListViewModel>>
    {
        private static string ConnectionString = "yuanxin";
        public static TopicsListViewModelAdapter Instance = new TopicsListViewModelAdapter();
        public TopicsListViewModelAdapter() : base(ConnectionString)
        {

        }
    }

    public class ConferenceTopicsListAdapter : ViewBaseAdapter<ConferenceTopicsList, List<ConferenceTopicsList>>
    {
        private static string ConnectionString = "yuanxin";
        public static ConferenceTopicsListAdapter Instance = new ConferenceTopicsListAdapter();
        public ConferenceTopicsListAdapter() : base(ConnectionString)
        {

        }

        public ViewPageBase<List<ConferenceTopicsList>> GetTopicsLists(string conferenceID,string agendaId, int pageIndex, DateTime searchTime, string userId, string name)
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            ViewPageBase<List<ConferenceTopicsList>> result = new ViewPageBase<List<ConferenceTopicsList>>();

            string selectSQL = " SELECT ID,TopicName,Presenter,Guide,Address,ConferenceTopicCount,BeginDate,EndDate,(CASE WHEN TopicsExist IS NULL then 0 ELSE  1 END) AS TopicsExist, CreateTime,TopicCode";

            string fromAndWhereSQL = string.Format(@"FROM (
SELECT C.ID,TopicName,TopicCode,Presenter,Guide,C.Address,COUNT(T.ID) AS ConferenceTopicCount,A.BeginDate,A.EndDate,F.ID AS TopicsExist,C.CreateTime
 FROM office.ConferenceTopic AS C
INNER JOIN office.Agenda AS A ON A.ID=C.AgendaID
INNER JOIN office.Conference AS Con ON A.ConferenceID=Con.ID
LEFT JOIN office.TopicPerson AS T ON T.ConferenceTopicID=C.ID
LEFT JOIN office.TopicPerson AS F ON  F.AttendeeID='{0}' AND F.ConferenceTopicID=C.ID
WHERE C.ValidStatus=1 AND C.CreateTime<='{1}'", userId, searchTime);
            //会议编码可为空
            if (conferenceID != "")
            {
                fromAndWhereSQL += string.Format(@" AND Con.ID='{0}'", conferenceID);
            }
            //议程编码可为空
            if (agendaId != "")
            {
                fromAndWhereSQL += string.Format(@" AND C.AgendaID='{0}'", agendaId);
            }
            //议题名称可为空
            if (name != "")
            {
                fromAndWhereSQL += string.Format(@" AND C.TopicName LIKE '%{0}%'", name);
            }
            fromAndWhereSQL += " GROUP BY T.ConferenceTopicID,C.ID,TopicName,TopicCode,Presenter,Guide,C.Address,A.BeginDate,A.EndDate,F.ID,C.CreateTime) AS Topic";
            string orderSQL = "ORDER BY ConferenceTopicCount DESC, CreateTime ASC";
            result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }


    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 会议基本信息数据适配器
    /// </summary>
    public class ConferenceAdapter : BaseAdapter<ConferenceModel, ConferenceModelCollection>
    {

        /// <summary>
        /// 实例
        /// </summary>
        public static ConferenceAdapter Instance = new ConferenceAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 构造
        /// </summary>
        public ConferenceAdapter()
        {
            BaseConnectionStr = ConnectionString;
        }

        /// <summary>
        /// 获取会议详情
        /// </summary>
        public ConferenceModel GetModel(string id)
        {
            return Load(w => w.AppendItem("ID", id)).SingleOrDefault();
        }

        /// <summary>
        /// 获取会议列表 - 所有已发布
        /// </summary>
        public ConferenceModelCollection GetList(int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"
                WITH [Temp] AS
                (
	                SELECT ROW_NUMBER() OVER(ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[Conference] WHERE [IsPublic] = 1
                )
                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);
            return QueryData(sql);
        }

        /// <summary>
        /// 获取会议列表 - 我的会议
        /// </summary>
        public ConferenceModelCollection GetList(int pageSize, int pageIndex, string userCode)
        {
            pageIndex--;
            var sql = @"
                WITH [Temp] AS
                (
	                SELECT ROW_NUMBER() OVER(ORDER BY [C].[CreateTime] DESC) AS [Row], [C].* FROM [office].[Attendee] A
		                LEFT JOIN [office].[Conference] C ON [A].[ConferenceID] = [C].[ID]
	                WHERE
		                [A].[AttendeeID] = '{0}' AND
		                [C].[IsPublic] = 1
                )
                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {1} AND {2};";
            sql = string.Format(sql, userCode, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);
            return QueryData(sql);
        }

        /// <summary>
        /// 根据会议名称查询会议
        /// </summary>
        public ConferenceModelCollection GetByConferenceName(string conferenceName)
        {
            return Load(m => m.AppendItem("Name", conferenceName));
        }

        /// <summary>
        /// 分页查询会议列表
        /// </summary>
        public ViewPageBase<ConferenceModelCollection> GetConferenceModelListByPage(int pageIndex, DateTime searchTime)
        {
            if (searchTime == DateTime.MinValue || searchTime == null)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "SELECT *";
            string fromAndWhereSQL = "FROM office.Conference WHERE CreateTime<'" + searchTime.ToString() + "'";
            string orderSQL = "order by CreateTime DESC";
            ViewPageBase<ConferenceModelCollection> pageData = GetTListByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            pageData.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return pageData;
        }

        /// <summary>
        /// 获取会议地点
        /// </summary>
        public ConferencePlaceView GetConferenceModelListByPage(string conferenceId)
        {
            ConferencePlaceView result = new ConferencePlaceView();

            var sql = @"SELECT Address,Longitude,Latitude FROM office.Conference WHERE ID='{0}';
                        SELECT DepartDate,BeginPlace,Title FROM office.BusRoute WHERE ConferenceID='{1}' ORDER BY DepartDate;";
            sql = string.Format(sql, conferenceId, conferenceId);

            SqlDataReader reader;
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionNameDefine.YuanXinForDBHelp].ToString()))
            {
                sqlConn.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);
                sqlCommand.CommandType = CommandType.Text;
                reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    result.Address = reader["Address"].ToString();
                    result.Longitude = reader["Longitude"].ToString();
                    result.Latitude = reader["Latitude"].ToString();
                }
                else
                {
                    result.Address = "";
                    result.Longitude = "";
                    result.Latitude = "";
                }
                if (reader.NextResult())
                {
                    result.BusRouteList = new List<BusRouteView>();
                    while (reader.Read())
                    {
                        BusRouteView bus = new BusRouteView();
                        bus.BeginPlace = reader["BeginPlace"].ToString();
                        bus.DepartDate = Convert.ToDateTime(reader["DepartDate"]);
                        bus.Title = reader["Title"].ToString();
                        result.BusRouteList.Add(bus);
                    }
                };
            }

            return result;
        }

        /// <summary>
        /// 根据会议Id获取会议信息
        /// </summary>
        public ConferenceModel GetConferenceByConferenceId(string conferenceId)
        {
            return Load(m => m.AppendItem("ID", conferenceId)).FirstOrDefault();
        }


        public ConferenceModelCollection GetMyConference(string userCode,DateTime s,DateTime e)
        {
            string sql = " select c.id,c.name,c.beginDate,c.endDate,c.city　from  office.Conference c left join office.Attendee a on c.id = a.conferenceid where c.IsPublic = 1 and a.AttendeeType = 1 and a.attendeeid = '"+ userCode + "' and c.beginDate >= '"+s+"' and c.endDate <= '"+e+"'";
            return this.QueryData(sql);
        }
    }
}
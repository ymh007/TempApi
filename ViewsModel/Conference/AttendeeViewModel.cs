using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using MCS.Library.SOA.DataObjects;
using System.Data;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Conference
{
	public class AttendeeList
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// 参会人的Code
		/// </summary>
		public string AttendeeID { get; set; }

		/// <summary>
		/// 会议的ID
		/// </summary>
		public string ConferenceID { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 电话
		/// </summary>
		public string MobilePhone { get; set; }

		/// <summary>
		/// 参会人头像
		/// </summary>
		public string PhotoAddress { get; set; }

		/// <summary>
		/// 组织机构的ID
		/// </summary>
		public string OrganizationID { get; set; }

		/// <summary>
		/// 组织结构
		/// </summary>
		public string OrganizationStructure { get; set; }
		/// <summary>
		/// 座位号
		/// </summary>
		public string SeatAddress { get; set; }
		/// <summary>
		/// 城市名
		/// </summary>
		public string CityName { get; set; }
		/// <summary>
		/// 是否参与抽奖
		/// </summary>
		public bool IsJoinPrize { get; set; }

		/// <summary>
		/// 有效性
		/// </summary>
		public bool ValidStatus { get; set; }

		/// <summary>
		/// 座位对应的座位分布图位置坐标
		/// </summary>
		public string SeatCoordinate { get; set; }
	}

	public class AttendeeSignList
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// 参会人海鸥二编码
		/// </summary>
		public string AttendeeID { get; set; }

		/// <summary>
		/// 议程编码
		/// </summary>
		public string AgendaID { get; set; }

		/// <summary>
		/// 会议的ID
		/// </summary>
		public string ConferenceID { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 邮箱
		/// </summary>
		public string Email { get; set; }

		/// <summary>
		/// 电话
		/// </summary>
		public string MobilePhone { get; set; }

		/// <summary>
		/// 参会人头像
		/// </summary>
		public string PhotoAddress { get; set; }

		/// <summary>
		/// 组织机构的ID
		/// </summary>
		public string OrganizationID { get; set; }

		/// <summary>
		/// 组织结构
		/// </summary>
		public string OrganizationStructure { get; set; }

		/// <summary>
		/// 签到时间
		/// </summary>
		public DateTime SignDate
		{
			get
			{
				try
				{
					SignInModel signIn = SignInAdapter.Instance.LoadByCode(this.AttendeeID, this.AgendaID);
					if (signIn != null)
					{
						this.IsSignDate = true;
						return signIn.SignDate;
					}
					else
					{
						this.IsSignDate = false;
						return DateTime.MinValue;
					}
				}
				catch (Exception e)
				{
					this.IsSignDate = false;
					return DateTime.MinValue;
				}
			}
		}

		/// <summary>
		/// 是否签到
		/// </summary>
		public bool IsSignDate { get; set; }
	}

	public class AttendeeListAdapter : ViewBaseAdapter<AttendeeList, List<AttendeeList>>
	{
		private static string ConnectionString = "yuanxin";
		public static AttendeeListAdapter Instance = new AttendeeListAdapter();
		public AttendeeListAdapter() : base(ConnectionString)
		{

		}
		/// <summary>
		/// 数据库连接
		/// </summary>
		/// <returns></returns>
		protected string GetConnectionName()
		{
			return ConnectionNameDefine.YuanXinBusiness;
		}

		/// <summary>
		/// 查询某个会议参会人列表--分页
		/// </summary>
		/// <param name="conferenceID"></param>
		/// <param name="pageIndex"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public ViewPageBase<List<AttendeeList>> GetAttendeeListView(string conferenceID, int attendeeType, int pageIndex, DateTime searchTime, string attendeeName, bool isJoinPrize = false)
		{
			string attendeeNameWhereSQL = "";
            if (!attendeeName.IsEmptyOrNull() && attendeeName != "null")
			{
				attendeeNameWhereSQL = " AND Name like '%" + attendeeName + "%'";
			}
			string isJoinPrizeWhereSQL = "";
			if (isJoinPrize)
			{
				isJoinPrizeWhereSQL = isJoinPrize ? " AND IsJoinPrize=1 " : "";
			}
			ViewPageBase<List<AttendeeList>> result = new ViewPageBase<List<AttendeeList>>();

			string selectSQL = string.Format(@"SELECT ValidStatus,ID,AttendeeID,ConferenceID,Name,Email,MobilePhone,PhotoAddress,OrganizationID,OrganizationStructure,CityName,CreateTime,IsJoinPrize,(SELECT top(1) SeatAddress FROM office.Seats WHERE ConferenceID='{0}' AND AttendeeID=office.Attendee.ID) AS SeatAddress", conferenceID);
			string fromAndWhereSQL = string.Format(@"FROM office.Attendee WHERE ConferenceID='{0}' AND AttendeeType={1} AND CreateTime<='{2}' {3} {4}", conferenceID, attendeeType, searchTime, attendeeNameWhereSQL, isJoinPrizeWhereSQL);
			string orderSQL = "ORDER BY Name";
			result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
			result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
			return result;
		}
		/// <summary>
		/// 查询某个会议所有参会人--全部
		/// </summary>
		/// <param name="conferenceID">会议编码</param>
		/// <param name="isJoinPrize">是否加入抽奖</param>
		/// <param name="isForAppWeb">是否为抽奖页面使用，后台默认false</param>
		/// <returns></returns>
		public List<AttendeeList> GetAllAttendeeList(string conferenceID, bool isJoinPrize = false, bool isForAppWeb = false)
		{
			string sql = string.Format(@"SELECT ID,AttendeeID,ConferenceID,Name,Email,MobilePhone,PhotoAddress,OrganizationID,OrganizationStructure,CreateTime,CityName,IsJoinPrize,(SELECT TOP(1) SeatAddress FROM office.Seats WHERE ConferenceID='{0}' AND AttendeeID=office.Attendee.ID) AS SeatAddress
                                        FROM office.Attendee WHERE ConferenceID='{1}' {2} {3} ORDER BY Name", conferenceID, conferenceID, isJoinPrize ? " AND IsJoinPrize=1 " : "", isForAppWeb ? " AND ValidStatus=1" : "");

			return LoadTColl(sql);
		}


		/// <summary>
		/// 查询某个会议所有已选座的参会人
		/// </summary>
		/// <param name="conferenceID">会议编码</param>
		/// <returns></returns>
		public DataTable GetAllHasSeatAttendeeList(string conferenceID, int AttendeeType,string userName="")
		{
            string sql = string.Empty;
            if (AttendeeType == 1)
            {
                sql = string.Format(@"SELECT o.ID,o.OrganizationStructure, o.Name,o.Email,s.SeatAddress,s.SeatCoordinate ,s.AttendeeID, o.ConferenceID  FROM office.Attendee o INNER JOIN office.Seats s ON o.ID=s.AttendeeID WHERE o.ConferenceID='{0}' AND s.SeatType={1} AND o.AttendeeType={2} AND s.SeatAddress!='' AND s.SeatCoordinate!='' AND o.ValidStatus=1 AND s.ValidStatus=1", conferenceID, AttendeeType, AttendeeType);
            }
            else {
                sql = string.Format(@"SELECT o.ID,o.OrganizationStructure, o.Name,o.Email,s.SeatAddress,s.SeatCoordinate,s.AttendeeID, o.ConferenceID FROM office.Attendee o INNER JOIN office.Seats s ON o.ID=s.AttendeeID WHERE o.ConferenceID='{0}' AND s.SeatType={1} AND o.AttendeeType={2} AND s.SeatAddress!='' AND o.ValidStatus=1 AND s.ValidStatus=1", conferenceID, AttendeeType, AttendeeType);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                sql = string.Format(sql + " AND o.Name like '%{0}%' ", userName);
            }
            return DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
		}

		/// <summary>
		/// 查询某个会议所有未选座的参会人
		/// </summary>
		/// <param name="conferenceID">会议编码</param>
		/// <param name="userName">人员姓名</param>
		/// <returns></returns>
		public List<AttendeeList> GetAllNoChooseSeatAttendeeList(string conferenceID, string userName, int AttendeeType)
		{
			string sql = "";
			if (userName != "")
			{
				sql = string.Format(@"SELECT o.ID,o.OrganizationStructure,o.Name,o.Email,s.SeatAddress,s.AttendeeID,o.ConferenceID FROM office.Attendee o INNER JOIN office.Seats s ON o.ID=s.AttendeeID WHERE o.ConferenceID='{0}' AND o.Name like '%{1}%' AND s.SeatAddress='' AND o.ValidStatus=1", conferenceID, userName);
			}
			else
			{
				sql = string.Format(@"SELECT o.ID,o.OrganizationStructure,o.Name,o.Email,s.SeatAddress,s.AttendeeID,o.ConferenceID FROM office.Attendee o INNER JOIN office.Seats s ON o.ID=s.AttendeeID WHERE o.ConferenceID='{0}' AND s.SeatAddress='' AND o.ValidStatus=1", conferenceID);
			}
            sql = string.Format(sql+ " AND s.SeatType={0} AND o.AttendeeType={1} ", AttendeeType, AttendeeType);

            return LoadTColl(sql);
		}
         
    }


	public class AttendeeListSigAdapter : ViewBaseAdapter<AttendeeSignList, List<AttendeeSignList>>
	{
		private static string ConnectionString = "yuanxin";
		public static AttendeeListSigAdapter Instance = new AttendeeListSigAdapter();
		public AttendeeListSigAdapter() : base(ConnectionString)
		{

		}
		/// <summary>
		/// 查询议题讨论人的列表（从参会人取）
		/// </summary>
		/// <param name="ConferenceTopicID"></param>
		/// <param name="pageIndex"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public ViewPageBase<List<AttendeeSignList>> GetTopicPersonView(string ConferenceTopicID, int pageIndex, DateTime searchTime)
		{
			if (pageIndex == 1)
			{
				searchTime = DateTime.Now;
			}
			ViewPageBase<List<AttendeeSignList>> result = new ViewPageBase<List<AttendeeSignList>>();

			string selectSQL = "SELECT A.ID,A.AttendeeID,A.ConferenceID,A.Name,A.Email,A.MobilePhone,A.PhotoAddress,A.OrganizationID,A.OrganizationStructure,A.CreateTime,age.ID AS AgendaID";
			string fromAndWhereSQL = string.Format(@"FROM office.TopicPerson T
                                                    INNER JOIN office.Attendee AS A ON A.ID=T.AttendeeID
                                                    INNER JOIN office.ConferenceTopic CT ON CT.ID=T.ConferenceTopicID
                                                    INNER JOIN office.Agenda age ON age.ID=CT.AgendaID
                                                    WHERE T.ConferenceTopicID='{0}' AND T.CreateTime<='{1}'", ConferenceTopicID, searchTime);
			string orderSQL = "ORDER BY T.CreateTime DESC";
			result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
			result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
			return result;
		}
	}


	/// <summary>
	/// 签到参会人员
	/// </summary>
	public class SignAttendee
	{
		/// <summary>
		/// 人员Code
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 组织机构
		/// </summary>
		public string Department { get; set; }

		/// <summary>
		/// 头像
		/// </summary>
		public string Photo { get; set; }
	}

     


}
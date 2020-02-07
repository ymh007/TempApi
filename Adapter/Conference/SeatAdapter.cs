using MCS.Library.SOA.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Models;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using System.Data;
using NPOI.SS.Formula.Functions;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{

    /// <summary>
    /// 座位
    /// </summary>
    public class SeatsAdapter : UpdatableAndLoadableAdapterBase<SeatsModel, SeatsCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly SeatsAdapter Instance = new SeatsAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 更新座位集合
        /// </summary>
        public void UpdateSeatColl(SeatsCollection seatColl)
        {
            seatColl.ForEach(seat =>
            {
                Update(seat);
            });
        }

        /// <summary>
        /// 获取用户在某个会议的座位号
        /// </summary>
        public SeatsModel GetUserSeatModel(string attendeeID, string conferenceID, int seatType = 1)
        {
            SeatsCollection smColl = Load(p =>
            {
                p.AppendItem("AttendeeID", attendeeID);
                p.AppendItem("ConferenceID", conferenceID);
                p.AppendItem("ValidStatus", true);
                p.AppendItem("SeatType", seatType);
            });
            return smColl.Count > 0 ? smColl[0] : null;
        }

        /// <summary>
        /// 更新座位集合
        /// </summary>
        public bool Update(SeatsCollection data, string conferenceID)
        {
            var mapping = ORMapping.GetMappingInfo<SeatsModel>();
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            builder.AppendItem("ConferenceID", conferenceID);
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE {1}", mapping.TableName, builder.ToSqlString(TSqlBuilder.Instance)));
            foreach (var item in data)
            {
                strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
                strB.Append(ORMapping.GetInsertSql(item, TSqlBuilder.Instance, ""));
            }
            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) >= 1 ? true : false;
        }

        /// <summary>
        /// 根据ID删除一个座位
        /// </summary>
        public bool DelUserSeat(string ID)
        {
            var mapping = ORMapping.GetMappingInfo<SeatsModel>();
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE ID={1}", mapping.TableName, ID));

            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) > 0 ? true : false;
        }



        /// <summary>
        /// 根据参会人编码和会议编码删除座位信息 新加删除条件 座位类型
        /// </summary>
        public void DelUserSeat(string conferenceID, string attendeeID, int SeatType)
        {
            Delete(m => m.AppendItem("ConferenceID", conferenceID).
            AppendItem("AttendeeID", attendeeID).AppendItem("SeatType", SeatType));
        }

        /// <summary>
        /// 根据会议Id获取群组Id
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <returns></returns>
        public List<int> GetGroupIdByConferenceId(string conferenceId)
        {
            List<int> list = new List<int>();
            DataSet dt = new DataSet();
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" Select DialogCode ");
            strSql.Append(" From [office].[ConferenceCompanyDialog] ");
            strSql.AppendFormat(" where ConferenceID = '{0}'", conferenceId);
            dt = DbHelper.RunSqlReturnDS(strSql.ToString(), GetConnectionName());
            foreach (DataRow item in dt.Tables[0].Rows)
            {
                int outResult;
                if (Int32.TryParse(item["DialogCode"].ToString(), out outResult))
                {
                    list.Add(outResult);
                }
            }
            return list;
        }

        /// <summary>
        /// 根据会议编码和座位坐标编号查询该座位是否被预定
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="seatNo">座位实际对应的坐标编号</param>
        /// <returns></returns>
        public SeatsModel GetSeatModelByID(string conferenceID, string seatNo)
        {
            SeatsCollection smColl = this.Load(p =>
            {
                p.AppendItem("SeatCoordinate", seatNo);
                p.AppendItem("ConferenceID", conferenceID);
            });
            return smColl.Count > 0 ? smColl[0] : null;
        }

        /// <summary>
        /// 根据会议编码和禁用的行或列编码查询该行或列是否已有被预订的座位
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="seatRow">编号</param>
        /// <param name="flag">标识当前编号是行还是列（1表示行2表示列）</param>
        /// <returns></returns>
        public bool GetDisSeatModelByID(string conferenceID, string seatRow, string flag)
        {
            DataTable dt = new DataTable();
            if (flag == "1")
            {
                string sql = string.Format("SELECT * FROM office.Seats WHERE SeatCoordinate LIKE '{0}-%' AND ConferenceID='{1}' AND ValidStatus=1", seatRow, conferenceID);
                dt = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, null);
            }
            else
            {
                string sql = string.Format("SELECT * FROM office.Seats WHERE SeatCoordinate LIKE '%-{0}' AND ConferenceID='{1}' AND ValidStatus=1", seatRow, conferenceID);
                dt = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, null);
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 根据会议编码判断当前会议是否有已被预订的座位
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        public bool GetHasSeatModelByID(string conferenceID)
        {
            string sql = string.Format("SELECT SeatAddress,SeatCoordinate  FROM office.Seats WHERE ConferenceID='{0}' and SeatAddress<> '' AND ValidStatus=1", conferenceID);
            DataTable dt = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 根据 会议编码  类型 获取座位集合
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="seatAddress">卓号</param>
        /// <param name="setaType"></param>
        /// <returns></returns>
        public SeatsCollection GetEntertainHallSeatByCID(string conferenceID, int setaType = 2)
        {
            return this.Load(p =>
            {
                p.AppendItem("ConferenceID", conferenceID);
                p.AppendItem("SeatType", setaType);
            });
        }

        /// <summary>
        /// 根据会议编码移除所有已被预订的座位信息且删除已被禁用的座位信息
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        public bool RemoveHasSeatList(string conferenceID, int seatType)
        {
            Boolean result;
            SqlDbHelper dbh = new SqlDbHelper();

            List<String> sqlArray = new List<String>();
            //移除已被预订的座位
            var mapping = ORMapping.GetMappingInfo<SeatsModel>();
            StringBuilder strA = new StringBuilder(200);
            strA.Append(string.Format("UPDATE {0} SET SeatAddress='',SeatCoordinate='' WHERE ConferenceID='{1}' and SeatType={2}", mapping.TableName, conferenceID, seatType));

            //删除已被禁用的座位
            var DisMapping = ORMapping.GetMappingInfo<DisableSeatsModel>();
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE ConferenceID='{1}'", DisMapping.TableName, conferenceID));

            sqlArray.Add(strA.ToString());
            sqlArray.Add(strB.ToString());
            result = dbh.ExecuteNonQueryTransation(sqlArray, CommandType.Text, null);
            return result;


            //return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) > 0 ? true : false;
        }

        /// <summary>
        /// 根据会议编码  座位类型 原来座位地址  修改某一卓座位号码
        /// </summary>
        /// <param name="conferenceID">会议id</param>
        /// <param name="seatAddress">原来座位地址</param>
        /// <param name="seatType">座位类型</param>
        /// <param name="newSeatAddress">新座位地址</param>
        /// <returns></returns>
        public bool UpdateDeskNum(string conferenceID, string seatAddress, string seatType, string newSeatAddress)
        {
            Boolean result;
            SqlDbHelper dbh = new SqlDbHelper();
            List<String> sqlArray = new List<String>();
            //移除已被预订的座位
            var mapping = ORMapping.GetMappingInfo<SeatsModel>();
            StringBuilder strA = new StringBuilder(200);
            strA.Append(string.Format("UPDATE {0} SET SeatAddress='{1}' WHERE ConferenceID='{2}' and seatAddress='{3}' and SeatType={4}", mapping.TableName, newSeatAddress, conferenceID, seatAddress, seatType));
            sqlArray.Add(strA.ToString());
            result = dbh.ExecuteNonQueryTransation(sqlArray, CommandType.Text, null);
            return result;
        }
        /// <summary>
        /// 清空当前行的座位
        /// </summary>
        public void ClearLikeSeat(string ConferenceID, string Row)
        {
            var sql = @"UPDATE [office].[Seats] SET [SeatAddress]='',[SeatCoordinate]=''  WHERE [ConferenceID]=@ConferenceID
                    AND [SeatType]=1 AND [SeatAddress] LIKE  +@Row+'-%'";

            SqlParameter[] parameters = { new SqlParameter("@ConferenceID", SqlDbType.NVarChar, 36),
                new SqlParameter("@Row", SqlDbType.NVarChar,36),
           };

            parameters[0].Value = ConferenceID;
            parameters[1].Value = Row;

            var helper = new SqlDbHelper();
            var result = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 获取当前排，以及未排人员
        /// </summary>
        public DataTable GetHasRowAndNoSeat(string ConferenceID, int Row, string where)
        {
           
            var sql = @" SELECT * FROM [office].[Attendee]  o INNER JOIN  office.Seats s  ON 
                     [o].[ID]=[s].[AttendeeID]  AND [o].[ConferenceID]=@ConferenceID  
                      and ( [s].[SeatAddress]='' or [s].[SeatAddress] like @Row ) AND [o].[AttendeeType]=1 AND [s].[SeatType]=1 
                      AND [o].[ValidStatus]=1 ";
            if (!string.IsNullOrEmpty(where))
            {
                sql = @" SELECT * FROM [office].[Attendee]  o INNER JOIN  office.Seats s  ON 
                     [o].[ID]=[s].[AttendeeID]  AND [o].[ConferenceID]=@ConferenceID 
                      and ([s].[SeatAddress]='' or [s].[SeatAddress] like  @Row ) AND [o].[AttendeeType]=1 AND [s].[SeatType]=1 
                      AND [o].[ValidStatus]=1  and (o.name like  @where or o.email like  @where )";
              
            }
            SqlParameter[] parameters = { new SqlParameter("@ConferenceID", SqlDbType.NVarChar, 36),
                new SqlParameter("@Row", SqlDbType.NVarChar,36),new SqlParameter("@where", SqlDbType.NVarChar, 100)
           };
            parameters[0].Value = ConferenceID;
            parameters[1].Value = Row+ "-%";
            parameters[2].Value = "%"+ where+"%";
            var helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        ///批量操作座位的方法
        /// </summary>
        /// <param name="coll"></param>
        public void SeatPersonInsert(SeatsCollection coll)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("ID", Type.GetType("System.String"));
            dt.Columns.Add("ConferenceID", Type.GetType("System.String"));
            dt.Columns.Add("AttendeeID", Type.GetType("System.String"));
            dt.Columns.Add("SeatAddress", Type.GetType("System.String"));
            dt.Columns.Add("SeatCoordinate", Type.GetType("System.String"));
            dt.Columns.Add("Creator", Type.GetType("System.DateTime"));
            dt.Columns.Add("CreateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", Type.GetType("System.Boolean"));
            dt.Columns.Add("SeatType", Type.GetType("System.Int"));

            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = m.ID;
                dr["ConferenceID"] = m.ConferenceID;
                dr["AttendeeID"] = m.AttendeeID;
                dr["SeatAddress"] = m.SeatAddress;
                dr["SeatCoordinate"] = m.SeatCoordinate;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dr["SeatType"] = m.SeatType;

                dt.Rows.Add(dr);
            });

            SqlDbHelper.BulkInsertData(dt, "[office].[Seats]", GetConnectionName());
        }
    }
}
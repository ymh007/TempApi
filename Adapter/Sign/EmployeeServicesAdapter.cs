using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Sign
{
    /// <summary>
    /// 打卡记录 Adapter
    /// </summary>
    public class EmployeeServicesAdapter : UpdatableAndLoadableAdapterBase<EmployeeServicesModel, EmployeeServicesCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly EmployeeServicesAdapter Instance = new EmployeeServicesAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.EmployeeAttendance);

        /// <summary>
        /// 打卡统计
        /// </summary>
        public DataTable GetPunchStatistical(string type, string code, string dates)
        {
            SqlParameter[] paramerers = {
                new SqlParameter("@type", SqlDbType.VarChar, 20),
                new SqlParameter("@code", SqlDbType.NVarChar,36),
                new SqlParameter("@dates", SqlDbType.VarChar,8000) };
            paramerers[0].Value = type;
            paramerers[1].Value = code;
            paramerers[2].Value = dates;

            var helper = new SqlDbHelper(ConnectionString);
            var ds = helper.ExecuteDataSet("[dbo].[ProcPunchStatistical]", CommandType.StoredProcedure, paramerers);
            var dt1 = ds.Tables[0];
            var dt2 = ds.Tables[1];

            dt1.PrimaryKey = new DataColumn[] { dt1.Columns["Name"] };
            dt2.PrimaryKey = new DataColumn[] { dt2.Columns["Name"] };

            dt1.Merge(dt2);

            return dt1;
        }

        /// <summary>
        /// 打卡记录
        /// </summary>
        public DataTable GetPunchRecord(string type, string code, DateTime start, DateTime end)
        {
            DataTable all = null;
            string users = "";
            switch (type)
            {
                case "ManagementCode":
                    {
                        users = @" SELECT DISTINCT [UserCode] FROM [dbo].[PunchPersonnel] WHERE [PunchCode] = @Code";
                    }
                    break;
                case "OrganizationCode":
                    {
                        users = @"SELECT DISTINCT [ObjectID] FROM [dbo].[Contacts] "
                              + "WHERE [FullPath] LIKE(SELECT[FullPath] FROM[dbo].[Contacts] WHERE"
                              + "[ObjectID] = @Code) + '%'  AND  SchemaType='Users'";
                    }
                    break;
                case "UserCode":
                    {
                        all = GetListByDetail(code, start, end);
                        return all;
                    }
            }
            var helper = new SqlDbHelper(ConnectionString);
            SqlParameter[] paramerers = { new SqlParameter("@Code", SqlDbType.NVarChar, 36) };
            paramerers[0].Value = code;
            var dtUsers = helper.ExecuteDataTable(users, CommandType.Text, paramerers);
            if (dtUsers.Rows.Count > 0)
            {
                all = GetListByDetail(dtUsers.Rows[0][0].ToString(), start, end).Clone();
            }
            for (int i = 0; i < dtUsers.Rows.Count; i++)
            {
                all.Merge(GetListByDetail(dtUsers.Rows[i][0].ToString(), start, end));
            }
            return all;

        }

        /// <summary>
        /// 获取按组织统计的数据
        /// </summary>
        public DataTable GetListByOrganization(string organizationCodes, DateTime date1, DateTime date2)
        {
            SqlParameter[] paramerers = {
                new SqlParameter("@organizationCodes", SqlDbType.NVarChar, 3000),
                new SqlParameter("@date1", SqlDbType.Date),
                new SqlParameter("@date2", SqlDbType.Date) };
            paramerers[0].Value = organizationCodes;
            paramerers[1].Value = date1;
            paramerers[2].Value = date2;

            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataTable("[dbo].[ProcByOrganization]", CommandType.StoredProcedure, paramerers);
        }

        /// <summary>
        /// 获取按人员统计的数据
        /// </summary>
        public DataTable GetListByUser(string organizationCode, DateTime date1, DateTime date2)
        {
            SqlParameter[] paramerers = {
                new SqlParameter("@organizationCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@date1", SqlDbType.Date),
                new SqlParameter("@date2", SqlDbType.Date) };
            paramerers[0].Value = organizationCode;
            paramerers[1].Value = date1;
            paramerers[2].Value = date2;

            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataTable("[dbo].[ProcByUser]", CommandType.StoredProcedure, paramerers);
        }

        /// <summary>
        /// 获取人员考勤明细
        /// </summary>
        public DataTable GetListByDetail(string userCode, DateTime date1, DateTime date2)
        {
            SqlParameter[] paramerers = {
                new SqlParameter("@userCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@date1", SqlDbType.Date),
                new SqlParameter("@date2", SqlDbType.Date) };
            paramerers[0].Value = userCode;
            paramerers[1].Value = date1;
            paramerers[2].Value = date2;

            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataTable("[dbo].[ProcByDetail]", CommandType.StoredProcedure, paramerers);
        }

        /// <summary>
        /// 查询当月考勤信息
        /// </summary>
        /// <returns>数据表</returns>
        public DataTable GetPunchException(int worDays, string endDate, string startDate)
        {
            worDays = worDays * 2; //一天打卡两次
            string sql = $" WITH [Temp] AS ( " +
                 $" SELECT  Creator FROM(SELECT Creator, COUNT(Creator)AS PunchCount FROM" +
                 $" [MobileBusiness].dbo.EmployeeServices WHERE " +
                 $" (PunchDate  between '{startDate}' and  '{endDate}')  GROUP BY Creator) AS temp " +
                 $" WHERE temp.PunchCount < {worDays}" +
                 $" UNION ALL" +
                 $" SELECT  Creator FROM[MobileBusiness].dbo.EmployeeServices WHERE IsUnusual = 1 and" +
                 $" UnusualDesc = '' and (PunchDate  between '{startDate}' and  '{endDate}') GROUP BY Creator" +
                 $" ) SELECT Creator FROM[TEMP] GROUP BY Creator ";
            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataTable(sql);
        }

        /// <summary>
        /// 删除原来的请休假信息防止因为请休假信息更改导致的说明不一致
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DeleteLeaveInfo(string userCode, DateTime start, DateTime end)
        {
            var sql = $" DELETE  [dbo].[EmployeeServices] WHERE  [Creator] = '{userCode}' and DescStatus=2  and  PunchDate BETWEEN '{start.ToString("yyyy-MM-dd")}' and '{end.ToString("yyyy-MM-dd")}' ";
            var helper = new SqlDbHelper(ConnectionString);
            helper.ExecuteDataTable(sql, CommandType.Text);
        }

        /// <summary>
        /// 查询当月考勤信息
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="month">月份 2018-06-06</param>
        /// <returns>数据表</returns>
        public DataTable GetResultByMonth(string userCode, DateTime month)
        {
            var sql = @"
                SELECT [A].[Code], [A].[PunchDate], [A].[PunchType], [A].[MapUrl], [A].[IsLate], [A].[IsEarly], [A].[IsUnusual], [A].[UnusualType], [A].[UnusualDesc], [A].[CreateTime], [A].[IsValid], [B].[Address],      [A].DescStatus,[A].ModifyTime   FROM
                (
	                SELECT ROW_NUMBER() OVER(PARTITION BY [PunchDate] ORDER BY [CreateTime] ASC) AS [Row], * FROM [dbo].[EmployeeServices]
	                WHERE
		                DATEDIFF(MONTH, [PunchDate], @Month) = 0 AND
                        [Creator] = @UserCode AND
                        [PunchType] = 0
	                UNION ALL
                    SELECT ROW_NUMBER() OVER(PARTITION BY [PunchDate] ORDER BY [CreateTime] DESC) AS [Row], * FROM [dbo].[EmployeeServices]
	                WHERE
		                DATEDIFF(MONTH, [PunchDate], @Month) = 0 AND
                        [Creator] = @UserCode AND
                        [PunchType] = 1
                ) [A]
                LEFT JOIN [dbo].[StandardPunch] [B] ON [A].[StandardPunchCode] = [B].[Code]
                WHERE
	                [A].[Row] = 1
                ORDER BY [A].[PunchDate] ASC, [A].[PunchType] ASC;";

            SqlParameter[] paramerers = {
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@Month", SqlDbType.Date) };
            paramerers[0].Value = userCode;
            paramerers[1].Value = month;
            lock (sql)
            {
                var helper = new SqlDbHelper(ConnectionString);
                return helper.ExecuteDataTable(sql, CommandType.Text, paramerers);
            }
        }

        /// <summary>
        /// 获取用户某天签到或签退记录
        /// </summary>
        public EmployeeServicesCollection GetByPunchDateType(string userCode, string punchDate, int punchType)
        {
            return Load(w =>
            {
                w.AppendItem("Creator", userCode);
                w.AppendItem("PunchDate", punchDate);
                w.AppendItem("PunchType", punchType);
            });
        }

        /// <summary>
        /// 获取用户某天签到或签退记录
        /// </summary>
        public EmployeeServicesModel GetByCode(string code, string userCode, string punchDate, int punchType)
        {
            return Load(w =>
            {
                w.AppendItem("Code", code);
                w.AppendItem("Creator", userCode);
                w.AppendItem("PunchDate", punchDate);
                w.AppendItem("PunchType", punchType);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 判断用户当天是否已经签到
        /// </summary>
        public bool IsPunchToday(string userCode)
        {
            try
            {
                var list = Load(w =>
                {
                    w.AppendItem("Creator", userCode);
                    w.AppendItem("PunchDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    w.AppendItem("PunchType", 0);
                });
                if (list.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }









        public EmployeeServicesModel LoadEmployess(string userCode, string startTime, string endTime, int index)
        {
            return Load(p =>
            {
                p.AppendItem("Creator", userCode);
                p.AppendItem("CreateTime", startTime, ">=");
                p.AppendItem("CreateTime", endTime, "<=");
            }).ElementAtOrDefault(index);
        }

        public EmployeeServicesCollection LoadEmployTodayAll(string userCode, string startTime, string endTime)
        {
            return Load(p =>
            {
                p.AppendItem("Creator", userCode);
                p.AppendItem("CreateTime", startTime, ">=");
                p.AppendItem("CreateTime", endTime, "<=");
            });
        }
    }
}
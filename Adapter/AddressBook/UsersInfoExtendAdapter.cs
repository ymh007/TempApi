using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using log4net;
using MCS.Library.Data.Builder;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System.Configuration;
using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.ViewsModel.AddressBook;

namespace Seagull2.YuanXin.AppApi.Adapter.AddressBook
{
    public class UsersInfoExtendAdapter : UpdatableAndLoadableAdapterBase<UsersInfoExtend, UsersInfoExtendCollection>
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly UsersInfoExtendAdapter Instance = new UsersInfoExtendAdapter();

        public static readonly string connecstring = Instance.GetSqlConnectionName("PERMISSIONS_CENTER");

        protected override string GetConnectionName()
        {
            return "PERMISSIONS_CENTER";
        }

        /// <summary>
        /// 根据名字查对象，支持模糊查询
        /// </summary>
        /// <param name="searchContent"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UsersInfoExtend> GetUsersInfoExtendCollectionByName(string searchContent, int pageIndex, int pageSize)
        {
            pageIndex--;
            List<UsersInfoExtend> result = new List<UsersInfoExtend>();

            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY DisplayName DESC) AS [Row], ID,DisplayName,MP as MOBILE ,Mail as InternetEmail FROM sc.SchemaUserSnapshot 
	                        WHERE 
		                       VersionEndTime='9999-09-09 00:00:00.000' AND (CONTAINS(SearchContent,@searchContent) OR MP = @searchContent) AND AccountDisabled=0
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row]  BETWEEN @pageStart AND @pageEnd;";
            SqlParameter[] parameters = {
                new SqlParameter("@searchContent", SqlDbType.NVarChar, 36),
                new SqlParameter("@pageStart", SqlDbType.Int,32),
                new SqlParameter("@pageEnd", SqlDbType.Int,32)
            };
            parameters[0].Value = TSqlBuilder.Instance.CheckQuotationMark(searchContent, false);
            parameters[1].Value = pageSize * pageIndex + 1;
            parameters[2].Value = pageSize * pageIndex + pageSize;

            SqlDbHelper helper = new SqlDbHelper(connecstring);
            var table = helper.ExecuteDataTable(sql, CommandType.Text, parameters);

            List<UsersInfoExtend> queryResult = DataConvertHelper<UsersInfoExtend>.ConvertToList(table);
            var rootPath = OguMechanismFactory.GetMechanism().GetRoot().FullPath;
            foreach (var item in queryResult)
            {
                OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, item.ID);
                if (users.Any())
                {
                    IUser user = users.FirstOrDefault();
                    var arr = user.FullPath.Replace(rootPath, "").Trim('\\').Split('\\').ToList<string>();
                    arr.RemoveAt(arr.Count - 1);
                    item.FullPath = string.Join("\\", arr);
                }
                result.Add(item);
            }
            return result;
        }

        /// <summary>
        /// 根据名字查对象，支持模糊查询
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UsersInfoExtend GetUsersInfoExtendById(string userid)
        {
            UsersInfoExtend result = new UsersInfoExtend();

            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ID,DisplayName as DISPLAY_NAME,MP as MOBILE ,Mail as INTERNET_EMAIL FROM sc.SchemaUserSnapshot WHERE");
            sql.AppendFormat(" VersionEndTime='9999-09-09 00:00:00.000' AND ID='{0}' AND AccountDisabled=0", TSqlBuilder.Instance.CheckQuotationMark(userid, false));
            sql.Append(" order by DisplayName");

            UsersInfoExtendCollection queryResult = QueryData(sql.ToString());
            if (queryResult.Count > 0)
                result = queryResult.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
		protected string GetSqlConnectionName(string connectionName)
        {
            return DbConnectionManager.GetConnectionString(connectionName);
        }

        public DataTable GetChildsBy(string parentId)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("SELECT SchemaType as ObjectType, OBJECTID AS ID,DisplayName as DISPLAY_NAME,MP as MOBILE ,Mail as INTERNET_EMAIL FROM dbo.Contacts WHERE");
            sql.AppendFormat(" ParentID = '{0}'", TSqlBuilder.Instance.CheckQuotationMark(parentId, false));
            sql.Append(" order by InnerSort");

            string connectionString = Instance.GetSqlConnectionName("MCS_ReportDB");
            return new SqlDbHelper(connectionString).ExecuteDataTable(sql.ToString());
        }

        /// <summary>
        /// 根据userCode获取用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public DataTable GetLoadSuperior(string userCode)
        {
            var connecString = Instance.GetSqlConnectionName("MCS_ReportDB");
            var sql = @"with cte(ObjectID,parentid,DisplayName) 
                         as 
                         (--下级父项 
                         select ObjectID,parentID,DisplayName from [dbo].[Contacts] where  ObjectID= @UserCode and isDefault = 1
                         union all 
                         --递归结果集中的父项 
                         select t.ObjectID,t.parentid,t.DisplayName from [dbo].[Contacts] as t 
                         inner join cte as c on t.ObjectID = c.parentid and t.ObjectID != @ObjectID 
                         ) 
                         select ObjectID,DisplayName from cte";
            SqlParameter[] parameters = {
                new SqlParameter("@UserCode",SqlDbType.NVarChar,36),
                new SqlParameter("@ObjectID",SqlDbType.NVarChar,36)
            };
            parameters[0].Value = userCode;
            parameters[1].Value = "efb29cac-5321-495b-844b-ed239a844ada";
            SqlDbHelper helper = new SqlDbHelper(connecString);
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 根据userCode获取用户信息
        /// </summary>
        /// <param name="logonName"></param>
        /// <returns></returns>
        public DataTable GetUserOfficeEmail(string logonName)
        {
            var connecString = Instance.GetSqlConnectionName(ConnectionNameDefine.EmployeeAttendance);
            var sql = $"select * from [MobileBusiness].[dbo].[Last_User] where  USER_ID='{logonName}'  and  sortID=1";
            SqlDbHelper helper = new SqlDbHelper(connecString);
            return helper.ExecuteDataTable(sql, CommandType.Text);
        }

    }
}
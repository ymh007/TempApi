using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.PunchManagement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.PunchManagement
{
    /// <summary>
    /// 打卡管理adpapter
    /// </summary>
    public class PunchManagementAdapter : UpdatableAndLoadableAdapterBase<PunchManagementModel, PunchManagementCollection>
    {
        /// <summary>
		/// 实例
		/// </summary>
		public static readonly PunchManagementAdapter Instance = new PunchManagementAdapter();

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
        /// 查询总记录数
        /// </summary>
        public int GetListByPage(string name,string conName)
        {
            var sql = @"SELECT COUNT(1) FROM [dbo].[PunchManagement] WHERE [Name] LIKE '%' + @Name + '%' and CreatorName like  '%' + @cName + '%'";

            SqlParameter[] parameters = { new SqlParameter("@Name", SqlDbType.NVarChar, 50), new SqlParameter("@cName", SqlDbType.NVarChar, 255) };
            parameters[0].Value = name;
            parameters[1].Value = conName;
            var helper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["EmployeeAttendance"].ConnectionString);
            var result = helper.ExecuteScalar(sql, CommandType.Text, parameters);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 查询当前页数据
        /// </summary>
        public DataTable GetListByPage(int pageIndex, int pageSize, string name,string conName)
        {
            pageIndex--;
            var sql = @"
                WITH [Temp] AS
                (
                    SELECT ROW_NUMBER() OVER(ORDER BY [Type] ASC, [CREATETIME] DESC) AS [Row], * FROM 
                    [dbo].[PunchManagement] WHERE [Name] LIKE '%' + @Name + '%' and CreatorName like  '%' + @cName + '%'
                )
                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize;";

            SqlParameter[] parameters = {
                new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                new SqlParameter("@PageSize", SqlDbType.Int, 4),
                new SqlParameter("@Name", SqlDbType.NVarChar, 36),
                new SqlParameter("@cName",SqlDbType.NVarChar,36)
            };
            parameters[0].Value = pageSize * pageIndex + 1;
            parameters[1].Value = pageSize * pageIndex + pageSize;
            parameters[2].Value = name;
            parameters[3].Value = conName;
            var helper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["EmployeeAttendance"].ConnectionString);
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 根据用户编码获取打卡管理单元列表（超管获取全部，分级管理员获取自己创建的）
        /// </summary>
        public DataTable GetList(string userCode)
        {
            var sql = @"
                IF EXISTS(SELECT * FROM [YuanXinBusiness].[office].[Sys_User] WHERE [UserCode] = @UserCode AND [IsEnabled] = 1 AND [Super] = 1)
	                SELECT *, (SELECT TOP 1 [ConcatCode] + '|' + [Type] FROM [dbo].[PunchDepartment] WHERE [PunchCode] = [A].[Code]) AS [Default] FROM [dbo].[PunchManagement] A ORDER BY [Type] ASC, [CreateTime] DESC;
                ELSE
	                SELECT *, (SELECT TOP 1 [ConcatCode] + '|' + [Type] FROM [dbo].[PunchDepartment] WHERE [PunchCode] = [A].[Code]) AS [Default] FROM [dbo].[PunchManagement] A WHERE [Creator] = @UserCode ORDER BY [Type] ASC, [CreateTime] DESC;";

            SqlParameter[] parameters = {
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36),
            };
            parameters[0].Value = userCode;

            var helper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["EmployeeAttendance"].ConnectionString);
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 根据打卡管理员编码获取打卡管理单元列表
        /// </summary>
        public DataTable GetListByManagerCode(string userCode)
        {
            var sql = @"
                SELECT DISTINCT [A].*, (SELECT TOP 1 [ConcatCode] + '|' + [Type] FROM [dbo].[PunchDepartment] WHERE [PunchCode] = [A].[Code]) AS [Default] FROM [dbo].[PunchManagement] A, [dbo].[PunchManager] B 
                WHERE 
	                [A].[Code] = [B].[PunchCode] AND 
	                [B].[ConcatCode] = @UserCode 
                ORDER BY [A].[Type] ASC, [A].[CreateTime] DESC;";

            SqlParameter[] parameters = {
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36),
            };
            parameters[0].Value = userCode;

            var helper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["EmployeeAttendance"].ConnectionString);
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 根据关键词查询用户列表
        /// </summary>
        public DataSet SearchUserListByKeyword(string type, string code, string keyword)
        {
            var sql = @"
                -- 搜索打卡管理单元
                IF @type = 'SettingUserCode'
	                BEGIN
		                IF EXISTS(SELECT * FROM [YuanXinBusiness].[office].[Sys_User] WHERE [UserCode] = @code AND [IsEnabled] = 1 AND [Super] = 1)
			                SELECT TOP 10 * FROM [dbo].[PunchManagement] WHERE [Name] LIKE '%' + @keyword + '%';
		                ELSE
			                SELECT TOP 10 * FROM [dbo].[PunchManagement] WHERE [Name] LIKE '%' + @keyword + '%' AND [Creator] = @code;
	                END
                IF @type = 'ManagerUserCode'
	                BEGIN
		                SELECT [A].* FROM [dbo].[PunchManagement] A, [dbo].[PunchManager] B
		                WHERE
			                [A].[Code] = [B].[PunchCode] AND
			                [B].[ConcatCode] = @code AND
                            [A].[Name] LIKE '%' + @keyword + '%';
	                END

                -- 搜索用户
                SELECT TOP 10 [A].* FROM [dbo].[Contacts] A, [dbo].[FuncTableGetUsers](@type, @code) B
                WHERE
	                [A].[ObjectID] = [B].[Code] AND 
	                ([A].[DisplayName] LIKE '%' + @keyword + '%' OR [A].[LOGON_NAME] LIKE '%' + @keyword + '%');";

            SqlParameter[] parameters = {
                new SqlParameter("@type", SqlDbType.VarChar, 20),
                new SqlParameter("@code", SqlDbType.NVarChar, 36),
                new SqlParameter("@keyword", SqlDbType.NVarChar, 30),
            };
            parameters[0].Value = type;
            parameters[1].Value = code;
            parameters[2].Value = keyword;

            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataSet(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 根据人员或组织机构编码查询打卡管理单元列表
        /// </summary>
        public PunchManagementCollection GetPunchManagementListByContactCode(string code)
        {
            var sql = $@"
                SELECT [A].* FROM [dbo].[PunchManagement] A
	                INNER JOIN [dbo].[PunchDepartment] B ON [A].[Code] = [B].[PunchCode]
                WHERE [A].[ValidStatus] = 1 AND [B].[ConcatCode] = '{code}'
                ORDER BY [A].[ModifyTime] DESC;";

            return QueryData(sql);
        }

        /// <summary>
        /// 根据用户编码获取用户所在的打卡管理单元
        /// </summary>
        public PunchManagementCollection GetListByUserCode(string userCode)
        {
            var sql = $"SELECT * FROM [dbo].[PunchManagement] WHERE [ValidStatus] = 1 AND [Code] IN (SELECT DISTINCT [PunchCode] FROM [dbo].[PunchPersonnel] WHERE [UserCode] = '{userCode}')";

            return QueryData(sql);
        }

        /// <summary>
        /// 获取所有配置人
        /// </summary>
        public PunchManagementCollection GetConfigPersons()
        {
            var sql = $" SELECT CreatorName FROM [dbo].[PunchManagement] group by CreatorName";
            return QueryData(sql);
        }
    }
}
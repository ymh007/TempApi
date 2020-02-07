using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{
    /// <summary>
    /// 活动主库适配器
    /// </summary>
    public class ActivityInfoAdapter : UpdatableAndLoadableAdapterBase<ActivityInfoModel, ActivityInfoCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }
        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly ActivityInfoAdapter Instance = new ActivityInfoAdapter();

        #region 获取全部活动
        /// <summary>
        /// 获取全部活动 - 总记录数
        /// </summary>
        public int GetListForAll(string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[ActivityInfo] 
                        WHERE 
	                        [IsApplyAll] = 1 OR
	                        [Code] IN (SELECT [ActivityCode] FROM [office].[ActivityApplySet] WHERE [UserCode] = @userCode);";

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 获取全部活动 - 当前页数据
        /// </summary>
        public DataTable GetListForAll(int pageSize, int pageIndex, string userCode)
        {
            pageIndex--;

            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [B].[CreateTime] DESC, [A].[CreateTime] DESC) AS [Row], [A].[Code], [A].[Title], [A].[Cover], [A].[StartTime], [A].[EndTime], [A].[IsOffline], [A].[OfflineAddress], [A].[CreateTime], [B].[CreateTime] AS [FollowTime] 
	                        FROM [office].[ActivityInfo] A
		                        LEFT JOIN 
		                        (
			                        SELECT * FROM 
			                        (
				                        SELECT ROW_NUMBER() OVER (PARTITION BY [ActivityCode] ORDER BY [CreateTime] DESC) AS [Row], [ActivityCode], [CreateTime] FROM [office].[ActivityRecord] WHERE [UserCode] = @userCode AND [Type] = 1
			                        ) AS [Record]
			                        WHERE 
				                        [Record].[Row] = 1
		                        ) B ON [A].[Code] = [B].[ActivityCode]
	                        WHERE 
		                        [A].[IsApplyAll] = 1 OR 
		                        [A].[Code] IN (SELECT [ActivityCode] FROM [office].[ActivityApplySet] WHERE [UserCode] = @userCode)
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            return new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取我参与的活动
        /// <summary>
        /// 获取我参与的活动 - 总记录数
        /// </summary>
        public int GetListForApply(string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[ActivityInfo]
                        WHERE
	                        [Code] IN (SELECT [ActivityCode] FROM [office].[ActivityApplyInfo] WHERE [UserCode] = @userCode);";

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 获取我参与的活动 - 当前页数据
        /// </summary>
        public DataTable GetListForApply(int pageSize, int pageIndex, string userCode)
        {
            pageIndex--;

            var sql = @"WITH [Temp] AS
                        (
                          SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], [Code], [Title], [Cover], [StartTime], [EndTime], [IsOffline], [OfflineAddress], [Contact], [Creator], [CreateTime]
                          FROM [office].[ActivityInfo]  
                          WHERE 
                          [Code] IN (SELECT [ActivityCode] FROM [office].[ActivityApplyInfo] WHERE [UserCode] = @userCode)
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            return new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取我发布的活动
        /// <summary>
        /// 获取我发布的活动 - 总记录数
        /// </summary>
        public int GetListForMy(string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[ActivityInfo] WHERE [Creator] = @userCode;";

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 获取我发布的活动 - 当前页数据
        /// </summary>
        public DataTable GetListForMy(int pageSize, int pageIndex, string userCode)
        {
            pageIndex--;

            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], [Code], [Title], [Cover], [StartTime], [EndTime], [IsOffline], [OfflineAddress], [CreateTime] 
	                        FROM [office].[ActivityInfo] 
	                        WHERE [Creator] = @userCode
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            return new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取后端管理列表
        /// <summary>
        /// 获取后端管理列表 - 总记录数
        /// </summary>
        public int GetList(string title)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[ActivityInfo] WHERE [Title] LIKE '%' + @Title + '%';";

            SqlParameter[] parameters = { new SqlParameter("@Title", SqlDbType.NVarChar, 50) };
            parameters[0].Value = title;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 获取后端管理列表 - 当前页数据
        /// </summary>
        public List<ActivityInfoModel> GetList(int pageSize, int pageIndex, string title)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], [Code], [Title], [Cover], [StartTime], [EndTime], [IsOffline], [OfflineAddress], [Creator], [CreateTime] FROM [office].[ActivityInfo] WHERE [Title] LIKE '%' + @Title + '%'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row]  BETWEEN {0} AND {1};";
            sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

            SqlParameter[] parameters = { new SqlParameter("@Title", SqlDbType.NVarChar, 50) };
            parameters[0].Value = title;

            var table = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
            return DataConvertHelper<ActivityInfoModel>.ConvertToList(table);
        }
        #endregion
    }
}
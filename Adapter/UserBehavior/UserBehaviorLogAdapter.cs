using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 用户行为日志适配器
    /// </summary>
    public class UserBehaviorLogAdapter : UpdatableAndLoadableAdapterBase<UserBehaviorLogModel, UserBehaviorLogCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly UserBehaviorLogAdapter Instance = new UserBehaviorLogAdapter();

        #region 添加日志
        /// <summary>
        /// 添加日志
        /// </summary>
        public int Add(UserBehaviorLogModel model)
        {
            var sql = @"INSERT INTO [office].[UserBehaviorLog]
                                ([UserCode],[UserAccount],[UserName],[DepartmentCode],[DepartmentName],[ModuleCode],[Name],[TimeStart],[TimeEnd],[Creator],[CreateTime],[ValidStatus])
                        VALUES  (@UserCode, @UserAccount, @UserName, @DepartmentCode, @DepartmentName, @ModuleCode, @Name, @TimeStart, @TimeEnd, @Creator, @CreateTime, @ValidStatus)";

            SqlParameter[] parameters = {
                new SqlParameter("@UserCode", model.UserCode),
                new SqlParameter("@UserAccount", model.UserAccount),
                new SqlParameter("@UserName", model.UserName),
                new SqlParameter("@DepartmentCode", model.DepartmentCode),
                new SqlParameter("@DepartmentName", model.DepartmentName),
                new SqlParameter("@ModuleCode", model.ModuleCode),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@TimeStart", model.TimeStart),
                new SqlParameter("@TimeEnd", model.TimeEnd),
                new SqlParameter("@Creator", model.Creator),
                new SqlParameter("@CreateTime", model.CreateTime),
                new SqlParameter("@ValidStatus", model.ValidStatus)
            };

            SqlDbHelper db = new SqlDbHelper();
            return db.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取统计列表
        /// <summary>
        /// 获取统计列表 - 总记录
        /// </summary>
        public int Statistical(string module, DateTime dateStart, DateTime dateEnd)
        {
            var sql = @"WITH Temp AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [A].[Name] ASC) AS [Row], [A].[Code], [A].[Name], COUNT(0) AS [ViewCount]
	                        FROM [office].[UserBehaviorModule] A, [office].[UserBehaviorLog] B
	                        WHERE
		                        [A].[Code] = [B].[ModuleCode] AND
		                        [A].[Name] LIKE '%' + @module + '%' AND
		                        [B].[CreateTime] BETWEEN @dateStart AND @dateEnd
	                        GROUP BY [A].[Code], [A].[Name]
                        )
                        SELECT COUNT(0) FROM [Temp];";

            SqlParameter[] parameters = {
                new SqlParameter("@module", SqlDbType.NVarChar, 20),
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = module;
            parameters[1].Value = dateStart;
            parameters[2].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            object result = helper.ExecuteScalar(sql, CommandType.Text, parameters);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        /// <summary>
        /// 获取统计列表 - 当前页数据
        /// </summary>
        public DataTable Statistical(string module, DateTime dateStart, DateTime dateEnd, int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH Temp AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [A].[Name] ASC) AS [Row], [A].[Code], [A].[Name], COUNT(0) AS [ViewCount]
	                        FROM [office].[UserBehaviorModule] A, [office].[UserBehaviorLog] B
	                        WHERE
		                        [A].[Code] = [B].[ModuleCode] AND
		                        [A].[Name] LIKE '%' + @module + '%' AND
		                        [B].[CreateTime] BETWEEN @dateStart AND @dateEnd
	                        GROUP BY [A].[Code], [A].[Name]
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            SqlParameter[] parameters = {
                new SqlParameter("@module", SqlDbType.NVarChar, 20),
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = module;
            parameters[1].Value = dateStart;
            parameters[2].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取统计详情
        /// <summary>
        /// 获取统计详情 - 总记录
        /// </summary>
        public int StatisticalInfo(string moduleCode, DateTime dateStart, DateTime dateEnd)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[UserBehaviorLog]
                        WHERE
	                        [ModuleCode] = @moduleCode AND
	                        [CreateTime] BETWEEN @dateStart AND @dateEnd;";

            SqlParameter[] parameters = {
                new SqlParameter("@moduleCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = moduleCode;
            parameters[1].Value = dateStart;
            parameters[2].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            object result = helper.ExecuteScalar(sql, CommandType.Text, parameters);
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }
        /// <summary>
        /// 获取统计详情 - 当前页数据
        /// </summary>
        public DataTable StatisticalInfo(string moduleCode, DateTime dateStart, DateTime dateEnd, int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH Temp AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], *, DATEDIFF(SECOND, [TimeStart], [TimeEnd]) AS [UseTime] 
	                        FROM [office].[UserBehaviorLog]
	                        WHERE
		                        [ModuleCode] = @moduleCode AND
                                [CreateTime] BETWEEN @dateStart AND @dateEnd
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            SqlParameter[] parameters = {
                new SqlParameter("@moduleCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = moduleCode;
            parameters[1].Value = dateStart;
            parameters[2].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取统计详情 -- 导出数据
        /// <summary>
        /// 获取统计详情 - 
        /// </summary>
        /// <param name="moduleCode"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public DataTable ExportStatisticalInfo(string moduleCode, DateTime dateStart, DateTime dateEnd)
        {
            var sql = @" SELECT  *, DATEDIFF(SECOND, [TimeStart], [TimeEnd]) AS [UseTime] 
	                        FROM [office].[UserBehaviorLog]
	                        WHERE
		                        [ModuleCode] = @moduleCode AND
                                [CreateTime] BETWEEN @dateStart AND @dateEnd
                                order by CreateTime;";

            SqlParameter[] parameters = {
                new SqlParameter("@moduleCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = moduleCode;
            parameters[1].Value = dateStart;
            parameters[2].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 导出数据 -- 全部记录(PC)
        /// <summary>
        /// 导出全部记录
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        public DataTable ExportAllByDate(DateTime dateStart, DateTime dateEnd)
        {
            var sql = @"SELECT  a.*,b.Name as ModuleName, DATEDIFF(SECOND, [TimeStart], [TimeEnd]) AS [UseTime] 
	                        FROM [office].[UserBehaviorLog] as a left   join [office].[UserBehaviorModule] as b on a.moduleCode = b.Code
	                        WHERE
                                a.[CreateTime] BETWEEN @dateStart AND @dateEnd
                                order by b.Name,a.CreateTime;";

            SqlParameter[] parameters = {
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = dateStart;
            parameters[1].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion


        #region 获取导出数据 - 根据模块编码
        /// <summary>
        /// 获取导出数据 - 根据模块编码
        /// </summary>
        public DataTable GetExportData(string moduleCode, DateTime dateStart, DateTime dateEnd)
        {
            var sql = @" SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], *, DATEDIFF(SECOND, [TimeStart], [TimeEnd]) AS [UseTime] 
	                        FROM [office].[UserBehaviorLog]
	                        WHERE
		                        [ModuleCode] = @moduleCode AND
                                [CreateTime] BETWEEN @dateStart AND @dateEnd;";

            SqlParameter[] parameters = {
                new SqlParameter("@moduleCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@dateStart", SqlDbType.DateTime),
                new SqlParameter("@dateEnd", SqlDbType.DateTime)
            };
            parameters[0].Value = moduleCode;
            parameters[1].Value = dateStart;
            parameters[2].Value = dateEnd;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion 

        #region 统计所有数据
        /// <summary>
        /// 统计所有数据（模块编码、模块名称、点击量）
        /// </summary>
        public DataTable GetModuleViewCount()
        {
            var sql = @"SELECT [A].[Code], [A].[Name], (SELECT COUNT(0) FROM [office].[UserBehaviorLog] [B] WHERE [B].[ModuleCode] = [A].[Code]) AS [ViewCount]
	                    FROM [office].[UserBehaviorModule] [A];";

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text);
        }
        #endregion
    }
}
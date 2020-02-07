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
using Seagull2.YuanXin.AppApi.Models.Version;

namespace Seagull2.YuanXin.AppApi.Adapter.Version
{
    /// <summary>
    /// 系统升级记录 Adapter
    /// </summary>
    public class SystemUpdateRecordAdapter : UpdatableAndLoadableAdapterBase<SystemUpdateRecordModel, SystemUpdateRecordCollection>
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
        public static readonly SystemUpdateRecordAdapter Instance = new SystemUpdateRecordAdapter();

        #region 获取系统升级记录列表

        /// <summary>
        /// 获取系统升级记录列表 - 总记录数
        /// </summary>
        public int GetList(string appId, string system)
        {
            var sql = @"SELECT COUNT(0) FROM [OAuth].[SystemUpdateRecord] 
                        WHERE 
	                        [AppId] = @AppId AND
	                        CASE @System WHEN '' THEN '' ELSE [System] END = @System;";

            SqlParameter[] parameters = { new SqlParameter("@AppId", SqlDbType.NVarChar, 64), new SqlParameter("@System", SqlDbType.NVarChar, 10) };
            parameters[0].Value = appId;
            parameters[1].Value = system;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
        /// <summary>
        /// 获取系统升级记录列表 - 当前页数据
        /// </summary>
        public List<SystemUpdateRecordModel> GetList(int pageSize, int pageIndex, string appId, string system)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [OAuth].[SystemUpdateRecord]
	                        WHERE 
		                        [AppId] = @AppId AND
		                        CASE @System WHEN '' THEN '' ELSE [System] END = @System
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row]  BETWEEN {0} AND {1};";
            sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

            SqlParameter[] parameters = { new SqlParameter("@AppId", SqlDbType.NVarChar, 64), new SqlParameter("@System", SqlDbType.NVarChar, 10) };
            parameters[0].Value = appId;
            parameters[1].Value = system;

            var table = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
            return DataConvertHelper<SystemUpdateRecordModel>.ConvertToList(table);
        }

        #endregion

        #region 获取最新版本信息
        /// <summary>
        /// 获取最新版本信息
        /// </summary>
        public SystemUpdateRecordModel GetNewVersion(string appid, string system, string version)
        {
            var sql = @"SELECT TOP 1 * FROM [OAuth].[SystemUpdateRecord] 
                        WHERE 
	                        [AppId] = @AppId AND 
	                        [System] = @System AND 
	                        CONVERT(INT, REPLACE([Version], '.', '')) > REPLACE(@Version, '.', '') ORDER BY CONVERT(INT, REPLACE([Version], '.', '')) DESC;";

            SqlParameter[] parameters = {
                new SqlParameter("@AppId", SqlDbType.NVarChar, 64),
                new SqlParameter("@System", SqlDbType.NVarChar, 10),
                new SqlParameter("@Version", SqlDbType.NVarChar, 20) };
            parameters[0].Value = appid;
            parameters[1].Value = system;
            parameters[2].Value = version;

            var dt = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
            if (dt.Rows.Count > 0)
            {
                return DataConvertHelper<SystemUpdateRecordModel>.ConvertToModel(dt.Rows[0]);
            }
            return null;
        }
        #endregion
    }
}
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Share;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;

namespace Seagull2.YuanXin.AppApi.Adapter.Share
{
    /// <summary>
    /// 图文消息组适配器
    /// </summary>
    public class GroupAdapter : UpdatableAndLoadableAdapterBase<GroupModel, GroupCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 日志
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly GroupAdapter Instance = new GroupAdapter();

        #region 获取数据列表 - PC

        /// <summary>
        /// 获取数据列表 - 总记录数
        /// </summary>
        public int GetListForPC()
        {
            var sql = @"SELECT COUNT(0) FROM [office].[S_Group];";

            var count = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 获取数据列表 -  当前页数据
        /// </summary>
        public DataTable GetListForPC(int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[S_Group]
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}";
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            return DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
        }

        #endregion

        #region 获取数据列表 - APP

        /// <summary>
        /// 获取数据列表 - 总记录数
        /// </summary>
        public int GetListForApp(string menuCode, string userCode)
        {
            var sql = @"SELECT COUNT(0) 
                        FROM [office].[S_Group] 
                        WHERE
	                        [IsEnable] = 'True' AND 
	                        CASE @MenuCode WHEN '' THEN '' ELSE [MenuCode] END = @MenuCode AND 
	                        ([SendGroupCode] = '' OR [SendGroupCode] IN 
		                        (
			                        SELECT DISTINCT [SendGroupCode] FROM [office].[S_SendGroupPerson] WHERE [UserCode] = @UserCode
		                        )
	                        );";
            sql = sql.Replace("@MenuCode", "'" + menuCode + "'");
            sql = sql.Replace("@UserCode", "'" + userCode + "'");

            var count = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 获取数据列表 -  当前页数据
        /// </summary>
        public DataTable GetListForApp(int pageSize, int pageIndex, string menuCode, string userCode)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[S_Group]
	                        WHERE 
		                        [IsEnable] = 'True' AND 
		                        CASE @MenuCode WHEN '' THEN '' ELSE [MenuCode] END = @MenuCode AND 
		                        ([SendGroupCode] = '' OR [SendGroupCode] IN 
			                        (
				                        SELECT DISTINCT [SendGroupCode] FROM [office].[S_SendGroupPerson] WHERE [UserCode] = @UserCode
			                        )
		                        )
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}";
            sql = sql.Replace("@MenuCode", "'" + menuCode + "'");
            sql = sql.Replace("@UserCode", "'" + userCode + "'");
            sql = string.Format(sql, (pageSize * pageIndex + 1), (pageSize * pageIndex + pageSize));

            return DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
        }

        #endregion
    }
}
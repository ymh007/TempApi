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
    /// 记录适配器
    /// </summary>
    public class RecordAdapter : UpdatableAndLoadableAdapterBase<RecordModel, RecordCollection>
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
        public static readonly RecordAdapter Instance = new RecordAdapter();

        /// <summary>
        /// 获取统计数量
        /// </summary>
        /// <param name="type">类型（0：阅读；1：点赞）</param>
        /// <param name="targetCode">目标Code</param>
        public int GetCount(int type, string targetCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[S_Record] WHERE Type = {0} AND TargetCode = '{1}'";
            sql = string.Format(sql, type, targetCode);

            var count = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());

            return Convert.ToInt32(count);
        }

        #region 获取阅读/点赞列表

        /// <summary>
        /// 获取阅读/点赞列表 - 总记录
        /// </summary>
        public int GetList(string targetCode, int type)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[S_Record] WHERE [TargetCode] = @TargetCode AND [Type] = @Type;";

            SqlParameter[] parameters = {
                new SqlParameter("@TargetCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@Type", SqlDbType.Int, 4) };
            parameters[0].Value = targetCode;
            parameters[1].Value = type;

            var count = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 获取阅读/点赞列表 - 当前页数据
        /// </summary>
        public DataTable GetList(int pageSize, int pageIndex, string targetCode, int type)
        {
            pageIndex--;

            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[S_Record] WHERE [TargetCode] = @TargetCode AND [Type] = @Type
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}";
            sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

            SqlParameter[] parameters = {
                new SqlParameter("@TargetCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@Type", SqlDbType.Int, 4) };
            parameters[0].Value = targetCode;
            parameters[1].Value = type;

            return new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        #endregion
    }
}
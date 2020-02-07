using System;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    /// <summary>
    /// 辅助生成sql类
    /// </summary>
    public static class SqlHelper
    {

        /// <summary>
        /// 生成删除和插入的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteBuilder"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetDeleteAndInsertSql<T>(IConnectiveSqlClause deleteBuilder, IEnumerable<T> data)
        {
            ISqlBuilder sqlBuilderInstance = TSqlBuilder.Instance;

            StringBuilder strSql = new StringBuilder();

            if (deleteBuilder != null && !deleteBuilder.IsEmpty)
            {
                strSql.Append(SqlHelper.GetDeleteSql<T>(deleteBuilder));
                strSql.Append(sqlBuilderInstance.DBStatementSeperator);
            }

            if (data != null)
            {
                foreach (T item in data)
                {
                    strSql.Append(ORMapping.GetInsertSql(item, sqlBuilderInstance));
                    strSql.Append(sqlBuilderInstance.DBStatementSeperator);
                }
            }
            return strSql.ToString();
        }

        public static string GetDeleteSql<T>(string where)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("DELETE FROM {0} WHERE {1}", ORMapping.GetMappingInfo<T>().TableName, @where);
            return sql.ToString();
        }

        public static string GetDeleteSql<T>(Action<WhereSqlClauseBuilder> whereAction)
        {
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            whereAction(builder);
            return GetDeleteSql<T>(builder.ToSqlString(TSqlBuilder.Instance));
        }

        public static string GetDeleteSql<T>(IConnectiveSqlClause deleteBuilder)
        {
            return GetDeleteSql<T>(deleteBuilder.ToSqlString(TSqlBuilder.Instance));
        }

        public static string GetUpdateSql<T>(UpdateSqlClauseBuilder update, IConnectiveSqlClause where)
        {
            update.NullCheck<ArgumentNullException>("update");
            update.Any().FalseThrow<ArgumentException>("updateAction");
            where.NullCheck<ArgumentNullException>("where");
            where.IsEmpty.TrueThrow<ArgumentException>("where");

            string sql = string.Format("{3}UPDATE {0} SET {1} WHERE {2}{3}",
                ORMapping.GetMappingInfo<T>().TableName,
                update.ToSqlString(TSqlBuilder.Instance),
                where.ToSqlString(TSqlBuilder.Instance),
                TSqlBuilder.Instance.DBStatementSeperator
            );

            return sql;
        }

        
    }
}

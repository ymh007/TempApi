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
using System.Configuration;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Common
{
    /// <summary>
    /// Adapter扩展功能类
    /// </summary>
    public static class AdapterExtension
    {

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="data"></param>
        /// <param name="connectionName"></param>
        public static void UpdateCollection<T>(this UpdatableAdapterBase<T> adapter,
            IEnumerable<T> data, string connectionName = null)
        {
            if (connectionName == null)
            {
                connectionName = adapter.ConnectionName;
            }

            var deleteWhere = new ConnectiveSqlClauseCollection(LogicOperatorDefine.Or);
            foreach (T item in data)
            {
                deleteWhere.Add(ORMapping.GetWhereSqlClauseBuilderByPrimaryKey(item));
            }

            string sql = SqlHelper.GetDeleteAndInsertSql(deleteWhere, data);

            if (sql.Length > 0)
            {
                DbHelper.RunSqlWithTransaction(sql, connectionName);
            }    
        }

        /// <summary>
        /// 根据主键批量删除，主键名称默认为Code
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="codes">主键，多个使用逗号分隔</param>
        /// <param name="defaultCodeName"></param>
        /// <param name="connectionName"></param>
        public static void DeleteByCodes<T>(this UpdatableAdapterBase<T> adapter,
            string codes, string defaultCodeName="Code", string connectionName = null)
        {
            if (connectionName == null)
            {
                connectionName = adapter.ConnectionName;
            }

            InSqlClauseBuilder inSql = new InSqlClauseBuilder();
            inSql.AppendItem(codes.Split(','));

            adapter.Delete(p =>
            {
                p.AppendItem(defaultCodeName, inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            });
        }

        #region SetFields
        /// <summary>
        /// 根据条件更新个别字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="updateAction">set字段条件</param>
        /// <param name="whereAction">where条件</param>
        /// <param name="connectionName"></param>
        public static void SetFields<T>(this UpdatableAdapterBase<T> adapter,
             Action<UpdateSqlClauseBuilder> updateAction, Action<WhereSqlClauseBuilder> whereAction, string connectionName = null)
        {
            updateAction.NullCheck<ArgumentNullException>("updateAction");
            whereAction.NullCheck<ArgumentNullException>("whereAction");

            if (connectionName == null)
            {
                connectionName = adapter.ConnectionName;
            }

            var update = new UpdateSqlClauseBuilder();
            var where = new WhereSqlClauseBuilder();
            updateAction(update);
            whereAction(where);

            string strSql = SqlHelper.GetUpdateSql<T>(update, where);
            DbHelper.RunSqlWithTransaction(strSql, connectionName);
        }

        /// <summary>
        /// 更新单个字段值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="filedValue">字段值</param>
        /// <param name="whereAction">where条件</param>
        /// <param name="connectionName"></param>
        public static void SetFields<T>(this UpdatableAdapterBase<T> adapter,
            string fieldName, object filedValue, Action<WhereSqlClauseBuilder> whereAction, string connectionName = null)
        {
            adapter.SetFields(field =>
            {
                field.AppendItem(fieldName, filedValue);
            }, whereAction, connectionName);
        }



        /// <summary>
        /// 设置字段值递增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="size">增长大小</param>
        /// <param name="whereAction">where条件</param>
        /// <param name="connectionName"></param>
        public static void SetInc<T, TCollection>(this UpdatableAndLoadableAdapterBase<T, TCollection> adapter,
            string fieldName, int size, Action<WhereSqlClauseBuilder> whereAction, string connectionName = null)
            where TCollection : EditableDataObjectCollectionBase<T>,new()
        {
            if (connectionName == null)
            {
                connectionName = adapter.ConnectionName;
            }

            var where = new WhereSqlClauseBuilder();
            whereAction(where);
            TCollection collection = adapter.LoadByBuilder(where);

            string strSql = string.Format("SELECT {0} FROM {1} WHERE {2}", fieldName, 
                ORMapping.GetMappingInfo<T>().TableName,
                where.ToSqlString(TSqlBuilder.Instance));

            int result=0;

            int.TryParse(DbHelper.RunSqlReturnScalar(strSql, connectionName).ToString(), out result);
             
            //int result = (int)DbHelper.RunSqlReturnScalar(strSql, connectionName);
            
            adapter.SetFields(fieldName, result + size, whereAction, connectionName);
        }

        public static void SetInc<T, TCollection>(this UpdatableAndLoadableAdapterBase<T, TCollection> adapter,
            string fieldName, Action<WhereSqlClauseBuilder> whereAction, string connectionName = null)
            where TCollection : EditableDataObjectCollectionBase<T>, new()
        {
            adapter.SetInc(fieldName, 1, whereAction, connectionName);
        }

        /// <summary>
        /// 设置字段值递减
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="adapter"></param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="size">递减大小</param>
        /// <param name="whereAction">where条件</param>
        /// <param name="connectionName"></param>
        public static void SetDec<T, TCollection>(this UpdatableAndLoadableAdapterBase<T, TCollection> adapter,
            string fieldName, int size, Action<WhereSqlClauseBuilder> whereAction, string connectionName = null)
            where TCollection : EditableDataObjectCollectionBase<T>, new()
        {
            if (connectionName == null)
            {
                connectionName = adapter.ConnectionName;
            }

            var where = new WhereSqlClauseBuilder();
            whereAction(where);
            TCollection collection = adapter.LoadByBuilder(where);

            string strSql = string.Format("SELECT {0} FROM {1} WHERE {2}", fieldName,
                ORMapping.GetMappingInfo<T>().TableName,
                where.ToSqlString(TSqlBuilder.Instance));

            int result = (int)DbHelper.RunSqlReturnScalar(strSql, connectionName);

            adapter.SetFields(fieldName, result - size, whereAction, connectionName);
        }

        public static void SetDec<T, TCollection>(this UpdatableAndLoadableAdapterBase<T, TCollection> adapter,
            string fieldName, Action<WhereSqlClauseBuilder> whereAction, string connectionName = null)
            where TCollection : EditableDataObjectCollectionBase<T>, new()
        {
            adapter.SetDec(fieldName, 1, whereAction, connectionName);
        }


        #endregion
    }
}

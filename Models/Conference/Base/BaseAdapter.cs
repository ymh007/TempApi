using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    public class BaseAdapter<T, TCollection> : UpdatableAndLoadableAdapterBase<T, TCollection> where TCollection : EditableDataObjectCollectionBase<T>, new()
    {
        protected static string BaseConnectionStr = "";

        protected override string GetConnectionName()
        {
            return BaseConnectionStr;
        }

        public virtual void AddOrUpdateT(T t)
        {
            Update(t);
        }
        public virtual void DelTById(string id)
        {
            //WhereSqlClauseBuilder whereSql = new WhereSqlClauseBuilder();
            //whereSql.;
            Delete(m => m.AppendItem("ID", id));
        }
        public virtual T GetTByID(string id)
        {
            return Load(m => m.AppendItem("ID", id)).FirstOrDefault();
        }
        public virtual TCollection GetTColl()
        {
            return Load(m => m.AppendItem("1", "1"));
        }
        public virtual ViewPageBase<TCollection> GetTListByPage(string selectSql, string fromAndWhereSQL, string orderBySQL, int pageIndex)
        {
            ViewPageBase<TCollection> pageList = new ViewPageBase<TCollection>();
            try
            {
                int pageSize = ViewPageBase<TCollection>.PageSize;

                //string dataListSql = selectSql + " " + fromAndWhereSQL + " " + orderBySQL + " OFFSET " + (pageIndex - 1) * pageSize + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY";
                string dataListSql = string.Format(@"SELECT * FROM (
                                                        {0}, ROW_NUMBER() OVER ({1}) AS 'RowNumberForSplit' 
                                                        {2}) temp 
                                         WHERE RowNumberForSplit BETWEEN {3} AND {4}", selectSql, orderBySQL, fromAndWhereSQL, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
                string dataCountSql = "select count(*) " + fromAndWhereSQL;

                pageList.dataList = QueryData(dataListSql);
                SqlDbHelper sqlhelp = new SqlDbHelper();
                int totalCount = (int)sqlhelp.ExecuteScalar(dataCountSql);
                pageList.PageCount = totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0);
                pageList.IsLastPage = pageList.PageCount <= pageIndex ? true : false;
                pageList.State = true;
            }
            catch (Exception e)
            {
                pageList.State = false;
                pageList.Message = e.Message;
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
            }
            return pageList;
        }
    }
}
//using Seagull2.YuanXin.AppApi.DataAccess;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 自定义类Adapter访问基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="ListT"></typeparam>
    public class ViewBaseAdapter<T, ListT> where T : new() where ListT : List<T>, new()
    {
        private T t = new T();
        private string ConnectionName;

        /// <summary>
        /// 实例化
        /// </summary>
        public static ViewBaseAdapter<T, ListT> Instance = new ViewBaseAdapter<T, ListT>();

        /// <summary>
        /// 构造
        /// </summary>
        public ViewBaseAdapter()
        {

        }
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="connectionName"></param>
        public ViewBaseAdapter(string connectionName)
        {
            ConnectionName = ConfigurationManager.ConnectionStrings[connectionName].ToString();
        }
        /// <summary>
        /// 自定义类分页查询方法
        /// </summary>
        /// <param name="selectSql">Select语句</param>
        /// <param name="fromAndWhereSQL">From+Where语句</param>
        /// <param name="orderBySQL">排序字段</param>
        /// <param name="pageIndex">获取数据的页码</param>
        /// <returns></returns>
        public ViewPageBase<ListT> LoadViewModelCollByPage(string selectSql, string fromAndWhereSQL, string orderBySQL, int pageIndex)
        {
            var pageSize = ViewPageBase<ListT>.PageSize;
            //sqlserver2012
            //string sql = selectSql + " " + fromAndWhereSQL + " " + orderBySQL + " OFFSET " + (pageIndex - 1) * pageSize + " ROWS FETCH NEXT " + pageSize + " ROWS ONLY;";
            string sql = string.Format(@"SELECT * FROM (
                                                        {0}, ROW_NUMBER() OVER ({1}) AS 'RowNumberForSplit' 
                                                        {2}) temp 
                                         WHERE RowNumberForSplit BETWEEN {3} AND {4}", selectSql, orderBySQL, fromAndWhereSQL, (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
            string sqlCount = "SELECT COUNT(*) " + fromAndWhereSQL;
            ViewPageBase<ListT> model = new ViewPageBase<ListT>();
            try
            {
                int totalCount = (int)LoadFirstValue(sqlCount);
                model = new ViewPageBase<ListT>()
                {
                    dataList = (ListT)LoadTColl(sql),
                    PageCount = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0),
                    TotalRowsCount = totalCount > 0 ? totalCount : 0
                };
                model.IsLastPage = pageIndex >= model.PageCount ? true : false;
                model.State = true;
            }
            catch (Exception e)
            {
                model.State = false;
                model.Message = e.Message;
            }
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlParamterColl"></param>
        /// <returns></returns>
        protected ListT LoadTColl(string sql, SqlParameter[] sqlParamterColl)
        {
            ListT tList = new ListT();

            using (SqlConnection sqlConn = new SqlConnection(ConnectionName))
            {
                sqlConn.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);
                if (sqlParamterColl != null && sqlParamterColl.Length > 0)
                {
                    sqlCommand.Parameters.AddRange(sqlParamterColl);
                }

                sqlCommand.CommandType = CommandType.Text;

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    t = new T();
                    Type tType = t.GetType();
                    PropertyInfo[] propertyInfoColl = tType.GetProperties();
                    foreach (PropertyInfo pro in propertyInfoColl)
                    {
                        if (reader.IsHasFiled(pro.Name) && reader["" + pro.Name + ""] != DBNull.Value)
                        {
                            //pro.SetValue(t, reader["" + pro.Name + ""]);
                            this.setTProperty(pro, reader["" + pro.Name + ""]);
                        }
                    }
                    tList.Add(t);
                    ;
                }
            }
            return tList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected List<T> LoadTColl(string sql)
        {
            List<T> tList = new List<T>();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionName))
            {
                sqlConn.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);

                sqlCommand.CommandType = CommandType.Text;

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    t = new T();
                    Type tType = t.GetType();
                    PropertyInfo[] propertyInfoColl = tType.GetProperties();
                    foreach (PropertyInfo pro in propertyInfoColl)
                    {
                        if (reader.IsHasFiled(pro.Name) && reader["" + pro.Name + ""] != DBNull.Value)
                        {
                            //pro.SetValue(t, reader["" + pro.Name + ""]);
                            this.setTProperty(pro, reader["" + pro.Name + ""]);
                        }
                    }
                    tList.Add(t);
                }
            }
            return tList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        protected List<T> LoadTCollByName(string sql, string connectionName)
        {
            List<T> tList = new List<T>();
            //string m_ConnectionName = string.IsNullOrEmpty(connectionName) ? this.ConnectionName : connectionName;

            using (SqlConnection sqlConn = new SqlConnection(connectionName))
            {
                sqlConn.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);

                sqlCommand.CommandType = CommandType.Text;

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    t = new T();
                    Type tType = t.GetType();
                    PropertyInfo[] propertyInfoColl = tType.GetProperties();
                    foreach (PropertyInfo pro in propertyInfoColl)
                    {
                        if (reader.IsHasFiled(pro.Name) && reader["" + pro.Name + ""] != DBNull.Value)
                        {
                            //pro.SetValue(t, reader["" + pro.Name + ""]);
                            this.setTProperty(pro, reader["" + pro.Name + ""]);
                        }
                    }
                    tList.Add(t);
                }
            }
            return tList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object LoadFirstValue(string sql)
        {
            object result = new object();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionName))
            {
                sqlConn.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);

                sqlCommand.CommandType = CommandType.Text;

                result = sqlCommand.ExecuteScalar();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public object LoadFirstValue(string sql, string connectionName)
        {
            object result = new object();
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString()))
            {
                sqlConn.Open();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlConn);

                sqlCommand.CommandType = CommandType.Text;

                result = sqlCommand.ExecuteScalar();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        protected DataSet GetDataSetBySql(string sql)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionName))
            {
                SqlCommand sCommand = new SqlCommand(sql, sqlConn);
                sCommand.CommandType = CommandType.Text;
                SqlDataAdapter adapter = new SqlDataAdapter(sCommand);
                adapter.Fill(dataSet);
            }
            return dataSet;
        }

        /// <summary>
        /// 通过事务执行SQL
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="connectionStr">连接字符串</param>
        /// <returns>受影响行数</returns>
        public int RunSQLByTransaction(string sql, string connectionStr)
        {
            int result = 0;
            using (SqlConnection sqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionStr].ToString()))
            {
                sqlConn.Open();
                using (SqlTransaction tran = sqlConn.BeginTransaction())
                {
                    try
                    {
                        SqlCommand sCommand = new SqlCommand(sql, sqlConn, tran);
                        sCommand.CommandType = CommandType.Text;
                        result = sCommand.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
            return result;
        }
        private void setTProperty(PropertyInfo pro, Object obj)
        {
            if (pro.PropertyType == typeof(int))
            {
                pro.SetValue(t, Convert.ToInt32(obj));
            }
            else if (pro.PropertyType == typeof(string))
            {
                pro.SetValue(t, Convert.ToString(obj));
            }
            else if (pro.PropertyType == typeof(DateTime))
            {
                pro.SetValue(t, Convert.ToDateTime(obj));
            }
            else if (pro.PropertyType == typeof(bool))
            {
                var data = true;
                if (obj.ToString() == "0" || obj.ToString().ToLower() == "false")
                {
                    data = false;
                }
                pro.SetValue(t, data);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string getPropertyValue(T t, string propertyName)
        {
            PropertyInfo info = t.GetType().GetProperty(propertyName);
            //获取属性值转换暂设置如下字段，可根据实际情况添加
            if (info.PropertyType == typeof(DateTime))
            {
                return Convert.ToDateTime(info.GetValue(t)).ToString("yyyy-MM-dd HH:mm");
            }
            return info.GetValue(t).ToString();
        }
    }
}
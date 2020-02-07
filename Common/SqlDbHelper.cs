using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;
using log4net;
using MCS.Library.Data;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 针对SQL Server数据库操作的通用类
    /// </summary>
    public class SqlDbHelper
    {
        private string connectionString;

        /// <summary>
        /// 设置数据库连接字符串
        /// </summary>
        public string ConnectionString
        {
            set { connectionString = value; }
        }

        /// <summary>
        /// 构造函数（默认连接：yuanxin）
        /// </summary>
        public SqlDbHelper() : this(ConfigurationManager.ConnectionStrings["yuanxin"].ConnectionString)
        {

        }

        /// <summary>
        /// 构造函数（需要传入新的连接字符串）
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public SqlDbHelper(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #region 执行一个查询，并返回 DataTable

        /// <summary>
        /// 执行一个查询，并返回 DataTable
        /// </summary>
        /// <param name="sql">要执行的查询SQL文本命令</param>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 执行一个查询，并返回 DataTable
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        public DataTable ExecuteDataTable(string sql, CommandType commandType)
        {
            return ExecuteDataTable(sql, commandType, null);
        }

        /// <summary>
        /// 执行一个查询，并返回 DataTable
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        public DataTable ExecuteDataTable(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            DataTable data = new DataTable();//实例化DataTable，用于装载查询结果集

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType

                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    //通过包含查询SQL的SqlCommand实例来实例化SqlDataAdapter
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                }
            }
            return data;
        }

        /// <summary>
        /// 执行一个查询，并返回 DataSet
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        public DataSet ExecuteDataSet(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            DataSet data = new DataSet();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType

                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    //通过包含查询SQL的SqlCommand实例来实例化SqlDataAdapter
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);
                }
            }
            return data;
        }

        #endregion

        #region 执行一个查询，并返回 SqlDataReader

        /// <summary>
        /// 执行一个查询，并返回 SqlDataReader
        /// </summary>
        /// <param name="sql">要执行的查询SQL文本命令</param>
        public SqlDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 执行一个查询，并返回 SqlDataReader
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        public SqlDataReader ExecuteReader(string sql, CommandType commandType)
        {
            return ExecuteReader(sql, commandType, null);
        }

        /// <summary>
        /// 执行一个查询，并返回 SqlDataReader
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        public SqlDataReader ExecuteReader(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(sql, connection);
            //如果同时传入了参数，则添加这些参数
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            connection.Open();
            //CommandBehavior.CloseConnection参数指示关闭Reader对象时关闭与其关联的Connection对象
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        #endregion

        #region 执行一个查询，并返回一行一列的 Object

        /// <summary>
        /// 执行一个查询，并返回一行一列的 Object
        /// </summary>
        /// <param name="sql">要执行的查询SQL文本命令</param>
        public Object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 执行一个查询，并返回一行一列的 Object
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        public Object ExecuteScalar(string sql, CommandType commandType)
        {
            return ExecuteScalar(sql, commandType, null);
        }

        /// <summary>
        /// 执行一个查询，并返回一行一列的 Object
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        public Object ExecuteScalar(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            object result = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接
                    result = command.ExecuteScalar();
                }
            }
            return result;//返回查询结果的第一行第一列，忽略其它行和列
        }

        #endregion

        #region 对数据库执行增删改操作

        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="sql">要执行的查询SQL文本命令</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            return ExecuteNonQuery(sql, commandType, null);
        }

        /// <summary>
        /// 对数据库执行增删改操作
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parameters">Transact-SQL 语句或存储过程的参数数组</param>
        public int ExecuteNonQuery(string sql, CommandType commandType, SqlParameter[] parameters)
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                    //如果同时传入了参数，则添加这些参数
                    if (parameters != null)
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }
                    connection.Open();//打开数据库连接

                    count = command.ExecuteNonQuery();

                }
            }
            return count;//返回执行增删改操作之后，数据库中受影响的行数
        }

        #endregion

        #region 以事务的模式对数据库执行增删改操作

        /// <summary>
        /// 以事务的模式对数据库执行增删改操作
        /// </summary>
        /// <param name="sqlArray">要执行的SQL语句数组</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <param name="parametersArray">Transact-SQL 语句或存储过程的参数数组</param>
        /// <returns></returns>
        public bool ExecuteNonQueryTransation(List<string> sqlArray, CommandType commandType, List<SqlParameter[]> parametersArray)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();//打开数据库连接
                //事务处理
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqlArray.Count; i++)
                        {
                            string sqlString = sqlArray[i];
                            using (SqlCommand command = new SqlCommand(sqlString, connection, transaction))
                            {
                                command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                                //如果同时传入了参数，则添加这些参数
                                if (parametersArray!=null)
                                {
                                    if (parametersArray[i] != null)
                                    {
                                        foreach (SqlParameter parameter in parametersArray[i])
                                        {
                                            if (parameter.SqlValue == null)
                                            {
                                                parameter.SqlValue = DBNull.Value;
                                            }
                                            command.Parameters.Add(parameter);
                                        }
                                    }
                                }                              
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();   //事务提交
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); //事务回滚
                        return false;
                    }
                }
            }
            return result;//返回执行增删改操作之后，数据库中受影响的行数
        }

        /// <summary>
        /// 以事务的模式对数据库执行增删改操作
        /// </summary>
        /// <param name="sqlArray">要执行的SQL语句数组</param>
        /// <param name="commandType">要执行的查询语句的类型，如存储过程或者SQL文本命令</param>
        /// <returns></returns>
        public bool ExecuteNonQueryTransation(List<string> sqlArray, CommandType commandType)
        {
            bool result = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();//打开数据库连接
                //事务处理
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < sqlArray.Count; i++)
                        {
                            string sqlString = sqlArray[i];
                            using (SqlCommand command = new SqlCommand(sqlString, connection, transaction))
                            {
                                command.CommandType = commandType;//设置command的CommandType为指定的CommandType
                                command.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();   //事务提交
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); //事务回滚
                        return false;
                    }
                }
            }
            return result;//返回执行增删改操作之后，数据库中受影响的行数
        }

		#endregion

		#region 批量插入数据
		public static void BulkInsertData(DataTable dt, string tableName, string connName)
		{
			var connStr = DbConnectionManager.GetConnectionString(connName);

			SqlConnection conn = new SqlConnection(connStr);
			conn.Open();
			SqlTransaction tran = conn.BeginTransaction();
			SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity, tran);//创建SqlBulkCopy对象    
																								   // bulkCopy = new SqlBulkCopy(sqlConn, SqlBulkCopyOptions.KeepNulls, tran);   

			try
			{
				sqlBulkCopy.DestinationTableName = tableName;//目标数据库表名  
				sqlBulkCopy.BatchSize = 1000;//一次批量执行的数据量
				sqlBulkCopy.ColumnMappings.Clear();
				for (int j = 0; j < dt.Columns.Count; j++)
				{
					sqlBulkCopy.ColumnMappings.Add((string)dt.Columns[j].ColumnName, (string)dt.Columns[j].ColumnName);//添加要保存的列  
				}
				sqlBulkCopy.WriteToServer(dt);//将源表中的数据写入数据库中目标表中  
				tran.Commit();
			}
			catch (Exception ex)
			{
				tran.Rollback();
				throw new Exception(ex.ToString());
			}
			finally
			{
				sqlBulkCopy.Close();
				conn.Close();
				//GC.Collect();
			}
		}
		#endregion
	}
}
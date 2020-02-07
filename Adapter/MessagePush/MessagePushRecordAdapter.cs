using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.MessagePush;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter.Message
{
    /// <summary>
    /// 消息推送记录 Adapter
    /// </summary>
    public class MessagePushRecordAdapter : UpdatableAndLoadableAdapterBase<MessagePushRecordModel, MessagePushRecordCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly MessagePushRecordAdapter Instance = new MessagePushRecordAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }


        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public DataTable GetList(string name, string title, DateTime startTime, DateTime endTime, int pageIndex, int pageSize, string sourceType="1")
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
                          SELECT ROW_NUMBER() OVER (ORDER BY [TimingSend] DESC) AS [Sort],*
						
                          FROM  [office].[MessagePushRecord]

						  WHERE  Title like '%'+@Title+'%' and CreateName like '%'+@CreateName+'%' and  TimingSend BETWEEN @StartTime and @EndTime and SourceType=@SourceType
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Sort] BETWEEN @PageStart AND @PageEnd ";
            SqlParameter[] parameters = {
                new SqlParameter("@Title",SqlDbType.NVarChar,36), 
                new SqlParameter("@StartTime",SqlDbType.DateTime),
                new SqlParameter("@EndTime",SqlDbType.DateTime),
                new SqlParameter("@PageStart",SqlDbType.Int),
                new SqlParameter("@PageEnd",SqlDbType.Int),
                new SqlParameter("@CreateName",SqlDbType.NVarChar,36),
                new SqlParameter("@SourceType",SqlDbType.NVarChar,36),            };
            parameters[0].Value = title;
            parameters[1].Value = startTime;
            parameters[2].Value = endTime;
            parameters[3].Value = pageIndex * pageSize + 1;
            parameters[4].Value = (pageIndex + 1) * pageSize;
            parameters[5].Value =name;
            parameters[6].Value = sourceType;
            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);

        }

        /// <summary>
        /// 获取数据量
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public int GetCount(DateTime startTime, DateTime endTime, string title,string name, string sourceType="1")
        {
            var sql = @"  SELECT COUNT(*)
						
                          FROM  [office].[MessagePushRecord]

						  WHERE  Title like '%' + @Title + '%' and CreateName like '%' + @CreateName + '%' and TimingSend BETWEEN @startTime AND @endTime and SourceType=@SourceType";
            SqlParameter[] parameters = {
                new SqlParameter("@Title",SqlDbType.NVarChar,36),
                new SqlParameter("@startTime",SqlDbType.DateTime),
                new SqlParameter("@endTime",SqlDbType.DateTime),
                new SqlParameter("@CreateName",SqlDbType.NVarChar,36),
                 new SqlParameter("@SourceType",SqlDbType.NVarChar,36),
            };
            parameters[0].Value = title;
            parameters[1].Value = startTime;
            parameters[2].Value = endTime;
            parameters[3].Value = name;
            parameters[4].Value = sourceType;
            SqlDbHelper helper = new SqlDbHelper();
            return Convert.ToInt32(helper.ExecuteScalar(sql, CommandType.Text, parameters));
        }
    }
}
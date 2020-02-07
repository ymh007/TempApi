using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static Seagull2.YuanXin.AppApi.Models.TaskManage.TaskMessageModel;

namespace Seagull2.YuanXin.AppApi.Adapter.Task
{
	/// <summary>
	/// 任务消息——Adapter
	/// </summary>
	public class TaskMessageApapter : UpdatableAndLoadableAdapterBase<TaskMessageModel, TaskMessageCollection>
	{

		/// <summary>
		/// 实例
		/// </summary>
		public static readonly TaskMessageApapter Instance = new TaskMessageApapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;
		/// <summary>
		/// 获取消息列表
		/// </summary>
		/// <param name="pageIndex">页索引</param>
		/// <param name="pageSize">页大小</param>
		/// <param name="code">任务编码</param>
		/// <param name="type">消息类型0：全部；1：评论</param>
		/// <returns></returns>
		public List<TaskMessageModel> SelectMessageList(int pageIndex, int pageSize, string code, int type)
		{
			string whereSql = string.Empty;
			if (type == 1)
			{
				whereSql = @" AND [Type] = 1";
			}
			pageIndex--;
			var sql = string.Format(@"
            WITH [Temp] AS
                (
	                SELECT ROW_NUMBER() OVER( ORDER BY [CreateTime] DESC ) AS [Row], * FROM [office].[TaskMessage] 
                     WHERE [TaskCode]=@Code {0}
                )
                SELECT * FROM  [Temp] WHERE [Temp].[Row]  BETWEEN @PageIndex AND @PageSize", whereSql);
			SqlParameter[] parameters = {
				new SqlParameter("@Code",SqlDbType.NVarChar,36),
				new SqlParameter("@PageIndex",SqlDbType.NVarChar,36),
				new SqlParameter("@PageSize",SqlDbType.NVarChar,36),
			};
			parameters[0].Value = code;
			parameters[1].Value = pageSize * pageIndex + 1;
			parameters[2].Value = pageSize * pageIndex + pageSize;
			SqlDbHelper helper = new SqlDbHelper();
			var table = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
			var list = DataConvertHelper<TaskMessageModel>.ConvertToList(table);
			return list;
		}
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int SelectMessageList( string code, int type)
        {
            string whereSql = string.Empty;
            if (type == 1)
            {
                whereSql = @" AND [Type] = 1";
            }

            var sql = string.Format(@"
           
                
	                SELECT count(*) FROM [office].[TaskMessage] 
                     WHERE [TaskCode]=@Code {0}
               
                ;", whereSql);
            SqlParameter[] parameters = {
                new SqlParameter("@Code",SqlDbType.NVarChar,36),
            
            };
            parameters[0].Value = code;
           
            SqlDbHelper helper = new SqlDbHelper();
            var count = helper.ExecuteScalar(sql, CommandType.Text, parameters);
           
            return  (int )count;
        }

        /// <summary>
        /// 批量插入记录
        /// </summary>
        public void BatchInsert(TaskMessageCollection coll)
		{
			//构造DataTable
			var dt = new DataTable();
			dt.Columns.Add("Code", Type.GetType("System.String"));
			dt.Columns.Add("TaskCode", Type.GetType("System.String"));
			dt.Columns.Add("Content", Type.GetType("System.String"));
			dt.Columns.Add("Type", Type.GetType("System.Int32"));
			dt.Columns.Add("CreatorName", Type.GetType("System.String"));
			dt.Columns.Add("Creator", Type.GetType("System.String"));
			dt.Columns.Add("CreateTime", Type.GetType("System.DateTime"));
			dt.Columns.Add("ValidStatus", Type.GetType("System.Boolean"));

			coll.ForEach(m =>
			{
				DataRow dr = dt.NewRow();
				dr["Code"] = m.Code;
				dr["TaskCode"] = m.TaskCode;
				dr["Content"] = m.Content;
				dr["Type"] = m.Type;
				dr["CreatorName"] = m.CreatorName;
				dr["Creator"] = m.Creator;
				dr["CreateTime"] = m.CreateTime;
				dr["ValidStatus"] = m.ValidStatus;
				dt.Rows.Add(dr);
			});

			SqlDbHelper.BulkInsertData(dt, "[office].[TaskMessage]", GetConnectionName());
		}
	}
}
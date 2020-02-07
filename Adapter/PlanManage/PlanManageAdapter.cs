using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel.PlanManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.PlanManage
{
	/// <summary>
	/// 适配器
	/// </summary>
	public class PlanManageAdapter
	{
		/// <summary>
		/// 实例化
		/// </summary>
		public static readonly new PlanManageAdapter Instance = new PlanManageAdapter();


		/// <summary>
		/// Sql连接字符串
		/// </summary>
		/// <param name="connectionName"></param>
		/// <returns></returns>
		protected string GetSqlConnectionName(string connectionName)
		{
			return DbConnectionManager.GetConnectionString(connectionName);
		}
		public List<DataHelper> GetPointList(string planCode)
		{
			var connecString = Instance.GetSqlConnectionName("SubjectDB_PlanManage");
			var sql = "";
			if (string.IsNullOrEmpty(planCode))
			{
				sql = @"SELECT DISTINCT pp.CnName,
                        t.Code,
                        t.PlanCode,
                        t.TaskName,
                        t.ParentCode,
                        t.RowIndex,
                        t.IsKeyPath,
                        t.StartTime,
                        t.EndTime
                FROM SubjectDB.PlanManage.[ProjectPlanTask] t
                INNER JOIN SubjectDB.[PlanManage].[ProjectPlan] p
                    ON t.PlanCode = p.Code
                        AND t.PlanVersionStartTime = p.VersionStartTime
                        AND p.VersionEndTime is null
                        AND p.ValidStatus = '1'
                INNER JOIN SubjectDB.PlanManage.[Plan] AS pp
                    ON pp.Code = p.Code
                ORDER BY  pp.CnName,t.RowIndex ASC ";
			}
			else
			{
				sql = string.Format(@"SELECT DISTINCT
                       pp.CnName,
                        t.Code,
                        t.PlanCode,
                        t.TaskName,
                        t.ParentCode,
                        t.RowIndex,
                        t.IsKeyPath,
                        t.StartTime,
                        t.EndTime
                FROM SubjectDB.PlanManage.[ProjectPlanTask] t
                INNER JOIN SubjectDB.[PlanManage].[ProjectPlan] p
                    ON t.PlanCode = p.Code
                        AND t.PlanVersionStartTime = p.VersionStartTime
                        AND p.VersionEndTime is null
                        AND p.ValidStatus = '1'
                INNER JOIN SubjectDB.PlanManage.[Plan] AS pp
                    ON pp.Code = p.Code
                WHERE planCode = '{0}'
                ORDER BY  pp.CnName,t.RowIndex ASC ", planCode);
			}
			SqlDbHelper helper = new SqlDbHelper(connecString);
			var table = helper.ExecuteDataTable(sql, CommandType.Text);
			List<DataHelper> list = DataConvertHelper<DataHelper>.ConvertToList(table);
			return list;
		}
	}
}
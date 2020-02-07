using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static Seagull2.YuanXin.AppApi.Models.TaskManage.TaskModel;
using static Seagull2.YuanXin.AppApi.ViewsModel.TaskManage.TaskManageViewModel;

namespace Seagull2.YuanXin.AppApi.Adapter.Task
{
	/// <summary>
	/// 任务-Adapter
	/// </summary>
	public class TaskApapter : UpdatableAndLoadableAdapterBase<TaskModel, TaskCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly TaskApapter Instance = new TaskApapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

		/// <summary>
		/// 根据code获取主任务及子任务
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public TaskCollection GetTaskCollByCode(string code)
		{
			return Instance.Load(m =>
			{
				m.AppendItem("Code", code);
				m.AppendItem("ParentCode", code);
				m.LogicOperator = LogicOperatorDefine.Or;
			});
		}

		/// <summary>
		/// 根据code获取所有子任务
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public TaskCollection GetChildCollByCode(string code)
		{
			return Instance.Load(m =>
			{
				m.AppendItem("ParentCode", code);
			});
		}
		/// <summary>
		/// 通过code获取任务
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public TaskModel GetTaskInfoByCode(string code)
		{
			return Instance.Load(m =>
			{
				m.AppendItem("Code", code);
			}).FirstOrDefault();
		}

		/// <summary>
		/// 根据code删除任务
		/// </summary>
		/// <param name="code"></param>
		public void Delete(string code)
		{
			Instance.Delete(m =>
			{
				m.AppendItem("Code", code);
				m.AppendItem("ParentCode", code);
				m.LogicOperator = LogicOperatorDefine.Or;
			});
		}

		//根据类型 type=0未完成的，type=1已完成的，type=2我发起的，type=3我执行的，type=4，抄送我的
		/// <summary>
		/// 分页查询我发起我执行的
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// /// <param name="type"></param>
		/// /// <param name="id"></param>
		/// <returns></returns>
		public List<TaskListsViewModel> SelectMyTaskList(int pageIndex, int pageSize, string id, int type)
		{
			string whereSql = string.Empty;
			if (type == 0)//未完成的
			{
				whereSql = @" [task1].[CompletionState]= 0 AND( task1.[Creator] = @UserId or task1.[Executor]=@UserId) ";//我发起的
			}
			if (type == 1)//已完成的
			{
				whereSql = @" [task1].[CompletionState]= 1 AND( task1.[Creator] = @UserId or task1.[Executor]=@UserId) ";//我发起的
			}
			if (type == 2)//我发起的
			{
				whereSql = @" task1.[Creator] = @UserId ";
			}
			if (type == 3)//我执行的
			{
				whereSql = @"  task1.[Executor]=@UserId ";
			}
			if (type == 4)//抄送我的
			{
				whereSql = @" [task1].[Code] IN (SELECT [TaskCode] FROM [office].[TaskCopyPerson] WHERE [office].[TaskCopyPerson].[UserCode]= @UserId) ";//抄送我的
			}

			pageIndex--;
			var sql = string.Format(@"
           WITH [Temp] AS
              (
                  SELECT  ROW_NUMBER() OVER( ORDER BY  task1. [CompletionState], task1.[CreateTime]   DESC ) AS [Row],task1.*,IsNull(task2.[TitleContent], '') AS ParentTitle,task2.[CompletionState] AS ParentCompletionState,
                   (SELECT COUNT(*) FROM [office].[Task]   WHERE [ParentCode] = [task1].[Code]) AS [taskNumber],
		          (SELECT COUNT(*) FROM [office].[Task] WHERE [ParentCode] = [task1].[Code] AND [CompletionState] = 1) AS [taskCompleteNumber]
                  FROM [office].[Task] AS task1
                  LEFT  JOIN  office.[Task] AS task2
                  ON task1.[ParentCode] = task2.[Code]
                  WHERE  {0}
                  AND  [task1].[Code] NOT IN (SELECT [TaskCode] FROM [office].[TaskHide] WHERE [office].[TaskHide].[Creator]= @UserId)
               )
                SELECT * FROM  [Temp] WHERE [Temp].[Row]  BETWEEN @PageIndex AND @PageSize", whereSql);
			SqlParameter[] parameters = {
				new SqlParameter("@PageIndex",SqlDbType.NVarChar,36),
				new SqlParameter("@PageSize",SqlDbType.NVarChar,36),
				new SqlParameter("@UserId",SqlDbType.NVarChar,36),
			};
			parameters[0].Value = pageSize * pageIndex + 1;
			parameters[1].Value = pageSize * pageIndex + pageSize;
			parameters[2].Value = id;
			SqlDbHelper helper = new SqlDbHelper();
			var table = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
			var list = DataConvertHelper<TaskListsViewModel>.ConvertToList(table);
			return list;
		}


		/// <summary>
		/// 任务搜索（分页）
		/// </summary>
		/// <param name="pageIndex">页索引</param>
		/// <param name="pageSize">页大小</param>
		/// <param name="userId">用户id</param>
		/// <param name="searchText">搜索条件</param>
		/// <returns>List<TaskListsViewModel></returns>
		public List<TaskListsViewModel> TaskSearch(int pageIndex, int pageSize, string userId, string searchText)
		{
			pageIndex--;
			var sql = string.Format(@"
            WITH [Temp] AS 
            (
			SELECT  ROW_NUMBER() OVER( ORDER BY t.[CreateTime] DESC ) AS [Row], t.*,IsNull((SELECT TitleContent FROM office.Task WHERE code = t.ParentCode), '')ParentTitle,(SELECT COUNT(*) AS childCount FROM office.Task WHERE ParentCode = t.Code)taskNumber,(SELECT COUNT(*) AS ChildCount FROM office.Task WHERE ParentCode = t.Code AND CompletionState =1)TaskCompleteNumber,IsNull((SELECT CompletionState FROM office.Task WHERE code = t.ParentCode), '')ParentCompletionState
            FROM office.Task AS t
            WHERE (t.Executor = @UserId OR t.Creator =@UserId OR EXISTS(SELECT p.UserCode FROM office.TaskCopyPerson p WHERE t.Code = p.TaskCode AND p.UserCode = @UserId)) 
            AND (t.ExecutorName LIKE '%' + @serachText + '%' OR 
            t.CreatorName LIKE '%' + @serachText + '%' OR 
            t.TitleContent LIKE '%' + @serachText + '%' OR 
            EXISTS(SELECT p.UserCode FROM office.TaskCopyPerson p WHERE t.Code = p.TaskCode AND  p.UserName like '%' + @serachText + '%'))
            AND( NOT EXISTS(SELECT * FROM office.TaskHide WHERE Creator = @UserId AND TaskCode = t.Code))
			)
			 SELECT * FROM  [Temp] WHERE [Temp].[Row]  BETWEEN @PageIndex AND @PageSize;");
			SqlParameter[] parameters = {
				new SqlParameter("@PageIndex",SqlDbType.NVarChar,36),
				new SqlParameter("@PageSize",SqlDbType.NVarChar,36),
				new SqlParameter("@UserId",SqlDbType.NVarChar,36),
				new SqlParameter("@serachText",SqlDbType.NVarChar,36),
			};
			parameters[0].Value = pageSize * pageIndex + 1;
			parameters[1].Value = pageSize * pageIndex + pageSize;
			parameters[2].Value = userId;
			parameters[3].Value = searchText;
			SqlDbHelper helper = new SqlDbHelper();
			var table = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
			var list = DataConvertHelper<TaskListsViewModel>.ConvertToList(table);
			return list;
		}


        public TaskCollection GetDeadLine(string userCode,DateTime start,DateTime end)
        {
            string sql = "select t.Code,t.Creator,t.Deadline,t.TitleContent　from office.Task t where t.CompletionState = 0   and(t.Creator = '"+ userCode + "' or t.Executor = '"+ userCode + "') and t.Deadline between '2020-01-01' and '2020-01-31'  and(NOT EXISTS(SELECT TaskCode FROM office.TaskHide WHERE  Creator='"+ userCode + "' and  TaskCode = t.Code))";
           return  this.QueryData(sql);
        }


	}
}
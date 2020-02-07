using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;

namespace Seagull2.YuanXin.AppApi.Services
{
    /// <summary>
    /// 待办、流转中、已办结、传阅和通知
    /// </summary>
    public class UserTaskService : UpdatableAndLoadableAdapterBase<UserTask, UserTaskCollection>, IUserTaskService
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return "HB2008_SinoOcean";
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString("HB2008_SinoOcean");

        #region 加载待办、流转中、已办结、传阅和通知、收藏 - 总记录数
        /// <summary>
        /// 加载待办、流转中、已办结、传阅和通知、收藏 - 总记录数
        /// </summary>
        public Task<int> LoadUserTask(string userCode, string type, string keyword)
        {
            try
            {
                var helper = new SqlDbHelper(ConnectionString);
                var count = 0;

                // 待办
                if (type == "task")
                {
                    var sql = @"SELECT COUNT(0) FROM [dbo].[USER_TASK] 
                                WHERE 
	                                [TASK_TITLE] LIKE '%' + @Keyword + '%' AND
	                                [SEND_TO_USER] = @SEND_TO_USER AND
                                    [STATUS] = '1' AND
                                    ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()));";
                    sql = sql.Replace("' + @Keyword + '", keyword);
                    sql = sql.Replace("@SEND_TO_USER", "'" + userCode + "'");

                    var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
                    if (result != null && result != DBNull.Value)
                    {
                        int.TryParse(result.ToString(), out count);
                    }
                }

                // 流转中、已办结
                if (type == "running" || type == "completed")
                {
                    var sql = @"SELECT COUNT(0) FROM [dbo].[USER_ACCOMPLISHED_TASK]
                                WHERE
	                                [TASK_TITLE] LIKE '%' + @Keyword + '%' AND
                                    [SEND_TO_USER] = @SEND_TO_USER AND
                                    [PROCESS_STATUS] = @PROCESS_STATUS;";
                    sql = sql.Replace("' + @Keyword + '", keyword);
                    sql = sql.Replace("@SEND_TO_USER", "'" + userCode + "'");
                    sql = sql.Replace("@PROCESS_STATUS", "'" + type + "'");

                    var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
                    if (result != null && result != DBNull.Value)
                    {
                        int.TryParse(result.ToString(), out count);
                    }
                }

                // 传阅和通知
                if (type == "notice")
                {
                    var sql = @"SELECT COUNT(0) FROM [dbo].[USER_TASK] 
                                WHERE 
                                    [TASK_TITLE] LIKE '%' + @Keyword + '%' AND
	                                [SEND_TO_USER] = @SEND_TO_USER AND
                                    ([STATUS] = '2' OR [STATUS] = '4') AND
                                    ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()));";
                    sql = sql.Replace("' + @Keyword + '", keyword);
                    sql = sql.Replace("@SEND_TO_USER", "'" + userCode + "'");

                    var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
                    if (result != null && result != DBNull.Value)
                    {
                        int.TryParse(result.ToString(), out count);
                    }
                }

                // 收藏
                if (type == "collection")
                {
                    var sql = @"WITH [Temp] AS
                                (
	                                SELECT ROW_NUMBER() OVER(ORDER BY [NewTable].[DELIVER_TIME] DESC) AS [Row], * FROM
                                    (
		                                -- 待办 和 传阅
		                                SELECT [TASK_GUID], [PROGRAM_NAME], [TASK_TITLE], [RESOURCE_ID], [ACTIVITY_ID], [URL], [READ_TIME], [DELIVER_TIME], [COLLECTION] FROM [dbo].[USER_TASK] WITH(READPAST)
		                                WHERE 
			                                [SEND_TO_USER] = @UserCode AND
			                                [COLLECTION] = 1 AND
			                                ([STATUS] = '1' OR [STATUS] = '2' OR [STATUS] = '4') AND
			                                ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()))
		                                UNION ALL
		                                -- 流转中 和 已办结
		                                SELECT [TASK_GUID], [PROGRAM_NAME], [TASK_TITLE], [RESOURCE_ID], [ACTIVITY_ID], [URL], [READ_TIME], [DELIVER_TIME], [COLLECTION] FROM [dbo].[USER_ACCOMPLISHED_TASK] WITH(READPAST)
		                                WHERE 
			                                [SEND_TO_USER] = @UserCode AND
			                                [COLLECTION] = 1 AND
			                                ([PROCESS_STATUS] = 'Running' OR [PROCESS_STATUS] = 'Completed')
	                                ) AS [NewTable]
                                )
                                SELECT COUNT(*) FROM [Temp];";

                    SqlParameter[] parameters = { new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
                    parameters[0].Value = userCode;

                    var result = helper.ExecuteScalar(sql, CommandType.Text, parameters);
                    if (result != null && result != DBNull.Value)
                    {
                        int.TryParse(result.ToString(), out count);
                    }
                }

                return Task.FromResult(count);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 加载待办、流转中、已办结、传阅和通知、收藏 - 当前页数据
        /// <summary>
        /// 加载待办、流转中、已办结、传阅和通知、收藏 - 当前页数据
        /// </summary>
        public Task<IEnumerable<UserTaskModel>> LoadUserTask(int pageSize, int pageIndex, string userCode, string type, string keyword)
        {
            try
            {
                var helper = new SqlDbHelper(ConnectionString);
                var table = new DataTable();

                // 待办
                if (type == "task")
                {
                    var sql = @"WITH Temp AS
                                (
	                                SELECT ROW_NUMBER() OVER (ORDER BY [TOP_FLAG] DESC, [DELIVER_TIME] DESC) AS [Row], * FROM [dbo].[USER_TASK]
	                                WHERE 
                                        [TASK_TITLE] LIKE '%' + @Keyword + '%' AND
		                                [SEND_TO_USER] = @SEND_TO_USER AND
		                                [STATUS] = '1' AND
		                                ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()))
                                )
                                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
                    sql = sql.Replace("' + @Keyword + '", keyword);
                    sql = sql.Replace("@SEND_TO_USER", "'" + userCode + "'");
                    sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

                    table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
                }

                // 流转中、已办结
                if (type == "running" || type == "completed")
                {
                    var sql = @"WITH [Temp] AS
                                (
	                                SELECT ROW_NUMBER() OVER (ORDER BY [COMPLETED_TIME] DESC) AS [Row], * FROM [dbo].[USER_ACCOMPLISHED_TASK] (READPAST)
	                                WHERE 
                                        [TASK_TITLE] LIKE '%' + @Keyword + '%' AND
		                                [SEND_TO_USER] = @SEND_TO_USER AND 
		                                [PROCESS_STATUS] = @PROCESS_STATUS
                                )
                                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
                    sql = sql.Replace("' + @Keyword + '", keyword);
                    sql = sql.Replace("@SEND_TO_USER", "'" + userCode + "'");
                    sql = sql.Replace("@PROCESS_STATUS", "'" + type + "'");
                    sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

                    table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
                }

                // 传阅和通知
                if (type == "notice")
                {
                    var sql = @"WITH Temp AS
                                (
	                                SELECT ROW_NUMBER() OVER (ORDER BY [TOP_FLAG] DESC, [DELIVER_TIME] DESC) AS [Row], * FROM [dbo].[USER_TASK]
	                                WHERE 
                                        [TASK_TITLE] LIKE '%' + @Keyword + '%' AND
		                                [SEND_TO_USER] = @SEND_TO_USER AND
		                                ([STATUS] = '2' OR [STATUS] = '4') AND
		                                ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()))
                                )
                                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}";
                    sql = sql.Replace("' + @Keyword + '", keyword);
                    sql = sql.Replace("@SEND_TO_USER", "'" + userCode + "'");
                    sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

                    table = DbHelper.RunSqlReturnDS(sql, GetConnectionName()).Tables[0];
                }

                // 收藏
                if (type == "collection")
                {
                    var sql = @"WITH [Temp] AS
                                (
	                                SELECT ROW_NUMBER() OVER(ORDER BY [NewTable].[DELIVER_TIME] DESC) AS [Row], * FROM
                                    (
		                                -- 待办 和 传阅
		                                SELECT [TASK_GUID], [PROGRAM_NAME], [TASK_TITLE], [RESOURCE_ID], [ACTIVITY_ID], [URL], [READ_TIME], [DELIVER_TIME], [COLLECTION], [STATUS] AS [Type] FROM [dbo].[USER_TASK] WITH(READPAST)
		                                WHERE 
			                                [SEND_TO_USER] = @UserCode AND
			                                [COLLECTION] = 1 AND
			                                ([STATUS] = '1' OR [STATUS] = '2' OR [STATUS] = '4') AND
			                                ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()))
		                                UNION ALL
		                                -- 流转中 和 已办结
		                                SELECT [TASK_GUID], [PROGRAM_NAME], [TASK_TITLE], [RESOURCE_ID], [ACTIVITY_ID], [URL], [READ_TIME], [DELIVER_TIME], [COLLECTION], [PROCESS_STATUS] AS [Type] FROM [dbo].[USER_ACCOMPLISHED_TASK] WITH(READPAST)
		                                WHERE 
			                                [SEND_TO_USER] = @UserCode AND
			                                [COLLECTION] = 1 AND
			                                ([PROCESS_STATUS] = 'Running' OR [PROCESS_STATUS] = 'Completed')
	                                ) AS [NewTable]
                                )
                                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
                    sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

                    SqlParameter[] parameters = { new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
                    parameters[0].Value = userCode;

                    table = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
                }

                var list = Convert(table);

                return Task.FromResult<IEnumerable<UserTaskModel>>(list);
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw ex;
            }
        }
        #endregion

        #region 收藏/取消收藏
        /// <summary>
        /// 收藏/取消收藏
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="taskId">taskId</param>
        public async Task Collection(string userCode, string taskId)
        {
            var helper = new SqlDbHelper(ConnectionString);

            var sql = @"IF (EXISTS(SELECT * FROM [dbo].[USER_TASK] WHERE [TASK_GUID] = @taskId AND [SEND_TO_USER] = @userCode AND [COLLECTION] = 1))
	                        UPDATE [dbo].[USER_TASK] SET [COLLECTION] = 0 WHERE [TASK_GUID] = @taskId AND [SEND_TO_USER] = @userCode;
                        ELSE
	                        UPDATE [dbo].[USER_TASK] SET [COLLECTION] = 1 WHERE [TASK_GUID] = @taskId AND [SEND_TO_USER] = @userCode;
                        IF (EXISTS(SELECT * FROM [dbo].[USER_ACCOMPLISHED_TASK] WHERE [TASK_GUID] = @taskId AND [SEND_TO_USER] = @userCode AND [COLLECTION] = 1))
	                        UPDATE [dbo].[USER_ACCOMPLISHED_TASK] SET [COLLECTION] = 0 WHERE [TASK_GUID] = @taskId AND [SEND_TO_USER] = @userCode;
                        ELSE
	                        UPDATE [dbo].[USER_ACCOMPLISHED_TASK] SET [COLLECTION] = 1 WHERE [TASK_GUID] = @taskId AND [SEND_TO_USER] = @userCode;";

            SqlParameter[] parameters = { new SqlParameter("@userCode", SqlDbType.NVarChar, 36), new SqlParameter("@taskId", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;
            parameters[1].Value = taskId;

            helper.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }
        #endregion

        #region 获取流程待办列表
        /// <summary>
        /// 获取流程待办列表
        /// </summary>
        public List<UserTaskModel> GetUserTask(string resourceId, string processId)
        {
            var sql = @"
                SELECT * FROM [dbo].[USER_TASK]
                WHERE
	                [RESOURCE_ID] = '{0}' AND
                    [PROCESS_ID] = '{1}' AND
                    [STATUS] = '1' AND
                    ([EXPIRE_TIME] IS NULL OR ([EXPIRE_TIME] IS NOT NULL AND [EXPIRE_TIME] > GETDATE()));";
            sql = string.Format(sql, resourceId, processId);

            var helper = new SqlDbHelper(ConnectionString);
            var dt = helper.ExecuteDataTable(sql);
            var list = Convert(dt);
            return list;
        }
        #endregion

        #region 更新读待办取时间
        /// <summary>
        /// 更新读待办取时间
        /// </summary>
        public async Task<string> SetTaskReadFlag(string taskID)
        {
            string IsResult = "";
            try
            {
                string sql = "UPDATE dbo.USER_TASK SET READ_TIME = GETDATE() WHERE TASK_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(taskID, true);

                DbHelper.RunSql(sql, GetConnectionName());
                IsResult = "Yes";
                return IsResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 更新读流转中和已办结时间
        /// <summary>
        /// 更新读流转中和已办结时间
        /// </summary>
        public async Task<string> SetAccomplihd(string taskID)
        {
            string IsResult = "";
            try
            {
                string sql = "UPDATE dbo.USER_ACCOMPLISHED_TASK  SET READ_TIME = GETDATE() WHERE TASK_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(taskID, true);

                DbHelper.RunSql(sql, GetConnectionName());
                IsResult = "Yes";
                return IsResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 获取待办详情
        /// <summary>
        /// 获取待办详情
        /// </summary>
        public UserTaskModel GetDetail(string taskId)
        {
            var sql = @"SELECT * FROM [dbo].[USER_TASK] WHERE [TASK_GUID] = @taskId;";

            SqlParameter[] parameters = { new SqlParameter("@taskId", SqlDbType.NVarChar, 36) };
            parameters[0].Value = taskId;

            var helper = new SqlDbHelper(ConnectionString);
            var dt = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
            if (dt.Rows.Count < 1)
            {
                return null;
            }
            var list = Convert(dt);
            return list.FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// 数据转换
        /// </summary>
        List<UserTaskModel> Convert(DataTable dt)
        {
            var urlMappingNG = ConfigurationManager.AppSettings["UrlMappingNG"];
            var listUserTaskEx = new List<UserTaskModel>();
            foreach (DataRow row in dt.Rows)
            {
                var item = new UserTaskModel();
                ORMapping.DataRowToObject(row, item);

                // 屏蔽笔记本设备待办
                if (item.Url.ToLower().Contains("OfficeSpace/default.htm#/ComputerApplyPrepare".ToLower()))
                {
                    continue;
                }

                item.Url = item.Url.Replace(" ", "");
                if (item.Url.ToLower().Contains("thrwebapp"))
                {
                    item.Url = urlMappingNG + item.Url;
                    item.IsArchitecture = true;
                    item.Enabled = true;
                }
                listUserTaskEx.Add(item);
            }

            var convertList = UrlConvertHelper.Convert(listUserTaskEx);

            var userTaskExCollection = new List<UserTaskModel>();

            foreach (var item in convertList)
            {
                var userTaksEx = new UserTaskModel();
                userTaksEx.TaskID = item.TaskID;
                userTaksEx.ProgramName = item.ProgramName;
                userTaksEx.TaskTitle = item.TaskTitle;
                userTaksEx.ResourceID = item.ResourceID;
                userTaksEx.ProcessID = item.ProcessID;
                userTaksEx.ActivityID = item.ActivityID;
                userTaksEx.Url = item.Url;
                userTaksEx.SendToUser = item.SendToUser;
                userTaksEx.ReadTime = item.ReadTime;
                userTaksEx.DeliverTime = item.DeliverTime;
                userTaksEx.CompletedTime = item.CompletedTime;
                userTaksEx.Collection = item.Collection;
                userTaksEx.Type = item.Type;
                if (item.Url.ToLower().Contains("thrwebapp"))
                {
                    userTaksEx.IsArchitecture = true;
                }
                else
                {
                    userTaksEx.IsArchitecture = false;
                }
                userTaksEx.Enabled = item.Enabled;
                userTaskExCollection.Add(userTaksEx);
            }

            return userTaskExCollection;
        }
    }
}
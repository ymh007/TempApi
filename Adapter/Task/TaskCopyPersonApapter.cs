using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static Seagull2.YuanXin.AppApi.Models.TaskManage.TaskCopyPersonModel;

namespace Seagull2.YuanXin.AppApi.Adapter.Task
{
    /// <summary>
    ///任务抄送人—Adapter 
    /// </summary>
    public class TaskCopyPersonApapter : UpdatableAndLoadableAdapterBase<TaskCopyPersonModel, TaskCopyPersonCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly TaskCopyPersonApapter Instance = new TaskCopyPersonApapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 批量插入抄送人员
        /// </summary>
        public void TaskCopyPersonInsert(TaskCopyPersonCollection coll)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", Type.GetType("System.String"));
            dt.Columns.Add("TaskCode", Type.GetType("System.String"));
            dt.Columns.Add("UserCode", Type.GetType("System.String"));
            dt.Columns.Add("UserName", Type.GetType("System.String"));
            dt.Columns.Add("Creator", Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("Modifier", Type.GetType("System.String"));
            dt.Columns.Add("ModifyTime", Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", Type.GetType("System.Boolean"));

            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["TaskCode"] = m.TaskCode;
                dr["UserCode"] = m.UserCode;
                dr["UserName"] = m.UserName;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["Modifier"] = m.Modifier;
                dr["ModifyTime"] = m.ModifyTime;
                dr["ValidStatus"] = m.ValidStatus;

                dt.Rows.Add(dr);
            });

            SqlDbHelper.BulkInsertData(dt, "[office].[TaskCopyPerson]", GetConnectionName());
        }

        /// <summary>
        /// 根据taskCode查询抄送人员
        /// </summary>
        /// <param name="taskCode">任务编码</param>
        /// <returns></returns>
        public TaskCopyPersonCollection Select(string taskCode)
        {
            return Instance.Load(m => m.AppendItem("TaskCode", taskCode));
        }

        /// <summary>
        /// 根据code删除任务的抄送人员
        /// </summary>
        /// <param name="coll"></param>
        public void Delete(List<string> coll)
        {
            InSqlClauseBuilder inSql = new InSqlClauseBuilder();
            inSql.AppendItem(coll.ToArray());
            if (coll.Count > 0)
            {
                Instance.Delete(where => where.AppendItem("TaskCode", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true));
            }
        }

		/// <summary>
		/// 获取抄送人员信息
		/// </summary>
		/// <returns></returns>
		public List<TaskCopyPersonModel> GetPersonList(List<string> list)
		{
			InSqlClauseBuilder inSqls = new InSqlClauseBuilder();
			inSqls.AppendItem(list.ToArray());
			var wheres = new WhereSqlClauseBuilder();
			wheres.AppendItem("TaskCode", inSqls.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
			var whereSqls = wheres.ToSqlString(TSqlBuilder.Instance);
			var sql = new StringBuilder();
			sql.Append(string.Format(@"SELECT * FROM [office].[TaskCopyPerson] 
                         Where {0}", whereSqls));
			var table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
			return DataConvertHelper<TaskCopyPersonModel>.ConvertToList(table);
		}
	}
}
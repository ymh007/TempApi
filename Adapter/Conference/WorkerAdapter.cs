using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 工作人员数据适配器
    /// </summary>
    public class WorkerAdapter : BaseAdapter<WorkerModel, WorkerCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static WorkerAdapter Instance = new WorkerAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 构造
        /// </summary>
        public WorkerAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        /// <summary>
        /// 根据编码获取工作人员详情
        /// </summary>
        public WorkerModel LoadByID(string id)
        {
            return Load(p => { p.AppendItem("ID", id, "="); }).FirstOrDefault();
        }

        /// <summary>
        /// 根据会议编码获取工作人员列表
        /// </summary>
        public WorkerCollection LoadByConferenceID(string conferenceID)
        {
            return Load(
                w =>
                {
                    w.AppendItem("ConferenceID", conferenceID);
                },
                o =>
                {
                    o.AppendItem("Sort", FieldSortDirection.Ascending);
                }
            );
        }

        /// <summary>
        /// 分页查询工作人员列表
        /// </summary>
        public ViewPageBase<WorkerCollection> LoadByConferenceIDByPage(int pageIndex, string conferenceID)
        {
            string selectSQL = "SELECT *";
            string fromAndWhereSQL = string.Format(@"from Office.Worker where ConferenceID='{0}'", conferenceID);
            string orderSQL = "ORDER BY [Sort] ASC";
            ViewPageBase<WorkerCollection> pageData = GetTListByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            return pageData;
        }

        /// <summary>
        /// 更新工作人员集合
        /// </summary>
        /// <param name="data">工作人员集合</param>
        /// <param name="conferenceID">会议ID</param>
        public bool Update(WorkerCollection data, string conferenceID)
        {
            var mapping = ORMapping.GetMappingInfo<WorkerModel>();
            WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();
            builder.AppendItem("ConferenceID", conferenceID);
            StringBuilder strB = new StringBuilder(200);
            strB.Append(string.Format("DELETE FROM {0} WHERE {1}", mapping.TableName, builder.ToSqlString(TSqlBuilder.Instance)));
            foreach (var item in data)
            {
                strB.Append(TSqlBuilder.Instance.DBStatementSeperator);
                strB.Append(ORMapping.GetInsertSql(item, TSqlBuilder.Instance, ""));
            }
            return DbHelper.RunSqlWithTransaction(strB.ToString(), GetConnectionName()) >= 1 ? true : false;
        }

        /// <summary>
        /// 根据ID删除
        /// </summary>
        public void DelWorker(string id)
        {
            Delete(m => m.AppendItem("ID", id));
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="modelColl"></param>
        public void AddWorkerModelCollection(WorkerCollection modelColl)
        {
            StringBuilder sql = new StringBuilder();
            modelColl.ForEach(model =>
            {
                sql.Append(ORMapping.GetInsertSql<WorkerModel>(model, TSqlBuilder.Instance, "") + ";");
            });
            ViewBaseAdapter<object, List<object>>.Instance.RunSQLByTransaction(sql.ToString(), ConnectionNameDefine.YuanXinForDBHelp);
        }

        /// <summary>
        /// 根据会议id和用户id查询该工作人员是否存在
        /// </summary>
        public bool IsNullIndoById(string conferenceId, string userId)
        {
            try
            {
                WorkerModel model = this.Load(p =>
                {
                    p.AppendItem("UserID", userId);
                    p.AppendItem("ConferenceID", conferenceId);
                }).FirstOrDefault();
                if (model != null) return true;
                return false;
            }
            catch
            {
                throw new Exception("用户id查询该工作人员是否存在失败");
            }
        }
    }
}
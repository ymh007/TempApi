using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MobileBusiness.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.Controllers;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class OnlineProjectViewDataQuery : ObjectDataSourceQueryAdapterBase<OnlineProjectViewData, OnlineProjectViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"*";
            qc.FromClause = @"(
                             select *,
                             case when getdate() >= StartTime and getdate() <= EndTime then 1
                             when getdate() < StartTime  then 2
                             when getdate() > EndTime AND getdate() <= EnrollDeadline then 3 
                             when getdate() > EnrollDeadline  then 4
                             end state from zc.Project) ss";

            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = "ModifyTime DESC";//默认排序
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }
        public string ProjectCode { get; set; }
        public string ProjectType { get; set; }
        public string Time { get; set; }

        public string Title { get; set; }

        private string _auditStatus = string.Empty;
        /// <summary>
        /// 审核状态
        /// </summary>
        public string AuditStatus
        {
            get { return _auditStatus; }
            set { _auditStatus = value; }
        }
        public string Creator { get; set; }

        public string City { get; set; }

        public string ProjectState { get; set; }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (!this.Title.IsNullOrEmpty() && this.Title != "null")
            {
                where.AppendItem("Name", "%" + this.Title + "%", "LIKE");
            }

            if (!this.AuditStatus.IsNullOrEmpty())
            {

                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(this.AuditStatus.Split(','));
                where.AppendItem("AuditStatus", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);

            }
            if (!this.City.IsNullOrEmpty())
            {
                where.AppendItem("City", "%" + this.City + "%", "LIKE");
                //where.AppendItem("City", this.City);
            }
            if (!this.Creator.IsNullOrEmpty())
            {
                where.AppendItem("Creator", this.Creator);
            }
            if (!this.ProjectType.IsNullOrEmpty())
            {
                where.AppendItem("Type", this.ProjectType);
            }
            if (!this.Time.IsNullOrEmpty())
            {
                where.AppendItem("ModifyTime", this.Time, ">");
            }

            if (!this.ProjectState.IsNullOrEmpty())
            {
                if (this.ProjectState == Enums.ProjectState.Soon.ToString("D"))
                {
                    where.AppendItem("StartTime", DateTime.Now, ">");
                }
                else if (this.ProjectState == Enums.ProjectState.Voting.ToString("D"))
                {
                    where.AppendItem("StartTime", DateTime.Now, "<=");
                    where.AppendItem("EndTime", DateTime.Now, ">=");
                }
                else if (this.ProjectState == Enums.ProjectState.Collecting.ToString("D"))
                {
                    where.AppendItem("EndTime", DateTime.Now, "<");
                }
                else if (this.ProjectState == Enums.ProjectState.ActivityEnd.ToString("D"))
                {
                    where.AppendItem("EnrollDeadline", DateTime.Now, "<");
                }
            }
            where.AppendItem("IsValid", true);

            where.AppendItem("IssueRange", ((int)ActivityController.ISSUE_RANGE.staff).ToString());

            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

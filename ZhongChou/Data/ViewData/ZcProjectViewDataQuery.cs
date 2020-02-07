using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class ZcProjectViewDataQuery : ObjectDataSourceQueryAdapterBase<ZcProjectViewData, ZcProjectViewDataCollection>
    {
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"* ";
            qc.FromClause = @"(select  * ,case when getdate() >= StartTime and getdate() <= EndTime then 1 when getdate() < StartTime  then 2 when getdate() > EndTime  then 3 end [state] from  zc.Project) aa";
            qc.WhereClause = this.ToWhere();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }
        private string _orderByClause = "CreateTime DESC";
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }
        public string Type { get; set; }

        public string City { get; set; }

        private string _auditStatus = string.Empty;
        /// <summary>
        /// 审核状态
        /// </summary>
        public string AuditStatus
        {
            get { return _auditStatus; }
            set { _auditStatus = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (!string.IsNullOrEmpty(this.Type))
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(this.Type.Split(','));
                where.AppendItem("Type", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            }
            if (!string.IsNullOrEmpty(this.AuditStatus))
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(this.AuditStatus.Split(','));
                where.AppendItem("AuditStatus", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);

            }
            if (!string.IsNullOrEmpty(this.City) && this.City != "-1")
            {
                where.AppendItem("City", "%" + this.City + "%", "Like");
            }
            where.AppendItem("IsValid", true);
            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

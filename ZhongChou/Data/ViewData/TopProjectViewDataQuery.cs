using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class TopProjectViewDataQuery : ObjectDataSourceQueryAdapterBase<TopProjectViewData, TopProjectViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"p.*,
                tp.StartTime AS TopStartTime, tp.EndTime AS TopEndTime";
            qc.FromClause = @"zc.TopProject tp 
                    INNER JOIN zc.Project p ON tp.ProjectCode = p.Code";
            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = string.Empty;
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }
        public string ProjectCode { get; set; }

        /// <summary>
        /// 置顶结束时间
        /// </summary>
        public DateTime TopEndTime { get; set; }
        public string ProjectType { get; set; }

        public string City { get; set; }

        public string TopBoardCode { get; set; }
        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (!this.ProjectType.IsNullOrEmpty())
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(this.ProjectType.Split(','));
                where.AppendItem("p.Type", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            }

            if (!this.City.IsNullOrEmpty())
            {
                where.AppendItem("p.City", "%" + this.City + "%", "LIKE");
            }

            if (!this.TopBoardCode.IsNullOrEmpty())
            {
                where.AppendItem("tp.TopBoardCode", this.TopBoardCode);
            }
            where.AppendItem("tp.EndTime", DateTime.Now, ">=");
            where.AppendItem("tp.StartTime", DateTime.Now, "<=");

            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

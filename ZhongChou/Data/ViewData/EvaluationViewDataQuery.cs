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
    public class EvaluationViewDataDataQuery : ObjectDataSourceQueryAdapterBase<EvaluationViewData, EvaluationViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @" zc.UserEvaluation.*,zc.Project.Name as ProjectName ";
            qc.FromClause = @" zc.Project
                inner join zc.[Order] on zc.Project.Code=zc.[Order].ProjectCode 
                inner join zc.[UserEvaluation] on zc.UserEvaluation.OrderCode=zc.[Order].Code 
                            ";
            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = "zc.UserEvaluation.CreateTime desc";
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }
        public string ProjectCode { get; set; }
        public string CreatorCode { get; set; }
        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();
            if (this.ProjectCode.IsNotEmpty())
            {
                where.AppendItem("zc.[Project].Code", this.ProjectCode);
            }
            if (this.CreatorCode.IsNotEmpty())
            {
                where.AppendItem("zc.[Project].Creator", this.CreatorCode);
            }
            //where.AppendItem("zc.[Order].type", OrderType.Anchang.GetHashCode());
            where.AppendItem("zc.UserEvaluation.ParentID", "");
            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

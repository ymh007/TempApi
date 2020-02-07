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
    public class CommentViewDataDataQuery : ObjectDataSourceQueryAdapterBase<CommentViewData, CommentViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"*";
            qc.FromClause = @"zc.UserComment";
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
        public string WorksCode { get; set; }
        public string ToWhere()
        {
            ConnectiveSqlClauseCollection connect = new ConnectiveSqlClauseCollection();
            var where = new WhereSqlClauseBuilder();
            var andWhere = new WhereSqlClauseBuilder(LogicOperatorDefine.And);
            var orWhere = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);
            var orWhere1 = new WhereSqlClauseBuilder(LogicOperatorDefine.Or);
            if (this.ProjectCode.IsNotEmpty())
            {
                andWhere.AppendItem("ProjectCode", this.ProjectCode);
            }
            //
            if (this.WorksCode.IsNotEmpty())
            {
                andWhere.AppendItem("WorksCode", this.WorksCode);
            }
            else
            {
                orWhere.AppendItem("WorksCode","NULL","is",true);
                orWhere.AppendItem("WorksCode", "");
            }
            orWhere1.AppendItem("ParentCode", "NULL", "is", true);
            orWhere1.AppendItem("ParentCode", "");
            connect.Add(andWhere);
            connect.Add(orWhere);
            connect.Add(orWhere1);
            return connect.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

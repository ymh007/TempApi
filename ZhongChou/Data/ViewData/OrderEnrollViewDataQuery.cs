using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class OrderEnrollViewDataQuery : ObjectDataSourceQueryAdapterBase<OrderEnrollViewData, OrderEnrollViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"o.*,zc.ActivityEvent.StartTime,zc.ActivityEvent.EndTime";
            qc.FromClause = @"zc.[Order] o left join zc.ActivityEvent on o.SubProjectCode=zc.ActivityEvent.Code";
            //left join zc.OrderAddress on o.Code=zc.OrderAddress.OrderCode
            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = "o.CreateTime DESC";
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        public string ProjectCode { get; set; }
        public OrderType Type { get; set; }
        public bool IsValid { get; set; }

        /// <summary>
        /// 如果多个状态，示例：1,2,3
        /// </summary>
        public string OrderStatus { get; set; }

        public string UserID { get; set; }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (this.ProjectCode.IsNotEmpty())
            {
                where.AppendItem("o.ProjectCode", this.ProjectCode);
            }
            if (IsValid)
            {
                where.AppendItem("o.IsValid", true);
            }
            if(this.UserID.IsNotEmpty())
            {
                where.AppendItem("o.Creator", this.UserID);
            }
            where.AppendItem("o.Type", Convert.ToInt32(Type));
            if (this.OrderStatus.IsNotEmpty())
            {
                InSqlClauseBuilder inSql = new InSqlClauseBuilder();
                inSql.AppendItem(this.OrderStatus.Split(','));
                where.AppendItem("o.Status", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            }
            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

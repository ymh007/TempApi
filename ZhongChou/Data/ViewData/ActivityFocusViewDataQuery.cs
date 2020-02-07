using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MobileBusiness.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class ActivityFocusViewDataQuery : ObjectDataSourceQueryAdapterBase<ActivityFocusViewData, ActivityFocusViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"*";
            qc.FromClause = @"zc.UserFocus";

            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = "CreateTime DESC";//默认排序
        /// <summary>
        /// 
        /// </summary>
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        /// <summary>
        /// 关注人
        /// </summary>
        public string FocusCreator { get; set; }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();
            var typeWhere = new WhereSqlClauseBuilder();
            typeWhere.LogicOperator = LogicOperatorDefine.Or;
            if (this.FocusCreator.IsNotEmpty())
            {
                where.AppendItem("Creator", this.FocusCreator);
            }
            typeWhere.AppendItem("Type", UserFocusType.Online.GetHashCode());
            typeWhere.AppendItem("Type", UserFocusType.Anchang.GetHashCode());
            where.AppendItem("(" + typeWhere.ToSqlString(TSqlBuilder.Instance) + ")", "", "",true);
            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

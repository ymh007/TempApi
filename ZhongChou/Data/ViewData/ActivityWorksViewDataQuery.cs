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
    /// <summary>
    /// 
    /// </summary>
    public class ActivityWorksViewDataDataQuery : ObjectDataSourceQueryAdapterBase<ActivityWorksViewData, ActivityWorksViewDataCollection>
    {
        #region 基类方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qc"></param>
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"*";
            qc.FromClause = @"zc.ActivityWorks";
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

        #endregion

        private string _orderByClause = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();
            if (this.ProjectCode.IsNotEmpty() && this.ProjectCode != "null")
            {
                where.AppendItem("ProjectCode", this.ProjectCode);
            }
            if (this.Title.IsNotEmpty())
            {
                where.AppendItem("Content", this.Title);
            }

            if (this.UserCode.IsNotEmpty())
            {
                where.AppendItem("Creator", this.UserCode);
            }

            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

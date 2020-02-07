using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class WorksAwardsViewDataQuery : ObjectDataSourceQueryAdapterBase<WorksAwardsViewData, WorksAwardsViewDataCollection>
    {
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @" s.*,a.Code ActivityWorksCode,a.HaveImage,a.Creator UserCode,a.CreateTime,a.Content";
            qc.FromClause = @"zc.WorksAwards w

                            INNER JOIN zc.ActivityWorks a

                            ON w.ActivityWorksCode =a.Code

                            INNER JOIN zc.AwardsSetting s

                            ON w.AwardsSettingCode=s.Code";

            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        private string _orderByClause = "s.SortNo";//默认排序
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        public string ProjectCode { get; set; }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (!string.IsNullOrEmpty(ProjectCode))
            {
                where.AppendItem("s.ProjectCode", this.ProjectCode);
            }
          
            return where.ToSqlString(TSqlBuilder.Instance);
        }

    }
}

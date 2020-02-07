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
using Seagull2.YuanXin.AppApi.Controllers;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 
    /// </summary>
    public class CaseProjecttViewDataQuery : ObjectDataSourceQueryAdapterBase<CaseProjectViewData, CaseProjectViewDataCollection>
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
            qc.FromClause = this.FromClause;
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

        private string _orderByClause = "[state] ASC,ModifyTime DESC";//默认排序
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
        public string ProjectCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProjectType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProjectState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProjectTagCode { get; set; }

        /// <summary>
        /// 当筛选所有标签的时候用左连接，会出现重复数据
        /// </summary>
        public string FromClause
        {
            get
            {
                if (!this.ProjectTagCode.IsNullOrEmpty() && this.ProjectTagCode != "0")
                {
                    return @"(select p.*,t.TagCode,t.TagName,
                                case when type = '7' then 
                                 case when getdate() >= StartTime and getdate() <= EndTime then 1
	                                  when getdate() > EndTime AND getdate() <= EnrollDeadline  then 1 
	                                  when getdate() < StartTime  then 2
	                                  when getdate() > EnrollDeadline  then 3 end
                                else
                                 case when getdate() >= StartTime and getdate() <= EndTime then 1
	                                  when getdate() < StartTime then 2
	                                  when getdate() > EndTime then 3 end
                                end [state] 
                               from zc.Project p
                               left join zc.ProjectTag t 
                               on p.Code = t.ProjectCode
                              ) aa";
                }
                else
                {
                    return @"(select p.*,
                                case when type = '7' then 
                                 case when getdate() >= StartTime and getdate() <= EndTime then 1
	                                  when getdate() > EndTime AND getdate() <= EnrollDeadline  then 1 
	                                  when getdate() < StartTime  then 2
	                                  when getdate() > EnrollDeadline  then 3 end
                                else
                                 case when getdate() >= StartTime and getdate() <= EndTime then 1
	                                  when getdate() < StartTime then 2
	                                  when getdate() > EndTime then 3 end
                                end [state] 
                               from zc.Project p
                              ) aa";
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

            if (!this.ProjectTagCode.IsNullOrEmpty() && this.ProjectTagCode != "0")
            {
                where.AppendItem("TagCode", this.ProjectTagCode);
                //where.AppendItem("City", this.City);
            }
            if (!this.Creator.IsNullOrEmpty())
            {
                where.AppendItem("Creator", this.Creator);
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
                else if (ProjectType.Equals(Enums.ProjectTypeEnum.Anchang.ToString("D")))
                {
                    if (this.ProjectState == Enums.ProjectState.Enrolling.ToString("D"))
                    {
                        where.AppendItem("StartTime", DateTime.Now, "<=");
                        where.AppendItem("EndTime", DateTime.Now, ">=");
                    }
                    else if (this.ProjectState == Enums.ProjectState.EnrollEnd.ToString("D"))
                    {
                        where.AppendItem("EndTime", DateTime.Now, "<");
                    }
                }
                else if (ProjectType.Equals(Enums.ProjectTypeEnum.Online.ToString("D")))
                {
                    if (this.ProjectState == Enums.ProjectState.Collecting.ToString("D"))
                    {
                        where.AppendItem("StartTime", DateTime.Now, "<=");
                        where.AppendItem("EndTime", DateTime.Now, ">=");
                    }
                    else if (this.ProjectState == Enums.ProjectState.Voting.ToString("D"))
                    {
                        where.AppendItem("EndTime", DateTime.Now, "<");
                        where.AppendItem("EnrollDeadline", DateTime.Now, ">=");
                    }
                    else if (this.ProjectState == Enums.ProjectState.ActivityEnd.ToString("D"))
                    {
                        where.AppendItem("EnrollDeadline", DateTime.Now, "<");
                    }
                }
            }
            where.AppendItem("StartTime", "", "<>");
            where.AppendItem("[state]", "", "<>");
            where.AppendItem("EndTime", "", "<>");
            where.AppendItem("IsValid", true);

            var sql = where.ToSqlString(TSqlBuilder.Instance);

            if (!this.City.IsNullOrEmpty() && this.City != "-1")
            {
                this.City = this.City.Replace("市", "");
                sql += " AND CityCode='" + this.City + "' ";
            }
            if (!this.ProjectType.IsNullOrEmpty())
            {
                if (this.ProjectType == "6")
                {
                    //案场活动且发布范围为移动办公
                    sql += " AND (Type='" + (int)ProjectTypeEnum.Anchang + "' AND IssueRange=" + (int)ActivityController.ISSUE_RANGE.staff + ")";
                }
                else if (this.ProjectType == "7")
                {
                    //案场活动且发布范围为移动办公
                    sql += " AND (Type='" + (int)ProjectTypeEnum.Anchang + "' AND IssueRange=" + (int)ActivityController.ISSUE_RANGE.staff + ")";
                }
                else
                {
                    sql += " AND ((Type='" + (int)ProjectTypeEnum.Anchang + "' OR Type='" + (int)ProjectTypeEnum.Online + "') AND IssueRange=" + (int)ActivityController.ISSUE_RANGE.staff + ") ";
                }
            }

            return sql;
        }
    }
}

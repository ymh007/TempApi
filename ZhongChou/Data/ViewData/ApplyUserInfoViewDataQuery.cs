using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class ApplyUserInfoViewDataQuery : ObjectDataSourceQueryAdapterBase<ApplyUserInfoViewData, ApplyUserInfoViewDataCollection>
    {

        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"DISTINCT u.Nickname NickName,
	                                        u.HeadImage HeadImage,
	                                        e.StartTime EventStartTime,
	                                        o.CreateTime ApplyTime,
                                            o.GoodsCount ";
            qc.FromClause = @"zc.[Order] o 
	                                        INNER JOIN zc.UserInfo u ON u.Code=o.Creator
	                                        INNER JOIN zc.ActivityEvent e ON e.Code=o.SubProjectCode";
            qc.WhereClause = this.ToWhere();
            qc.GroupBy = @"
	                                        u.Nickname ,
	                                        u.HeadImage ,
	                                        e.StartTime ,
	                                        o.CreateTime,
                                            o.GoodsCount";
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

        private string _orderByClause = "o.CreateTime DESC";//默认排序
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
        /// <returns></returns>
        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();
            if (!this.ProjectCode.IsNullOrEmpty())
            {
                where.AppendItem("o.ProjectCode", this.ProjectCode.Trim());
            }
            string strStatus = "21,22,23";
            InSqlClauseBuilder inSql = new InSqlClauseBuilder();
            inSql.AppendItem(strStatus.Split(','));
            where.AppendItem("o.Status", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
            return where.ToSqlString(TSqlBuilder.Instance);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ApplyUserInfoViewDataCollection GetApplyUserInfo8(string projectCode, int count)
        {
            ApplyUserInfoViewDataCollection coll = new ApplyUserInfoViewDataCollection();

            string strSql = string.Format(@"SELECT DISTINCT TOP {1} 
                                            (SELECT COUNT(Code) FROM zc.[Order] WHERE (Status='21' or Status='22' or Status='23') 
                                            AND ProjectCode='{0}') ApplyUserCount ,
                                            o.Creator UserID,
	                                        e.StartTime EventStartTime,
	                                        o.CreateTime ApplyTime,
                                            o.GoodsCount 
	                                        FROM zc.[Order] o 
	                                        INNER JOIN zc.ActivityEvent e ON e.Code=o.SubProjectCode
	                                        WHERE (o.Status='{2}' or o.Status='{3}' or Status='{4}') AND o.ProjectCode='{0}'  ORDER BY o.CreateTime DESC ", projectCode, count, OrderStatus.Anchang_Enrolled.GetHashCode(), OrderStatus.Anchang_Signed.GetHashCode(), OrderStatus.Anchang_Evaluated.GetHashCode());

            DataView dv = DbHelper.RunSqlReturnDS(strSql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(coll, dv);

            return coll;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public ApplyUserInfoViewDataCollection GetApplyUserInfo(string projectCode)
        {
            ApplyUserInfoViewDataCollection coll = new ApplyUserInfoViewDataCollection();

            string strSql = string.Format(@"SELECT DISTINCT
                                            (SELECT COUNT(Code) FROM zc.[Order] WHERE (Status='21' or Status='22' or Status='23') 
                                            AND ProjectCode='{0}') ApplyUserCount ,
                                            o.Creator UserID,
	                                        e.StartTime EventStartTime,
	                                        o.CreateTime ApplyTime,
                                            o.GoodsCount 
	                                        FROM zc.[Order] o 
	                                        INNER JOIN zc.ActivityEvent e ON e.Code=o.SubProjectCode
	                                        WHERE (o.Status='{1}' or o.Status='{2}' or Status='{3}') AND o.ProjectCode='{0}'  ORDER BY o.CreateTime DESC ", projectCode, OrderStatus.Anchang_Enrolled.GetHashCode(), OrderStatus.Anchang_Signed.GetHashCode(), OrderStatus.Anchang_Evaluated.GetHashCode());

            DataView dv = DbHelper.RunSqlReturnDS(strSql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(coll, dv);

            return coll;
        }
    }
}


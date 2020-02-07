using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MobileBusiness.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 用户评论视图数据源查询(wangsf)
    /// </summary>
    public class UserCommentViewDataQuery : ObjectDataSourceQueryAdapterBase<UserCommentViewData, UserCommentViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"c.Code, c.Creator userID,
                                c.Content Content,
                                c.ProjectCode ProjectCode,
                                c.CreateTime CommentTime";
            qc.FromClause = @" zc.UserComment AS c";

            qc.WhereClause = this.ToWhere();
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion

        private string _orderByClause = "c.CreateTime DESC";//默认排序

        public string ProjectCode { get; set; }

        public string ParentCode { get; set; }

        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (!this.ProjectCode.IsNullOrEmpty())
            {
                where.AppendItem("c.ProjectCode", this.ProjectCode);
            }
            if (!this.ParentCode.IsNullOrEmpty())
            {
                where.AppendItem("c.ParentCode", this.ParentCode);
            }

            return where.ToSqlString(TSqlBuilder.Instance);
        }

        /// <summary>
        /// 获取前几条评论
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public UserCommentViewDataCollection GetUserCommentListAny(string projectCode, string parentCode, int topNum)
        {
            UserCommentViewDataCollection coll = new UserCommentViewDataCollection();

            string strSql = string.Format(@"SELECT TOP {0} c.Code,

                                        (SELECT COUNT(Code) FROM zc.UserComment WHERE ISNULL(ParentCode,'')='' AND ProjectCode='{1}') CommentCount,

                                         c.Creator UserID,

                                        c.Content Content,

                                        c.ProjectCode ProjectCode,c.CreateTime CommentTime  FROM  zc.UserComment AS c

                                        WHERE c.ProjectCode='{1}' And WorksCode is null And ISNULL(c.ParentCode,'')='{2}'
                                              
                                        ORDER BY c.CreateTime DESC ", topNum, projectCode, parentCode == null ? "" : parentCode);


            DataView dv = DbHelper.RunSqlReturnDS(strSql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(coll, dv);

            return coll;
        }
        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public UserCommentViewDataCollection GetUserCommentList(string projectCode)
        {
            UserCommentViewDataCollection coll = new UserCommentViewDataCollection();

            string strSql = string.Format(@"SELECT c.Code,

                                         c.Creator UserID,

                                        c.Content Content,

                                        c.ProjectCode ProjectCode,c.CreateTime CommentTime  FROM  zc.UserComment AS c

                                        WHERE c.ProjectCode='{0}'  And WorksCode is null And ISNULL(c.ParentCode,'')=''
                                              
                                        ORDER BY c.CreateTime DESC ", projectCode);


            DataView dv = DbHelper.RunSqlReturnDS(strSql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(coll, dv);

            return coll;
        }
    }


}

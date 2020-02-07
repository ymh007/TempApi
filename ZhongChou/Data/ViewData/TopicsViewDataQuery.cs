using MCS.Library.Data.Builder;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    /// <summary>
    /// 生活家话题列表
    /// </summary>
    public class TopicsViewDataQuery : ObjectDataSourceQueryAdapterBase<TopicsViewData, TopicsViewDataCollection>
    {
        #region 基类方法
        protected override void OnBuildQueryCondition(QueryCondition qc)
        {
            qc.OrderByClause = this.OrderByClause;
            qc.SelectFields = @"p.Creator,u.HeadImage LiferHeadImage,p.Name TopicName,p.CreateTime CreateTime,p.SupportNo,p.FocusNo,p.EvaluationUserTotal,p.EvaluationScoreTotal,
                u.RealName LiferName,l.BriefDesc,l.JobTitle JobTitle,t.*,tt.Name TypeName,p.IsValid IsValid";
            qc.FromClause = @"zc.Project p 
                 INNER JOIN zc.Topic t ON p.Code=t.ProjectCode
                INNER JOIN zc.TopicType tt ON t.TopicTypeCode=tt.Code
                LEFT JOIN zc.LiferInfo l ON p.Creator=l.Creator 
                INNER JOIN zc.UserInfo u ON l.Creator=u.Code";
            qc.WhereClause = this.ToWhere();
            qc.GroupBy = @"p.Creator,u.HeadImage,p.Name,p.CreateTime,p.SupportNo,p.FocusNo,p.EvaluationUserTotal,p.EvaluationScoreTotal,
                u.RealName,l.BriefDesc,l.JobTitle,t.Code,t.TopicTypeCode,t.ProjectCode,t.TopicDurationCode,t.Price,tt.Name,p.IsValid,p.ModifyTime";
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        #endregion


        private string _orderByClause = "p.ModifyTime DESC";
        /// <summary>
        /// p.SupportNo Desc              见过最多
        /// p.EvaluationScoreTotal Desc   评分最高
        /// t.Price  Asc                  价格最低
        /// </summary>
        public string OrderByClause
        {
            get { return _orderByClause; }
            set { _orderByClause = value; }
        }

        /// <summary>
        /// 话题类型
        /// </summary>
        public string TopicTypeCode { get; set; }

        public string CityCode { get; set; }


        public string City { get; set; }
        
        /// <summary>
        /// 话题创建人
        /// </summary>
        public string TopicCreator { get; set; }

        /// <summary>
        /// 话题名称
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 是否全部加载(上架或下架的)
        /// </summary>
        public bool IsAll { get; set; }


        private bool _isValid = true;

        /// <summary>
        /// 是否上下架（默认上架）
        /// </summary>
        public bool IsValid
        {
            get { return _isValid; }
            set { _isValid = value; }
        }

        public string ToWhere()
        {
            var where = new WhereSqlClauseBuilder();

            if (!string.IsNullOrEmpty(this.TopicTypeCode))
            {
                 where.AppendItem("t.TopicTypeCode", this.TopicTypeCode);
            }
            if (!string.IsNullOrEmpty(this.CityCode))
            {
                where.AppendItem("p.CityCode", this.CityCode);
            }
            if (!string.IsNullOrEmpty(this.City) && this.City != "-1")
            {
                where.AppendItem("p.City", "%" + this.City + "%", "Like");
            }
            if (!string.IsNullOrEmpty(this.TopicCreator))
            {
                where.AppendItem("p.Creator", this.TopicCreator);
            }
            if (!string.IsNullOrEmpty(this.TopicName))
            {
                where.AppendItem("p.Name", "%" + this.TopicName + "%", "LIKE");
            }

            where.AppendItem("p.Type", ProjectTypeEnum.Topic.ToString("D"));

            if (!IsAll)
            {
                where.AppendItem("p.IsValid", IsValid);
            }
          

            return where.ToSqlString(TSqlBuilder.Instance);
        }
    }
}

using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class TopicsViewData : Topic
    {
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 生活家真实姓名
        /// </summary>
        public string LiferName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建时间格式化
        /// </summary>
        public string CreateTimeFomart
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd");
            }
        }
        /// <summary>
        /// 生活家头像
        /// </summary>
        public string LiferHeadImage { get; set; }

        /// <summary>
        /// 生活家简介
        /// </summary>
        public string BriefDesc { get; set; }
        /// <summary>
        /// 职位/头衔
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// 话题成交数
        /// </summary>
        public int SupportNo { get; set; }

        /// <summary>
        /// 关注人数
        /// </summary>
        public int FocusNo { get; set; }

        /// <summary>
        /// 评价人数
        /// </summary>
        public int EvaluationUserTotal { get; set; }

        /// <summary>
        /// 评价总分
        /// </summary>
        public double EvaluationScoreTotal { get; set; }

        /// <summary>
        /// 评价平均分
        /// </summary>
        public double EvaluationAvgScore
        {
            get
            {
                if (this.EvaluationUserTotal != 0)
                {
                    double score = (double)this.EvaluationScoreTotal / this.EvaluationUserTotal;
                    return Math.Round(score, 1);
                }
                else
                {
                    return 0;
                }
            }
        }        

        /// <summary>
        /// 话题类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 是否发布
        /// </summary>
        public string IsValidFormart
        {
            get
            {
                if (IsValid)
                {
                    return "已上架";
                }
                else
                {
                    return "已下架";
                }
            }
        }

        /// <summary>
        /// 是否有空
        /// </summary>
        public bool IsFree
        {
            get
            {
                var journeys = JourneyAdapter.Instance.LoadFreeJourneys(this.Creator);
                return journeys != null && journeys.Count > 0;
            }
        }
    }
    public class TopicsViewDataCollection : EditableDataObjectCollectionBase<TopicsViewData>
    {
        /// <summary>
        /// 转化为ListDataView
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="totalCount">总行数</param>
        /// <returns></returns>
        public ListDataView ToListDataView(int pageIndex, int pageSize, int totalCount)
        {
            var result = new ListDataView
            {
                PageIndex = pageIndex,
                PageCount = totalCount % pageSize > 0 ? totalCount / pageSize + 1 : totalCount / pageSize,
                TotalCount = totalCount,
                ListData = this
            };

            return result;
        }
    }
}

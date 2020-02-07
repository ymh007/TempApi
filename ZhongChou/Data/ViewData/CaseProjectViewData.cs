using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.ViewData
{
    public class CaseProjectViewData : Project
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string RowNumberForSplit { get; set; }

        public override ProjectState ProgressState
        {
            get
            {
                if (this.Type == ProjectTypeEnum.Anchang)
                {
                    if (this.AuditStatus == Enums.AuditStatus.None)
                    {
                        return ProjectState.Draft;
                    }
                    else if (this.AuditStatus == Enums.AuditStatus.Auditing)
                    {
                        return ProjectState.Auditing;
                    }
                    else if (this.AuditStatus == Enums.AuditStatus.Faid)
                    {
                        return ProjectState.AuditFailed;
                    }
                    else if (this.AuditStatus == Enums.AuditStatus.Success)
                    {
                        if (!this.IsStart)  
                        {
                            //活动未开始
                            return ProjectState.Soon;
                        }
                        else
                        {
                            //活动已经开始
                            if (!this.IsEnd)
                            {
                                //活动未结束
                                if (!this.IsEnrollEnd)
                                {
                                    //投票未截止
                                    return ProjectState.Polling;
                                }
                                else
                                {   //投票已截止
                                    return ProjectState.Selection;
                                }
                              
                            }
                            else
                            {
                                //活动已经结束
                                return ProjectState.ActivityEnds;
                            }
                        }

                    }
                    return ProjectState.Draft;

                }
                else
                {
                    if (this.AuditStatus == Enums.AuditStatus.None)
                    {
                        return ProjectState.Draft;
                    }
                    else if (this.AuditStatus == Enums.AuditStatus.Auditing)
                    {
                        return ProjectState.Auditing;
                    }
                    else if (this.AuditStatus == Enums.AuditStatus.Faid)
                    {
                        return ProjectState.AuditFailed;
                    }
                    else if (this.AuditStatus == Enums.AuditStatus.Success)
                    {
                        if (!this.IsStart)
                        {
                            return ProjectState.Soon;
                        }
                        else
                        {
                            if (!this.IsEnd)
                            {
                                return ProjectState.Collecting;
                            }
                            else if (!IsEnrollEnd)
                            {
                                if (!this.IsSuccess)
                                {
                                    return ProjectState.ActivityEnd;
                                }
                                return ProjectState.Voting;
                            }
                            else
                            {
                                return ProjectState.ActivityEnd;
                            }
                        }

                    }
                    return ProjectState.Draft;
                }

            }

        }

        public override string ProgressStateText
        {
            get
            {
                var projectState = EnumItemDescriptionAttribute.GetDescriptionList(typeof(ProjectState)).Where(w => w.Category.IndexOf(this.Type.ToString("D")) >= 0 && w.EnumValue == (int)this.ProgressState).FirstOrDefault();
                return projectState.Description;
            }
        }
        /// <summary>
        /// 开始时间格式化字符串
        /// </summary>
        [NoMapping]
        public string StartTimeFormat { get { return this.StartTime.ToString("yyyy-MM-dd"); } }

        /// <summary>
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeFormat { get { return this.EndTime.ToString("yyyy-MM-dd"); } }

        /// <summary>
        /// 审核状态时间
        /// </summary>
        [NoMapping]
        public string AuditStatusTimeStr
        {
            get
            {
                DateTime time = this.ModifyTime != DateTime.MinValue ? this.ModifyTime : this.CreateTime;
                return time.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [NoMapping]
        public bool IsVideo
        {
            get
            {
                if (ActivityBroadcastAdapter.Instance.LoadByProjectCode(Code) != null)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [NoMapping]
        public string data1 { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CaseProjectViewDataCollection : EditableDataObjectCollectionBase<CaseProjectViewData>
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

using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 在线活动
    /// </summary>
    public class OnlineProject : Project
    {
        /// <summary>
        /// 活动状态
        /// </summary>
        public override ProjectState ProgressState
        {
            get
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
                            //if (this.WorksSelectedType == WorksSelectedTypeEnum.UserVote)
                            //{
                            return ProjectState.Voting;
                            //}
                            //else if (this.WorksSelectedType == WorksSelectedTypeEnum.BusinessSelection)
                            //{
                            //    return ProjectState.InSelection;
                            //}
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
        /// <summary>
        /// 活动状态描述
        /// </summary>
        [NoMapping]
        public string ProgressStateStr
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(this.ProgressState);
            }
        }
        /// <summary>
        /// 作品评选方式描述
        /// </summary>
        [NoMapping]
        public string WorksSelectedTypeStr
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(this.WorksSelectedType);
            }
        }
        /// <summary>
        /// 报名(投票,评选)截止时间Str
        /// </summary>
        [NoMapping]
        public string EnrollDeadlineStr
        {
            get
            {
                return this.EnrollDeadline.ToString("yyyy-MM-dd");
            }
        }
        /// <summary>
        /// 父级前几条用户评论
        /// </summary>
        [NoMapping]
        public UserCommentViewDataCollection UserCommentListAny
        {
            get
            {
                if (this.Loaded)
                {
                    return new UserCommentViewDataQuery().GetUserCommentListAny(this.Code, "", 5);
                }
                return null;
            }
        }
        /// <summary>
        /// 获取所有评论
        /// </summary>
        [NoMapping]
        public UserCommentViewDataCollection UserCommentList
        {
            get
            {
                return new UserCommentViewDataQuery().GetUserCommentList(this.Code);
            }
        }

        private AwardsSettingCollection _awardsSettingCollection = null;
        /// <summary>
        /// 奖项设置
        /// </summary>
        [NoMapping]
        public AwardsSettingCollection AwardsSettingCollection
        {
            get
            {
                if (this.Loaded && _awardsSettingCollection == null)
                {
                    _awardsSettingCollection = AwardsSettingAdapter.Instance.LoadByProjectCode(this.Code);
                }
                return _awardsSettingCollection;
            }
            set
            {
                _awardsSettingCollection = value;
            }
        }

        private WorksAwardsCollection _worksAwardsCollection = null;
        /// <summary>
        /// 获奖作品
        /// </summary>
        [NoMapping]
        public WorksAwardsCollection WorksAwardsCollection
        {
            get
            {
                if (this.Loaded && _awardsSettingCollection == null)
                {
                    _worksAwardsCollection = WorksAwardsAdapter.Instance.LoadByProjectCode(this.Code);
                }
                return _worksAwardsCollection;
            }
            set
            {
                _worksAwardsCollection = value;
            }
        }

        private ActivityWorksCollection _activityWorksCollection = null;
        /// <summary>
        /// 参与作品
        /// </summary>
        [NoMapping]
        public ActivityWorksCollection ActivityWorksCollection
        {
            get
            {
                if (this.Loaded && _activityWorksCollection == null)
                {
                    _activityWorksCollection = ActivityWorksAdapter.Instance.LoadLastThree(this.Code);
                }
                return _activityWorksCollection;
            }
            set
            {
                _activityWorksCollection = value;
            }
        }

        [NoMapping]
        public AttachmentCollection DetailImages
        {
            get
            {
                return AttachmentAdapter.Instance.LoadByResourceIDAndType(Code, AttachmentTypeEnum.ActivityDetailImg);
            }
        }

        /// <summary>
        /// 审核意见
        /// </summary>
        [NoMapping]
        public OpinionCollection OpinionCollection
        {
            get
            {
                if (this.Loaded)
                {
                    return OpinionAdapter.Instance.LoadByResourceID(this.Code, Convert.ToInt32(OpinionType.Project));
                }
                return null;
            }
        }

        /// <summary>
        /// 开始时间格式化字符串
        /// </summary>
        [NoMapping]
        public string StartTimeFormat { get { return this.StartTime.ToString("yyyy/MM/dd HH:mm"); } }
        /// <summary>
        /// 开始时间格式化字符串
        /// </summary>
        [NoMapping]
        public string StartTimeDate { get { return this.StartTime.ToString("yyyy-MM-dd"); } }

        /// <summary>
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeFormat { get { return this.EndTime.ToString("yyyy/MM/dd  HH:mm"); } }
        /// <summary>
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeDate { get { return this.EndTime.ToString("yyyy-MM-dd"); } }

        /// <summary>
        /// 对应用户是否对该项目点赞
        /// </summary>
        [NoMapping]
        public bool IsUserPraise { get; set; }

        /// <summary>
        /// 对应用户是否对该项目关注
        /// </summary>
        [NoMapping]
        public bool IsUserFocus { get; set; }
    }

    /// <summary>
    /// 在线活动集合
    /// </summary>
    public class OnlineProjectCollection : EditableDataObjectCollectionBase<OnlineProject>
    {

    }

    public class OnlineProjectAdapter : UpdatableAndLoadableAdapterBase<OnlineProject, OnlineProjectCollection>
    {
        public static readonly OnlineProjectAdapter Instance = new OnlineProjectAdapter();

        private OnlineProjectAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }


        public OnlineProject LoadByCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", projectCode);
                where.AppendItem("Type", ProjectTypeEnum.Online.ToString("D"));
            }).FirstOrDefault();
        }
    }
}

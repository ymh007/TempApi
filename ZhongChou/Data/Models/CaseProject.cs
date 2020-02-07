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
    /// 案场活动项目
    /// </summary>
    public class CaseProject : Project
    {
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
                            return ProjectState.Enrolling;
                        }
                        else
                        {
                            return ProjectState.EnrollEnd;
                        }
                    }

                }
                return ProjectState.Draft;
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
        private ActivityEventCollection _activityEventCollection = null;
        /// <summary>
        /// 活动场次
        /// </summary>
        [NoMapping]
        public ActivityEventCollection ActivityEventCollection
        {
            get
            {
                if (this.Loaded && _activityEventCollection == null)
                {
                    _activityEventCollection = ActivityEventAdapter.Instance.LoadByProjectCode(this.Code);
                }
                return _activityEventCollection;
            }
            set
            {
                _activityEventCollection = value;
            }
        }

        /// <summary>
        /// 正真的活动场次数
        /// </summary>
        [NoMapping]
        public int EventCount
        {
            get
            {
                if (ActivityEventCollection != null)
                    return ActivityEventCollection.Distinct(new ActivityEventComparer()).ToList().Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 用户最新三条评价
        /// </summary>
        [NoMapping]
        public UserEvaluationCollection UserLastThreeEvaluation
        {
            get
            {
                if (this.Loaded)
                {
                    return UserEvaluationAdapter.Instance.LoadLastThree(this.Code);
                }
                return null;
            }
        }
        /// <summary>
        /// 活动所有评价
        /// </summary>
        [NoMapping]
        public UserEvaluationCollection UserEvaluation
        {
            get
            {
                if (this.Loaded)
                {
                    return UserEvaluationAdapter.Instance.LoadAll(this.Code);
                }
                return null;
            }
        }

        /// <summary>
        /// 项目标签
        /// </summary>
        [NoMapping]
        public string TagsStr
        {
            get
            {
                if (this.Loaded)
                {
                    string tagsStr = "";
                    ProjectTags.ForEach(p =>
                    {
                        tagsStr += p.TagName + " ";
                    });
                    return tagsStr.TrimEnd();
                }
                return null;
            }
        }

        /// <summary>
        /// 项目标签
        /// </summary>
        [NoMapping]
        public ProjectTagCollection ProjectTags
        {
            get
            {
                if (this.Loaded)
                {
                    return ProjectTagAdapter.Instance.LoadByprojectCode(this.Code);
                }
                return null;
            }
        }

        /// <summary>
        /// 报名用户
        /// </summary>
        public ApplyUserInfoViewDataCollection ApplyUserInfo8
        {
            get
            {
                if (this.Loaded)
                {
                    return new ApplyUserInfoViewDataQuery().GetApplyUserInfo8(this.Code, 8);
                }
                return null;
            }
        }
        /// <summary>
        /// 所有报名用户
        /// </summary>
        public ApplyUserInfoViewDataCollection ApplyUserInfo
        {
            get
            {
                return new ApplyUserInfoViewDataQuery().GetApplyUserInfo(this.Code);
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
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeFormat { get { return this.EndTime.ToString("yyyy/MM/dd  HH:mm"); } }

        /// <summary>
        /// 开始时间格式化字符串
        /// </summary>
        [NoMapping]
        public string StartTimeDate { get { return this.StartTime.ToString("yyyy/MM/dd"); } }

        /// <summary>
        /// 结束时间格式化字符串
        /// </summary>
        [NoMapping]
        public string EndTimeDate { get { return this.EndTime.ToString("yyyy/MM/dd"); } }

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


        /// <summary>
        /// 详情图片
        /// </summary>
        [NoMapping]
        public AttachmentCollection DetailImages
        {
            get
            {
                return AttachmentAdapter.Instance.LoadByResourceIDAndType(this.Code, AttachmentTypeEnum.zc_Project_Detail);
            }
        }
    }

    public class CaseProjectCollection : EditableDataObjectCollectionBase<CaseProject>
    {

    }

    public class CaseProjectAdapter : UpdatableAndLoadableAdapterBase<CaseProject, CaseProjectCollection>
    {
        public static readonly CaseProjectAdapter Instance = new CaseProjectAdapter();

        private CaseProjectAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }


        public CaseProject LoadByCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", projectCode);
                where.AppendItem("Type", ProjectTypeEnum.Anchang.ToString("D"));
            }).FirstOrDefault();
        }
    }
}

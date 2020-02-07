using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using Seagull2.YuanXin.AppApi.ZhongChouData.ViewData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class ActivityView : ProjectView
    {
        public Project Project { get; set; }
        public ActivityView(string projectCode, string userCode)
        {

            this.Project = ProjectAdapter.Instance.LoadByCode(projectCode);
            var userInfo = ContactsAdapter.Instance.LoadByCode(Project.Creator);
            this.UserInfo = userInfo;

            this.IsFocus = UserFocusAdapter.Instance.Exists(userCode, projectCode);
            this.UserCommentViewDataCollection = new UserCommentViewDataQuery().GetUserCommentListAny(projectCode, "", 3);

            string type = this.Project.Type.ToString("D");
            this.ProjectTagNames = new List<string>();
            if (type == ProjectTypeEnum.Anchang.ToString("D"))
            {
                //OnlineProjectAdapter
                this.Project = CaseProjectAdapter.Instance.LoadByCode(projectCode);
                var projectTags = ProjectTagAdapter.Instance.LoadByprojectCode(projectCode);
                var evdQuery = new EvaluationViewDataDataQuery();
                evdQuery.ProjectCode = projectCode;
                int a = 0;
                this.EvaluationCollection = evdQuery.Query(0, 3, ref a);
                EvaluationTotalCount = a;//评价数
                projectTags.ForEach(p =>
                {
                    this.ProjectTagNames.Add(p.TagName);
                });
                this.ApplyUserInfoViewDataCollection = new ApplyUserInfoViewDataQuery().GetApplyUserInfo8(projectCode, 3);
                VideoView = new VideoView(projectCode);
                if (this.Project.SubItemJoinLimit == 1)
                {
                    SubmitLimit = true;
                }
                else
                {
                    SubmitLimit = false;
                }
            }
            else if (type == ProjectTypeEnum.Online.ToString("D"))
            {
                this.Project = OnlineProjectAdapter.Instance.LoadByCode(projectCode);
                this.AwardsSettingCollection = AwardsSettingAdapter.Instance.LoadByProjectCode(projectCode);
                this.ActivityWorksCollection = ActivityWorksAdapter.Instance.LoadLastThree(projectCode);
                this.VotedActivityWorkCodes = ActivityWorksAdapter.Instance.LoadVoted(projectCode, userCode);
                this.IsHaveWorks = ActivityWorksAdapter.Instance.Load(userCode, projectCode) != null;
                WorksTotalCount = ActivityWorksAdapter.Instance.GetWorksCount(projectCode);
            }
            if (VotedActivityWorkCodes != null && VotedActivityWorkCodes.Count > 0)
            {
                ActivityWorksCollection.Where(o => VotedActivityWorkCodes.Contains(o.Code)).All(x => x.IsVote = true);
                IsVoted = true;
            }
            else
            {
                IsVoted = false;
            }
            DetailPics = new List<string>();
            var attachments = AttachmentAdapter.Instance.LoadByResourceIDAndType(projectCode, AttachmentTypeEnum.ActivityDetailImg);
            if (attachments != null && attachments.Count > 0)
            {
                attachments.ForEach(p =>
                {
                    this.DetailPics.Add(p.URL);
                });
            }
            this.IsFocus = UserFocusAdapter.Instance.Load(userCode, projectCode) != null ? true : false;
        }


        /// <summary>
        /// 详情图片
        /// </summary>
        public List<string> DetailPics { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProjectState ProjectState
        {
            get
            {
                if (!this.Project.IsStart)
                {
                    return ProjectState.Soon;
                }
                else
                {
                    if (!this.Project.IsEnd)
                    {
                        return ProjectState.Enrolling;
                    }
                    else
                    {
                        return ProjectState.EnrollEnd;
                    }
                }
            }
        }

        /// <summary>
        /// 评论
        /// </summary>
        public UserCommentViewDataCollection UserCommentViewDataCollection { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        public EvaluationViewDataCollection EvaluationCollection { get; set; }

        /// <summary>
        /// 奖项设置
        /// </summary>
        public AwardsSettingCollection AwardsSettingCollection { get; set; }

        /// <summary>
        /// 报名用户
        /// </summary>
        public ApplyUserInfoViewDataCollection ApplyUserInfoViewDataCollection { get; set; }

        /// <summary>
        /// 参与作品
        /// </summary>
        public ActivityWorksCollection ActivityWorksCollection { get; set; }
        /// <summary>
        /// 作品数量 
        /// </summary>
        public int WorksTotalCount { get; set; }

        /// <summary>
        /// 当前用户是否有作品上传
        /// </summary>
        public bool IsHaveWorks { get; set; }
        /// <summary>
        /// 已投票作品
        /// </summary>
        public List<string> VotedActivityWorkCodes { get; set; }
        /// <summary>
        /// 当前用户是否已投票
        /// </summary>
        public bool IsVoted { get; set; }
        /// <summary>
        /// 直播详情
        /// </summary>
        public VideoView VideoView { get; set; }
        /// <summary>
        /// 评价数量
        /// </summary>
        public int EvaluationTotalCount { get; set; }
        /// <summary>
        /// 是否限制报名
        /// </summary>
        public bool SubmitLimit { get; set; }

    }
}
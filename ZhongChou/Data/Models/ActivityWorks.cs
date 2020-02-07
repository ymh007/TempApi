using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using System.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 活动作品
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.ActivityWorks")]
    public class ActivityWorks
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 众筹项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 作品内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 是否上传图
        /// </summary>
        [ORFieldMapping("HaveImage")]
        public bool HaveImage { get; set; }

        /// <summary>
        /// 得票数
        /// </summary>
        [ORFieldMapping("VoteCount")]
        public int VoteCount { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 评论总数
        /// </summary>
        [ORFieldMapping("CommentNo")]
        public int CommentNo { get; set; }

        /// <summary>
        /// 用户是否投票
        /// </summary>
        [NoMapping]
        public bool IsVote { get; set; }

        [NoMapping]
        public string CreateTimeFormat { get { return CommonHelper.APPDateFormateDiff(CreateTime, DateTime.Now); } }

        /// <summary>
        /// 用户信息
        /// </summary>
        [NoMapping]
        public ContactsModel UserInfo
        {
            get
            {
                //var result = UserInfoAdapter.Instance.LoadByCode(this.Creator);

                //result.NickName = CommonHelper.EncryptNickName(result.NickName);

                //result.Phone = CommonHelper.EncryptPhone(result.Phone);
                var result = ContactsAdapter.Instance.LoadByCode(this.Creator);

                return result;
            }
        }
        /// <summary>
        /// 用户信息头像
        /// </summary>
        [NoMapping]
        public string UserHeadUrl
        {
            get
            {
                return UserHeadPhotoService.GetUserHeadPhoto(this.UserInfo.ObjectID);
            }
        }

        [NoMapping]
        public AttachmentCollection Images
        {
            get
            {
                if (this.HaveImage)
                {
                    return AttachmentAdapter.Instance.LoadByResourceIDAndType(this.Code, AttachmentTypeEnum.ActivityWorks);
                }
                return null;
            }
        }
        [NoMapping]
        public AwardsSettingCollection AwardsSettingList
        {
            get
            {
                IEnumerable<AwardsSetting> IEnumerableAwardsSetting = AwardsSettingAdapter.Instance.LoadByProjectCode(ProjectCode).Where(o => o.GetAwardsList.Where(x => x.ActivityWorksCode == Code).Count() > 0);
                AwardsSettingCollection Acoll = new AwardsSettingCollection();
                IEnumerableAwardsSetting.ForEach(o => Acoll.Add(o));
                return Acoll;

            }
        }
    }

    /// <summary>
    /// 活动作品集合
    /// </summary>
    [Serializable]
    public class ActivityWorksCollection : EditableDataObjectCollectionBase<ActivityWorks>
    {
    }

    /// <summary>
    /// 活动作品操作类
    /// </summary>
    public class ActivityWorksAdapter : UpdatableAndLoadableAdapterBase<ActivityWorks, ActivityWorksCollection>
    {
        public static readonly ActivityWorksAdapter Instance = new ActivityWorksAdapter();

        private ActivityWorksAdapter() { }


        public ActivityWorks LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public ActivityWorksCollection LoadList(string validStatus = "")
        {
            return this.Load(p =>
            {
                if (validStatus.IsNotEmpty())
                {
                    p.AppendItem("ValidStatus", validStatus);
                }
            });
        }
        public ActivityWorksCollection LoadByUserCode(string useCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("Creator", useCode);

            });
        }

        public ActivityWorks Load(string userCode, string projectCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("Creator", userCode);
                p.AppendItem("ProjectCode", projectCode);
            }).FirstOrDefault();
        }

        public ActivityWorksCollection LoadByAwardsCode(string awardsCode)
        {
            var result = new ActivityWorksCollection();

            string sql = string.Format(@"select zc.ActivityWorks.*
                                         from zc.WorksAwards
                                         left join zc.ActivityWorks
                                         on zc.WorksAwards.ActivityWorksCode=zc.ActivityWorks.Code  
                                         where zc.WorksAwards.AwardsSettingCode='{0}' 
                                         order by zc.ActivityWorks.VoteCount desc"
                                        , awardsCode);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result;
        }

        public ActivityWorksCollection LoadLastThree(string projectCode)
        {
            var result = new ActivityWorksCollection();

            string sql = string.Format(@"select top 3 *                                        
                                         from zc.ActivityWorks
                                         where ProjectCode='{0}'
                                         order by CreateTime desc"
                                        , projectCode);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result;
        }


        public List<String> LoadVoted(string projectCode, string userCode)
        {
            var result = new ActivityWorksCollection();

            string sql = string.Format(@"select distinct atw.Code 
                                         from zc.ActivityWorks as atw
                                         inner join zc.UserVote uvt on uvt.ActivityWorksCode=atw.Code
                                         where atw.ProjectCode='{0}'  and uvt.Creator='{1}'                                       
                                         order by atw.Code desc"
                                        , projectCode, userCode);
            List<string> strs = new List<string>();
            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);
            foreach (DataRow dr in dv.Table.Rows)
            {
                strs.Add(dr["Code"].ToString());
            }
            return strs;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public int GetWorksCount(string projectCode)
        {
            string sql = string.Format(@" SELECT COUNT(Code) WorksCount FROM zc.ActivityWorks  where ProjectCode='{0}' ", projectCode);
            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            return (int)dv.Table.Rows[0]["WorksCount"];
        }

        /// <summary>
        /// 票数+1
        /// </summary>
        /// <param name="code"></param>
        public void SetIncVoteCount(string code)
        {
            this.SetInc("VoteCount", 1, where =>
            {
                where.AppendItem("Code", code);
            }, this.GetConnectionName());
        }
        /// <summary>
        /// 查询用户是否报名参加活动
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public ActivityWorks LoadByCreator(string projectCode,string userCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("Creator", userCode).AppendItem("ProjectCode", projectCode);
            }).FirstOrDefault();
        }
    }
}

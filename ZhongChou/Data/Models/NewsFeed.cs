using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 好友动态
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.NewsFeed")]
    public class NewsFeed
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }
        /// <summary>
        /// 动作
        /// </summary>
        [ORFieldMapping("Action")]
        public NewsFeedAction Action { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        [ORFieldMapping("Score")]
        public float Score { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }
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
        [NoMapping]
        public String CreateTimeFormat { get { return CommonHelper.APPDateFormateDiff(CreateTime, DateTime.Now); } }
        /// <summary>
        /// 来源Id
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public String ResourceID { get; set; }
    }
    /// <summary>
    /// 动态
    /// </summary>
    [Serializable]
    public class NewsFeedCollection : EditableDataObjectCollectionBase<NewsFeed>
    {
    }

    /// <summary>
    /// 动态操作类
    /// </summary>
    public class NewsFeedAdapter : UpdatableAndLoadableAdapterBase<NewsFeed, NewsFeedCollection>
    {
        public static readonly NewsFeedAdapter Instance = new NewsFeedAdapter();

        private NewsFeedAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public NewsFeed LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }

        public NewsFeedCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public NewsFeedCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
        /// <summary>
        /// 新增一个好友动态
        /// </summary>
        /// <param name="nAct"></param>
        /// <param name="projectCode"></param>
        /// <param name="score"></param>
        public void AddOneFeed(NewsFeedAction nAct, string projectCode, string userCode, string content = "", int score = 0, string resourceID = "")
        {
            NewsFeed newFeed = new NewsFeed();
            newFeed.Code = Guid.NewGuid().ToString();
            newFeed.ProjectCode = projectCode;
            newFeed.Content = content;
            newFeed.Action = nAct;
            newFeed.Score = score;
            newFeed.Creator = userCode;
            newFeed.CreateTime = DateTime.Now;
            newFeed.ResourceID = resourceID;
            NewsFeedAdapter.Instance.Update(newFeed);
        }
        /// <summary>
        /// 删除一个动态
        /// </summary>
        public void DeleteOneFeed(NewsFeedAction nAct, string projectCode, string userCode, string resourceId)
        {
            NewsFeed nfd = this.Load(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("Creator", userCode);
                where.AppendItem("ResourceID", resourceId);
                where.AppendItem("Action", (int)nAct);
            }).FirstOrDefault();
            if (nfd != null)
            {
                NewsFeedAdapter.Instance.Delete(nfd);
            }
        }

        #region 扩展方法
        /// <summary>
        /// 获取某个用户的某个众筹项目下的活动
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="creator"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public NewsFeed GetUserNewsFeed(string projectCode, string creator, NewsFeedAction action = NewsFeedAction.Enlist)
        {
            NewsFeed userNewsFeed = NewsFeedAdapter.Instance.Load(nf => nf.AppendItem("ProjectCode", projectCode).AppendItem("Creator", creator).AppendItem("Action", action.GetHashCode())).SingleOrDefault();
            return userNewsFeed;
        }
        #endregion
    }
}

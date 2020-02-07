using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 话题表
    /// </summary>
    [ORTableMapping("office.Topic")]
    public class TopicModel
    {
        /// <summary>
        /// 话题编码
        /// </summary>
        [ORFieldMapping("TopicId", PrimaryKey = true)]
        public string TopicId { get; set; }
        /// <summary>
        /// 话题标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }
        /// <summary>
        /// 话题简介
        /// </summary>
        [ORFieldMapping("Summary")]
        public string Summary { get; set; }
        /// <summary>
        /// 话题详细
        /// </summary>
        [ORFieldMapping("Detail")]
        public string Detail { get; set; }
        /// <summary>
        /// 话题图片
        /// </summary>
        [ORFieldMapping("Image")]
        public string Image { get; set; }
        /// <summary>
        /// 话题匿名标志
        /// </summary>
        [ORFieldMapping("Anonymous")]
        public bool Anonymous { get; set; }
        /// <summary>
        /// 话题创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 话题创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        [ORFieldMapping("AttentionCount")]
        public int AttentionCount { get; set; }
        /// <summary>
        /// 讨论人数
        /// </summary>
        [ORFieldMapping("DiscussCount")]
        public int DiscussCount { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [ORFieldMapping("CreatorName")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建人姓名Str
        /// </summary>
        [NoMapping]
        public string CreatorNameStr
        {
            get
            {
                if (CreatorName.IsEmptyOrNull())
                {
                    OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.Creator);
                    if (users.Count > 0)
                    {
                        return users[0].DisplayName;
                    }
                }
                return this.CreatorName;
            }
        }

        /// <summary>
        /// 人员头像
        /// </summary>
        [ORFieldMapping("UserImage")]
        public string UserImage { get; set; }

        /// <summary>
        /// 是否添加置顶图片
        /// </summary>
        [NoMapping]
        public bool IsTop { get; set; }
    }
    public class TopicModelCollection : EditableDataObjectCollectionBase<TopicModel> { }
    public class TopicModelAdapter : BaseAdapter<TopicModel, TopicModelCollection>
    {
        public static TopicModelAdapter Instance = new TopicModelAdapter();
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public TopicModelAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }
        public TopicModel LoadByCode(string code)
        {
            return Load(m => m.AppendItem("TopicId", code)).SingleOrDefault();
        }
        public void DelByCode(string code)
        {
            Delete(m => m.AppendItem("TopicId", code));
        }

        /// <summary>
        /// 描述：查找某个时间段新添加的热议数据
        /// 作者：v-dengwh
        /// 邮箱：v-dengwh@sinooceanland.com
        /// 时间：2017-02-14 10:26
        /// </summary>
        /// <param name="lastTime">时间</param>
        /// <returns></returns>
        public TopicModelCollection LoadByCreateTime(DateTime lastTime)
        {
            return Load(p =>
            {
                p.AppendItem("CreateTime", lastTime, ">");
                p.AppendItem("ValidStatus", true);
            });
        }

        /// <summary>
        /// 分页查询会议圈列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="searchTime">首页查询时间</param>
        /// <returns></returns>
        public ViewPageBase<List<TopicModel>> GetTopicListByPage(int pageIndex, DateTime searchTime, string topicName = "")
        {
            if (searchTime == DateTime.MinValue || searchTime == null)
            {
                searchTime = DateTime.Now;
            }
            string topicNameWhereSQL = "";
            if (!topicName.IsEmptyOrNull() && topicName != "null")
            {
                topicNameWhereSQL = " AND t.Title like '%" + topicName + "%' ";
            }

            string selectSQL = "SELECT t.*,CASE WHEN bc.SourceId IS NOT NULL THEN 1 ELSE 0 END AS IsTop";
            string fromAndWhereSQL = string.Format(@"FROM office.Topic t
                                                    Left join office.BannerConfig bc ON bc.SourceId=t.TopicId
                                                    WHERE t.CreateTime<'{0}' {1}", searchTime.ToString(), topicNameWhereSQL);
            string orderSQL = "order by t.CreateTime DESC";
            ViewBaseAdapter<TopicModel, List<TopicModel>> adapter = new ViewBaseAdapter<TopicModel, List<TopicModel>>(ConnectionNameDefine.YuanXinForDBHelp);
            ViewPageBase<List<TopicModel>> pageData = adapter.LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            pageData.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return pageData;
        }

    }
}
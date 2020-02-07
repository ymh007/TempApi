using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using System;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// APP意见反馈
    /// </summary>
    [Serializable]
    [ORTableMapping("office.Feedback")]
    public class Feedback
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 反馈标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [ORFieldMapping("Phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 反馈内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 反馈图片
        /// </summary>
        [NoMapping]
        public AppApi.Models.AttachmentCollection attachmentColl
        {
            get
            {
                return AppApi.Models.AttachmentAdapter.Instance.LoadByResourceID(this.Code);
            }
        }
    }
    /// <summary>
    /// 活动直播集合
    /// </summary>
    [Serializable]
    public class FeedbackCollection : EditableDataObjectCollectionBase<Feedback>
    {
    }

    /// <summary>
    /// 活动直播操作类
    /// </summary>
    public class FeedbackAdapter : UpdatableAndLoadableAdapterBase<Feedback, FeedbackCollection>
    {
        public static readonly FeedbackAdapter Instance = new FeedbackAdapter();

        private FeedbackAdapter()
        {

        }

        protected override string GetConnectionName()
        {
            return AppApi.Models.ConnectionNameDefine.YuanXinBusiness;
        }

    }
}
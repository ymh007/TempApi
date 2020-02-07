using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.ActivityNew
{
    /// <summary>
    /// 活动主库
    /// </summary>
    [ORTableMapping("office.ActivityInfo")]
    public class ActivityInfoModel : BaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 封面图
        /// </summary>
        [ORFieldMapping("Cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("EndTime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否是线下活动
        /// </summary>
        [ORFieldMapping("IsOffline")]
        public bool IsOffline { get; set; }

        /// <summary>
        /// 线下活动地址
        /// </summary>
        [ORFieldMapping("OfflineAddress")]
        public string OfflineAddress { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [ORFieldMapping("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 咨询电话
        /// </summary>
        [ORFieldMapping("Contact")]
        public string Contact { get; set; }

        /// <summary>
        /// 是否全部人员可以报名
        /// </summary>
        [ORFieldMapping("IsApplyAll")]
        public bool IsApplyAll { get; set; }

        /// <summary>
        /// 报名结束时间
        /// </summary>
        [ORFieldMapping("ApplyEndTime")]
        public DateTime ApplyEndTime { get; set; }
    }

    /// <summary>
    /// 活动主库集合
    /// </summary>
    public class ActivityInfoCollection : EditableDataObjectCollectionBase<ActivityInfoModel>
    {

    }
}
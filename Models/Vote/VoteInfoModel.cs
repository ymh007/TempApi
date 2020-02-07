using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Runtime.Serialization;

namespace Seagull2.YuanXin.AppApi.Models.Vote
{
    /// <summary>
    /// 投票问卷基本信息 Model
    /// </summary>
    [ORTableMapping("office.VoteInfo")]
    public class VoteInfoModel : BaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [ORFieldMapping("Describe")]
        public string Describe { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("EndTime")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否显示票数
        /// </summary>
        [ORFieldMapping("IsShowPoll")]
        public bool IsShowPoll { get; set; }

        /// <summary>
        /// 是否显示结果
        /// </summary>
        [ORFieldMapping("IsShowResult")]
        public bool IsShowResult { get; set; }

        /// <summary>
        /// 类型 0：投票 1：问卷
        /// </summary>
        [ORFieldMapping("VoteType")]
        public int VoteType { get; set; }
    }

    /// <summary>
    /// 投票问卷基本信息 Collection
    /// </summary>
    public class VoteInfoCollection : EditableDataObjectCollectionBase<VoteInfoModel>
    {

    }
}
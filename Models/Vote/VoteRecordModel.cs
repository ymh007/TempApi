using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Vote
{
    /// <summary>
    /// 投票记录表
    /// </summary>
    [ORTableMapping("office.VoteRecord")]
    public class VoteRecordModel : BaseModel
    {
        /// <summary>
        /// 投票编码
        /// </summary>
        [ORFieldMapping("VoteCode")]
        public string VoteCode { get; set; }

        /// <summary>
        /// 问题编码
        /// </summary>
        [ORFieldMapping("QuestionCode")]
        public string QuestionCode { get; set; }

        /// <summary>
        /// 选项编码
        /// </summary>
        [ORFieldMapping("OptionCode")]
        public string OptionCode { get; set; }

        /// <summary>
        /// 填空内容
        /// </summary>
        [ORFieldMapping("FillContent")]
        public string FillContent { set; get; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

    }
    /// <summary>
    /// 投票记录-Collection
    /// </summary>
    public class VoteRecordCollection : EditableDataObjectCollectionBase<VoteRecordModel>
    {

    }
}
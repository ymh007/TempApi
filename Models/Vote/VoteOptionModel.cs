using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Vote
{

    /// <summary>
    /// 投票选项库-Model
    /// </summary>
    [ORTableMapping("office.VoteOption")]
    public class VoteOptionModel : BaseModel
    {
        /// <summary>
        /// 投票编码
        /// </summary>
        [ORFieldMapping("VoteCode")]
        public string VoteCode { get; set; }

        /// <summary>
        /// 题目编码
        /// </summary>
        [ORFieldMapping("QuestionCode")]
        public string QuestionCode { get; set; }

        /// <summary>
        /// 选项名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否填空选项
        /// </summary>
        [ORFieldMapping("IsFill")]
        public bool IsFill { set; get; }
    }
    /// <summary>
    /// 投票选项库-Collcetion
    /// </summary>
    public class VoteOptionCollection : EditableDataObjectCollectionBase<VoteOptionModel>
    {

    }
}
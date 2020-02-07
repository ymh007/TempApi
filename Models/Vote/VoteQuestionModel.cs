using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Vote
{
    /// <summary>
    /// 投票题库-Model
    /// </summary>
    [ORTableMapping("office.VoteQuestion")]
    public class VoteQuestionModel : BaseModel
    {
        /// <summary>
        /// 投票编码
        /// </summary>
        [ORFieldMapping("VoteCode")]
        public string VoteCode { get; set; }

        /// <summary>
        /// 题目
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

		/// <summary>
		/// 类型：0单选;1多选;2填空
		/// </summary>
		[ORFieldMapping("QuestionType")]
        public int QuestionType { get; set; }

        /// <summary>
        /// 最少选择
        /// </summary>
        [ORFieldMapping("MinChoice")]
        public int MinChoice { get; set; }

        /// <summary>
        /// 最多选择
        /// </summary>
        [ORFieldMapping("MaxChoice")]
        public int MaxChoice { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
		[ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }

    /// <summary>
    /// 投票题库-Collection
    /// </summary>
    public class VoteQuestionCollcetion : EditableDataObjectCollectionBase<VoteQuestionModel>
    {

    }
}
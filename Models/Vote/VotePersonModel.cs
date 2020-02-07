using System.Runtime.Serialization;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;


namespace Seagull2.YuanXin.AppApi.Models.Vote
{
    /// <summary>
    /// 投票人员范围-Model
    /// </summary>
    [ORTableMapping("office.VotePerson")]
    public class VotePersonModel : BaseModel
    {
        /// <summary>
        /// 投票编码
        /// </summary>
        [ORFieldMapping("VoteCode")]
        public string VoteCode { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
    }

    /// <summary>
    /// 投票人员范围-Collcetion
    /// </summary>
    public class VotePersonCollcetion : EditableDataObjectCollectionBase<VotePersonModel>
    {

    }
}
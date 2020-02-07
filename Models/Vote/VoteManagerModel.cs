using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Vote
{
    /// <summary>
	/// 投票问卷管理员 Model
	/// </summary>
	[ORTableMapping("office.VoteManager")]
    public class VoteManagerModel : BaseModel
    {
        /// <summary>
		/// 投票问卷编码
		/// </summary>
		[ORFieldMapping("VoteCode")]
        public string VoteCode { get; set; }

        /// <summary>
        /// 管理员编码
        /// </summary>
        [ORFieldMapping("ManagerCode")]
        public string ManagerCode { get; set; }

        /// <summary>
        /// 管理员名称
        /// </summary>
        [ORFieldMapping("ManagerDisplayName")]
        public string ManagerDisplayName { get; set; }
    }

    /// <summary>
	/// 投票问卷管理员 Collection
	/// </summary>
	public class VoteManagerCollection : EditableDataObjectCollectionBase<VoteManagerModel>
    {

    }
}
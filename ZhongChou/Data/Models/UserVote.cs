using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户投票
    /// </summary>
    [ORTableMapping("zc.UserVote")]
    public class UserVote
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 活动作品编码
        /// </summary>
        [ORFieldMapping("ActivityWorksCode")]
        public string ActivityWorksCode { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

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

    }

    /// <summary>
    /// 用户投票集合
    /// </summary>
    public class UserVoteCollection : EditableDataObjectCollectionBase<UserVote>
    {
    }

    /// <summary>
    /// 用户投票操作类
    /// </summary>
    public class UserVoteAdapter : UpdatableAndLoadableAdapterBase<UserVote, UserVoteCollection>
    {
        public static readonly UserVoteAdapter Instance = new UserVoteAdapter();

        private UserVoteAdapter() { }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserVote LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public UserVote Load(string userCode, string projectCode)
        {
            return this.Load(o =>
                   o.AppendItem("Creator", userCode)
                   .AppendItem("ProjectCode", projectCode)
               ).FirstOrDefault();
        }
        
        public UserVoteCollection LoadList(string validStatus = "")
        {
            return this.Load(p =>
            {
                if (validStatus.IsNotEmpty())
                {
                    p.AppendItem("ValidStatus", validStatus);
                }
            });
        }
    }
}

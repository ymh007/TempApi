using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Vote;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Vote
{
    /// <summary>
    /// 投票问卷管理员 Adapter
    /// </summary>
    public class VoteManagerAdapter : UpdatableAndLoadableAdapterBase<VoteManagerModel, VoteManagerCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VoteManagerAdapter Instance = new VoteManagerAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        public void Add(string voteCode, string managerCode, string managerDisplayName, string creator)
        {
            Update(new VoteManagerModel
            {
                Code = Guid.NewGuid().ToString(),
                VoteCode = voteCode,
                ManagerCode = managerCode,
                ManagerDisplayName = managerDisplayName,
                Creator = creator,
                CreateTime = DateTime.Now,
                ValidStatus = true,
            });
        }
    }
}
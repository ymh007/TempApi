using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Vote;
using Seagull2.YuanXin.AppApi.ViewsModel.Vote;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Vote
{
    /// <summary>
    /// VotePerson-Adapter
    /// </summary>
    public class VotePersonAdapter : UpdatableAndLoadableAdapterBase<VotePersonModel, VotePersonCollcetion>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VotePersonAdapter Instance = new VotePersonAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 批量插入人员范围
        /// </summary>
        /// <param name="userList">人员列表</param>
        /// <param name="voteCode">投票问卷编码</param>
        /// <param name="creator">创建人编码</param>
        public void BatchInsert(List<CreatePersonViewModel> userList, string voteCode, string creator)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("VoteCode", typeof(string));
            dt.Columns.Add("UserCode", typeof(string));
            dt.Columns.Add("UserName", typeof(string));
            dt.Columns.Add("Creator", typeof(string));
            dt.Columns.Add("CreateTime", typeof(DateTime));
            dt.Columns.Add("Modifier", typeof(string));
            dt.Columns.Add("ModifyTime", typeof(DateTime));
            dt.Columns.Add("ValidStatus", typeof(bool));
            userList.ForEach(user =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = Guid.NewGuid().ToString();
                dr["VoteCode"] = voteCode;
                dr["UserCode"] = user.UserCode;
                dr["UserName"] = user.UserName;
                dr["Creator"] = creator;
                dr["CreateTime"] = DateTime.Now;
                dr["ValidStatus"] = true;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[office].[VotePerson]", GetConnectionName());
        }
    }
}

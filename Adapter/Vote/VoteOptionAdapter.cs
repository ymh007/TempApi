using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Vote;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Vote
{
    /// <summary>
    /// 投票选项库-Adapter
    /// </summary>
    public class VoteOptionAdapter : UpdatableAndLoadableAdapterBase<VoteOptionModel, VoteOptionCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VoteOptionAdapter Instance = new VoteOptionAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 批量插入题目数据
        /// </summary>
        public void BatchInsert(VoteOptionCollection coll)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("VoteCode", System.Type.GetType("System.String"));
            dt.Columns.Add("QuestionCode", System.Type.GetType("System.String"));
            dt.Columns.Add("Name", System.Type.GetType("System.String"));
            dt.Columns.Add("Sort", System.Type.GetType("System.Int32"));
            dt.Columns.Add("IsFill", typeof(bool));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["VoteCode"] = m.VoteCode;
                dr["QuestionCode"] = m.QuestionCode;
                dr["Name"] = m.Name;
                dr["Sort"] = m.Sort;
                dr["IsFill"] = m.IsFill;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[office].[VoteOption]", GetConnectionName());
        }
    }
}

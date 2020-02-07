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
    /// 投票题库-Adapter
    /// </summary>
    public class VoteQuestionAdapter : UpdatableAndLoadableAdapterBase<VoteQuestionModel, VoteQuestionCollcetion>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VoteQuestionAdapter Instance = new VoteQuestionAdapter();

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
        public void BatchInsert(VoteQuestionCollcetion coll)
        {
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("VoteCode", System.Type.GetType("System.String"));
            dt.Columns.Add("Title", System.Type.GetType("System.String"));
            dt.Columns.Add("QuestionType", System.Type.GetType("System.Int32"));
            dt.Columns.Add("MinChoice", System.Type.GetType("System.Int32"));
            dt.Columns.Add("MaxChoice", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Sort", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            coll.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["VoteCode"] = m.VoteCode;
                dr["Title"] = m.Title;
                dr["QuestionType"] = m.QuestionType;
                dr["MinChoice"] = m.MinChoice;
                dr["MaxChoice"] = m.MaxChoice;
                dr["Sort"] = m.Sort;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[office].[VoteQuestion]", GetConnectionName());
        }
    }
}

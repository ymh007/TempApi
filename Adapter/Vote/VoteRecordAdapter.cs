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
    /// 投票记录-Adapter
    /// </summary>
    public class VoteRecordAdapter : UpdatableAndLoadableAdapterBase<VoteRecordModel, VoteRecordCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VoteRecordAdapter Instance = new VoteRecordAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取投票人员
        /// </summary>
        /// <param name="voteCode"></param>
        /// <returns></returns>
        public DataTable GetCastPerson(string voteCode)
        {
            var sql = @" SELECT DISTINCT Creator as UserCode,UserName FROM [office].[VoteRecord]
                         WHERE VoteCode = @VoteCode";
            SqlParameter[] parameters = {
                new SqlParameter("@VoteCode",SqlDbType.NVarChar,36),
            };
            parameters[0].Value = voteCode;
            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userCode"></param>
        public void BatchInsertVote(List<VoteRecordModel> model)
        {
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("VoteCode", System.Type.GetType("System.String"));
            dt.Columns.Add("QuestionCode", System.Type.GetType("System.String"));
            dt.Columns.Add("OptionCode", System.Type.GetType("System.String"));
            dt.Columns.Add("FillContent", typeof(string));
            dt.Columns.Add("UserName", System.Type.GetType("System.String"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("Modifier", System.Type.GetType("System.String"));
            dt.Columns.Add("ModifyTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            model.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = Guid.NewGuid().ToString();
                dr["VoteCode"] = m.VoteCode;
                dr["QuestionCode"] = m.QuestionCode;
                dr["OptionCode"] = m.OptionCode;
                dr["FillContent"] = m.FillContent;
                dr["UserName"] = m.UserName;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = DateTime.Now;
                dr["ValidStatus"] = true;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "[office].[VoteRecord]", GetConnectionName());
        }

        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetExcelList(string voteCode)
        {
            var sql = @" SELECT record.VoteCode,record.QuestionCode,record.OptionCode,record.Creator AS UserCode,info.title AS InfoTitle,question.Title AS QuestionTitle,options.Name AS OptionName FROM [office].[VoteRecord] AS record
                         INNER JOIN [office].[VoteInfo] AS info
                         ON info.Code = record.VoteCode AND record.VoteCode = @VoteCode
                         INNER JOIN [office].[VoteQuestion] AS question
                         ON question.VoteCode = record.VoteCode AND question.Code = record.QuestionCode
                         INNER JOIN [office].[VoteOption] AS options
                         ON options.VoteCode = record.VoteCode AND options.Code = record.OptionCode
                         ORDER BY options.Sort ASC ";
            SqlParameter[] parameters = {
                new SqlParameter("@VoteCode",SqlDbType.NVarChar,36),
            };
            parameters[0].Value = voteCode;
            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }
    }
}

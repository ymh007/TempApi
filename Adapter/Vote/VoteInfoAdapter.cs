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
    /// 投票信息-Adapter
    /// </summary>
    public class VoteInfoAdapter : UpdatableAndLoadableAdapterBase<VoteInfoModel, VoteInfoCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VoteInfoAdapter Instance = new VoteInfoAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取我创建的投票列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public DataTable GetCreatedList(int pageIndex, int pageSize, string userCode)
        {
            pageIndex--;
            var sql = $@"
                WITH [Temp] AS
                (
	                SELECT ROW_NUMBER() OVER(ORDER BY [EndTime] DESC) AS [Row], * FROM [office].[VoteInfo] WHERE [Creator] = @UserCode OR [Code] IN (SELECT DISTINCT [VoteCode] FROM [office].[VoteManager] WHERE [ManagerCode] = @UserCode)
                )
                SELECT
	                [Temp].[Row], [Temp].[Code], [Temp].[Title], [Temp].[VoteType], [Temp].[IsShowPoll], [Temp].[IsShowResult], [Temp].[EndTime], [Temp].[Creator],
	                (SELECT COUNT(*) FROM [office].[VotePerson] WHERE [VoteCode] = [Temp].[Code]) AS [AllPerson],
	                (SELECT COUNT(DISTINCT [Creator]) FROM [office].[VoteRecord] WHERE [VoteCode] = [Temp].[Code]) AS [CastPerson],
	                CASE
		                WHEN EXISTS(SELECT * FROM [office].[VoteRecord] WHERE [Creator] = @UserCode AND [VoteCode] = [Temp].[Code]) THEN 1
		                ELSE 0
	                END AS [IsCast],
	                CASE
		                WHEN [Temp].[Creator] = @UserCode THEN 0
		                ELSE 1
	                END AS [IsManager]
                FROM [Temp] WHERE [Temp].[Row] BETWEEN {pageSize * pageIndex + 1} AND {pageSize * pageIndex + pageSize};";

            SqlParameter[] parameters = {
                new SqlParameter("@UserCode",SqlDbType.NVarChar,36),
            };
            parameters[0].Value = userCode;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 获取我参与的投票列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public DataTable GetParticipatesList(int pageIndex, int pageSize, string userCode)
        {
            pageIndex--;
            var sql = @"
                WITH [Temp] AS
                (
                    SELECT ROW_NUMBER() OVER (ORDER BY [O].[IsCast], [O].[IsOver], [O].[EndTime] ASC) AS [Row], [O].* FROM
	                (
		                SELECT [Info].[Code], [Info].[Title], [Info].[VoteType], [Info].[IsShowPoll], [Info].[IsShowResult], 
                        [Info].[EndTime], [Info].[Creator], [Persons2].[AllPerson], 
			                ISNULL([Record1].[CastPerson], 0) AS [CastPerson], ISNULL([Record2].[IsCast], 0) AS [IsCast],
			                CASE
				                WHEN DATEDIFF(MINUTE, GETDATE(), [Info].[EndTime]) > 0 THEN 0
				                ELSE 1
			                END AS [IsOver]
		                FROM [office].[VoteInfo] [Info] 
		                INNER JOIN 
		                (
			                SELECT [VoteCode], COUNT(1) AS [AllPerson] FROM [office].[VotePerson] GROUP BY [VoteCode]
		                ) AS [Persons2] ON [Persons2].[VoteCode] = [Info].[Code]
		                LEFT JOIN 
		                (
			                SELECT [A].[VoteCode], COUNT(1) AS [CastPerson] FROM (SELECT DISTINCT [VoteCode], [Creator] FROM [office].[VoteRecord]) A GROUP BY [A].[VoteCode]
		                ) AS [Record1] ON [Record1].[VoteCode] = [Info].[Code]
		                Left JOIN
		                (
			                SELECT [A].[VoteCode], COUNT(1) AS [IsCast] FROM (SELECT DISTINCT [VoteCode], [Creator] FROM [office].[VoteRecord]) A WHERE [A].[Creator] = @UserCode GROUP BY  [A].[VoteCode]
		                ) AS [Record2] ON [Record2].[VoteCode] = [Info].[Code]
	                ) O
                )
                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @StartIndex AND @EndIndex;";

            SqlParameter[] parameters = {
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@StartIndex", SqlDbType.Int, 4),
                new SqlParameter("@EndIndex", SqlDbType.Int, 4) };
            parameters[0].Value = userCode;
            parameters[1].Value = pageSize * pageIndex + 1;
            parameters[2].Value = pageSize * pageIndex + pageSize;

            SqlDbHelper helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 删除我创建的投票列表
        /// </summary>
        public void DeleteVoteByCreated(string voteCode)
        {
            var sql = @"
                BEGIN
	                DELETE FROM [office].[VoteInfo] WHERE [Code] = @VoteCode;
	                DELETE FROM [office].[VoteManager] WHERE [VoteCode] = @VoteCode;
	                DELETE FROM [office].[VotePerson] WHERE [VoteCode] = @VoteCode;
	                DELETE FROM [office].[VoteQuestion] WHERE [VoteCode] = @VoteCode;
	                DELETE FROM [office].[VoteOption] WHERE [VoteCode] = @VoteCode;
	                DELETE FROM [office].[VoteRecord] WHERE [VoteCode] = @VoteCode;
                END";

            SqlParameter[] parameters = { new SqlParameter("@VoteCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = voteCode;

            SqlDbHelper helper = new SqlDbHelper();
            helper.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }
    }
}
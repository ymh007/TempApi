using MCS.Library.SOA.DataObjects;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 评论适配器
    /// </summary>
    public class CommentAdapter : UpdatableAndLoadableAdapterBase<CommentModel, CommentCollection>
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["SinooceanLandAddressList"].ConnectionString;

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly CommentAdapter Instance = new CommentAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="commentType"></param>
        /// <param name="relationID"></param>
        /// <param name="userID">如：v-sunzhh</param>
        /// <returns></returns>
        public List<CommentModel> ShowComment(int commentType, int relationID, string userID)
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@CommentType", SqlDbType.Int, 4),
                    new SqlParameter("@RelationID", SqlDbType.Int, 4),
                    new SqlParameter("@UserID", SqlDbType.NVarChar, 50) };
            parameters[0].Value = commentType;
            parameters[1].Value = relationID;
            parameters[2].Value = "SINOOCEANLAND\\" + userID;
            var data = new SqlDbHelper(connectionString).ExecuteDataTable("[dbo].[Getcomment]", CommandType.StoredProcedure, parameters);
            return DataConvertHelper<CommentModel>.ConvertToList(data);
        }

        /// <summary>
        /// 添加评论、回复
        /// </summary>
        public int SubmitComment(int commentType, int relationId, string content, string userName, string userId)
        {
            var sql = @"INSERT INTO [dbo].[Table_Comment]
		                        (
			                        [CommentType],
			                        [RelationID],
			                        [CommentContent],
			                        [CommentPeople],
			                        [CommentPeopleID]
		                        )
	                        VALUES
		                        (
			                        {0}, -- CommentType - int
			                        {1}, -- RelationID - int
			                        N'{2}', -- CommentContent - nvarchar(max)
			                        N'{3}', -- CommentPeople - nvarchar(50)
			                        N'{4}'  -- CommentPeopleID - nvarchar(50)
		                        );
                        SELECT @@IDENTITY;";
            sql = string.Format(sql, commentType, relationId, content, userName, userId);

            var result = new SqlDbHelper(connectionString).ExecuteScalar(sql, CommandType.Text);
            return Convert.ToInt32(result);
        }
    }
}
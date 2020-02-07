using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ManagementReport;

namespace Seagull2.YuanXin.AppApi.Adapter.ManagementReport
{
    /// <summary>
    /// 文章或评论点赞记录Adapter
    /// </summary>
    public class PointPraiseRecordAdapter : UpdatableAndLoadableAdapterBase<PointPraiseRecordModel, PointPraiseRecordCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static PointPraiseRecordAdapter Instance = new PointPraiseRecordAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 获取用户点赞记录列表
        /// </summary>
        /// <param name="userId">用户名，如：SINOOCEANLAND\zhangbm</param>
        /// <param name="type">点赞类型：0：评论点赞、1：文章点赞</param>
        /// <param name="listId">RelationID列表</param>
        public PointPraiseRecordCollection GetList(string userId, int type, List<int> listId)
        {
            var sql = @"SELECT * FROM [dbo].[Table_PointPraiseRecord] WHERE [UserID] = '{0}' AND [PointPraiseType] = {1} AND [RelationID] IN ({2})";
            sql = string.Format(sql, userId, type, string.Join(",", listId));
            return QueryData(sql);
        }

        /// <summary>
        /// 文章新增/取消点赞
        /// </summary>
        public void PraiseOfArticle(string userName, int articleId, string articleUrl)
        {
            var sql = @"IF(EXISTS(SELECT * FROM [dbo].[Table_PointPraiseRecord] WHERE [UserID] = @UserId AND [PointPraiseType] = 1 AND [RelationID] = @ArticleId))
	                        BEGIN
		                        --取消点赞
		                        --删除点赞记录
		                        DELETE FROM [dbo].[Table_PointPraiseRecord] WHERE [UserID] = @UserId AND [PointPraiseType] = 1 AND [RelationID] = @ArticleId;
		                        --文章点赞统计-1
		                        UPDATE [dbo].[Table_ArticlePointOfPraise] SET [PointOfPraise] = [PointOfPraise] - 1 WHERE [ArticleUrl] = @ArticleUrl;
	                        END
                        ELSE
	                        BEGIN
		                        --添加点赞
		                        --添加点赞记录
		                        INSERT INTO [dbo].[Table_PointPraiseRecord]
		                                (
		                                 [UserID],
		                                 [PointPraiseType],
		                                 [PointPraiseTime],
		                                 [RelationID]
		                                )
		                        VALUES  (
		                                 @UserId, -- UserID - nvarchar(50)
		                                 1, -- PointPraiseType - int
		                                 @PraiseTime, -- PointPraiseTime - datetime
		                                 @ArticleId  -- RelationID - int
		                                );
		                        --文章点赞统计+1
		                        IF(EXISTS(SELECT * FROM [dbo].[Table_ArticlePointOfPraise] WHERE [ArticleUrl] = @ArticleUrl))
			                        BEGIN
				                        UPDATE [dbo].[Table_ArticlePointOfPraise] SET [PointOfPraise] = [PointOfPraise] + 1 WHERE [ArticleUrl] = @ArticleUrl;
			                        END
                                ELSE
			                        BEGIN
				                        INSERT INTO [dbo].[Table_ArticlePointOfPraise]
				                                (
						                         [PointOfPraise], 
						                         [ArticleUrl]
						                        )
				                        VALUES  (
						                         1, -- PointOfPraise - int
				                                 @ArticleUrl  -- ArticleUrl - nvarchar(max)
				                                )
			                        END
	                        END";
            sql = sql.Replace("@UserId", "'SINOOCEANLAND\\" + userName + "'");
            sql = sql.Replace("@ArticleUrl", "'" + articleUrl + "'");
            sql = sql.Replace("@ArticleId", articleId.ToString());
            sql = sql.Replace("@PraiseTime", "'" + DateTime.Now.ToString() + "'");
            DbHelper.RunSql(sql, GetConnectionName());
        }

        /// <summary>
        /// 评论新增/取消点赞
        /// </summary>
        public void PraiseOfComment(string userName, int commentId)
        {
            var sql = @"IF(EXISTS(SELECT * FROM [dbo].[Table_PointPraiseRecord] WHERE [UserID] = @UserId AND [PointPraiseType] = 0 AND [RelationID] = @CommentId))
	                        BEGIN
		                        --取消点赞
		                        --删除点赞记录
		                        DELETE FROM [dbo].[Table_PointPraiseRecord] WHERE [UserID] = @UserId AND [PointPraiseType] = 0 AND [RelationID] = @CommentId;
		                        --评论点赞统计-1
		                        UPDATE [dbo].[Table_Comment] SET [PointOfPraise] = [PointOfPraise] - 1 WHERE [ID] = @CommentId;
	                        END
                        ELSE
	                        BEGIN
		                        --添加点赞
		                        --添加点赞记录
		                        INSERT INTO [dbo].[Table_PointPraiseRecord]
		                                (
		                                 [UserID],
		                                 [PointPraiseType],
		                                 [PointPraiseTime],
		                                 [RelationID]
		                                )
		                        VALUES  (
		                                 @UserId, -- UserID - nvarchar(50)
		                                 0, -- PointPraiseType - int
		                                 @PraiseTime, -- PointPraiseTime - datetime
		                                 @CommentId  -- RelationID - int
		                                );
		                        --评论点赞统计+1
		                        UPDATE [dbo].[Table_Comment] SET [PointOfPraise] = [PointOfPraise] + 1 WHERE [ID] = @CommentId;
	                        END";
            sql = sql.Replace("@UserId", "'SINOOCEANLAND\\" + userName + "'");
            sql = sql.Replace("@CommentId", commentId.ToString());
            sql = sql.Replace("@PraiseTime", "'" + DateTime.Now.ToString() + "'");
            DbHelper.RunSql(sql, GetConnectionName());
        }
    }
}
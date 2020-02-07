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
    /// 评论列表Adapter
    /// </summary>
    public class CommentListAdapter : UpdatableAndLoadableAdapterBase<CommentListModel, CommentListCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static CommentListAdapter Instance = new CommentListAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="userName">当前用户名</param>
        /// <param name="commentType">评论类型（1：文章评论、0：评论的评论）</param>
        /// <param name="relationId">父级关系编号（commentType=1：文章编号、commentType=0：评论编号）</param>
        public CommentListCollection GetList(string userName, int commentType, int relationId)
        {
            var sql = @"SELECT [A].[ID] AS [Id], [A].[CommentContent] AS [Content], [A].[CommentTime] AS [Time], [A].[PointOfPraise] AS [PraiseCount],
	                        (
		                        SELECT COUNT(0) FROM [dbo].[Table_PointPraiseRecord] R 
		                        WHERE 
			                        [R].[UserID] = 'SINOOCEANLAND\{0}' AND 
			                        [R].[PointPraiseType] = 0 AND 
			                        [R].[RelationID] = [A].[ID]
	                        ) AS [MyPraiseCount],
	                        (
		                        SELECT TOP 1 [U].[USER_GUID] FROM [dbo].[User_Syn] [U] WHERE [U].[LOGON_NAME] = [A].[CommentPeopleID]
	                        ) AS [UserId],
	                        [A].[CommentPeople] AS [UserName]
                        FROM [dbo].[Table_Comment] [A]
                        WHERE 
                            [A].[CommentType] = {1} AND
                            [A].[RelationID] = {2}
                        ORDER BY [A].[CommentTime] DESC;";
            sql = string.Format(sql, userName, commentType, relationId);
            return QueryData(sql);
        }
    }
}
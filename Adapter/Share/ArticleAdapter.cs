using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using System.Data.SqlClient;
using System.Data;

namespace Seagull2.YuanXin.AppApi.Adapter.Share
{
    /// <summary>
    /// 文章适配器
    /// </summary>
    public class ArticleAdapter : UpdatableAndLoadableAdapterBase<ArticleModel, ArticleCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }
        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly ArticleAdapter Instance = new ArticleAdapter();

        /// <summary>
        /// 增加浏览次数
        /// </summary>
        /// <param name="code">文章code</param>
        public void UpdateViews(string code)
        {
            var sql = @"UPDATE [office].[S_Article] SET [Views] = [Views] + 1 WHERE [Code] = '" + code + "'";

            DbHelper.RunSql(sql, GetConnectionName());
        }

        #region 搜索文章

        /// <summary>
        /// 搜索文章 - 总记录
        /// </summary>
        public int Search(string keyword, string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[S_Article] A
                        WHERE 
	                        [A].[GroupCode] IN 
	                        (
		                        SELECT DISTINCT [G].[Code] FROM [office].[S_Group] G
		                        WHERE 
			                        [G].[IsEnable] = 1 AND 
			                        (
				                        [G].[SendGroupCode] = '' OR 
				                        [G].[SendGroupCode] IN 
				                        (
					                        SELECT DISTINCT [GP].[SendGroupCode] FROM [office].[S_SendGroupPerson] GP 
					                        WHERE 
						                        [GP].[UserCode] = @UserCode
				                        )
			                        )
	                        ) AND 
	                        [A].[Title] LIKE '%' + @Keyword + '%';";

            SqlParameter[] parameters = {
                new SqlParameter("@Keyword", SqlDbType.NVarChar, 20),
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = keyword;
            parameters[1].Value = userCode;

            var count = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(count);
        }

        /// <summary>
        /// 搜索文章 - 当前页数据
        /// </summary>
        public DataTable Search(string keyword, string userCode, int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [A].[CreateTime] DESC) AS [Row], * FROM [office].[S_Article] A
	                        WHERE 
		                        [A].[GroupCode] IN 
		                        (
			                        SELECT DISTINCT [G].[Code] FROM [office].[S_Group] G
			                        WHERE 
				                        [G].[IsEnable] = 1 AND 
				                        (
					                        [G].[SendGroupCode] = '' OR 
					                        [G].[SendGroupCode] IN 
					                        (
						                        SELECT DISTINCT [GP].[SendGroupCode] FROM [office].[S_SendGroupPerson] GP 
						                        WHERE 
							                        [GP].[UserCode] = @UserCode
					                        )
				                        )
		                        ) AND 
		                        [A].[Title] LIKE '%' + @Keyword + '%'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);

            SqlParameter[] parameters = {
                new SqlParameter("@Keyword", SqlDbType.NVarChar, 20),
                new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = keyword;
            parameters[1].Value = userCode;

            return new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        #endregion
    }
}
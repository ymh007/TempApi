using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;

namespace Seagull2.YuanXin.AppApi.Adapter.Share
{
    /// <summary>
    /// 评论适配器
    /// </summary>
    public class CommentAdapter : UpdatableAndLoadableAdapterBase<CommentModel, CommentCollection>
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
        public static readonly CommentAdapter Instance = new CommentAdapter();

        /// <summary>
        /// 获取文章的评论条数
        /// </summary>
        public int GetCount(string articleCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[S_Comment] WHERE [ArticleCode] = '{0}';";
            sql = string.Format(sql, articleCode);

            var count = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());

            return Convert.ToInt32(count);
        }
    }
}
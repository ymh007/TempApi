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
    /// 管理报告浏览次数或点赞次数Adapter
    /// </summary>
    public class ViewOrPraiseCountAdapter : UpdatableAndLoadableAdapterBase<ViewOrPraiseCountModel, ViewOrPraiseCountCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static ViewOrPraiseCountAdapter Instance = new ViewOrPraiseCountAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 根据Url列表查询文章浏览次数
        /// </summary>
        public ViewOrPraiseCountCollection GetViewCountList(List<string> listUrl)
        {
            var sql = @"SELECT [URL], COUNT(0) AS [Count] FROM [dbo].[SITE_VISIT_LOG] WHERE [URL] IN (" + string.Join(",", listUrl) + ") GROUP BY [URL]";

            return QueryData(sql);
        }

        /// <summary>
        /// 根据Url列表查询文章点赞次数
        /// </summary>
        public ViewOrPraiseCountCollection GetPraiseCountList(List<string> listUrl)
        {
            var sql = @"SELECT [ArticleUrl] AS [Url], [PointOfPraise] AS [Count] FROM [dbo].[Table_ArticlePointOfPraise] WHERE [ArticleUrl] IN (" + string.Join(",", listUrl) + ")";

            return QueryData(sql);
        }

    }
}
using MCS.Library.SOA.DataObjects;
using System;
using System.Configuration;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 文章点赞适配器
    /// </summary>
    public class ArticlePointAdapter : UpdatableAndLoadableAdapterBase<ArticlePointModel, ArticlePointCollection>
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["SinooceanLandAddressList"].ConnectionString;

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly ArticlePointAdapter Instance = new ArticlePointAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 获取RelationId
        /// </summary>
        public int GetRelationID(string url)
        {
            try
            {
                string path = "";
                if (url.IndexOf("?") < 0)
                {
                    path = url;
                }
                else
                {
                    path = url.Substring(0, url.IndexOf("?"));
                }
                if (path.Contains("/sites/"))
                {
                    path = path.Substring(path.IndexOf("/sites/"));
                }

                var rowCount = Load(m => m.AppendItem("ArticleUrl", path)).Count;
                if (rowCount <= 0)
                {
                    string sqlInsert = string.Format("INSERT INTO [dbo].[Table_ArticlePointOfPraise] ([PointOfPraise], [ArticleUrl]) VALUES  (0, N'{0}');", path);
                    new SqlDbHelper(connectionString).ExecuteNonQuery(sqlInsert);
                }
                return Load(m => m.AppendItem("ArticleUrl", path)).FirstOrDefault().ID;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取文章点赞数量
        /// </summary>
        public int GetPointCount(int relationId)
        {
            var model = Load(w => w.AppendItem("ID", relationId)).SingleOrDefault();
            if (model != null)
            {
                return model.PointOfPraise;
            }
            return 0;
        }
    }
}
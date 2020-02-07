using MCS.Library.SOA.DataObjects;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Models.InformationNews
{
    /// <summary>
    /// 访问记录适配器
    /// </summary>
    public class VisitLogsAdapter : UpdatableAndLoadableAdapterBase<VisitLogsModel, VisitLogsCollection>
    {
        readonly string connectionString = ConfigurationManager.ConnectionStrings["SinooceanLandAddressList"].ConnectionString;

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly VisitLogsAdapter Instance = new VisitLogsAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 根据标题获取浏览次数
        /// </summary>
        public int GetCountByTitle(string title)
        {
            string sql = "SELECT COUNT(*) FROM [dbo].[SITE_VISIT_LOG] WHERE [TITLE] = @Title;";

            SqlParameter[] parameters = { new SqlParameter("@Title", SqlDbType.NVarChar, 200) };
            parameters[0].Value = title;

            var result = new SqlDbHelper(connectionString).ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 添加访问记录
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="url">地址</param>
        /// <param name="title">标题</param>
        /// <returns>受影响的行数</returns>
        public int AddVisitLog(string userName, string url, string title)
        {
            string sql = "INSERT [dbo].[SITE_VISIT_LOG] ([URL], [USER_NAME], [DROP_TIME], [TITLE], [SOURCE]) VALUES (@Url, @UserName, @DropTime, @Title, @Source);";

            SqlParameter[] parameters = {
                new SqlParameter("@Url", SqlDbType.NVarChar, 250),
                new SqlParameter("@UserName", SqlDbType.NVarChar, 100),
                new SqlParameter("@DropTime", SqlDbType.DateTime),
                new SqlParameter("@Title", SqlDbType.NVarChar, 200),
                new SqlParameter("@Source", SqlDbType.NVarChar, 200) };
            parameters[0].Value = url;
            parameters[1].Value = userName;
            parameters[2].Value = DateTime.Now;
            parameters[3].Value = title;
            parameters[4].Value = "Office-APP";

            return new SqlDbHelper(connectionString).ExecuteNonQuery(sql, CommandType.Text, parameters);
        }
    }
}
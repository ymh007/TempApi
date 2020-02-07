using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using log4net;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.UserFav;

namespace Seagull2.YuanXin.AppApi.Adapter.UserFav
{
    /// <summary>
    /// 用户资讯收藏适配器
    /// </summary>
    public class UserFavInformationAdapter : UpdatableAndLoadableAdapterBase<UserFavInformationModel, UserFavInformationCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.YuanXinBusiness);

        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly UserFavInformationAdapter Instance = new UserFavInformationAdapter();

        /// <summary>
        /// 根据指定类型判断是否收藏
        /// </summary>
        public bool IsFav(string webId, string listId, int listItemId, string type, string userCode)
        {
            return Exists(w =>
            {
                w.AppendItem("WebId", webId);
                w.AppendItem("ListId", listId);
                w.AppendItem("ListItemId", listItemId);
                w.AppendItem("Type", type);
                w.AppendItem("Creator", userCode);
            });
        }

        /// <summary>
        /// 是否收藏（新闻中心、党建新闻）
        /// </summary>
        public bool IsFavNew(string webId, string listId, int listItemId, string userCode)
        {
            var sql = @"SELECT COUNT(*) FROM [office].[UserFavInformation]
                        WHERE
	                        [WebId] = @WebId AND
                            [ListId] = @ListId AND
                            [ListItemId] = @ListItemId AND
	                        [Creator] = @Creator AND
                            ([Type] = '{0}' OR [Type] = '{1}');";
            sql = string.Format(sql,
                Enum.EnumUserFavInformationType.New.ToString(),
                Enum.EnumUserFavInformationType.PartyNew.ToString());

            SqlParameter[] parameters = {
                new SqlParameter("@WebId", SqlDbType.NVarChar, 36),
                new SqlParameter("@ListId", SqlDbType.NVarChar, 36),
                new SqlParameter("@ListItemId", SqlDbType.Int, 4),
                new SqlParameter("@Creator", SqlDbType.NVarChar, 36) };
            parameters[0].Value = webId;
            parameters[1].Value = listId;
            parameters[2].Value = listItemId;
            parameters[3].Value = userCode;

            var result = new SqlDbHelper(ConnectionString).ExecuteScalar(sql, CommandType.Text, parameters);
            var count = Convert.ToInt32(result);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否收藏（重要发文、单位通知、部门通知、会议纪要、党建发文）
        /// </summary>
        public bool IsFav(string webId, string listId, int listItemId, string userCode)
        {
            var sql = @"SELECT COUNT(*) FROM [office].[UserFavInformation]
                        WHERE
	                        [WebId] = @WebId AND
                            [ListId] = @ListId AND
                            [ListItemId] = @ListItemId AND
	                        [Creator] = @Creator AND
                            ([Type] = '{0}' OR [Type] = '{1}' OR [Type] = '{2}' OR [Type] = '{3}' OR [Type] = '{4}');";
            sql = string.Format(sql,
                Enum.EnumUserFavInformationType.Important.ToString(),
                Enum.EnumUserFavInformationType.Unit.ToString(),
                Enum.EnumUserFavInformationType.Department.ToString(),
                Enum.EnumUserFavInformationType.Meeting.ToString(),
                Enum.EnumUserFavInformationType.PartyNotice.ToString());

            SqlParameter[] parameters = {
                new SqlParameter("@WebId", SqlDbType.NVarChar, 36),
                new SqlParameter("@ListId", SqlDbType.NVarChar, 36),
                new SqlParameter("@ListItemId", SqlDbType.Int, 4),
                new SqlParameter("@Creator", SqlDbType.NVarChar, 36) };
            parameters[0].Value = webId;
            parameters[1].Value = listId;
            parameters[2].Value = listItemId;
            parameters[3].Value = userCode;

            var result = new SqlDbHelper(ConnectionString).ExecuteScalar(sql, CommandType.Text, parameters);
            var count = Convert.ToInt32(result);
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除收藏
        /// </summary>
        public void Cancel(string webId, string listId, int listItemId, string type, string userCode)
        {
            Delete(w =>
            {
                w.AppendItem("WebId", webId);
                w.AppendItem("ListId", listId);
                w.AppendItem("ListItemId", listItemId);
                w.AppendItem("Type", type);
                w.AppendItem("Creator", userCode);
            });
        }

        /// <summary>
        /// 获取用户资讯收藏列表 - 总记录数
        /// </summary>
        public int GetList(string userCode, string type)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[UserFavInformation]
                        WHERE
	                        CASE @Type WHEN '' THEN '' ELSE [Type] END = @Type AND
                            [Creator] = @UserCode;";

            SqlParameter[] parameters = { new SqlParameter("@Type", SqlDbType.NVarChar, 20), new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = type;
            parameters[1].Value = userCode;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 获取用户资讯收藏列表 - 当前页数据
        /// </summary>
        public List<UserFavInformationModel> GetList(int pageSize, int pageIndex, string userCode, string type)
        {
            pageIndex--;

            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER(ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[UserFavInformation]
	                        WHERE
		                        CASE @Type WHEN '' THEN '' ELSE [Type] END = @Type AND
                                [Creator] = @UserCode
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1};";
            sql = string.Format(sql, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);

            SqlParameter[] parameters = { new SqlParameter("@Type", SqlDbType.NVarChar, 20), new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = type;
            parameters[1].Value = userCode;

            var result = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);

            return DataConvertHelper<UserFavInformationModel>.ConvertToList(result);
        }
    }
}
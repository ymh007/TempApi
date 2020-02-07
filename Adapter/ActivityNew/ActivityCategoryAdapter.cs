using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{
    /// <summary>
    /// 活动配型适配器
    /// </summary>
    public class ActivityCategoryAdapter : UpdatableAndLoadableAdapterBase<ActivityCategoryModel, ActivityCategoryCollection>
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
        public static readonly ActivityCategoryAdapter Instance = new ActivityCategoryAdapter();

        #region 获取分类列表 - 总记录数
        /// <summary>
        /// 获取活动分类列表 - 总记录数
        /// </summary>
        public int GetList(string name)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[ActivityCategory] WHERE [Name] LIKE '%' + @Name + '%';";

            SqlParameter[] parameters = { new SqlParameter("@Name", SqlDbType.NVarChar, 10) };
            parameters[0].Value = name;

            var result = new SqlDbHelper().ExecuteScalar(sql, CommandType.Text, parameters);
            return Convert.ToInt32(result);
        }
        #endregion

        #region 获取活动分类列表
        /// <summary>
        /// 获取活动分类列表 - 当前页数据
        /// </summary>
        public List<ActivityCategoryModel> GetList(int pageSize, int pageIndex, string name)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [Sort] ASC) AS [Row], * FROM [office].[ActivityCategory] WHERE [Name] LIKE '%' + @Name + '%'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row]  BETWEEN {0} AND {1};";
            sql = string.Format(sql, pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);

            SqlParameter[] parameters = { new SqlParameter("@Name", SqlDbType.NVarChar, 10) };
            parameters[0].Value = name;

            var table = new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
            return DataConvertHelper<ActivityCategoryModel>.ConvertToList(table);
        }
        #endregion

        #region 根据活动编码获取活动分类列表
        /// <summary>
        /// 根据活动编码获取活动分类列表
        /// </summary>
        public DataTable GetCategoryByActivityCode(string activityCode)
        {
            var sql = @"SELECT [Code], [Name], [Icon] FROM [office].[ActivityCategory]
                        WHERE 
	                        [Code] IN
		                        (        
			                        SELECT [ActivityCategoryCode] FROM [office].[ActivityCategoryRecord] WHERE [ActivityCode] = @ActivityCode
		                        );";

            SqlParameter[] parameters = { new SqlParameter("@ActivityCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = activityCode;

            return new SqlDbHelper().ExecuteDataTable(sql, CommandType.Text, parameters);
        }
        #endregion
    }
}
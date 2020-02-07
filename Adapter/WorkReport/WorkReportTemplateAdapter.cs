using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.WorkReport;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.WorkReport
{
    /// <summary>
    /// 模板
    /// </summary>
    public class WorkReportTemplateAdapter : UpdatableAndLoadableAdapterBase<WorkReportTemplateModel, WorkReportTemplateCollection>
    {
        /// <summary>
		/// 实例
		/// </summary>
		public static readonly WorkReportTemplateAdapter Instance = new WorkReportTemplateAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 查询总记录数
        /// </summary>
        public int GetListByPage(string Creator)
        {
            var sql = @"SELECT COUNT(*) FROM [office].[WorkReportTemplate] WHERE  IsSystem=1 OR Creator=@Creator";

            SqlParameter[] parameters = { new SqlParameter("@Creator", SqlDbType.NVarChar, 36) };
            parameters[0].Value = Creator;

            var helper = new SqlDbHelper();
            var result = helper.ExecuteScalar(sql, CommandType.Text, parameters);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 查询当前页数据
        /// </summary>
        public DataTable GetListByPage(int pageIndex, int pageSize, string creator)
        {
            pageIndex--;
            var sql = @"
                WITH [Temp] AS
                (
                    SELECT ROW_NUMBER() OVER(ORDER BY [IsSystem] DESC,[CreateTime] DESC) AS [Row], * FROM [office].[WorkReportTemplate] WHERE IsSystem=1 OR Creator=@Creator
                )
                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize;";

            SqlParameter[] parameters = {
                new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                new SqlParameter("@PageSize", SqlDbType.Int, 4),
                new SqlParameter("@Creator", SqlDbType.NVarChar, 36)
            };
            parameters[0].Value = pageSize * pageIndex + 1;
            parameters[1].Value = pageSize * pageIndex + pageSize;
            parameters[2].Value = creator;

            var helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }


    }
}
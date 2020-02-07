using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.WorkReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.WorkReport
{
    /// <summary>
    /// 工作汇报详情
    /// </summary>
    public class WorkReportDetailsAdapter : UpdatableAndLoadableAdapterBase<WorkReportDetailsModel, WorkReportDetailsCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly WorkReportDetailsAdapter Instance = new WorkReportDetailsAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 查询总记录数
        /// </summary>
        public int GetWorkReportListByPage(string userCode, bool isSender)
        {
            var sql = @"SELECT COUNT(1) FROM [office].[WorkReportDetails] WHERE  IsSender=@IsSender and ReceiveCode=@ReceiveCode";



            SqlParameter[] parameters = {
                new SqlParameter("@IsSender", SqlDbType.Bit),
                new SqlParameter("@ReceiveCode", SqlDbType.NVarChar, 36),
            };

            parameters[0].Value = isSender;
            parameters[1].Value = userCode;
            var helper = new SqlDbHelper();
            var result = helper.ExecuteScalar(sql, CommandType.Text, parameters);

            return Convert.ToInt32(result);
        }

        /// <summary>
        /// 查询当前页数据
        /// </summary>
        public DataTable GetWorkReportListByPage(int pageIndex, int pageSize, string userCode, bool isSender)
        {
            pageIndex--;
            var sql = @"
                WITH [Temp] AS
                (
                    SELECT ROW_NUMBER() OVER(ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[WorkReportDetails] WHERE  IsSender=@IsSender and ReceiveCode=@ReceiveCode
                )
                SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize;";

            SqlParameter[] parameters = {
                new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                new SqlParameter("@PageSize", SqlDbType.Int, 4),
                  new SqlParameter("@IsSender", SqlDbType.Bit),
                new SqlParameter("@ReceiveCode", SqlDbType.NVarChar, 36)
            };
            parameters[0].Value = pageSize * pageIndex + 1;
            parameters[1].Value = pageSize * pageIndex + pageSize;
            parameters[2].Value = isSender;
            parameters[3].Value = userCode;

            var helper = new SqlDbHelper();
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }



        /// <summary>
        /// 查询导出数据
        /// </summary>
        public DataTable ExportWorkReport(string userCode, DateTime start, DateTime end)
        {
            SqlDbHelper helper = new SqlDbHelper();
            var sql = $"SELECT Code,IsSender,ReportCode,CreateName,ReceiveName,CopyDetails,Title,Mark,CreateTime FROM [office].[WorkReportDetails] WHERE ReceiveCode='{userCode}' and  CreateTime between '{start}' and '{end}'  ORDER BY  CreateTime DESC";
            return helper.ExecuteDataTable(sql, CommandType.Text);
        }
    }
}
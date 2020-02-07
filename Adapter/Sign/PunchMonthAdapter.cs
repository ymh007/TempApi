using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter.Sign
{
    /// <summary>
    /// 我的考勤适配器
    /// </summary>
    public class PunchMonthAdapter : UpdatableAndLoadableAdapterBase<PunchMonth, PunchMonthCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly PunchMonthAdapter Instance = new PunchMonthAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }

        #region 按月查询打卡记录
        /// <summary>
        ///按月查询打卡记录
        /// </summary>
        /// <param name="Creator">userID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="sort">ASC：签到、DESC：签退</param>
        public PunchMonthCollection LoadDataOfMonth(string Creator, string startTime, string endTime, string sort)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append("SELECT B.*, C.[Address] FROM");
                strSql.Append("(");
                strSql.Append(" SELECT ROW_NUMBER() OVER (PARTITION BY CONVERT(VARCHAR(10), A.[CreateTime], 120) ORDER BY A.[CreateTime] " + sort + ") AS RowNo, A.[CreateTime], A.[MapUrl], A.[StandardPunchCode]");
                strSql.Append(" FROM EmployeeServices A");
                strSql.Append(" WHERE A.[Creator]='" + Creator + "' AND A.[CreateTime] BETWEEN '" + startTime + "' AND '" + endTime + "' AND DATEPART(HH,A.[CreateTime])" + (sort == "ASC" ? "<" : ">=") + "12");
                strSql.Append(") B LEFT JOIN [dbo].[StandardPunch] C ON B.[StandardPunchCode]=C.[Code] ");
                strSql.Append("WHERE B.[RowNo]=1 ORDER BY B.[CreateTime] ASC");

                return QueryData(strSql.ToString());
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw;
            }
        }
        #endregion
    }
}
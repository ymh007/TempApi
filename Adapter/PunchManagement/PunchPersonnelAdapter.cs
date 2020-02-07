using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.PunchManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.PunchManagement
{
    /// <summary>
    /// 打卡部门人员apdapter
    /// </summary>
    public class PunchPersonnelAdapter
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PunchPersonnelAdapter Instance = new PunchPersonnelAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.EmployeeAttendance);

        /// <summary>
        /// 更新打卡管理单元人员
        /// </summary>
        public void UpdatePserson(string punchManagementCode)
        {
            try
            {
                SqlParameter[] paramerers = { new SqlParameter("@punchManagementCode", SqlDbType.NVarChar, 36) };
                paramerers[0].Value = punchManagementCode;

                var helper = new SqlDbHelper(ConnectionString);
                helper.ExecuteNonQuery("[dbo].[ProcPunchManagementConvertToUsers]", CommandType.StoredProcedure, paramerers);
            }
            catch (Exception e)
            {
                Log.WriteLog($"更新打卡管理单元人员 - 失败：{e.Message}");
            }
        }

        /// <summary>
        /// 删除打卡管理单元人员
        /// </summary>
        public void Delete(string punchManagementCode)
        {
            try
            {
                SqlParameter[] paramerers = { new SqlParameter("@punchManagementCode", SqlDbType.NVarChar, 36) };
                paramerers[0].Value = punchManagementCode;

                var helper = new SqlDbHelper(ConnectionString);
                helper.ExecuteNonQuery("DELETE FROM [dbo].[PunchPersonnel] WHERE [PunchCode] = @punchManagementCode", CommandType.Text, paramerers);
            }
            catch (Exception e)
            {
                Log.WriteLog($"删除打卡管理单元人员 - 失败：{e.Message}");
            }
        }
    }
}
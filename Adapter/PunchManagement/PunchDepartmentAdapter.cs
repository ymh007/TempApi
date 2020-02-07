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
    /// 打卡部门adapter
    /// </summary>
    public class PunchDepartmentAdapter : UpdatableAndLoadableAdapterBase<PunchDepartmentModel, PunchDepartmentCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PunchDepartmentAdapter Instance = new PunchDepartmentAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.EmployeeAttendance);

        /// <summary>
        /// 获取所有考勤组织及人员列表
        /// </summary>
        public PunchDepartmentCollection GetList()
        {
            return base.Load(w => { w.AppendItem("ValidStatus", true); });
        }

        /// <summary>
        /// 根据打卡管理单元编码获取考勤组织及人员列表
        /// </summary>
        public PunchDepartmentCollection GetList(string managementCode)
        {
            return Load(w => w.AppendItem("PunchCode", managementCode));
        }

        /// <summary>
        /// 根据打卡管理单元编码获取考勤组织及人员列表
        /// </summary>
        public DataTable GetListByManagementCode(string code)
        {
            var sql = @"SELECT * FROM [dbo].[PunchDepartment] WHERE [PunchCode] = @ManagementCode;";

            SqlParameter[] parameters = {
                new SqlParameter("@ManagementCode", SqlDbType.NVarChar, 36),
            };
            parameters[0].Value = code;

            var helper = new SqlDbHelper(ConnectionString);
            return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 判断打卡管理单元组合是否存在
        /// </summary>
        public bool IsExist(string managementCode, List<string> departCodeList)
        {
            var sql = @"
                SELECT COUNT(*) FROM
                (
	                SELECT [Code], STUFF((SELECT ',' + [B].[ConcatCode] FROM [dbo].[PunchDepartment] B WHERE [B].[PunchCode] = [A].Code ORDER BY [B].[ConcatCode] ASC FOR XML PATH('')), 1, 1, '') [DepartCodeList] FROM [dbo].[PunchManagement] A
                ) B
                WHERE [B].[Code] <> @ManagementCode AND [B].[DepartCodeList] = @DepartCodeList;";

            SqlParameter[] parameters = {
                new SqlParameter("@ManagementCode", SqlDbType.NVarChar, 36),
                new SqlParameter("@DepartCodeList", SqlDbType.NVarChar, 4000),
            };
            parameters[0].Value = managementCode;
            parameters[1].Value = string.Join(",", departCodeList);

            var helper = new SqlDbHelper(ConnectionString);
            var result = helper.ExecuteScalar(sql, CommandType.Text, parameters);
            var count = Convert.ToInt16(result);
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
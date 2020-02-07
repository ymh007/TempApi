using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using SinoOcean.Seagull2.Framework.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Seagull2.YuanXin.AppApi.Services.MyMessage;
using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Services
{
    /// <summary>
    /// 员工服务
    /// </summary>
    public class EmployeeService : UpdatableAndLoadableAdapterBase<Employee, EmployeeCollection>, IEmployeeService
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly EmployeeService Instance = new EmployeeService();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SubjectDB_EmpService;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.SubjectDB_EmpService);

        /// <summary>
        /// 获取员工信息
        /// </summary>
        public string LoadEmployee(string Id)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM EmpService.Employee WHERE Code ='{0}' AND ValidStatus = 1 AND VersionEndTime IS NULL ", Id);

                string list = null;

                foreach (var item in QueryData(sql))
                {
                    list = item.PhotoID;
                }
                MyInfoimage myinfoimage = new MyInfoimage();
                string Code = myinfoimage.LoadByResourceID(list);
                return Code;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 获取入职日期及出生日期
        /// </summary>
        public DataRow GetEmployedDateAndBirthday(string userCode)
        {
            var sql = "SELECT [E].[EmployedDate], [P].[Birthday] FROM [EmpService].[Employee] E JOIN [Common].[Person] P ON [E].[PersonCode] = [P].[Code] AND [E].[VersionEndTime] IS NULL AND [P].[VersionEndTime] IS NULL WHERE [E].[Code] = @UserCode;";

            SqlParameter[] parameters = { new SqlParameter("@UserCode", SqlDbType.NVarChar, 36) };
            parameters[0].Value = userCode;

            var helper = new SqlDbHelper(ConnectionString);
            var dt = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
        }
    }
}
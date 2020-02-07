using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using Seagull2.Owin.Organization.Services;
using System.Threading.Tasks;
using MCS.Library.Core;
using System.Data;
using System.Data.SqlClient;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.Core.Models;
using Seagull2.Permission;
using Seagull2.Permission.Organization;
using SinoOcean.Seagull2.Framework.MasterData;
using System.Diagnostics;


namespace Seagull2.YuanXin.AppApi.Services
{
    public class PunchService : UpdatableAndLoadableAdapterBase<EmployeeServicesModel, EmployeeServicesCollection>, IPunchService
    {
        protected override string GetConnectionName()
        {
            return "EmployeeService";
        }

        #region 查询详细打卡数据
        /// <summary>
        /// 查询详细打卡数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Sort"></param>
        /// <param name="Creator"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EmployeeServicesModel>> LoadCompareList(int page, string Sort, string Creator, string startTime, string endTime)
        {
            int pageSize = 5;
            int taskPageSize = pageSize * page;
            try
            {
                string sql = string.Format(@"SELECT CnName ,CreateTime from
                                (
                                SELECT Rank() over(PARTITION BY  Convert ( VARCHAR(10),  CreateTime,  120),CnName ORDER BY CreateTime {0} ) as rowno,  t.* FROM EmployeeServices t
                                WHERE  EnName='{1}' and  CreateTime BETWEEN '{2}' AND '{3}'
                                ) aa
                                WHERE rowno=1 ORDER BY CreateTime  OFFSET {4} ROW FETCH NEXT {5} ROWS ONLY", Sort, Creator, startTime, endTime, taskPageSize, pageSize);

                List<EmployeeServicesModel> list = new List<EmployeeServicesModel>();
                foreach (var item in this.QueryData(sql))
                {
                    list.Add(item);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 

        #endregion

        #region 查询原始数据
        /// <summary>
        /// 查询原始数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="Creator"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EmployeeServicesModel>> LoadData(int page, string Creator, string startTime, string endTime)
        {
            int pageSize = 5;
            int taskPageSize = pageSize * page;
            try
            {
                string sql = string.Format(@"SELECT EnName, CreateTime FROM dbo.EmployeeServices e 
	WHERE e.Creator='{0}' AND  e.CreateTime BETWEEN '{1}' AND  '{2}' ORDER BY CreateTime DESC 
	OFFSET {3} ROW FETCH NEXT {4} ROWS ONLY	", Creator, startTime, endTime, taskPageSize, pageSize);
                List<EmployeeServicesModel> list = new List<EmployeeServicesModel>();
                foreach (var item in this.QueryData(sql))
                {
                    list.Add(item);
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw ex;
            }
        }



        #endregion
    }
}
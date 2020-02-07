using Seagull2.YuanXin.AppApi.Models;
using SinoOcean.Seagull2.Framework.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Services
{
    public interface IPunchService
    {
        #region 查询已统计的打卡记录
        /// <summary>
        /// 查询已统计的打卡记录
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="Sort">排序方式</param>
        /// <param name="CnName">中文名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        Task<IEnumerable<EmployeeServicesModel>> LoadCompareList(int page, string Sort, string Creator, string startTime, string endTime); 
        #endregion

        #region 查询原始打卡记录
        /// <summary>
        ///查询原始打卡记录
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="CnName">中文姓名</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        Task<IEnumerable<EmployeeServicesModel>> LoadData(int page, string Creator, string startTime, string endTime); 
        #endregion
    }
}
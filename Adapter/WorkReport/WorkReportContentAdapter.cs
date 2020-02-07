using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.WorkReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.WorkReport
{
    /// <summary>
    /// 工作汇报内容
    /// </summary>
    public class WorkReportContentAdapter : UpdatableAndLoadableAdapterBase<WorkReportContentModel, WorkReportContentCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly WorkReportContentAdapter Instance = new WorkReportContentAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;


        /// <summary>
        /// 获取某个时间段内的所有工作汇报内容 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public WorkReportContentCollection GetContents(DateTime start, DateTime end)
        {
            string sql = $"SELECT ReportCode,Content,Title FROM  office.WorkReportContent where CreateTime between '{start}' and '{end}' ";
            return this.QueryData(sql);
        }
    }

}
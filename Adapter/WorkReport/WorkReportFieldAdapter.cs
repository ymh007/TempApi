using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.WorkReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.WorkReport
{
    /// <summary>
    /// 模板字符
    /// </summary>
    public class WorkReportFieldAdapter : UpdatableAndLoadableAdapterBase<WorkReportFieldModel, WorkReportFieldCollection>
    {
        /// <summary>
		/// 实例
		/// </summary>
		public static readonly WorkReportFieldAdapter Instance = new WorkReportFieldAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;
    }
}
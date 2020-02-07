using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 业绩报表用户关注菜单适配器
    /// Powered By: v-sunzhh
    /// Date: 2017-05-10
    /// </summary>
    public class PerformanceReportUserFocusAdapter : UpdatableAndLoadableAdapterBase<PerformanceReportUserFocusModel, PerformanceReportUserFocusCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly PerformanceReportUserFocusAdapter Instance = new PerformanceReportUserFocusAdapter();

        
    }
}
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.PunchManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.PunchManagement
{
    /// <summary>
    /// 打卡管理员adapter
    /// </summary>
    public class PunchManagerAdapter : UpdatableAndLoadableAdapterBase<PunchManagerModel, PunchManagerCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PunchManagerAdapter Instance = new PunchManagerAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.EmployeeAttendance;
        }
    }
}
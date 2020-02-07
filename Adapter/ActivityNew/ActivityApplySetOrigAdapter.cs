using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Text;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{
    /// <summary>
    /// 活动参加范围原始设置适配器
    /// </summary>
    public class ActivityApplySetOrigAdapter : UpdatableAndLoadableAdapterBase<ActivityApplySetOrigModel, ActivityApplySetOrigCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }
        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly ActivityApplySetOrigAdapter Instance = new ActivityApplySetOrigAdapter();
    }
}
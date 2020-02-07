using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

namespace Seagull2.YuanXin.AppApi.Adapter.ActivityNew
{
    /// <summary>
    /// 活动分类关联 Adapter
    /// </summary>
    public class ActivityCategoryRecordAdapter : UpdatableAndLoadableAdapterBase<ActivityCategoryRecordModel, ActivityCategoryRecordCollection>
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
        public static readonly ActivityCategoryRecordAdapter Instance = new ActivityCategoryRecordAdapter();
    }
}
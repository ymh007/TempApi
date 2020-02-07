using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Share
{
    /// <summary>
    /// 发送范围群组适配器
    /// </summary>
    public class SendGroupAdapter : UpdatableAndLoadableAdapterBase<SendGroupModel, SendGroupCollection>
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
        log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly SendGroupAdapter Instance = new SendGroupAdapter();

    }
}
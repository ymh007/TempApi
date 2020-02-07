using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Channel;
using Seagull2.YuanXin.AppApi.Models;
using log4net;
using System.Reflection;

namespace Seagull2.YuanXin.AppApi.Adapter.Channel
{
    /// <summary>
    /// 推荐频道适配器
    /// </summary>
    public class ChannelFavAdapter : UpdatableAndLoadableAdapterBase<ChannelFavModel,ChannelFavCollection>
    {
        /// <summary>
        /// 适配器实例化
        /// </summary>
        public static readonly ChannelFavAdapter Instance = new ChannelFavAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 
    }
}
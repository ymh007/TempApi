using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Greetings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Greetings
{
    /// <summary>
    /// 
    /// </summary>
    public class GreetingsContentAdapter : UpdatableAndLoadableAdapterBase<GreetingsContentModel, GreetingsContentCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly GreetingsContentAdapter Instance = new GreetingsContentAdapter();
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

    }
}
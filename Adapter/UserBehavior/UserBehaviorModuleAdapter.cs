using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 用户行为模块适配器
    /// </summary>
    public class UserBehaviorModuleAdapter : UpdatableAndLoadableAdapterBase<UserBehaviorModuleModel, UserBehaviorModuleCollection>
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
        public static readonly UserBehaviorModuleAdapter Instance = new UserBehaviorModuleAdapter();
    }
}
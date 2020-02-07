using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 用户行为模块表
    /// </summary>
    [ORTableMapping("office.UserBehaviorModule")]
    public class UserBehaviorModuleModel : BaseModel
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 用户行为模块集合
    /// </summary>
    public class UserBehaviorModuleCollection : EditableDataObjectCollectionBase<UserBehaviorModuleModel>
    {

    }
}
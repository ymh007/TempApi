using System;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 用户行为日志表
    /// </summary>
    [ORTableMapping("office.UserBehaviorLog")]
    public class UserBehaviorLogModel
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        [ORFieldMapping("UserAccount")]
        public string UserAccount { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
        /// <summary>
        /// 部门编号
        /// </summary>
        [ORFieldMapping("DepartmentCode")]
        public string DepartmentCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [ORFieldMapping("DepartmentName")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// 模块编号
        /// </summary>
        [ORFieldMapping("ModuleCode")]
        public string ModuleCode { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [NoMapping]
        public string ModuleName { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("TimeStart")]
        public DateTime TimeStart { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary> 
        [ORFieldMapping("TimeEnd")]
        public DateTime TimeEnd { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [ORFieldMapping("Modifier")]
        public string Modifier { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 用时
        /// </summary>
        [NoMapping]
        public int UseTime { get; set; }
        /// <summary>
        /// 有效状态
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 用户行为日志集合
    /// </summary>
    public class UserBehaviorLogCollection : EditableDataObjectCollectionBase<UserBehaviorLogModel>
    {

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Version
{
    /// <summary>
    /// 系统升级记录 Model
    /// </summary>
    [ORTableMapping("OAuth.SystemUpdateRecord")]
    public class SystemUpdateRecordModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 设备ID(商家、移动办公设备ID)
        /// </summary>
        [ORFieldMapping("AppId")]
        public string AppId { get; set; }

        /// <summary>
        /// 系统(android,ios)
        /// </summary>
        [ORFieldMapping("System")]
        public string System { set; get; }

        /// <summary>
        /// 版本号
        /// </summary>
        [ORFieldMapping("Version")]
        public string Version { get; set; }

        /// <summary>
        /// 更新内容
        /// </summary>
        [ORFieldMapping("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        [ORFieldMapping("DownLoadUrl")]
        public string DownLoadUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [ORFieldMapping("ModifyTime")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [ORFieldMapping("Modifier")]
        public string Modifier { get; set; }

        /// <summary>
        /// 是否可用
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 系统类别（android,ios）
    /// </summary>
    public enum SystemUpdateRecordSystem
    {
        /// <summary>
        /// android
        /// </summary>
        android,
        /// <summary>
        /// ios
        /// </summary>
        ios
    }

    /// <summary>
    /// 系统升级记录 Collection
    /// </summary>
    public class SystemUpdateRecordCollection : EditableDataObjectCollectionBase<SystemUpdateRecordModel>
    {

    }
}
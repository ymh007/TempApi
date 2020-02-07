using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.ShareFile
{
    /// <summary>
    /// 共享文件存储记录 Model
    /// </summary>
    [ORTableMapping("office.ShareFile")]
    public class ShareFileModel : BaseModel
    {
        /// <summary>
        /// 模块标识
        /// </summary>
        [ORFieldMapping("Module")]
        public string Module { get; set; }

        /// <summary>
        /// 目标编码（需要时传入）
        /// </summary>
        [ORFieldMapping("TargetCode")]
        public string TargetCode { get; set; }

        /// <summary>
        /// 文件类型（image,flash,media,file等）
        /// </summary>
        [ORFieldMapping("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 存储路径
        /// </summary>
        [ORFieldMapping("Path")]
        public string Path { get; set; }

        /// <summary>
        /// 文件完整路径
        /// </summary>
        [NoMapping]
        public string PathFull
        {
            get
            {
                return FileService.DownloadFile(Path);
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { set; get; }
    }

    /// <summary>
    /// 共享文件存储记录 Collection
    /// </summary>
    public class ShareFileCollection : EditableDataObjectCollectionBase<ShareFileModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 众筹项目
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.Project")]
    public class Project
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 项目简介
        /// </summary>
        [ORFieldMapping("Summary")]
        public string Summary { get; set; }

        /// <summary>
        /// 封面图片
        /// </summary>
        [ORFieldMapping("CoverImg")]
        public string CoverImg { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [ORFieldMapping("City")]
        public string City { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [ORFieldMapping("Address")]
        public string Address { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [NoMapping]
        public string CreateTimeStr
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [NoMapping]
        public string StartTimeStr
        {
            get
            {
                return StartTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }
    /// <summary>
    /// 众筹项目集合
    /// </summary>
    [Serializable]
    public class ProjectCollection : EditableDataObjectCollectionBase<Project>
    {

    }
}
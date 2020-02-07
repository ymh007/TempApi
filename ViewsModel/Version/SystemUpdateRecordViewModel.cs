using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Models.Version;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Version
{
    /// <summary>
    /// 系统升级记录基本
    /// </summary>
    public abstract class SystemUpdateRecordBase
    {
        /// <summary>
        /// 记录编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 系统
        /// </summary>
        public string System { set; get; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }
    }

    /// <summary>
    /// 系统升级记录保存 ViewModel
    /// </summary>
    public class SystemUpdateRecordSaveViewModel : SystemUpdateRecordBase
    {
        /// <summary>
        /// Android下载地址
        /// </summary>
        public string DownLoadUrlAndroid { set; get; }
        /// <summary>
        /// IOS下载地址
        /// </summary>
        public string DownLoadUrlIos { set; get; }
    }

    /// <summary>
    /// 系统升级记录列表 PC ViewModel
    /// </summary>
    public class SystemUpdateRecordPCListViewModel : SystemUpdateRecordBase
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownLoadUrl { set; get; }
        /// <summary>
        /// 发布时间 JsonIgnore
        /// </summary>
        [JsonIgnore]
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string CreateTimeStr
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 描述，过滤HTML标签并截取指定长度
        /// </summary>
        public string DescriptionShort
        {
            get
            {
                var desc = Regex.Replace(base.Description, "<[^>]*>", "");
                var len = 20;
                if (desc.Length < 20)
                {
                    return desc;
                }
                else
                {
                    return desc.Substring(0, len - 1) + "...";
                }
            }
        }
    }

    /// <summary>
    /// 系统升级记录详情 PC ViewModel
    /// </summary>
    public class SystemUpdateRecordPCDetailViewModel : SystemUpdateRecordBase
    {
        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownLoadUrl { set; get; }
    }

    /// <summary>
    /// 最新版本信息
    /// </summary>
    public class SystemUpdateRecordAppNewViewModel
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }
        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownLoadUrl { set; get; }
    }
}
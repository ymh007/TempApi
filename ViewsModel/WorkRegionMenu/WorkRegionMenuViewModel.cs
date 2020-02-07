using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.WorkRegionMenu
{
    /// <summary>
    /// 菜单ViewModel
    /// </summary>
    public class WorkRegionMenuViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 事件
        /// </summary>
        public string Event { set; get; }
        /// <summary>
        /// 图标Url
        /// </summary>
        public string IcoUrl { set; get; }
        /// <summary>
        /// 图标Url，网络路径
        /// </summary>
        public string IcoUrl_
        {
            get
            {
                return FileService.DownloadFile(IcoUrl);
            }
        }
        /// <summary>
        /// 分类编号
        /// </summary>
        public int CategoryId { set; get; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName
        {
            get
            {
                return Models.ModelExtention.GetEnumDescriptionByValue<Enum.EnumWorkRegionMenuCategory>(CategoryId);
            }
        }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool Visible { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        /// APP版本类别（旧版：oldVersion、新版：newVersion）
        /// </summary>
        public string VersionType { set; get; }

        /// <summary>
        /// ‘推’ 或 ‘新’  图标
        /// </summary>
        public string RecommendIco { get; set; }

        /// <summary>
        /// ‘appkey
        /// </summary>
        public string AppKey { get; set; }
    }

    /// <summary>
    /// 工作通菜单列表 - APP
    /// </summary>
    public class WorkRegionMenuAPPViewModel
    {
        /// <summary>
        /// 分类编号
        /// </summary>
        public int CategoryId { set; get; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { set; get; }
        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<WorkRegionMenuViewModel> Menus { set; get; }
    }
    /// <summary>
    ///  推荐新 标识
    /// </summary>
    public class WorkRegionMenuRecomm
    {
    
      /// <summary>
      /// code
      /// </summary>
        public int Code { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string RecommendIco { set; get; }
        /// <summary>
        /// 类别
        /// </summary>
        public int RecommendType { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { set; get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.AppConfig
{
    /// <summary>
    /// 首页轮播图 ViewModel
    /// </summary>
    public class AppHomeSlideViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 图片Url
        /// </summary>
        public string ImageUrl { set; get; }
        /// <summary>
        /// 图片Url，网络路径
        /// </summary>
        public string ImageUrl_
        {
            get
            {
                return FileService.DownloadFile(ImageUrl);
            }
        }
        /// <summary>
        /// 事件类型编码
        /// </summary>
        public int Type { set; get; }
        /// <summary>
        /// 事件类型名称
        /// </summary>
        public string TypeName
        {
            get
            {
                return Models.ModelExtention.GetEnumDescriptionByValue<Enum.EnumAppHomeSlideType>(Type);
            }
        }
        /// <summary>
        /// 事件内容
        /// </summary>
        public string Event { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
    }
}
using System;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.AppConfig
{
    /// <summary>
    /// 首页轮播图
    /// </summary>
    [ORTableMapping("office.AppHomeSlide")]
    public class AppHomeSlideModel : BaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        [ORFieldMapping("ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 事件类型编码
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }

        /// <summary>
        /// 事件内容
        /// </summary>
        [ORFieldMapping("Event")]
        public string Event { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }
    }

    /// <summary>
    /// 首页轮播图集合
    /// </summary>
    public class AppHomeSlideCollection : EditableDataObjectCollectionBase<AppHomeSlideModel>
    {

    }
}
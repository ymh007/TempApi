using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System.Collections.Generic;

namespace Seagull2.YuanXin.AppApi.Models.WorkRegionMenu
{
    /// <summary>
    /// 工作通菜单表
    /// </summary>
    [ORTableMapping("office.WorkRegionMenu")]
    public class WorkRegionMenuModel : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public string Type { get; set; }

        /// <summary>
        /// 事件名称或Url地址
        /// </summary>
        [ORFieldMapping("Event")]
        public string Event { get; set; }

        /// <summary>
        /// 图标Url
        /// </summary>
        [ORFieldMapping("IcoUrl")]
        public string IcoUrl { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        [ORFieldMapping("CategoryId")]
        public int CategoryId { set; get; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        [ORFieldMapping("IsTop")]
        public bool IsTop { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [ORFieldMapping("Sort")]
        public int Sort { get; set; }

        /// <summary>
        /// APP版本类别（旧版：oldVersion、新版：newVersion）
        /// </summary>
        [ORFieldMapping("VersionType")]
        public string VersionType { set; get; }

        /// <summary>
        /// ‘推’ 或 ‘新’  图标
        /// </summary>
        [ORFieldMapping("RecommendIco")]
        public string RecommendIco { get; set; }

        /// <summary>
        /// ‘appkey
        /// </summary>
        [ORFieldMapping("AppKey")]
        public string AppKey { get; set; }


        /// <summary>
        /// 权限数据
        /// </summary>
        [NoMapping]
        public List<object> PermissionData { get; set; }


        /// <summary>
        /// 是否保存权限数据
        /// </summary>
        [NoMapping]
        public bool IsSavePermission { get; set; }
    }

  
    /// <summary>
    /// 工作通菜单集合
    /// </summary>
    public class WorkRegionMenuCollection : EditableDataObjectCollectionBase<WorkRegionMenuModel>
    {

    }

  
}
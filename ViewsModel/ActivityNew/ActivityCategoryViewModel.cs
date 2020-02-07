using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew
{
    /// <summary>
    /// 活动分类基类
    /// </summary>
    public abstract class ActivityCategoryBase
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标地址
        /// </summary>
        public string Icon
        {
            set { _Icon = value; }
            get
            {
                return FileService.DownloadFile(_Icon);
            }
        }
        private string _Icon;

    }

    #region 活动分类保存 ViewModel
    /// <summary>
    /// 活动分类保存 ViewModel
    /// </summary>
    public class ActivityCategorySaveViewModel : ActivityCategoryBase
    {
        /// <summary>
        /// 图标地址
        /// </summary>
        public new string Icon { set; get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
        /// <summary>
        /// 排序数字
        /// </summary>
        public int Sort { set; get; }
    }
    #endregion

    #region 活动分类 PC ViewModel
    /// <summary>
    /// 活动分类 PC ViewModel
    /// </summary>
    public class ActivityCategoryViewModel : ActivityCategoryBase
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { set; get; }
        /// <summary>
        /// 排序数字
        /// </summary>
        public int Sort { set; get; }
    }
    #endregion

    #region 活动分类 APP ViewModel
    /// <summary>
    /// 活动分类 APP ViewModel
    /// </summary>
    public class ActivityCategoryForAppViewModel : ActivityCategoryBase
    {

    }
    #endregion
}
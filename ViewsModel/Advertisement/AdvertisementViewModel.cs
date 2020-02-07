using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Advertisement
{
    #region 广告保存 ViewModel
    /// <summary>
    /// 广告保存 ViewModel
    /// </summary>
    public class SaveViewModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public Enum.EnumAdvertisementType Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图像
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 有效开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 有效结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


        /// <summary>
        /// 权限数据
        /// </summary>
        public List<object> PermissionData { get; set; }

        /// <summary>
        /// 是否保存权限数据
        /// </summary>
        public bool IsSavePermission { get; set; }


        /// <summary>
        /// 是否是视频
        /// </summary>
        public bool IsVideo { get; set; }

        /// <summary>
        /// 是否保存视频数据
        /// </summary>
        public bool IsUpdateVideo { get; set; }

        public string VideoUrl { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackGroundImg { get; set; }


        /// <summary>
        /// 点击
        /// </summary>
        public int ClickType { get; set; }
    }




    #endregion

    #region PC端获取广告列表 ViewModel
    /// <summary>
    /// PC端获取广告列表 ViewModel
    /// </summary>
    public class GetListForPC : SaveViewModel
    {
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public new string StartTime { get; set; }
        /// <summary>
        /// 有效结束时间
        /// </summary>
        public new string EndTime { set; get; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName
        {
            get
            {
                return Models.ModelExtention.GetEnumDescriptionByValue<Enum.EnumAdvertisementType>((int)base.Type);
            }
        }


        /// <summary>
        /// 是否视频
        /// </summary>
        public bool IsVideo { get; set; }
    }
    #endregion

    #region APP端
    /// <summary>
    /// APP端
    /// </summary>
    public class GetListForAPP
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 广告图片
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// 链接新地址地址
        /// </summary>
        public string NewLink { get; set; }

        /// <summary>
        /// 视频链接
        /// </summary>
        public string Video { get; set; }

        public DateTime CreateTime { get; set; }
    }
    #endregion
}
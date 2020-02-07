using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Enum;

namespace Seagull2.YuanXin.AppApi.ViewsModel.UserFav
{
    /// <summary>
    /// 用户资讯收藏基类 ViewModel
    /// </summary>
    public abstract class UserFavInformationBaseViewModel
    {
        /// <summary>
        /// WebId
        /// </summary>
        public string WebId;
        /// <summary>
        /// ListId
        /// </summary>
        public string ListId;
        /// <summary>
        /// ListItemId
        /// </summary>
        public int ListItemId;
    }
    /// <summary>
    /// 用户资讯收藏添加 ViewModel
    /// </summary>
    public class UserFavInformationAddViewModel : UserFavInformationBaseViewModel
    {
        /// <summary>
        /// 资讯类型
        /// </summary>
        public EnumUserFavInformationType Type;
    }

    /// <summary>
    /// 用户资讯收藏列表 ViewModel
    /// </summary>
    public class UserFavInformationListViewModel : UserFavInformationBaseViewModel
    {
        /// <summary>
        /// 收藏编码
        /// </summary>
        public string Code;
        /// <summary>
        /// 资讯类型
        /// </summary>
        public string Type;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 来源
        /// </summary>
        public string Source;
        /// <summary>
        /// 发布时间
        /// </summary>
        public string ReleaseTime;
    }

    /// <summary>
    /// 用户资讯收藏批量取消 ViewModel
    /// </summary>
    public class UserFavInformationCancelViewModel
    {
        /// <summary>
        /// 收藏编码列表
        /// </summary>
        public List<string> CodeList;
    }
}
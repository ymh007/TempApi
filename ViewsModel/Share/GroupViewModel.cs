using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Share
{
    #region 图文保存
    /// <summary>
    /// 图文保存
    /// </summary>
    public class GroupPostViewModel
    {
        /// <summary>
        /// 图文组Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsEnable { set; get; }
        /// <summary>
        /// 发送范围群组编码
        /// </summary>
        public string SendGroupCode { set; get; }
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string MenuCode { set; get; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<ArticlePostViewModel> Articles { set; get; }
    }
    #endregion

    #region 图文列表 - PC
    /// <summary>
    /// 图文列表 - PC
    /// </summary>
    public class GroupForPCViewModel
    {
        SendGroupCollection sendGroups = null;
        MenuCollection menus = null;
        ArticleCollection articles = null;

        /// <summary>
        /// 传入基础数据
        /// 实例化对象之前就将需要的数据传入，避免每次实例化都查询数据库
        /// </summary>
        /// <param name="sgc">发送范围群组集合</param>
        /// <param name="mc">菜单集合</param>
        /// <param name="ac">文章集合</param>
        public GroupForPCViewModel(SendGroupCollection sgc, MenuCollection mc, ArticleCollection ac)
        {
            sendGroups = sgc;
            menus = mc;
            articles = ac;
        }

        /// <summary>
        /// 图文组Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsEnable { set; get; }
        /// <summary>
        /// 发送范围群组编码
        /// </summary>
        public string SendGroupCode { set; get; }
        /// <summary>
        /// 发送范围群组名称
        /// </summary>
        public string SendGroupName
        {
            get
            {
                //var sendGroupCollection = CacheService.Get("Share_SendGroupCollection", () => { return SendGroupAdapter.Instance.Load(w => { }); });
                if (string.IsNullOrWhiteSpace(SendGroupCode))
                {
                    return "全部用户";
                }
                var sendGroup = sendGroups.Find(f => f.Code == this.SendGroupCode);
                if (sendGroup == null)
                {
                    return "该群组已删除";
                }
                return sendGroup.Name;
            }
        }
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string MenuCode { set; get; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MenuCode))
                {
                    return "暂无设置菜单";
                }
                var menu = menus.Find(f => f.Code == this.MenuCode);
                if (menu == null)
                {
                    return "该菜单已删除";
                }
                if (string.IsNullOrWhiteSpace(menu.ParentCode))
                {
                    return menu.Name;
                }
                var parentMenu = menus.Find(f => f.Code == menu.ParentCode);
                if (parentMenu == null)
                {
                    return "无父菜单 - " + menu.Name;
                }
                return parentMenu.Name + " - " + menus[0].Name;
            }
        }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateTime { set; get; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<ArticleForPCViewModel> Articles
        {
            get
            {
                var articleViews = new List<ArticleForPCViewModel>();
                articles.FindAll(f => f.GroupCode == this.Code).OrderBy(o => o.Sort).ToList().ForEach(article =>
                {
                    articleViews.Add(new ArticleForPCViewModel()
                    {
                        Code = article.Code,
                        Title = article.Title,
                        Cover = article.Cover,
                        Summary = article.Summary
                    });
                });
                return articleViews;
            }
        }
    }
    #endregion

    #region 图文列表 - APP
    /// <summary>
    /// 图文列表 - APP
    /// </summary>
    public class GroupForAppViewModel
    {
        /// <summary>
        /// 图文组Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string CreateTime { set; get; }
        /// <summary>
        /// 发布时间 短日期
        /// </summary>
        public string CreateTimeShort { set; get; }
        /// <summary>
        /// 文章列表
        /// </summary>
        public List<ArticleForAPPViewModel> Articles
        {
            get
            {
                var articles = new List<ArticleForAPPViewModel>();
                ArticleAdapter.Instance.Load(w => w.AppendItem("GroupCode", Code)).OrderBy(o => o.Sort).ToList().ForEach(article =>
                {
                    articles.Add(new ArticleForAPPViewModel()
                    {
                        Code = article.Code,
                        Title = article.Title,
                        Cover = article.Cover,
                        Summary = article.Summary
                    });
                });
                return articles;
            }
        }
    }
    #endregion
}
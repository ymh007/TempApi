using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ManagementReport
{
    #region 管理报告列表项实体类
    /// <summary>
    /// 管理报告列表项实体类
    /// </summary>
    public class ListItemViewModel
    {
        string _userName;
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="userName">用户登录名</param>
        public ListItemViewModel(string userName)
        {
            _userName = userName;
        }
        /// <summary>
        /// WebId
        /// </summary>
        public string WebId { set; get; }
        /// <summary>
        /// ListId
        /// </summary>
        public string ListId { set; get; }
        /// <summary>
        /// 文章Id
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 日期
        /// </summary>
        public string Date { set; get; }
        /// <summary>
        /// 完整日期
        /// </summary>
        public string DateFull { set; get; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount { set; get; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int PraiseCount { set; get; }
        /// <summary>
        /// 是否点赞
        /// </summary>
        public bool IsPraise { set; get; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary
        {
            set { _Summary = value; }
            get
            {
                var value = _Summary.Trim();
                value = Regex.Replace(value, "<[^>]*>", "");
                value = value.Replace("\r\n", "");
                if (value.Length > 50)
                {
                    value = value.Substring(0, 50) + "......";
                }
                return value;
            }
        }
        private string _Summary;
        /// <summary>
        /// 文章URL
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source
        {
            get
            {
                MossNewService ms = new MossNewService("News.xml");
                var web = ms.GetWebModel(WebId, _userName);
                if (web != null)
                {
                    return web.Title;
                }
                return string.Empty;
            }
        }
    }
    #endregion

    #region 管理报告详细内容实体类
    /// <summary>
    /// 管理报告详细内容实体类
    /// </summary>
    public class DetailsViewModel
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 作者
        /// </summary>
        public string Author { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
        /// <summary>
        /// 详细内容
        /// </summary>
        public string Content
        {
            set
            {
                _Content = value;
            }
            get
            {
                var value = _Content.Trim();
                var regex = @"<(?!img|br|p|/p|a|/a).*?>|\sstyle=""([^"";]+;?)+""|class=\w+|align=\w+|border=\w+|<a[\s\S]*?>|</a>";
                value = Regex.Replace(value, regex, "", RegexOptions.IgnoreCase);//只保留img,br,p,a 不区分大小写
                value = Regex.Replace(value, @"\s", "");//去空格
                value = Regex.Replace(value, @"\r\n", "");//去回车、去换行
                value = Regex.Replace(value, @"<p></p>", "");//去空P标签
                var imgs = new Regex(@"<img[\s\S]*?>", RegexOptions.IgnoreCase).Matches(value);//匹配所有图片标签
                log.Info("图片数量：" + imgs.Count);
                foreach (Match img in imgs)
                {
                    try
                    {
                        var src = new Regex(@""".*""").Match(img.Value).Value;//匹配图片src
                        src = Regex.Replace(src, @"[\""]+", "");

                        var newUrl = "";
                        if (src.ToLower().IndexOf("http://") == 0 || src.ToLower().IndexOf("https://") == 0)
                        {
                            newUrl = src;
                        }
                        else
                        {
                            newUrl = ConfigAppSetting.AppFileService + (src.IndexOf("/") == 0 ? "" : "/") + src;
                        }

                        var api = string.Format(ConfigAppSetting.MossImageApiUrl, newUrl);

                        var newImg = @"<img src='{0}' />";
                        newImg = string.Format(newImg, api);

                        value = value.Replace(img.Value, newImg);
                    }
                    catch (Exception e)
                    {
                        log.Error("转换图片URL异常：" + JsonConvert.SerializeObject(e));
                    }
                }
                return value;
            }
        }
        private string _Content;
        /// <summary>
        /// 评论列表
        /// </summary>
        public List<Comment> CommentList { set; get; }
        /// <summary>
        /// 评论实体类
        /// </summary>
        public class Comment
        {
            /// <summary>
            /// 评论编号
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 评论内容
            /// </summary>
            public string Content { set; get; }
            /// <summary>
            /// 评论时间
            /// </summary>
            public string Time { set; get; }
            /// <summary>
            /// 点赞次数
            /// </summary>
            public int PraiseCount { set; get; }
            /// <summary>
            /// 当前用户是否点赞
            /// </summary>
            public bool IsPraise { set; get; }
            /// <summary>
            /// 用户名称
            /// </summary>
            public string UserName { set; get; }
            /// <summary>
            /// 用户头像
            /// </summary>
            public string UserAvatar { set; get; }
        }
    }
    #endregion





    /// <summary>
    /// 管理报告视图
    /// </summary>
    public class ManagementReportViewModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        public string Created { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int PointCount { get; set; }
        /// <summary>
        /// 点赞状态
        /// </summary>
        public bool PointCountState { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string PublishingPageContent
        {
            set { publishingPageContent = value; }
            get
            {
                //string content = string.Empty;
                //string WebUrl = ConfigurationManager.AppSettings["WebUrl"];//获取config域名
                //content = publishingPageContent.Replace("src=\"/sites/NewsCenter/", "src=\""+ WebUrl + "/sites/NewsCenter/");
                //return content;
                return HtmlHelper.Replace(publishingPageContent);
            }
        }
        private string publishingPageContent;
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary
        {
            get
            {
                PublishingPageContent = System.Text.RegularExpressions.Regex.Replace(PublishingPageContent, "<[^>]*>", "");
                PublishingPageContent = PublishingPageContent.Replace("\r\n", "");
                if (PublishingPageContent.Length > 20)
                {
                    PublishingPageContent = PublishingPageContent.Substring(0, 20);
                }
                PublishingPageContent = PublishingPageContent.Substring(0, PublishingPageContent.Length) + ".....";
                return PublishingPageContent;
            }
        }
        /// <summary>
        /// 公司
        /// </summary>
        public string Company1 { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Author { get { return ArticleByLine; } }
        /// <summary>
        /// 作者
        /// </summary>
        public string ArticleByLine { set; get; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkFilename { get; set; }
    }

    /// <summary>
    /// 管理报告集合
    /// </summary>
    [Serializable]
    public class ManagementReportViewModelCollection : MCS.Library.Data.DataObjects.EditableDataObjectCollectionBase<ManagementReportViewModel>
    {

    }


}
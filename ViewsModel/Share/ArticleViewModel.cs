using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Share
{
    #region 文章保存 ViewModel
    /// <summary>
    /// 图文保存 ViewModel
    /// </summary>
    public class ArticlePostViewModel
    {
        /// <summary>
        /// 文章编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { set; get; }
        /// <summary>
        /// 正文
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 原文链接
        /// </summary>
        public string Link { set; get; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Views { set; get; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Cover { set; get; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { set; get; }
        /// <summary>
        /// 是否为文档
        /// </summary>
        public bool IsFile { set; get; }
        /// <summary>
        /// 文档路径
        /// </summary>
        public string FileUrl { set; get; }
    }
    #endregion

    #region 文章列表 PC ViewModel
    /// <summary>
    /// 文章列表 PC ViewModel
    /// </summary>
    public class ArticleForPCViewModel
    {
        /// <summary>
        /// 文章编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Cover
        {
            set { _Cover = value; }
            get
            {
                if (_Cover.ToLower().StartsWith("http://") || _Cover.ToLower().StartsWith("https://"))
                {
                    return _Cover;
                }
                var arr = Regex.Split(_Cover, "kindupload/", RegexOptions.IgnoreCase);
                if (arr.Length == 2)
                {
                    return ConfigAppSetting.OfficePath + "kindupload/" + arr[1];
                }
                else
                {
                    return ConfigAppSetting.OfficePath + "images/404.jpg";
                }
            }
        }
        private string _Cover;
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 阅读次数
        /// </summary>
        public int RecordOfView
        {
            get
            {
                return RecordAdapter.Instance.GetCount(0, Code);
            }
        }
        /// <summary>
        /// 点赞个数
        /// </summary>
        public int RecordOfPraise
        {
            get
            {
                return RecordAdapter.Instance.GetCount(1, Code);
            }
        }
        /// <summary>
        /// 评论条数
        /// </summary>
        public int CommentCount
        {
            get
            {
                return CommentAdapter.Instance.GetCount(Code);
            }
        }
    }
    #endregion

    #region 文章列表 APP ViewModel
    /// <summary>
    /// 文章列表 APP ViewModel
    /// </summary>
    public class ArticleForAPPViewModel
    {
        /// <summary>
        /// 文章编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Cover
        {
            set { _Cover = value; }
            get
            {
                if (_Cover.ToLower().StartsWith("http://") || _Cover.ToLower().StartsWith("https://"))
                {
                    return _Cover;
                }
                var arr = Regex.Split(_Cover, "kindupload/", RegexOptions.IgnoreCase);
                if (arr.Length == 2)
                {
                    return ConfigAppSetting.OfficePath + "kindupload/" + arr[1];
                }
                else
                {
                    return ConfigAppSetting.OfficePath + "images/404.jpg";
                }
            }
        }
        private string _Cover;
        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { set; get; }
    }
    #endregion

    #region 获取文章详情及评论列表 ViewModel
    /// <summary>
    /// 获取文章详情及评论列表 ViewModel
    /// </summary>
    public class ArticleDetailsViewModel
    {
        /// <summary>
        /// Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string Content
        {
            set { _Content = value; }
            get
            {
                if (IsFile == true)
                {
                    return string.Empty;
                }
                var content = _Content;
                var imgs = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase).Matches(_Content);
                foreach (Match img in imgs)
                {
                    var src = img.Groups["imgUrl"].Value;

                    if (src.ToLower().StartsWith("http://") || src.ToLower().StartsWith("https://"))
                    {
                        continue;
                    }

                    var newUrl = "";
                    var arr = Regex.Split(src, "kindupload/", RegexOptions.IgnoreCase);
                    if (arr.Length == 2)
                    {
                        newUrl = ConfigAppSetting.OfficePath + "kindupload/" + arr[1];
                    }
                    else
                    {
                        newUrl = ConfigAppSetting.OfficePath + "images/404.jpg";
                    }

                    var newImg = @"<img src=""{0}"" />";
                    newImg = string.Format(newImg, newUrl);

                    content = content.Replace(img.Value, newImg);
                }
                return content;
            }
        }
        private string _Content;
        /// <summary>
        /// 原文链接
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>
        public int Views { get; set; }
        /// <summary>
        /// 发表时间
        /// </summary>
        public string CreatTime { get; set; }
        /// <summary>
        /// 当前用户是否点过赞
        /// </summary>
        public bool IsPraise { set; get; }
        /// <summary>
        /// 点赞数量
        /// </summary>
        public int PraiseCount
        {
            get
            {
                return RecordAdapter.Instance.GetCount(1, Code);
            }
        }
        /// <summary>
        /// 评论列表
        /// </summary>
        public List<CommentViewModel> Comment { get; set; }
        /// <summary>
        /// 是否为文档
        /// </summary>
        public bool IsFile { set; get; }
        /// <summary>
        /// PDF文件路径
        /// </summary>
        public string FileUrl
        {
            set
            {
                _FileUrl = value;
            }
            get
            {
                if (IsFile == false)
                {
                    return string.Empty;
                }
                if (_FileUrl.ToLower().StartsWith("http://") || _FileUrl.ToLower().StartsWith("https://"))
                {
                    return _FileUrl;
                }
                var arr = Regex.Split(_FileUrl, "kindupload/", RegexOptions.IgnoreCase);
                if (arr.Length == 2)
                {
                    return ConfigAppSetting.OfficePath + "kindupload/" + arr[1];
                }
                else
                {
                    return ConfigAppSetting.OfficePath + "images/404.pdf";
                }
            }
        }
        private string _FileUrl;
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FileUrl))
                {
                    return string.Empty;
                }
                return System.IO.Path.GetExtension(FileUrl);
            }
        }
    }
    #endregion
}
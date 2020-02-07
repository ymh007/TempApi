using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using log4net;
using MCS.Library.OGUPermission;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Moss
{
    #region 新闻基类
    /// <summary>
    /// 新闻基类
    /// </summary>
    public class NewsBase
    {
        /// <summary>
        /// WebId（如：A8F5690C-24D1-4A28-AF6A-B1A634E0AE50）
        /// </summary>
        public string WebId { set; get; }
        /// <summary>
        /// ListId（如：FBB85F14-D478-4F56-B120-B3F163710DD9）
        /// </summary>
        public string ListId { set; get; }
        /// <summary>
        /// 编号（如：2189）
        /// </summary>
        public int ID { set; get; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 创建时间（如：2017-07-03 11:18:05）
        /// </summary>
        public string Created { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
    }
    #endregion

    #region 图片新闻、推荐新闻
    /// <summary>
    /// 图片新闻、推荐新闻
    /// </summary>
    public class ImageRecommendNewsViewModel : NewsBase
    {
        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public int Sort { set; get; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl
        {
            set { imageUrl = value; }
            get
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    return string.Empty;
                }
                var url = imageUrl.Split(',')[0];
                return string.Format(ConfigAppSetting.MossImageApiUrl, HttpUtility.UrlEncode(url));
            }
        }
        private string imageUrl;
    }
    #endregion

    #region 新闻列表
    /// <summary>
    /// 新闻列表
    /// </summary>
    public class NewsListViewModel : NewsBase
    {

    }
    #endregion

    #region 新闻详情
    /// <summary>
    /// 新闻详情
    /// </summary>
    public class NewsDetailViewModel
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string Created { set; get; }
        /// <summary>
        /// 新闻内容
        /// </summary>
        public string Content
        {
            set { content = value; }
            get
            {
                #region 格式化内容
                if (string.IsNullOrWhiteSpace(content))
                {
                    return string.Empty;
                }
                var value = content.Trim();
                if (Regex.IsMatch(value, @"<table[\s\S]*?>"))
                {
                    value = Regex.Replace(value, @"\s", " ");//去空格
                }
                else
                {
                    var regex = @" < (?!img|br|p|/p|a|/a).*?>|\sstyle=""(.*?)""|\sclass=""(.*?)""|\salign=""(.*?)""|\sborder=""(.*?)""|<a[\s\S]*?>|</a>";
                    value = Regex.Replace(value, regex, "", RegexOptions.IgnoreCase);//只保留img,br,p,a 不区分大小写
                    value = Regex.Replace(value, @"\s", "");//去空格
                }

                value = Regex.Replace(value, @"\r\n", "");//去回车、去换行
                value = Regex.Replace(value, @"<p></p>", "");//去空P标签
                var imgs = new Regex(@"<img[\s\S]*?>", RegexOptions.IgnoreCase).Matches(value);//匹配所有图片标签
                foreach (Match img in imgs)
                {
                    try
                    {
                        var match = new Regex(@"src=""([^""]*)""").Match(img.Value);
                        var src = match.Groups[1].Value;//匹配图片src

                        var newUrl = "";
                        if (src.ToLower().IndexOf("http://") == 0 || src.ToLower().IndexOf("https://") == 0)
                        {
                            newUrl = src;
                        }
                        else
                        {
                            newUrl = ConfigAppSetting.AppFileService + (src.IndexOf("/") == 0 ? "" : "/") + src;
                        }
                        newUrl = HttpUtility.UrlEncode(newUrl);

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
                #endregion
            }
        }
        private string content;
        /// <summary>
        /// RelationId，评论点赞时使用
        /// </summary>
        public int RelationId { set; get; }
        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ReadCount { set; get; }
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int PointCount { set; get; }
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsPoint { set; get; }
        /// <summary>
        /// 是否收藏
        /// </summary>
        public bool IsFav { set; get; }
        /// <summary>
        /// 评论列表
        /// </summary>
        public List<NewsCommentViewModel> CommentList;
    }
    #endregion

    #region 资讯信息列表
    /// <summary>
    /// 资讯信息列表
    /// </summary>
    public class InformationListViewModel : NewsBase
    {

    }
    #endregion

    #region 资讯信息详情
    /// <summary>
    /// 资讯信息详情
    /// </summary>
    public class InformationDetailViewModel
    {
        /// <summary>
        /// 附件列表
        /// </summary>
        public List<AttachmentViewModel> Attachments { set; get; }
    }
    #endregion

    #region 附件 ViewModel
    /// <summary>
    /// 附件 ViewModel
    /// </summary>
    public class AttachmentViewModel
    {
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 附件预览地址
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 附件原始地址
        /// </summary>
        public string OriginalUrl { set; get; }
    }
    #endregion

    #region 评论列表 ViewModel
    /// <summary>
    /// 评论列表 ViewModel
    /// </summary>
    public class NewsCommentViewModel
    {
        /// <summary>
        /// 评论编号
        /// </summary>
        public int Id;
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content;
        /// <summary>
        /// 评论时间
        /// </summary>
        public string CommentTime;
        /// <summary>
        /// 用户名 如：v-sunzhh
        /// </summary>
        public string UserName;
        /// <summary>
        /// 用户名 如：v-孙志昊
        /// </summary>
        public string UserDisplayName;
        /// <summary>
        /// 点赞次数
        /// </summary>
        public int PointCount;
        /// <summary>
        /// 当前用户是否点赞
        /// </summary>
        public bool IsPoint;
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserHeadPhoto
        {
            get
            {
                try
                {
                    var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, this.UserName).FirstOrDefault();
                    return UserHeadPhotoService.GetUserHeadPhoto(user.ID);
                }
                catch
                {
                    return UserHeadPhotoService.GetUserHeadPhoto(Guid.NewGuid().ToString());
                }
            }
        }
    }
    #endregion

    #region 提交评论，回复 ViewModel
    /// <summary>
    /// 提交评论，回复 ViewModel
    /// </summary>
    public class SubmitCommentViewModel
    {
        /// <summary>
        /// 1:评论 0:回复
        /// </summary>
        public int CommentType { get; set; }
        /// <summary>
        /// relationID
        /// </summary>
        public int RelationId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string CommentContent { get; set; }
        /// <summary>
        /// 0：系统管理员；1：人员；2：匿名
        /// </summary>
        public int CommentPeopleState { get; set; }
    }
    #endregion


}
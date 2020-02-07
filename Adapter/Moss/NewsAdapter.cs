using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.UserFav;
using Seagull2.YuanXin.AppApi.Models.InformationNews;
using Seagull2.YuanXin.AppApi.ViewsModel.Moss;
using static Seagull2.YuanXin.AppApi.MossNewService;

namespace Seagull2.YuanXin.AppApi.Adapter.Moss
{
    /// <summary>
    /// 新闻 Adapter
    /// </summary>
    public class NewsAdapter
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 实例
        /// </summary>
        public static readonly NewsAdapter Instance = new NewsAdapter();

        /// <summary>
        /// 缓存图片新闻和推荐新闻地址
        /// </summary>
        public static List<string> CacheData = new List<string>();


        const string XmlNews = "News.xml";
        const string XmlRecommendNews = "RecommendedNews.xml";
        const string XmlInformation = "Information.xml";

        #region 获取图片新闻
        /// <summary>
        /// 获取图片新闻
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isPart">是否党建</param>
        public List<ImageRecommendNewsViewModel> GetImageNews(string userName, bool isPart)
        {
            var key = "图片";
            if (isPart)
            {
                key = "党建图片";
            }
            var moss = new MossNewService(XmlRecommendNews);
            var data = moss.GetImageRecommendNews(userName, key);
            var list = new List<ImageRecommendNewsViewModel>();
            data.ForEach(item =>
            {
                // 过滤不符合条件的新闻
                var guid = item.NewsAddress.Substring(item.NewsAddress.LastIndexOf('/') + 1);
                Guid newGuid;
                if (!Guid.TryParse(guid, out newGuid))
                {
                    return;
                }
                // 排序
                var sort = 0;
                var sortArr = item.PicOrder.Split('#');
                if (sortArr.Length >= 2)
                {
                    sort = int.Parse(sortArr[1]);
                }
                string tempNewsAddress = item.NewsAddress.ToLower().Trim();
                if (!CacheData.Contains(tempNewsAddress) && !string.IsNullOrEmpty(tempNewsAddress))
                {
                    CacheData.Add(tempNewsAddress);
                }
                list.Add(new ImageRecommendNewsViewModel()
                {
                    WebId = item.WebId,
                    ListId = item.ListId,
                    ID = int.Parse(item.ID),
                    Title = item.Title,
                    Created = item.Modified,
                    Source = item.WebName,
                    Sort = sort,
                    ImageUrl = item.EIPImage
                });
            });
            return list.OrderBy(o => o.Sort).ToList();
        }
        #endregion

        #region 获取推荐新闻
        /// <summary>
        /// 获取推荐新闻
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="isPart">是否党建</param>
        public List<ImageRecommendNewsViewModel> GetRecommendNews(string userName, bool isPart)
        {
            var key = "推荐";
            if (isPart)
            {
                key = "党建推荐";
            }
            var moss = new MossNewService(XmlRecommendNews);
            var data = moss.GetImageRecommendNews(userName, key);
            var list = new List<ImageRecommendNewsViewModel>();
            data.ForEach(item =>
            {
                // 过滤不符合条件的新闻
                var guid = item.NewsAddress.Substring(item.NewsAddress.LastIndexOf('/') + 1);
                Guid newGuid;
                if (!Guid.TryParse(guid, out newGuid))
                {
                    return;
                }
                // 排序
                var sort = 0;
                var sortArr = item.RecOrder.Split('#');
                if (sortArr.Length >= 2)
                {
                    sort = int.Parse(sortArr[1]);
                }
                string tempNewsAddress = item.NewsAddress.ToLower().Trim();
                if (!CacheData.Contains(tempNewsAddress) && !string.IsNullOrEmpty(tempNewsAddress))
                {
                    CacheData.Add(tempNewsAddress);
                }
                list.Add(new ImageRecommendNewsViewModel()
                {
                    WebId = item.WebId,
                    ListId = item.ListId,
                    ID = int.Parse(item.ID),
                    Title = item.Title,
                    Created = item.Modified,
                    Source = item.WebName,
                    Sort = sort,
                    ImageUrl = item.EIPImage
                });
            });
            return list.OrderBy(o => o.Sort).ToList();
        }
        #endregion

        #region 获取新闻列表
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="created">时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        public List<NewsListViewModel> GetNewsList(string userName, string created, string company, string keyword, string newsType)
        {
            var moss = new MossNewService(XmlNews);
            var data = moss.GetNewsList(userName, created, company, keyword, newsType);

            var newsDistinctUrl = ConfigurationManager.AppSettings["newDistinctUrl"];
            var list = new List<NewsListViewModel>();
            data.ForEach(item =>
            {
                bool isAdd = true;
                string guid = item.GUID.Replace("{", "").Replace("}", "");
                if (CacheData.Count > 0)
                {
                    var newsUrl = newsDistinctUrl.ToLower().Replace("distincturlnewsid", guid);
                    if (CacheData.Contains(newsUrl.ToLower()))
                    {
                        isAdd = false;
                    }
                }
                else
                {
                    //去除推荐新闻和图片新闻
                    var repeatMoss = new MossNewService(XmlRecommendNews);
                    var newsRecommended = repeatMoss.GetImageRecommendNews(userName, "推荐");
                    var newsImage = repeatMoss.GetImageRecommendNews(userName, "图片");
                    #region 判断新闻是否是图片新闻
                    if (newsRecommended.Count > 0)
                    {
                        foreach (ImageRecommendNews imageNews in newsImage)
                        {
                            if (!string.IsNullOrWhiteSpace(newsDistinctUrl) && !string.IsNullOrWhiteSpace(imageNews.NewsAddress))
                            {
                                string link = imageNews.NewsAddress.ToLower().Trim();
                                var newsUrl = newsDistinctUrl.ToLower().Replace("distincturlnewsid", guid);
                                if (link == newsUrl.ToLower())
                                {
                                    isAdd = false;
                                }
                                if (!CacheData.Contains(link) && !string.IsNullOrEmpty(link))
                                {
                                    CacheData.Add(link);
                                }
                            }
                        }
                    }
                    #endregion
                    #region 判断新闻是否是推荐新闻
                    if (newsRecommended.Count > 0)
                    {
                        foreach (ImageRecommendNews recommendedNews in newsRecommended)
                        {
                            if (!string.IsNullOrWhiteSpace(newsDistinctUrl) && !string.IsNullOrWhiteSpace(recommendedNews.NewsAddress))
                            {
                                string link = recommendedNews.NewsAddress.ToLower().Trim();
                                var newsUrl = newsDistinctUrl.ToLower().Replace("distincturlnewsid", guid);
                                if (link == newsUrl.ToLower())
                                {
                                    isAdd = false;
                                }
                                if (!CacheData.Contains(link) && !string.IsNullOrEmpty(link))
                                {
                                    CacheData.Add(link);
                                }
                            }
                        }
                    }
                    #endregion
                }

                if (isAdd)
                {
                    list.Add(new NewsListViewModel()
                    {
                        WebId = item.WebId,
                        ListId = item.ListId,
                        ID = int.Parse(item.ID),
                        Title = item.Title,
                        Created = item.Created,
                        Source = item.Company1 == string.Empty ? item.WebName : item.Company1
                    });
                }
            });
            return list;
        }


        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="created">时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        public List<NewsListViewModel> GetNewsList1(string userName, string created, string company, string keyword, string newsType)
        {
            var moss = new MossNewService(XmlNews);
            var data = moss.GetNewsList(userName, created, company, keyword, newsType);
            var list = new List<NewsListViewModel>();
            data.ForEach(item =>
            {
                list.Add(new NewsListViewModel()
                {
                    WebId = item.WebId,
                    ListId = item.ListId,
                    ID = int.Parse(item.ID),
                    Title = item.Title,
                    Created = item.Created,
                    Source = item.Company1 == string.Empty ? item.WebName : item.Company1
                });
            });
            return list;
        }

        #endregion

        #region 获取新闻详情
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        public NewsDetailViewModel GetNewsDetail(Seagull2Identity user, string webId, string listId, int id)
        {
            try
            {
                var model = new NewsDetailViewModel();

                Stopwatch sw = new Stopwatch();

                sw.Start();
                // 获取新闻内容
                var moss = new MossNewService(XmlNews);
                var data = moss.GetNewsDetailByListItemId(user.Name, webId, listId, id);
                if (string.IsNullOrWhiteSpace(data.PublishingPageContent))
                {
                    // 如果内容为空，通过NewsAddress字段中取到的Guid再查询一次，这种情况一般是推荐新闻
                    var guid = data.NewsAddress.Substring(data.NewsAddress.LastIndexOf('/') + 1);
                    data = moss.GetNewsDetailByGuid(user.Name, guid);
                }
                model.Title = data.Title;
                model.Created = data.Created;
                model.Content = data.PublishingPageContent;
                var newsUri = data.EncodedAbsUrl;
                if (!newsUri.EndsWith(".aspx"))
                {
                    var pattern = ".*\\#";
                    var replacement = "";
                    var rgx = new Regex(pattern);
                    var result = rgx.Replace(data.FileRef, replacement);
                    newsUri = data.EncodedAbsUrl + result;
                }
                sw.Stop();

                log.Info("-------------------------------------查询新闻详情，耗时：" + sw.ElapsedMilliseconds + "毫秒");

                // 获取RelationId
                var taskRelationId = System.Threading.Tasks.Task.Run(() =>
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    try
                    {
                        model.RelationId = ArticlePointAdapter.Instance.GetRelationID(newsUri);
                    }
                    catch (Exception e)
                    {
                        log.Error("查询RelationID失败：" + e.Message);
                    }
                    sw1.Stop();

                    log.Info("-------------------------------------查询RelationID，耗时：" + sw1.ElapsedMilliseconds + "毫秒");
                });
                taskRelationId.Wait();

                // 获取阅读次数
                var taskReadCount = System.Threading.Tasks.Task.Run(() =>
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    try
                    {
                        model.ReadCount = VisitLogsAdapter.Instance.GetCountByTitle(model.Title);
                    }
                    catch (Exception e)
                    {
                        log.Error("查询新闻阅读次数失败：" + e.Message);
                    }
                    sw1.Stop();

                    log.Info("-------------------------------------查询新闻阅读次数，耗时：" + sw1.ElapsedMilliseconds + "毫秒");
                });

                // 获取点赞次数
                var taskPoint = System.Threading.Tasks.Task.Run(() =>
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    try
                    {
                        model.PointCount = ArticlePointAdapter.Instance.GetPointCount(model.RelationId);
                        model.IsPoint = PointRecordAdapter.Instance.IsPoint(user.Name, 1, model.RelationId);
                    }
                    catch (Exception e)
                    {
                        log.Error("查询新闻点赞次数失败：" + e.Message);
                    }
                    sw1.Stop();

                    log.Info("-------------------------------------查询新闻点赞次数，耗时：" + sw1.ElapsedMilliseconds + "毫秒");
                });

                // 获取评论列表
                var taskComment = System.Threading.Tasks.Task.Run(() =>
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    try
                    {
                        var comment = CommentAdapter.Instance.ShowComment(1, model.RelationId, user.Name);
                        var commentView = new List<NewsCommentViewModel>();
                        comment.ForEach(item =>
                        {
                            commentView.Add(new NewsCommentViewModel()
                            {
                                Id = item.ID,
                                Content = item.CommentContent,
                                CommentTime = item.CommentTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                UserName = item.CommentPeopleID.Replace("SINOOCEANLAND\\", "").Replace("SINOOCEANGROUP\\", ""),
                                UserDisplayName = item.CommentPeople.Replace("SINOOCEANLAND\\", "").Replace("SINOOCEANGROUP\\", ""),
                                PointCount = item.PointOfPraise,
                                IsPoint = item.TotalPointPraise > 0 ? true : false
                            });
                        });
                        model.CommentList = commentView;
                    }
                    catch (Exception e)
                    {
                        log.Error("获取评论列表失败：" + e.Message);
                    }
                    sw1.Stop();

                    log.Info("-------------------------------------获取评论列表，耗时：" + sw1.ElapsedMilliseconds + "毫秒");
                });

                // 用户是否收藏此新闻
                var taskIsFav = System.Threading.Tasks.Task.Run(() =>
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    try
                    {
                        model.IsFav = UserFavInformationAdapter.Instance.IsFavNew(webId, listId, id, user.Id);
                    }
                    catch (Exception e)
                    {
                        log.Error("用户是否收藏此新闻失败：" + e.Message);
                    }
                    sw1.Stop();

                    log.Info("-------------------------------------用户是否收藏此新闻，耗时：" + sw1.ElapsedMilliseconds + "毫秒");
                });

                // 添加访问记录
                System.Threading.Tasks.Task.Run(() =>
                {
                    Stopwatch sw1 = new Stopwatch();
                    sw1.Start();
                    try
                    {
                        VisitLogsAdapter.Instance.AddVisitLog(user.DisplayName, newsUri, model.Title);
                    }
                    catch (Exception e)
                    {
                        log.Error("添加访问记录失败：" + e.Message);
                    }
                    sw1.Stop();

                    log.Info("-------------------------------------添加访问记录，耗时：" + sw1.ElapsedMilliseconds + "毫秒");
                });

                taskReadCount.Wait();
                taskPoint.Wait();
                taskComment.Wait();
                taskIsFav.Wait();

                return model;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 获取资讯信息列表
        /// <summary>
        /// 获取资讯信息列表
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="created">时间</param>
        /// <param name="company">所属公司</param>
        /// <param name="keyword">标题关键词</param>
        public List<InformationListViewModel> GetInformationList(string userName, string created, string company, string keyword)
        {
            var moss = new MossNewService(XmlInformation);
            var data = moss.GetInformationList(userName, created, company, keyword);
            var list = new List<InformationListViewModel>();
            data.ForEach(item =>
            {
                list.Add(new InformationListViewModel()
                {
                    WebId = item.WebId,
                    ListId = item.ListId,
                    ID = int.Parse(item.ID),
                    Title = item.Title,
                    Created = item.Created,
                    Source = item.ClassB
                });
            });
            return list;
        }
        #endregion

        #region 获取资讯信息详情
        /// <summary>
        /// 获取资讯信息详情
        /// </summary>
        public InformationDetailViewModel GetInformationDetail(Seagull2Identity user, string webId, string listId, int id)
        {
            try
            {
                // 获取附件列表
                var moss = new MossNewService(XmlInformation);
                var attachments = moss.GetAttachments(user.Name, webId, listId, id);

                var viewAttachments = new List<AttachmentViewModel>();
                attachments.ForEach(attachment =>
                {
                    var url = attachment.Url;
                    if (!url.ToLower().EndsWith(".pdf"))
                    {
                        url = AttachmentPreviewService.GetPreviewUrl(attachment.Url);
                    }
                    viewAttachments.Add(new AttachmentViewModel()
                    {
                        Name = attachment.Name,
                        Url = url,
                        OriginalUrl = attachment.Url
                    });
                });

                var model = new InformationDetailViewModel();
                model.Attachments = viewAttachments;
                return model;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
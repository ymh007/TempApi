using log4net;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Models.InformationNews;
using Seagull2.YuanXin.AppApi.ViewsModel.Moss;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 新闻中心
    /// </summary>
    public class NewsController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 获取筛选公司列表
        /// <summary>
        /// 获取筛选公司列表
        /// </summary>
        [HttpGet]
        public Task<IHttpActionResult> GetCompanyList()
        {
            var result = ControllerService.Run(() =>
            {
                string[] companys = {
                    "开发事业一部",
                    "开发事业二部",
                    "开发事业三部",
                    "开发事业四部",
                    "远洋邦邦",
                    "商业地产事业部",
                    "写字楼事业部",
                    "客户服务事业部",
                    "资本运营事业部",
                    "产品营造事业部",
                };
                return companys;
            });
            return Task.FromResult<IHttpActionResult>(Ok(result));
        }
        #endregion

        #region 资讯搜索
        /// <summary>
        /// 资讯搜索
        /// </summary>
        [HttpGet]
        public IHttpActionResult Search(int count, string company = "", string keyword = "")
        {
            var result = ControllerService.Run(() =>
            {
                var isYB = !User.IsInRole(ConfigAppSetting.EIPACCESSER) && User.IsInRole(ConfigAppSetting.YBACCESSER);

                var user = (Seagull2Identity)User.Identity;
                var created = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

                var view = new List<ViewsModel.BaseViewNameData>();

                var tasks = new List<Task>();
                tasks.Add(Task.Run(() =>
                {
                    var newCompany = company;
                    if (isYB)
                    {
                        newCompany = "远洋邦邦";
                    }
                    var newList = Adapter.Moss.NewsAdapter.Instance.GetNewsList(user.LogonName, created, newCompany, keyword, string.Empty).Take(count);
                    view.Add(new ViewsModel.BaseViewNameData() { Name = "新闻中心", Data = newList });
                }));
                tasks.Add(Task.Run(() =>
                {
                    var newCompany = company;
                    if (isYB)
                    {
                        newCompany = "远洋邦邦";
                    }
                    var partyNewList = Adapter.Moss.NewsAdapter.Instance.GetNewsList(user.LogonName, created, newCompany, keyword, "远洋党建").Take(count);
                    view.Add(new ViewsModel.BaseViewNameData { Name = "党建新闻", Data = partyNewList });
                }));
                tasks.Add(Task.Run(() =>
                {
                    var newCompany = company;
                    if (isYB)
                    {
                        newCompany = "远洋邦邦";
                    }
                    var importantList = Adapter.Moss.NoticeAdapter.Instance.GetImportantNoticeList(user.LogonName, created, company, keyword).Take(count);
                    view.Add(new ViewsModel.BaseViewNameData() { Name = "重要发文", Data = importantList });
                }));
                tasks.Add(Task.Run(() =>
                {
                    var unitList = Adapter.Moss.NoticeAdapter.Instance.GetNoticeList(user.LogonName, "FAWEN", created, company, keyword, isYB).Take(count);
                    view.Add(new ViewsModel.BaseViewNameData() { Name = "单位通知", Data = unitList });
                }));
                //tasks.Add(Task.Run(() =>
                //{
                //    var departmentList = Adapter.Moss.NoticeAdapter.Instance.GetNoticeList(user.LogonName, "NOTICE", created, company, keyword, isYB).Take(count);
                //    view.Add(new ViewsModel.BaseViewNameData() { Name = "部门通知", Data = departmentList });
                //}));
                tasks.Add(Task.Run(() =>
                {
                    var meetingList = Adapter.Moss.NoticeAdapter.Instance.GetNoticeList(user.LogonName, "MEETING", created, company, keyword, isYB).Take(count);
                    view.Add(new ViewsModel.BaseViewNameData() { Name = "会议纪要", Data = meetingList });
                }));
                tasks.Add(Task.Run(() =>
                {
                    var partyNoticeList = Adapter.Moss.NoticeAdapter.Instance.GetImportantNoticeList(user.LogonName, created, company, keyword, "party").Take(count);
                    view.Add(new ViewsModel.BaseViewNameData { Name = "党建发文", Data = partyNoticeList });
                }));
                tasks.Add(Task.Run(() =>
                {
                    var informationList = Adapter.Moss.NewsAdapter.Instance.GetInformationList(user.LogonName, created, company, keyword).Take(count);
                    view.Add(new ViewsModel.BaseViewNameData { Name = "每日精选", Data = informationList });
                }));
                Task.WaitAll(tasks.ToArray());

                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 获取图片新闻、推荐新闻
        /// <summary>
        /// 获取图片新闻、推荐新闻
        /// </summary>
        /// <param name="isParty">是否党建新闻</param>
        [HttpGet]
        public IHttpActionResult GetImageRecommendNews(bool isParty = false)
        {
            Stopwatch sw = new Stopwatch();

            log.Info("----------获取图片新闻、推荐新闻开始----------");
            sw.Start();

            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var data = new
                {
                    ImageNews = Adapter.Moss.NewsAdapter.Instance.GetImageNews(user.LogonName, isParty),
                    RecommendNews = Adapter.Moss.NewsAdapter.Instance.GetRecommendNews(user.LogonName, isParty)
                };
                return data;
            });

            sw.Stop();
            log.Info("----------获取图片新闻、推荐新闻结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");

            return Ok(result);
        }
        #endregion

        #region 获取新闻列表
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetNewsList(string date = "", string company = "", string keyword = "", string newsType = "")
        {
            Stopwatch sw = new Stopwatch();

            log.Info("----------获取新闻列表开始----------");
            sw.Start();

            var result = ControllerService.Run(() =>
            {
                var isEIP = User.IsInRole(ConfigAppSetting.EIPACCESSER);
                var isYB = User.IsInRole(ConfigAppSetting.YBACCESSER);
                if (!isEIP && isYB)
                {
                    company = "远洋邦邦";
                }

                var user = (Seagull2Identity)User.Identity;
                var created = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(date))
                {
                    created = Convert.ToDateTime(date);
                }
                var newsTypeName = "";
                switch (newsType)
                {
                    case "party": newsTypeName = "远洋党建"; break;
                }
                return Adapter.Moss.NewsAdapter.Instance.GetNewsList(user.LogonName, created.ToString("yyyy-MM-ddTHH:mm:ssZ"), company, keyword, newsTypeName);
            });

            sw.Stop();
            log.Info("----------获取新闻列表结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");

            return Ok(result);
        }
        #endregion

        #region 获取新闻详情
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetNewsDetail(string webId, string listId, int id)
        {
            Stopwatch sw = new Stopwatch();

            log.Info("----------获取新闻详情开始----------");
            sw.Start();

            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var model = Adapter.Moss.NewsAdapter.Instance.GetNewsDetail(user, webId, listId, id);
                if (model == null)
                {
                    throw new Exception("抱歉，移动端暂不支持当前类型的新闻！");
                }
                return model;
            });

            sw.Stop();
            log.Info("----------获取新闻详情结束----------用时：" + sw.ElapsedMilliseconds + "毫秒");

            return Ok(result);
        }
        #endregion

        #region 获取资讯信息列表
        /// <summary>
        /// 获取资讯信息列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetInformationList(string date = "", string company = "", string keyword = "")
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var created = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(date))
                {
                    created = Convert.ToDateTime(date);
                }
                return Adapter.Moss.NewsAdapter.Instance.GetInformationList(user.LogonName, created.ToString("yyyy-MM-ddTHH:mm:ssZ"), company, keyword);
            });

            return Ok(result);
        }
        #endregion

        #region 获取资讯信息详情
        /// <summary>
        /// 获取资讯信息详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetInformationDetail(string webId, string listId, int id)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var model = Adapter.Moss.NewsAdapter.Instance.GetInformationDetail(user, webId, listId, id);
                if (model == null)
                {
                    throw new Exception("抱歉，移动端暂不支持当前类型的资讯！");
                }
                return model;
            });
            return Ok(result);
        }
        #endregion

        #region 点赞
        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="commentType">1：文章点赞、0：评论点赞</param>
        /// <param name="relationID">文章或评论ID</param>
        [HttpGet]
        public IHttpActionResult CommentPointOfPraise(string commentType, string relationID)
        {
            var result = ControllerService.Run(() =>
            {
                var userID = "SINOOCEANLAND\\" + ((Seagull2Identity)User.Identity).LogonName;
                return PointRecordAdapter.Instance.CommentPointOfPraise(commentType, relationID, userID);
            });
            return Ok(result);
        }
        #endregion

        #region 发表评论
        /// <summary>
        /// 发表评论
        /// </summary>
        [HttpPost]
        public IHttpActionResult SubmitComment(SubmitCommentViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var userName = "";
                var userId = "";
                switch (model.CommentPeopleState)
                {
                    case 2:
                        userName = "匿名";
                        userId = "";
                        break;
                    default:
                        userName = ((Seagull2Identity)User.Identity).DisplayName;
                        userId = "SINOOCEANLAND\\" + ((Seagull2Identity)User.Identity).Name;
                        break;
                }
                return CommentAdapter.Instance.SubmitComment(model.CommentType, model.RelationId, model.CommentContent, userName, userId);
            });
            return Ok(result);
        }
        #endregion
    }
}
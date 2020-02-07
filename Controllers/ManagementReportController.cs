using System;
using System.Reflection;
using System.Web.Http;
using log4net;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ManagementReport;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 管理报告API控制器
    /// </summary>
    public class ManagementReportController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 获取管理报告列表
        /// <summary>
        /// 获取管理报告列表
        /// data字段说明：
        /// 1、首页不传date
        /// 2、从第二页开始，传上一页最后一条数据的dateFull字段的值
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(string date = "")
        {
            var data = ManagementReportAdapter.Instance.GetList(date, User.Identity.Name);
            if (data.Count < 1)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "暂无更多数据！"
                });
            }

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 获取管理报告详情及评论
        /// <summary>
        /// 获取管理报告详情及评论
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetContent(string webId, string listId, int articleId)
        {
            MossNewService ms = new MossNewService("News.xml");

            // 获取Moss数据 - 文章来源
            var web = ms.GetWebModel(webId, User.Identity.Name);

            // 获取Moss数据 - 文章内容
            var model = ms.GetManagementReportModel(webId, listId, articleId, User.Identity.Name);
            if (model == null)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "暂无详细内容！"
                });
            }

            // 组织数据
            var source = web == null ? "" : web.Title;
            var data = ManagementReportAdapter.Instance.GetModel(model, source, User.Identity.Name, articleId);

            //添加浏览次数
            SiteVisitLogAdapter.Instance.Add(new Models.ManagementReport.SiteVisitLogModel()
            {
                Url = model.EncodedAbsUrl,
                UserName = User.Identity.Name,
                DropTime = DateTime.Now,
                Title = model.Title,
                Source = ""
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 管理报告点赞
        /// <summary>
        /// 管理报告点赞
        /// </summary>
        [HttpPost]
        public IHttpActionResult PraiseOfArticle(PraiseOfArticlePost post)
        {
            PointPraiseRecordAdapter.Instance.PraiseOfArticle(User.Identity.Name, post.ArticleId, post.ArticleUrl);

            return Ok(new BaseView()
            {
                State = true,
                Message = "success."
            });
        }
        /// <summary>
        /// 文章点赞Post
        /// </summary>
        public class PraiseOfArticlePost
        {
            /// <summary>
            /// 文章编号
            /// </summary>
            public int ArticleId { set; get; }
            /// <summary>
            /// 文章完整URL
            /// </summary>
            public string ArticleUrl { set; get; }
        }
        #endregion

        #region 发表管理报告评论
        /// <summary>
        /// 发表管理报告评论
        /// </summary>
        [HttpPost]
        public IHttpActionResult PublishComment(PublishCommentPost post)
        {
            CommentAdapter.Instance.Update(new Models.ManagementReport.CommentModel()
            {
                CommentType = 1,
                RelationID = post.ArticleId,
                CommentContent = post.Content,
                CommentTime = DateTime.Now,
                CommentPeople = ((Seagull2Identity)User.Identity).DisplayName,
                PointOfPraise = 0,
                OperationStatue = 1,
                OperationTime = DateTime.Now,
                CommentPeopleID = User.Identity.Name
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "success."
            });
        }
        /// <summary>
        /// 发表管理报告评论Post
        /// </summary>
        public class PublishCommentPost
        {
            /// <summary>
            /// 文章编号
            /// </summary>
            public int ArticleId { set; get; }
            /// <summary>
            /// 评论内容
            /// </summary>
            public string Content { set; get; }
        }
        #endregion

        #region 评论点赞
        /// <summary>
        /// 评论点赞
        /// </summary>
        [HttpPost]
        public IHttpActionResult PraiseOfComment(PraiseOfCommentPost post)
        {
            PointPraiseRecordAdapter.Instance.PraiseOfComment(User.Identity.Name, post.CommentId);

            return Ok(new BaseView()
            {
                State = true,
                Message = "success."
            });
        }
        /// <summary>
        /// 评论点赞Post
        /// </summary>
        public class PraiseOfCommentPost
        {
            /// <summary>
            /// 评论编号
            /// </summary>
            public int CommentId { set; get; }
        }
        #endregion
    }
}
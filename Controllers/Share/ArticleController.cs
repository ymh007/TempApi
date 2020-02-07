using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Share;

namespace Seagull2.YuanXin.AppApi.Controllers.Share
{
    /// <summary>
    /// 文章控制器
    /// </summary>
    public class Share_ArticleController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 获取文章详情
        /// <summary>
        /// 获取文章详情
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDetail(string articleCode)
        {
            // current user
            var user = (Seagull2Identity)User.Identity;

            // get article details
            var list = ArticleAdapter.Instance.Load(p => { p.AppendItem("Code", articleCode); });
            if (list.Count != 1)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "Can not find the article."
                });
            }
            var model = list[0];

            // article view-model
            var vmodel = new ArticleDetailsViewModel()
            {
                Code = model.Code,
                Title = model.Title,
                Author = model.Author,
                Content = model.Content,
                Link = model.Link,
                Views = model.Views,
                CreatTime = model.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                IsPraise = RecordAdapter.Instance.Exists(w =>
                     {
                         w.AppendItem("TargetCode", model.Code);
                         w.AppendItem("Type", 1);
                         w.AppendItem("Creator", user.Id);
                     }),
                Comment = new List<CommentViewModel>(),
                IsFile = model.IsFile,
                FileUrl = model.FileUrl
            };

            // comment list
            var comment = CommentAdapter.Instance.Load(w => w.AppendItem("ArticleCode", model.Code));
            // root
            comment.Where(w => w.CommentCode == string.Empty).OrderBy(o => o.CreateTime).ToList().ForEach(root =>
            {
                var item = new CommentViewModel()
                {
                    Code = root.Code,
                    Content = root.Content,
                    UserCodeFrom = root.UserCodeFrom,
                    UserNameFrom = root.UserNameFrom,
                    CreateTime = root.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    SubComment = new List<CommentBaseViewModel>()
                };
                comment.Where(w => w.CommentCode == root.Code).OrderBy(o => o.CreateTime).ToList().ForEach(sub =>
                {
					// sub
					item.SubComment.Add(new CommentBaseViewModel()
                    {
                        Code = sub.Code,
                        Content = sub.Content,
                        UserCodeFrom = sub.UserCodeFrom,
                        UserNameFrom = sub.UserNameFrom,
                        UserCodeTo = sub.UserCodeTo,
                        UserNameTo = sub.UserNameTo,
                        CreateTime = sub.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                });
                vmodel.Comment.Add(item);
            });

            // add a reading record
            RecordAdapter.Instance.Update(new RecordModel()
            {
                Code = Guid.NewGuid().ToString(),
                TargetCode = articleCode,
                Type = 0,
                UserName = user.DisplayName,
                Creator = user.Id,
                CreateTime = DateTime.Now,
                ValidStatus = true
            });

            // update the number of views
            ArticleAdapter.Instance.UpdateViews(articleCode);

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = vmodel
            });
        }
        #endregion

        #region 文章搜索
        /// <summary>
        /// 文章搜索
        /// </summary>
        [HttpGet]
        public IHttpActionResult Search(string keyword, int pageSize, int pageIndex)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var dataCount = ArticleAdapter.Instance.Search(keyword, userCode);
            var dataList = ArticleAdapter.Instance.Search(keyword, userCode, pageSize, pageIndex);

            var list = new List<ArticleForAPPViewModel>();

            foreach (DataRow dr in dataList.Rows)
            {
                list.Add(new ArticleForAPPViewModel
                {
                    Code = dr["Code"].ToString(),
                    Title = dr["Title"].ToString(),
                    Cover = dr["Cover"].ToString(),
                    Summary = dr["Summary"].ToString()
                });
            }

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = list
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion
    }
}
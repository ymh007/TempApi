using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.ViewsModel.Share;
using log4net;

namespace Seagull2.YuanXin.AppApi.Controllers.Share
{
    /// <summary>
    /// 评论控制器
    /// </summary>
    public class Share_CommentController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 新增评论
        /// <summary>
        /// 新增评论
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(CommentPostViewModel post)
        {
            // get article info
            var articles = ArticleAdapter.Instance.Load(w => w.AppendItem("Code", post.ArticleCode));
            if (articles.Count != 1)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "Can not find the article."
                });
            }

            // user
            var fromUser = (Seagull2Identity)User.Identity;

            // comment
            CommentModel model = new CommentModel();
            model.Code = Guid.NewGuid().ToString();
            model.Content = post.Content;
            model.ArticleCode = post.ArticleCode;
            model.UserCodeFrom = fromUser.Id;
            model.UserNameFrom = fromUser.DisplayName;
            model.Creator = fromUser.Id;
            model.CreateTime = DateTime.Now;
            model.ValidStatus = true;

            if (string.IsNullOrWhiteSpace(post.CommentCode))
            {
                // root
                model.CommentCode = string.Empty;
                model.UserCodeTo = string.Empty;
                model.UserNameTo = string.Empty;
            }
            else
            {
                // sub
                model.CommentCode = post.CommentCode;
                model.UserCodeTo = post.UserCodeTo;
                model.UserNameTo = post.UserNameTo;
            }

            CommentAdapter.Instance.Update(model);

            return Ok(new BaseView()
            {
                State = true,
                Message = "sucess.",
                Data = new
                {
                    Code = model.Code,
                    UserCode = fromUser.Id,
                    UserName = fromUser.DisplayName
                }
            });
        }
        #endregion

        #region 删除评论
        /// <summary>
        /// 删除评论
        /// </summary>
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            // get comment info
            var comments = CommentAdapter.Instance.Load(w => w.AppendItem("Code", code));
            if (comments.Count != 1)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "Can not find the comment."
                });
            }
            var comment = comments[0];

            // check the code of user
            if (!string.Equals(comment.Creator, ((Seagull2Identity)User.Identity).Id))
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "You can not delete others' comments."
                });
            }

            // delete
            CommentAdapter.Instance.Delete(w =>
            {
                w.AppendItem("Code", code);
            });
            CommentAdapter.Instance.Delete(w =>
            {
                w.AppendItem("CommentCode", code);
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "sucess."
            });
        }
        #endregion

        #region 获取评论列表 - PC
        /// <summary>
        /// 获取评论列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForPC(string articleCode)
        {
            var view = new List<CommentViewModel>();

            // comment list
            var comment = CommentAdapter.Instance.Load(w => w.AppendItem("ArticleCode", articleCode));
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
                view.Add(item);
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }
        #endregion

        #region 删除评论 - PC
        /// <summary>
        /// 删除评论 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult DeleteForPC(string code)
        {
            // get comment info
            var comment = CommentAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
            if (comment == null)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "Can not find the comment."
                });
            }

            // delete
            CommentAdapter.Instance.Delete(w =>
            {
                w.AppendItem("Code", code);
            });
            CommentAdapter.Instance.Delete(w =>
            {
                w.AppendItem("CommentCode", code);
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "sucess."
            });
        }
        #endregion
    }
}
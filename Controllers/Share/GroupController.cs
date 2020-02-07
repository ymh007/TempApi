using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    /// 图文消息组控制器
    /// </summary>
    public class Share_GroupController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 保存图文消息 - PC
        /// <summary>
        /// 保存图文消息 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(GroupPostViewModel post)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            // 现有文章列表
            var articles = new ArticleCollection();

            // 图文组
            var group = new GroupModel();
            if (string.IsNullOrWhiteSpace(post.Code))
            {
                // 新增
                group.Code = Guid.NewGuid().ToString();
                group.Creator = userCode;
                group.CreateTime = DateTime.Now;
            }
            else
            {
                // 编辑
                group = GroupAdapter.Instance.Load(w => w.AppendItem("Code", post.Code)).SingleOrDefault();
                group.Modifier = userCode;
                group.ModifyTime = DateTime.Now;
                articles = ArticleAdapter.Instance.Load(w => w.AppendItem("GroupCode", group.Code));
            }
            group.IsEnable = post.IsEnable;
            group.SendGroupCode = post.SendGroupCode;
            group.MenuCode = post.MenuCode;
            group.ValidStatus = true;
            GroupAdapter.Instance.Update(group);

            // 文章
            post.Articles.ForEach(postArticle =>
            {
                var article = new ArticleModel();
                if (postArticle.Cover.IndexOf("data:image") == 0)
                {
                    //上传图片
                    string imgUrl = FileService.UploadFile(postArticle.Cover);
                    postArticle.Cover = imgUrl;
                }
                if (string.IsNullOrWhiteSpace(postArticle.Code) || postArticle.Code.StartsWith("temp-article-"))
                {
                    // 新增
                    article.Code = Guid.NewGuid().ToString();
                    article.Creator = userCode;
                    article.CreateTime = DateTime.Now;
                }
                else
                {
                    // 编辑
                    article = ArticleAdapter.Instance.Load(w => w.AppendItem("Code", postArticle.Code)).SingleOrDefault();
                    group.Modifier = userCode;
                    group.ModifyTime = DateTime.Now;
                }
                article.GroupCode = group.Code;
                article.Title = postArticle.Title;
                article.Author = postArticle.Author;
                article.Content = postArticle.Content;
                article.Link = postArticle.Link;
                article.Views = postArticle.Views;
                article.Cover = postArticle.Cover;
                article.Summary = postArticle.Summary;
                article.Sort = postArticle.Sort;
                article.ValidStatus = true;
                article.IsFile = postArticle.IsFile;
                article.FileUrl = postArticle.FileUrl;
                ArticleAdapter.Instance.Update(article);
            });

            // 从库中删除多余的文章
            articles.ForEach(article =>
            {
                var find = post.Articles.Where(w => w.Code == article.Code).ToList();
                if (find.Count < 1)
                {
                    ArticleAdapter.Instance.Delete(w => w.AppendItem("Code", article.Code));
                }
            });

            // 推送
            //var push = new PushService.Model()
            //{
            //    BusinessDesc= "营销分享",
            //    Title = "营销分享通知",
            //    Content = "您收到一条新的营销分享",
            //    Extras = new PushService.ModelExtras()
            //    {
            //        action = "MarketingShare",
            //        bags = group.MenuCode
            //    }
            //};
            //var pushResult = string.Empty;
            //if (string.IsNullOrWhiteSpace(group.SendGroupCode))
            //{
            //    // 全员推送
            //    push.SendType = PushService.SendType.All;
            //}
            //else
            //{
            //    // 给当前群组人员推送
            //    var userCodes = new List<string>();
            //    SendGroupPersonAdapter.Instance.Load(w => w.AppendItem("SendGroupCode", group.SendGroupCode)).ForEach(user =>
            //    {
            //        userCodes.Add(user.UserCode);
            //    });
            //    push.SendType = PushService.SendType.Person;
            //    push.Ids = string.Join(",", userCodes);
            //}
            //PushService.Push(push, out pushResult);
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "新建应用管理-营销学院-新建群发-群发消息");
            return Ok(new BaseView()
            {
                State = true,
                Message = "sucess."
            });
        }
        #endregion

        #region 获取图文详情 - PC
        /// <summary>
        /// 获取图文详情 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetModel(string groupCode)
        {
            var groupList = GroupAdapter.Instance.Load(w => w.AppendItem("Code", groupCode));
            if (groupList.Count != 1)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "Can not find the group."
                });
            }
            var group = groupList[0];
            var articles = ArticleAdapter.Instance.Load(w => w.AppendItem("GroupCode", groupCode));

            var view = new GroupPostViewModel()
            {
                Code = group.Code,
                IsEnable = group.IsEnable,
                SendGroupCode = group.SendGroupCode,
                MenuCode = group.MenuCode,
                Articles = new List<ArticlePostViewModel>() { }
            };

            articles.ForEach(model =>
            {
                view.Articles.Add(new ArticlePostViewModel()
                {
                    Code = model.Code,
                    Title = model.Title,
                    Author = model.Author,
                    Content = model.Content,
                    Link = model.Link,
                    Views = model.Views,
                    Cover = model.Cover,
                    Summary = model.Summary,
                    Sort = model.Sort,
                    IsFile = model.IsFile,
                    FileUrl = model.FileUrl
                });
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }
        #endregion

        #region 图文消息列表 - PC
        /// <summary>
        /// 图文消息列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(int pageIndex, int pageSize)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var list = new List<GroupForPCViewModel>();

            // 获取分页数据
            var dataCount = GroupAdapter.Instance.GetListForPC();
            var dataList = GroupAdapter.Instance.GetListForPC(pageSize, pageIndex);

            // 获取基础数据
            var sendGroups = SendGroupAdapter.Instance.Load(w => { });
            var menus = MenuAdapter.Instance.Load(w => { });
            var articles = ArticleAdapter.Instance.Load(w =>
            {
                w.LogicOperator = MCS.Library.Data.Builder.LogicOperatorDefine.Or;
                foreach (DataRow dr in dataList.Rows)
                {
                    w.AppendItem("GroupCode", dr["Code"]);
                }
            });

            foreach (DataRow dr in dataList.Rows)
            {
                list.Add(new GroupForPCViewModel(sendGroups, menus, articles)
                {
                    Code = dr["Code"].ToString(),
                    IsEnable = Convert.ToBoolean(dr["IsEnable"]),
                    SendGroupCode = dr["SendGroupCode"].ToString(),
                    MenuCode = dr["MenuCode"].ToString(),
                    CreateTime = Convert.ToDateTime(dr["CreateTime"])
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

        #region 删除图文消息 - PC
        /// <summary>
        /// 删除图文消息 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult Delete(dynamic post)
        {
            try
            {
                // 数据库创建触发器实现
                GroupAdapter.Instance.Delete(w => w.AppendItem("Code", Convert.ToString(post.code)));
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了应用管理-营销学院-素材库-素材库");
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "success."
                });
            }
            catch (Exception e)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = e.Message
                });
            }
        }
        #endregion

        #region 图文消息列表 - APP
        /// <summary>
        /// 图文消息列表 - APP
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForApp(int pageIndex, int pageSize, string menuCode = "")
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            var list = new List<GroupForAppViewModel>();

            var dataCount = GroupAdapter.Instance.GetListForApp(menuCode, userCode);
            var dataList = GroupAdapter.Instance.GetListForApp(pageSize, pageIndex, menuCode, userCode);

            foreach (DataRow dr in dataList.Rows)
            {
                list.Add(new GroupForAppViewModel()
                {
                    Code = dr["Code"].ToString(),
                    CreateTime = Convert.ToDateTime(dr["CreateTime"]).ToString("yyyy年MM月dd日 HH:mm"),
                    CreateTimeShort = Convert.ToDateTime(dr["CreateTime"]).ToString("MM月dd日")
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
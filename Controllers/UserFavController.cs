using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.UserFav;
using Seagull2.YuanXin.AppApi.Models.UserFav;
using Seagull2.YuanXin.AppApi.ViewsModel.UserFav;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 用户收藏
    /// </summary>
    public class UserFavController : ApiController
    {
        const string XmlNews = "News.xml";
        const string XmlNotice = "Notifications.xml";

        #region 添加/取消用户资讯收藏
        /// <summary>
        /// 添加/取消用户资讯收藏
        /// </summary>
        [HttpPost]
        public IHttpActionResult InformationFav(UserFavInformationAddViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                // 判断是否已经收藏
                var isFav = UserFavInformationAdapter.Instance.IsFav(post.WebId, post.ListId, post.ListItemId, post.Type.ToString(), user.Id);
                if (isFav)
                {
                    UserFavInformationAdapter.Instance.Cancel(post.WebId, post.ListId, post.ListItemId, post.Type.ToString(), user.Id);
                }
                else
                {
                    // 分类处理构建信息
                    var model = new UserFavInformationModel();
                    model.Code = Guid.NewGuid().ToString();
                    model.WebId = post.WebId;
                    model.ListId = post.ListId;
                    model.ListItemId = post.ListItemId;
                    model.Type = post.Type.ToString();
                    if (post.Type == Enum.EnumUserFavInformationType.New || post.Type == Enum.EnumUserFavInformationType.PartyNew)
                    {
                        #region 新闻中心、党建发文
                        var moss = new MossNewService(XmlNews);
                        var newModel = moss.GetNewsDetailByListItemId(user.Name, post.WebId, post.ListId, post.ListItemId);
                        if (newModel == null)
                        {
                            throw new Exception("参数错误，获取详情失败！");
                        }
                        model.Title = newModel.Title;
                        if (!string.IsNullOrWhiteSpace(newModel.Company1))
                        {
                            model.Source = newModel.Company1;
                        }
                        else
                        {
                            var web = moss.GetWebModel(post.WebId, user.Name);
                            if (web != null)
                            {
                                model.Source = web.Title;
                            }
                            else
                            {
                                model.Source = string.Empty;
                            }
                        }
                        model.ReleaseTime = Convert.ToDateTime(newModel.Created);
                        #endregion
                    }
                    else if (
                        post.Type == Enum.EnumUserFavInformationType.Important ||
                        post.Type == Enum.EnumUserFavInformationType.Unit ||
                        post.Type == Enum.EnumUserFavInformationType.Department ||
                        post.Type == Enum.EnumUserFavInformationType.Meeting ||
                        post.Type == Enum.EnumUserFavInformationType.PartyNotice)
                    {
                        #region 通知纪要收藏
                        var moss = new MossNoticeService(XmlNotice);
                        var noticeModel = moss.GetNoticeDetail(user.Name, post.WebId, post.ListId, post.ListItemId);
                        if (noticeModel == null)
                        {
                            throw new Exception("参数错误，获取详情失败！");
                        }
                        model.Title = noticeModel.Title;
                        model.Source = noticeModel._x4e3b__x9001__x5355__x4f4d_;
                        model.ReleaseTime = Convert.ToDateTime(noticeModel.Created);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("收藏类型错误！");
                    }
                    model.Creator = user.Id;
                    model.CreateTime = DateTime.Now;
                    model.ValidStatus = true;
                    UserFavInformationAdapter.Instance.Update(model);
                }
            });
            return Ok(result);
        }
        #endregion

        #region 获取用户资讯收藏
        /// <summary>
        /// 获取用户资讯收藏
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetFavListOfInformation(int pageSize, int pageIndex, string type = "")
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var count = UserFavInformationAdapter.Instance.GetList(user.Id, type);
                var data = UserFavInformationAdapter.Instance.GetList(pageSize, pageIndex, user.Id, type);
                var view = new List<UserFavInformationListViewModel>();
                data.ForEach(item =>
                {
                    view.Add(new UserFavInformationListViewModel()
                    {
                        Code = item.Code,
                        WebId = item.WebId,
                        ListId = item.ListId,
                        ListItemId = item.ListItemId,
                        Type = item.Type,
                        Title = item.Title,
                        Source = item.Source,
                        ReleaseTime = item.ReleaseTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                });
                return new ViewsModel.BaseViewPage()
                {
                    DataCount = count,
                    PageCount = count / pageSize + count % pageSize == 0 ? 0 : 1,
                    PageData = view
                };
            });
            return Ok(result);
        }
        #endregion

        #region 用户资讯收藏批量取消
        /// <summary>
        /// 用户资讯收藏批量取消
        /// </summary>
        [HttpPost]
        public IHttpActionResult InformationFavCancel(UserFavInformationCancelViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                if (post.CodeList.Count < 1)
                {
                    throw new Exception("参数不能为空！");
                }
                foreach (var code in post.CodeList)
                {
                    UserFavInformationAdapter.Instance.Delete(w => w.AppendItem("Code", code));
                }
            });
            return Ok(result);
        }
        #endregion
    }
}
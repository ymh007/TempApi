using System;
using System.Collections.Generic;
using System.Linq;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel.ManagementReport;

namespace Seagull2.YuanXin.AppApi.Adapter.ManagementReport
{
    /// <summary>
    /// 管理报告数据适配器
    /// </summary>
    public class ManagementReportAdapter : UpdatableAndLoadableAdapterBase<ManagementReportViewModel, ManagementReportViewModelCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static ManagementReportAdapter Instance = new ManagementReportAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        #region 获取管理报告列表
        /// <summary>
        /// 获取管理报告列表
        /// </summary>
        public List<ListItemViewModel> GetList(string date, string userName)
        {
            var listRet = new List<ListItemViewModel>();

            DateTime Created;
            try
            {
                Created = Convert.ToDateTime(date);
            }
            catch
            {
                Created = DateTime.Now;
            }

            // 获取Moss数据
            MossNewService ms = new MossNewService("News.xml");
            var moss = ms.GetManagementReportList(Created, userName);
            if (moss.Count < 1)
            {
                return listRet;
            }

            // 获取Id和Url列表
            var listId = new List<int>();
            var listUrl = new List<string>();
            foreach (var item in moss)
            {
                listId.Add(Convert.ToInt32(item.ID));
                listUrl.Add("'" + item.ArticleUrl + "'");
            }

            // 获取浏览次数列表
            var viewCountCollection = ViewOrPraiseCountAdapter.Instance.GetViewCountList(listUrl);

            // 获取点赞次数列表
            var praiseCountCollection = ViewOrPraiseCountAdapter.Instance.GetPraiseCountList(listUrl);

            // 获取用户点赞记录列表
            var praiseRecordCollection = PointPraiseRecordAdapter.Instance.GetList("SINOOCEANLAND\\" + userName, 1, listId);

            foreach (var item in moss)
            {
                var model = new ListItemViewModel(userName);
                model.WebId = item.WebId;
                model.ListId = item.ListId;
                model.Id = Convert.ToInt32(item.ID);
                model.Title = item.Title;
                model.Date = Convert.ToDateTime(item.Created).ToString("yyyy-MM-dd");
                model.DateFull = item.Created;
                model.ViewCount = 0;//---
                var viewCount = viewCountCollection.FirstOrDefault(m => m.Url == item.ArticleUrl);
                if (viewCount != null)
                {
                    model.ViewCount = viewCount.Count;
                }
                model.PraiseCount = 0;//---
                var praiseCount = praiseCountCollection.FirstOrDefault(m => m.Url == item.ArticleUrl);
                if (praiseCount != null)
                {
                    model.PraiseCount = praiseCount.Count;
                }
                model.IsPraise = false;//---
                var isPraise = praiseRecordCollection.FirstOrDefault(m => m.RelationID == Convert.ToInt32(item.ID));
                if (isPraise != null)
                {
                    model.IsPraise = true;
                }
                model.Summary = item.PublishingPageContent;
                model.Url = item.ArticleUrl;
                listRet.Add(model);
            }
            return listRet;
        }
        #endregion

        #region 获取管理报告详情及评论
        /// <summary>
        /// 获取管理报告详情及评论
        /// </summary>
        public DetailsViewModel GetModel(MossNewService.ManagementReportArticleModel moss, string source, string userName, int articleId)
        {
            DetailsViewModel model = new DetailsViewModel();
            model.Author = moss.ArticleByLine;
            model.Source = source;
            model.Content = moss.PublishingPageContent;
            model.CommentList = new List<DetailsViewModel.Comment>();
            var list = CommentListAdapter.Instance.GetList(userName, 1, articleId);
            foreach (var item in list)
            {
                model.CommentList.Add(new DetailsViewModel.Comment()
                {
                    Id = item.Id,
                    Content = item.Content,
                    Time = item.Time.ToString("yyyy-MM-dd HH:mm"),
                    PraiseCount = item.PraiseCount,
                    IsPraise = item.MyPraiseCount > 0 ? true : false,
                    UserName = item.UserName,
                    UserAvatar = UserHeadPhotoService.GetUserHeadPhoto(item.UserId)
                });
            }
            return model;
        }
        #endregion
    }
}
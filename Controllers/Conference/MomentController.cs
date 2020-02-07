using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 会议圈API控制器 
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        #region 公用

        #region 删除会议圈
        /// <summary>
        /// 删除会议圈
        /// </summary>
        [Route("DelMoment")]
        [HttpPost]
        public IHttpActionResult DelMoment(string id)
        {
            //删除会议圈时，同时删除相关图片
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                PraiseDetailModelAdapter.Instance.DelbyMomentID(id);
                MomentCommentModelAdapter.Instance.DelByMomentID(id);
                AttachmentAdapter.Instance.DelPicColl(id);
                MomentModelAdapter.Instance.DelTById(id);

            });
            return Ok(result);
        }
        #endregion

        #region 会议圈评论/回复评论(添加会议圈评论)--APP
        /// <summary>
        /// 会议圈评论/回复评论(添加会议圈评论)--APP
        /// </summary>
        [Route("AddMomentComment")]
        [HttpPost]
        public IHttpActionResult AddMomentComment(MomentCommentView model)
        {
            //APP输入-->文字+表情
            ViewModelBase result = new ViewModelBase() { State = false };
            ControllerHelp.RunAction(() =>
           {
               UserInfo userInfo = CurrentUser;
               MomentCommentModel momentComment = new MomentCommentModel
               {
                   ID = Guid.NewGuid().ToString("N"),
                   UserID = userInfo.CodeSeagull,
                   MomentCommentID = model.MomentComentID,
                   UserName = userInfo.DiaplayName,
                   Content = model.Content,
                   MomentID = model.MomentID,
                   ContentDate = DateTime.Now,
                   ValidStatus = true
               };
               MomentCommentModelAdapter.Instance.Update(momentComment);
               //返回数据
               MomentCommentView returnData = new MomentCommentView()
               {
                   ID = momentComment.ID,
                   Content = momentComment.Content,
                   IsCurrentUserAdd = true,
                   MomentComentID = momentComment.MomentCommentID,
                   MomentID = momentComment.MomentID,
                   UserID = momentComment.UserID,
                   UserName = momentComment.UserName
               };
               result.State = true;
               result.Data1 = returnData;
           });
            return Ok(result);
        }
        #endregion

        #region 删除会议圈评论--APP
        /// <summary>
        /// 删除会议圈评论--APP
        /// </summary>
        [Route("DelMomentComment")]
        [HttpPost]
        public IHttpActionResult DelMomentComment(string momentCommentID)
        {
            ViewModelBase result = new ViewModelBase() { State = false, Message = "删除失败" };
            ControllerHelp.RunAction(() =>
           {
               int delResult = MomentCommentModelAdapter.Instance.DelById(momentCommentID);
               if (delResult > 0)
               {
                   result.State = true;
                   result.Message = "";
               }
           });
            return Ok(result);
        }
        #endregion

        #region 会议圈点赞/取消点赞--APP
        /// <summary>
        /// 会议圈点赞/取消点赞--APP
        /// </summary>
        [Route("AddOrCancelPraiseDetail")]
        [HttpPost]
        public IHttpActionResult AddOrCancelPraiseDetail(string momentID)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = CurrentUser;

                bool isLike = MomentModelViewAdapter.Instance.CurrentUserIsLike(momentID, userInfo.CodeSeagull);
                //添加点赞数据，会议圈相关数据点赞数+1
                if (isLike == false)
                {
                    PraiseDetailModel pdetail = new PraiseDetailModel
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        UserID = userInfo.CodeSeagull,
                        UserName = userInfo.DiaplayName,
                        MomentID = momentID,
                        PraiseDate = DateTime.Now,
                        ValidStatus = true
                    };
                    PraiseDetailModelAdapter.Instance.Update(pdetail);
                    MomentModelAdapter.Instance.AddMomentLikes(momentID);
                }
                //删除点赞数据，会议圈相关数据点赞数-1
                else
                {
                    PraiseDetailModelAdapter.Instance.DeletePraiseDetailModel(momentID, userInfo.CodeSeagull);

                    MomentModelAdapter.Instance.DelMomentLikes(momentID);
                }
            });
            return Ok(result);
        }
        #endregion

        #endregion

        #region 会议会议圈

        #region 增加会议会议圈--APP
        /// <summary>
        /// 增加会议会议圈--APP
        /// </summary>
        [Route("AddMomentForApp")]
        [HttpPost]
        public IHttpActionResult AddMomentForApp(AddMomentModelView momentView)
        {
            //添加会议圈，APP--> 会议编码，发布内容，发布图片
            ViewModelBase result = new ViewModelBase();

            string momentCode = Guid.NewGuid().ToString("N");
            ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = CurrentUser;
                MomentModel moment = new MomentModel
                {
                    ID = momentCode,
                    ConferenceID = momentView.ConferenceID,
                    Content = momentView.Message,
                    PraiseCount = 0,
                    PublicUserID = userInfo.CodeSeagull,
                    PublicUserName = userInfo.DiaplayName,
                    PublicDate = DateTime.Now,
                    MomentTypeId = Enum.EnumMomentType.Conference,
                    ValidStatus = true
                };
                MomentModelAdapter.Instance.Update(moment);

                //上传图片处理
                if (momentView.AppPicColl != null && momentView.AppPicColl.Count > 0)
                {
                    PictureHelp picHelp = new PictureHelp()
                    {
                        ResourceID = momentCode,
                        AppPicColl = momentView.AppPicColl
                    };
                    AttachmentAdapter.Instance.SaveAppPicColl(picHelp);
                }
                //新增成功后返回发布人姓名及头像信息
                result.State = true;
                result.Data1 = new
                {
                    PublicUserCode = userInfo.CodeSeagull,
                    PublicUserName = userInfo.DiaplayName,
                    PublicUserHeadImageUrl = UserHeadPhotoService.GetUserHeadPhoto(CurrentUserCode),
                };
                result.Data2 = moment;
            });
            return Ok(result);
        }
        #endregion

        #region 查询会议会议圈列表（带点赞数，评论数,本人是否点赞）--APP
        /// <summary>
        /// 查询会议会议圈列表（带点赞数，评论数,本人是否点赞）--APP
        /// </summary>
        [Route("GetMomentListForAPP")]
        [HttpGet]
        public IHttpActionResult GetMomentListForAPP(int pageIndex, string conferenceID, DateTime searchTime)
        {
            ViewPageBase<List<MomentModelView>> list = new ViewPageBase<List<MomentModelView>>();
            ControllerHelp.SelectAction(() =>
            {
                list = MomentModelViewAdapter.Instance.GetViewByPage(pageIndex, conferenceID, searchTime, CurrentUserCode);
            });
            return Ok(list);
        }
        #endregion

        #region 查询会议圈列表（带点赞数，评论数）--PC
        /// <summary>
        /// 查询会议圈列表（带点赞数，评论数）--PC
        /// </summary>
        [Route("GetMomentListForPC")]
        [HttpGet]
        public IHttpActionResult GetMomentListForPC(int pageIndex, string conferenceID, DateTime searchTime)
        {
            ViewPageBase<List<MomentModelView>> list = new ViewPageBase<List<MomentModelView>>();
            ControllerHelp.SelectAction(() =>
            {
                list = MomentModelViewAdapter.Instance.GetViewByPage(pageIndex, conferenceID, searchTime, "");
            });
            return Ok(list);
        }
        #endregion

        #endregion

        #region 企业会议圈

        #region 增加企业会议圈--APP
        /// <summary>
        /// 增加企业会议圈--APP
        /// </summary>
        [Route("AddCompanyMomentForApp")]
        [HttpPost]
        public IHttpActionResult AddCompanyMomentForApp(AddMomentModelView momentView)
        {
            //添加会议圈，APP--> 发布内容，发布图片
            ViewModelBase result = new ViewModelBase();

            string momentCode = Guid.NewGuid().ToString("N");
            ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = CurrentUser;
                MomentModel moment = new MomentModel
                {
                    ID = momentCode,
                    Content = momentView.Message,
                    ConferenceID = "",
                    PraiseCount = 0,
                    PublicUserID = userInfo.CodeSeagull,
                    PublicUserName = userInfo.DiaplayName,
                    PublicDate = DateTime.Now,
                    MomentTypeId = Enum.EnumMomentType.Company,
                    ValidStatus = true
                };
                MomentModelAdapter.Instance.Update(moment);

                //上传图片处理
                if (momentView.AppPicColl != null && momentView.AppPicColl.Count > 0)
                {
                    PictureHelp picHelp = new PictureHelp()
                    {
                        ResourceID = momentCode,
                        AppPicColl = momentView.AppPicColl
                    };
                    AttachmentAdapter.Instance.SaveAppPicColl(picHelp);
                }

                //初始化当前登录人信息并返回
                //新增成功后返回发布人姓名及头像信息
                result.State = true;
                result.Data1 = new
                {
                    PublicUserCode = userInfo.CodeSeagull,
                    PublicUserName = userInfo.DiaplayName,
                    PublicUserHeadImageUrl = UserHeadPhotoService.GetUserHeadPhoto(CurrentUserCode),
                };
                result.Data2 = moment;
            });
            return Ok(result);
        }
        #endregion

        #region 查询企业会议圈列表（带点赞数，评论数,本人是否点赞）--APP
        /// <summary>
        /// 查询企业会议圈列表（带点赞数，评论数,本人是否点赞）--APP
        /// </summary>
        [Route("GetCompanyMomentListForAPP")]
        [HttpGet]
        public IHttpActionResult GetCompanyMomentListForAPP(int pageIndex, DateTime searchTime)
        {
            ViewPageBase<List<MomentModelView>> list = new ViewPageBase<List<MomentModelView>>();
            ControllerHelp.SelectAction(() =>
            {
                list = MomentModelViewAdapter.Instance.GetViewByPage(pageIndex, "", searchTime, CurrentUserCode);
            });
            return Ok(list);
        }
        #endregion

        #region 热议或企业圈是否有新的消息
        /// <summary>
        /// 描述：热议或企业圈是否有新的消息
        /// 作者：v-dengwh
        /// 邮箱：v-dengwh@sinooceanland.com
        /// 时间：2017-02-15 9:13
        /// </summary>
        [Route("GetTheNewTopicMonmentManager")]
        [HttpGet]
        public IHttpActionResult GetTheNewTopicMonmentManager(DateTime lastTime)
        {
            NewMangager newManager = new NewMangager();
            newManager.IsManager = false;
            newManager.ManagerTime = DateTime.Now;
            TopicModelCollection topicList = new TopicModelCollection();
            MomentModelCollection momentList = new MomentModelCollection();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                //热议数据
                topicList = TopicModelAdapter.Instance.LoadByCreateTime(lastTime);
                //企业圈数据
                momentList = MomentModelAdapter.Instance.GetMonentByLastTime(lastTime);
                if (topicList.Count > 0)
                {
                    newManager.IsManager = true;
                }
                else if (momentList.Count > 0)
                {
                    newManager.IsManager = true;
                }
            });
            return Ok(newManager);
        }
        #endregion

        #endregion

        #region 我发布的企业圈 - APP
        /// <summary>
        /// 我发布的企业圈 - APP
        /// </summary>
        [HttpGet, Route("GetMyList")]
        public IHttpActionResult GetMyList(int pageIndex, string userCode)
        {
            ViewPageBase<List<MomentModelView>> list = new ViewPageBase<List<MomentModelView>>();
            ControllerHelp.SelectAction(() =>
            {
                list = MomentModelViewAdapter.Instance.GetMyList(pageIndex, userCode, CurrentUserCode);
            });
            return Ok(list);
        }
        #endregion
    }

    public class NewMangager
    {
        /// <summary>
        /// 是否有新的消息
        /// </summary>
        public bool IsManager { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime ManagerTime { get; set; }
    }
}

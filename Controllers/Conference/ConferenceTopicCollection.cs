using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //会议议题控制器
    public partial class ConferenceController : ControllerBase
    {
        /// <summary> 
        /// 查询会议议题列表
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="agendaId">议程编码</param>
        /// <param name="name">查询名称</param>
        /// <returns></returns>
        [Route("GetTopicList")]
        [HttpGet]
        public IHttpActionResult GetTopicList(string conferenceID, int pageIndex, DateTime searchTime, string agendaId = "", string name = "")
        {
            ViewPageBase<List<ConferenceTopicsList>> conferenceTopicsList = new ViewPageBase<List<ConferenceTopicsList>>();
            ControllerHelp.SelectAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(conferenceID, false);

                conferenceTopicsList = ConferenceTopicsListAdapter.Instance.GetTopicsLists(conferenceID, agendaId, pageIndex, searchTime, userInfo.CodeUserInConference, name);
            });
            return Ok(conferenceTopicsList);
        }


        /// <summary>
        /// 查询单个议题
        /// </summary>
        /// <returns></returns>.
        [Route("GetTopicByID")]
        [HttpGet]
        public IHttpActionResult GetTopicByID(string topicsId)
        {
            
            ConferenceTopicsModel conferenceTopicsModel = ConferenceTopicsAdapter.Instance.LoadByConferenceTopics(topicsId);
            return Ok(conferenceTopicsModel);
        }
        /// <summary>
        /// 删除单个议题
        /// </summary>
        /// <returns></returns>
        [Route("DelConferenceTopicByID")]
        [HttpGet]
        public IHttpActionResult DelConferenceTopicByID(string topicsId)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                ConferenceTopicsAdapter.Instance.DelTById(topicsId);
            });
            return Ok(result);
        }

        /// <summary>
        /// 添加或修改议题--PC
        /// </summary>
        [Route("AddOrUpdateTopicPerson")]
        [HttpPost]
        public IHttpActionResult AddOrUpdateTopicPerson(ConferenceTopicView conferenceTopicsModel)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                bool isAdd = true;
                Log.WriteLog("conferenceTopicsModel" + Newtonsoft.Json.JsonConvert.SerializeObject(conferenceTopicsModel));
                string TopicImagePath = "";
                if (!conferenceTopicsModel.ID.IsEmptyOrNull())
                {
                    isAdd = false;

                    ConferenceTopicsModel oldModel = ConferenceTopicsAdapter.Instance.GetTByID(conferenceTopicsModel.ID);
                    TopicModel oldTopicModel = TopicModelAdapter.Instance.LoadByCode(oldModel.TopicCode);
                    if (TopicImagePath != null)
                    {
                        //获取热议图片
                        TopicImagePath = ImageHelp.SaveImage(conferenceTopicsModel.TopicImage, conferenceTopicsModel.TopicImageType).FileLoadPath;
                    }
                    else
                    {
                        TopicImagePath = oldTopicModel.Image;
                    }

                    //删除议题数据
                    ConferenceTopicsAdapter.Instance.DelTById(conferenceTopicsModel.ID);
                    //删除热议数据
                    TopicModelAdapter.Instance.DelByCode(oldModel.TopicCode);
                }
                else
                {
                    TopicImagePath = ImageHelp.SaveImage(conferenceTopicsModel.TopicImage, conferenceTopicsModel.TopicImageType).FileLoadPath;
                }
                TopicModel topicModel = new TopicModel()
                {
                    TopicId = Guid.NewGuid().ToString("N"),
                    CreateTime = DateTime.Now,
                    ValidStatus = true,
                    Creator = CurrentUserCode,
                    AttentionCount = 0,
                    DiscussCount = 0,
                    Anonymous = false,
                    Detail = "",
                    Summary = "",
                    Image = TopicImagePath,
                    Title = conferenceTopicsModel.TopicName,
                    CreatorName = CurrentUser.DiaplayName,
                    UserImage = UserHeadPhotoService.GetUserHeadPhoto(CurrentUserCode)
                };

                TopicModelAdapter.Instance.Update(topicModel);
                //会议议题
                string UUID = conferenceTopicsModel.ID.IsEmptyOrNull() ? Guid.NewGuid().ToString() : conferenceTopicsModel.ID;
                ConferenceTopicsModel conferenceTopics = new ConferenceTopicsModel();
                conferenceTopics.ID = UUID;
                conferenceTopics.AgendaID = conferenceTopicsModel.AgendaID;
                conferenceTopics.TopicName = conferenceTopicsModel.TopicName;
                conferenceTopics.Presenter = conferenceTopicsModel.Presenter;
                conferenceTopics.Guide = conferenceTopicsModel.Guide;
                conferenceTopics.Address = conferenceTopicsModel.Address;
                conferenceTopics.Creator = CurrentUserCode;
                conferenceTopics.CreateTime = DateTime.Now;
                conferenceTopics.ValidStatus = true;
                conferenceTopics.TopicCode = topicModel.TopicId;
                ConferenceTopicsAdapter.Instance.Update(conferenceTopics);
            });

            return Ok(result);
        }
    }

    #region 帮助类
    public class ConferenceTopicView
    {
        /// <summary>
        /// 议题编码（修改必填）
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceID { get; set; }
        /// <summary>
        /// 会议议程的ID
        /// </summary>
        public string AgendaID { get; set; }

        /// <summary>
        /// 议题名称
        /// </summary>
        public string TopicName { get; set; }

        /// <summary>
        /// 主持人
        /// </summary>
        public string Presenter { get; set; }

        /// <summary>
        /// 引导师
        /// </summary>
        public string Guide { get; set; }

        /// <summary>
        /// 地点
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 热议图片类型（话题表字段）
        /// </summary>
        public string TopicImageType { get; set; }
        /// <summary>
        /// 热议图片数据Base64（话题表字段）
        /// </summary>
        public string TopicImage { get; set; }
    }
    #endregion

}
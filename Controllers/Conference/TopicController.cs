using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //热议控制器
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 热议列表
        /// </summary>
        /// <returns></returns>
        [Route("GetTopicList")]
        [HttpGet]
        public IHttpActionResult GetTopicList(int pageIndex, DateTime searchTime, string topicName = "")
        {
            ViewPageBase<List<TopicModel>> topicList = new ViewPageBase<List<TopicModel>>();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                topicList = TopicModelAdapter.Instance.GetTopicListByPage(pageIndex, searchTime, topicName);
            });
            return Ok(topicList);
        }

        /// <summary>
        /// 描述：热议是否有新的消息
        /// 作者：v-dengwh
        /// 邮箱：v-dengwh@sinooceanland.com
        /// 时间：2017-02-14 10:19
        /// </summary>
        /// <param name="lastTime">上次访问热议时间</param>
        /// <returns></returns>
        [Route("GetTheNewTopicManager")]
        [HttpGet]
        public IHttpActionResult GetTheNewTopicManager(string lastTime)
        {
            NewMangager newManager = new NewMangager();
            newManager.IsManager = false;
            newManager.ManagerTime = DateTime.Now;
            TopicModelCollection topicList = new TopicModelCollection();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                topicList = TopicModelAdapter.Instance.LoadByCreateTime(Convert.ToDateTime(lastTime));
                if (topicList.Count > 0) {
                    newManager.IsManager = true;
                }
            });
            return Ok(newManager);
        }



        /// <summary>
        /// 删除热议
        /// </summary>
        /// <returns></returns>
        [Route("DelTopic")]
        [HttpPost]
        public IHttpActionResult DelTopic(string topicCode)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
             {
                 TopicModelAdapter.Instance.DelByCode(topicCode);
             });
            return Ok(result);
        }

    }
}

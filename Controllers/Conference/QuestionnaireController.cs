using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Conference;
using SOS2.AnnualMeeting.MobileSite.DataObjects;
using SOS2.AnnualMeeting.MobileSite.Models;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 问卷调查控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        #region 同步问卷 - PC
        /// <summary>
        /// 同步问卷 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult QuestionnaireSync(QuestionnaireSyncViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                // 同步地址
                string syncUrl = ConfigurationManager.AppSettings["QuestionnaireSync"];

                // 答题地址
                string answerUrl = ConfigurationManager.AppSettings["QuestionnaireAnswer"];

                // 会议编码
                string conferenceCode = post.ConferenceCode;

                // 问卷编码
                string questionnaireCode = post.QuestionnaireCode;

                // 查询是否已经同步
                QuestionnaireCollection oldQueColl = QuestionnaireAdapter.Instance.LoadByMeetingInformationCode(conferenceCode);
                if (oldQueColl.Count > 0 && oldQueColl.Find(old => old.Url == string.Format(answerUrl, questionnaireCode)) != null)
                {
                    throw new Exception("问卷编码为：" + questionnaireCode + "，已同步至数据库");
                }

                var Authorization1 = HttpContext.Current.Request.Headers["Authorization"];
                var Authorization2 = ActionContext.Request.Headers.Authorization.Parameter;

                // 获取问卷基本信息
                var json = HttpService.Get(string.Format(syncUrl, questionnaireCode));
                var list = JsonConvert.DeserializeObject<List<QuestionnaireVersionViewModel>>(json);
                if (list.Count < 1)
                {
                    throw new Exception("问卷编码错误");
                }
                var questionnaire = list.OrderByDescending(o => o.Versions).First();

                // 验证问卷是否发布
                if (string.IsNullOrWhiteSpace(questionnaire.StartTime) || string.IsNullOrWhiteSpace(questionnaire.EndTime))
                {
                    throw new Exception("问卷尚未发布");
                }

                // 写入数据库
                QuestionnaireAdapter.Instance.Update(new Questionnaire()
                {
                    Code = Guid.NewGuid().ToString(),
                    CnName = questionnaire.Title,
                    EnName = "",
                    Creater = ((Seagull2Identity)User.Identity).Id,
                    CreatTime = DateTime.Now,
                    IsValid = true,
                    Title = questionnaire.Title,
                    StartTime = Convert.ToDateTime(questionnaire.StartTime),
                    EndTime = Convert.ToDateTime(questionnaire.EndTime),
                    MettingInfomationCode = conferenceCode,
                    Url = string.Format(answerUrl, questionnaireCode)
                });
            });
            return Ok(result);
        }
        #endregion

        #region 删除问卷 - PC
        /// <summary>
        /// 删除问卷 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult QuestionnaireDelete(string questionnaireCode)
        {
            var result = ControllerService.Run(() =>
            {
                QuestionnaireAdapter.Instance.DelByCode(questionnaireCode);
            });
            return Ok(result);
        }
        #endregion

        #region 查询问卷列表 - PC
        /// <summary>
        /// 查询问卷列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetQuestionnaireListForPC(string conferenceCode)
        {
            var result = ControllerService.Run<QuestionnaireCollection>(() =>
            {
                var data = QuestionnaireAdapter.Instance.LoadByMeetingInformationCode(conferenceCode);
                data.ForEach(item =>
                {
                    //新老平台区分
                    if (item.Url.Contains("/THRWebApp/"))
                    {
                        // 新平台
                    }
                    else
                    {
                        // 老平台
                        item.Url += "&UserCode=" + CurrentUserCode;
                    }
                });
                return data;
            });
            return Ok(result);
        }
        #endregion

        #region 查询问卷详情 - PC
        /// <summary>
        /// 查询问卷详情 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetQuestionnaireModelForPC(string questionnaireCode)
        {
            var result = ControllerService.Run<Questionnaire>(() =>
            {
                var data = QuestionnaireAdapter.Instance.Load(w => w.AppendItem("Code", questionnaireCode)).SingleOrDefault();
                if (data == null)
                {
                    throw new Exception("编码错误");
                }
                //新老平台区分
                if (data.Url.Contains("/THRWebApp/"))
                {
                    // 新平台
                }
                else
                {
                    // 老平台
                    data.Url += "&UserCode=" + CurrentUserCode;
                }
                return data;
            });
            return Ok(result);
        }
        #endregion

        #region 查询问卷列表
        /// <summary>
        /// 查询问卷列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetQuestionnaireList(string conferenceID)
        {
            // 获取答题情况地址
            string answerInfoUrl = ConfigurationManager.AppSettings["QuestionnaireAnswerInfo"];

            // 当前用户
            var user = (Seagull2Identity)User.Identity;

            QuestionnaireModel result = new QuestionnaireModel(conferenceID);
            result.Questionnaire = new QuestionnaireCollection();
            ControllerHelp.RunAction(() =>
            {
                conferenceID.CheckStringIsNullOrEmpty("会议信息Code不能为空");

                result.Questionnaire = QuestionnaireAdapter.Instance.LoadByMeetingInformationCode(conferenceID);

                result.Questionnaire.ForEach(que =>
                {
                    que.IsValid = false;
                    //新老平台区分
                    if (que.Url.Contains("/THRWebApp/"))
                    {
                        // 新平台
                        // 判断用户是否已经作答
                        var questionCode = que.Url.Split('/').Last();
                        var json = HttpService.Get(string.Format(answerInfoUrl, questionCode, user.Id));
                        var list = JsonConvert.DeserializeObject<List<object>>(json);
                        if (list.Count > 0)
                        {
                            que.IsValid = true;
                        }
                    }
                    else
                    {
                        // 老平台
                        // 判断用户是否已经作答
                        var arr = que.Url.Split('=');
                        if (arr.Length >= 2)
                        {
                            var instanceCode = arr[1];
                            var answerUser = QuestionnaireAnswerUserAdapter.Instance.LoadByCode(user.Id, instanceCode);
                            if (answerUser != null)
                            {
                                que.IsValid = true;
                            }
                        }
                        // 答题地址
                        que.Url += "&UserCode=" + CurrentUserCode;
                    }
                });
            });
            return Ok(result);
        }
        #endregion
    }
}
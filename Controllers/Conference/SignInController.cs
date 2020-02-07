using MCS.Library.SOA.DataObjects;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using SOS2.AnnualMeeting.MobileSite.DataObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using log4net;
using System.Reflection;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    // 签到API控制器
    [RoutePrefix("Conference")]
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 根据会议议程编码获取签到详情列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="agendaID">议程编码</param>
        /// <param name="organizationName">组织名称</param>
        /// <param name="IsSelectSignIn">0-都查，1-签到，2,-未签到</param>
        /// <param name="AttendeeName">参会人姓名</param>
        /// <param name="IsWorkMan">是否排除工作人员</param>
        /// <param name="IsWorkMan">是否排除工作人员</param>
        /// /// <returns></returns>
        [Route("GetSignDetailList")]
        [HttpGet]
        public IHttpActionResult GetSignDetailList(int pageIndex, string conferenceID, DateTime searchTime, string agendaID = "", string organizationName = "", string IsSelectSignIn = "1", string AttendeeName = "", bool IsWorkMan = false, int AttendeeType = 1)
        {
            ViewPageBase<List<SignInDetailViewModelList>> list = new ViewPageBase<List<SignInDetailViewModelList>>();
            ControllerHelp.SelectAction(() =>
            {
                list = SignInDetailViewModelListAdapter.Instance.GetSignInDetailViewModelByPage(pageIndex, conferenceID, agendaID, organizationName, IsSelectSignIn, searchTime, AttendeeName, IsWorkMan, AttendeeType);
            });
            return Ok(list);
        }

        /// <summary>
        ///查询某个用户在某个议程的签到详情
        /// </summary>
        /// <param name="AgendaID">议程ID</param>
        /// <param name="AttendeeID">参会人ID</param>
        /// <returns></returns>
        [Route("GetUserSignIn")]
        [HttpGet]
        public IHttpActionResult GetUserSignIn(string AgendaID, string AttendeeID)
        {
            ViewModelBase result = new ViewModelBase();

            ControllerHelp.RunAction(() =>
           {
               if (!string.IsNullOrEmpty(AgendaID))
               {
                   if (!string.IsNullOrEmpty(AttendeeID))
                   {
                       SignInModel seatModel = SignInAdapter.Instance.LoadByCode(AttendeeID, AgendaID);
                       if (seatModel != null)
                       {
                           result.Data1 = seatModel;
                           result.State = true;
                           result.Message = "用户已签到";
                       }
                       else
                       {
                           result.Message = "该用户未签到";
                           result.State = false;
                       }
                   }
                   else
                   {
                       result.State = false;
                       result.Message = "参会人编码不能为空！";
                   }
               }
               else
               {
                   result.State = false;
                   result.Message = "会议编码不能为空！";
               }
           });
            return Ok(result);
        }

        /// <summary>
        /// 扫描二维码签到(增加通用)--APP
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="testUserLoginName">调试（或特殊需要使用）--海鸥二用户登录名</param>
        /// <returns></returns>
        [Route("AddConferenceSignIn")]
        [HttpPost]
        public IHttpActionResult AddConferenceSignIn(string conferenceID, string testEmail = "", int AttendeeType = 1)
        {
            string errorStr = "";
            ViewModelBase result = new ViewModelBase() { State = true };

            ControllerHelp.RunAction(() =>
            {
                AttendeeModel attMoel = null;
                if (string.IsNullOrEmpty(testEmail))
                {
                    var user = (Seagull2Identity)User.Identity;
                    log.Info("签到人是:" + user.DisplayName + " id:" + user.Id + " 登录名:" + user.LogonName);
                    attMoel = AttendeeAdapter.Instance.GetAttendeeByAttendaID(user.Id, conferenceID, AttendeeType);

                }
                else
                {
                    attMoel = AttendeeAdapter.Instance.Load(p =>
                     p.AppendItem("Email", testEmail).
                      AppendItem("ConferenceID", conferenceID).
                     AppendItem("AttendeeType", AttendeeType)
                        ).FirstOrDefault();
                }
                if (attMoel == null)
                {
                    errorStr = "您不属于参会人，无法签到！";
                }
                else
                {
                    DateTime currentUserSignTime = DateTime.Now;

                    ConferenceAgendaCollection agendaColl = ConferenceAgendaAdapter.Instance.Load(m => m.AppendItem("ConferenceID", conferenceID));

                    errorStr = "没有找到可签到的议程！";
                    foreach (var agenda in agendaColl)
                    {
                        if (agenda.NeedSign && currentUserSignTime > agenda.SignBeginDate && currentUserSignTime < agenda.SingEndDate)
                        {
                            errorStr = "";

                            bool isSign = SignInAdapter.Instance.LoadByCode(attMoel.ID, agenda.ID) == null ? false : true;
                            if (isSign)
                            {
                                errorStr = "您已签到！";
                            }
                            else
                            {
                                SignInModel signModel = new SignInModel()
                                {
                                    CreateTime = DateTime.Now,
                                    AttendeeID = attMoel.ID,
                                    ValidStatus = true,
                                    ID = Guid.NewGuid().ToString(),
                                    AgendaID = agenda.ID,
                                    SignDate = currentUserSignTime,
                                    SignSourceType = Enum.EnumSignSourceType.App
                                };
                                SignInAdapter.Instance.AddSignIn(signModel);

                                #region 签到完成后，调接口将签到信息输送到签到展示页面
                                try
                                {

                                    //AttendeeModel attMoel = AttendeeAdapter.Instance.LoadByID(signModel.AttendeeID);
                                    string departmentName = attMoel.OrganizationStructure;
                                    if (departmentName.IsEmptyOrNull() || departmentName.Split('\\').Length < 2)
                                    {
                                        departmentName = "";
                                    }
                                    else
                                    {
                                        departmentName = departmentName.Split('\\')[departmentName.Split('\\').Length - 2];
                                    }
                                    object obj = new
                                    {
                                        photo = attMoel.PhotoAddress,
                                        name = attMoel.Name,
                                        department = departmentName,
                                        place = attMoel.CityName,
                                        signcount = SignInAdapter.Instance.SignInCount(agenda.ID),
                                        group = conferenceID
                                    };
                                    GoEasy(conferenceID, obj);
                                }
                                catch (Exception e)
                                {
                                    log.Error("签到推送服务出错" + e.Message.ToString());
                                }
                                #endregion
                            }
                            break;
                        }
                    }
                }
            });
            if (errorStr != "")
            {
                result.State = false;
                result.Message = errorStr;
            }

            return Ok(result);
        }

        /// <summary>
        /// GoEasy
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public void GoEasy(string conferenceId, object obj)
        {
            try
            {
                string postUrl = ConfigurationManager.AppSettings["GoEasyUrl"];
                string key = ConfigurationManager.AppSettings["GoEasyAppKey"];
                string channel = conferenceId;

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("appkey", key);
                dictionary.Add("channel", channel);
                dictionary.Add("content", JsonConvert.SerializeObject(obj));
                ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
                HttpService.Post(postUrl, dictionary);
            }
            catch (Exception e)
            {
                log.Error("GoEasy异常信息:" + e.ToString());
            }
        }


        /// <summary>
        /// 根据ID删除用户签到--PC
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [Route("DelUserSignIn")]
        [HttpPost]
        public IHttpActionResult DelUserSignIn(SignInModel model)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                SignInAdapter.Instance.DelUserSignIn(model.ID);
            });

            return Ok(result);
        }
        /// <summary>
        /// 获取会议下的所有第三级组织机构--PC
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        [Route("GetOrganizationColl")]
        [HttpGet]
        public IHttpActionResult GetOrganizationColl(string conferenceID)
        {
            List<string> orgList = new List<string>();
            ControllerHelp.SelectAction(() =>
            {
                AttendeeCollection attColl = AttendeeAdapter.Instance.GetAttendeeCollectionByConference(conferenceID);

                attColl.ForEach(att =>
                {
                    string org = "";
                    if (!att.OrganizationStructure.IsEmptyOrNull())
                    {
                        org = att.OrganizationStructure.Split('\\')[2];
                        if (!orgList.Contains(org))
                        {
                            orgList.Add(org);
                        }
                    }
                });
            });
            return Ok(orgList);
        }

        /// <summary>
        /// 签到数据导出成EXCEL
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ExportSigninData")]
        [HttpGet]
        public HttpResponseMessage ExportSigninData(string conferenceID, string agendaID = "", string isSelectSignIn = "0", string attendeeName = "", int AttendeeType = 1)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                List<SignInDetailViewModelList> list = SignInDetailViewModelListAdapter.Instance.GetSignInData(conferenceID, agendaID, isSelectSignIn, attendeeName, AttendeeType);

                List<dynamic> dataList = new List<dynamic>();
                for (int i = 0; i < list.Count; i++)
                {
                    dataList.Add(new
                    {
                        Index = i + 1,
                        AgendaName = list[i].AgendaTitle,
                        AttendeeName = list[i].AttendeeName,
                        OrganizationStructure = list[i].OrganizationStructure,
                        IsSigned = list[i].IsSigned ? "已签到" : "未签到",
                        SignDateStr = list[i].IsSigned ? list[i].SignDateStr : ""
                    });
                }

                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"序号","Index" },
                    {"会议议程","AgendaName" },
                    {"参会人","AttendeeName" },
                    {"部门","OrganizationStructure" },
                    {"签到状态","IsSigned" },
                    {"签到时间","SignDateStr" }
                    };
                string conferenceName = ConferenceAdapter.Instance.GetTByID(conferenceID).Name;
                result = ExcelHelp<dynamic, List<dynamic>>.ExportExcelData(dicColl, dataList, "[" + conferenceName + "]" + "签到记录");

            });
            return result;
        }

        private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
        {
            //为了通过证书验证，总是返回true
            return true;
        }
    }
}
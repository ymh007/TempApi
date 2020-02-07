using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using log4net;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Newtonsoft.Json;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    public abstract class ControllerBase : ApiController
    {
        /// <summary>
        /// Http请求
        /// </summary>
        public HttpRequest baseRequest = HttpContext.Current.Request;
        /// <summary>
        /// Http响应
        /// </summary>
        public HttpResponse baseResponse = HttpContext.Current.Response;
        /// <summary>
        /// 日志
        /// </summary>
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 海鸥二用户
        /// </summary>
        public UserInfo CurrentUser
        {
            get
            {
                try
                {
                    OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name);

                    UserInfo user = new UserInfo
                    {
                        CodeSeagull = users[0].ID,
                        DiaplayName = users[0].DisplayName,
                        LoginName = users[0].LogOnName,
                        FullPath = users[0].FullPath
                    };
                    return user;
                }
                catch (Exception e)
                {
                    Log.WriteLog("ControllerBase:" + e.Message);

                    if (ConfigurationManager.AppSettings["IsTest"] == "true")
                    {
                        //测试模式
                        string userLoginName = "liumh";   //刘闽辉

                        OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, userLoginName);

                        UserInfo user = new UserInfo
                        {
                            CodeSeagull = users[0].ID,
                            DiaplayName = users[0].DisplayName,
                            LoginName = users[0].LogOnName,
                            FullPath = users[0].FullPath
                        };
                        return user;
                    }
                    else
                    {
                        Log.WriteLog("非测试模式下：获取用户编码失败！");
                        throw new Exception("获取用户编码失败！");
                    }
                }
            }
        }

        /// <summary>
        /// 海鸥二编码
        /// </summary>
        public string CurrentUserCode
        {
            get
            {
                try
                {
                    return ((Seagull2Identity)User.Identity).Id;
                }
                catch (Exception e)
                {
                    log.Error("获取用户编码“((Seagull2Identity)User.Identity).Id”方式异常：" + JsonConvert.SerializeObject(e));
                    try
                    {
                        OguObjectCollection<IUser> users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, User.Identity.Name);
                        return users[0].ID;
                    }
                    catch
                    {
                        if (ConfigurationManager.AppSettings["IsTest"] == "true")
                        {
                            //测试模式
                            string userCode = "f4048590-feec-4c15-990d-2f7693146937";   //刘闽辉
                            //userCode = "ef31cf3b-a784-4cbe-bf74-a53a25be6559";   //周佳良
                            //userCode = "d48366cc-cf21-b855-4e16-6f3230f4e71c";  //朱鹏树
                            //userCode = "fb6883f9-a5ae-8af7-4f0d-5dd92466c414";  //肖小勇
                            Log.WriteLog("获取用户编码失败，已采用测试用户编码：" + userCode);
                            return userCode;
                        }
                        else
                        {
                            Log.WriteLog("非测试模式下：获取用户编码失败!");
                            throw new Exception("获取用户编码失败");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 当前登录用户是否是某个会议的参会人员,并返回当前用户在该会议下的用户编码
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="IsCheckingUser">是否校验用户属于该会议参会人</param>
        /// <param name="testUserLoginName">海鸥二用户登录名</param>
        /// <returns></returns>
        public UserInfo IsInConferenceCurrentUser(string conferenceID, bool IsCheckingUser = true, string testUserLoginName = "")
        {
            OguObjectCollection<IUser> users = new OguObjectCollection<IUser>();
            if (testUserLoginName == "")
            {
                users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, CurrentUserCode);
            }
            else
            {
                users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, testUserLoginName);
            }

            UserInfo userInfo = null;

            string currentUserCode = users.Count > 0 ? users[0].ID : CurrentUserCode;

            AttendeeModel attModel = new AttendeeModel();
            if (testUserLoginName.IsEmptyOrNull())
            {
                log.Info(string.Format("获取参会人员信息：用户编码：{0}，会议编码：{1}", currentUserCode, conferenceID));
                attModel = AttendeeAdapter.Instance.Load(m => m.AppendItem("AttendeeID", currentUserCode).AppendItem("ConferenceID", conferenceID).AppendItem("ValidStatus", true).AppendItem("AttendeeType", 1)).FirstOrDefault();
                if (attModel != null)
                {
                    log.Info("获取参会人员信息：" + JsonConvert.SerializeObject(attModel));
                }
            }
            else
            {
                if (users.Count > 0)
                {
                    attModel = AttendeeAdapter.Instance.Load(m => m.AppendItem("AttendeeID", currentUserCode).AppendItem("ConferenceID", conferenceID).AppendItem("ValidStatus", true)).FirstOrDefault();
                }
                else
                {
                    attModel = AttendeeAdapter.Instance.Load(m => m.AppendItem("Email", testUserLoginName + "@%", "like").AppendItem("ConferenceID", conferenceID).AppendItem("ValidStatus", true)).FirstOrDefault();
                }
            }

            userInfo = new UserInfo
            {
                CodeSeagull = currentUserCode,
                CodeUserInConference = attModel != null ? attModel.ID : "",
                IsInConference = attModel == null ? false : true,
                DiaplayName = users.Count > 0 ? users[0].DisplayName : "",
                LoginName = attModel != null ? attModel.LoginName : users.Count > 0 ? users[0].LogOnName : "",
                FullPath = attModel != null ? attModel.OrganizationStructure : users.Count > 0 ? users[0].FullPath : ""
            };

            if (IsCheckingUser)
            {
                if (users.Count == 0)
                {
                    throw new Exception("获取当前用户信息失败");
                };
                if (userInfo.IsInConference == false)
                {
                    throw new Exception("当前用户不属于会议参会人！操作失败");
                }
            }
            return userInfo;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 海鸥二用户编码
        /// </summary>
        public string CodeSeagull { get; set; }
        /// <summary>
        /// 海鸥二用户登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 海鸥二用户显示名
        /// </summary>
        public string DiaplayName { get; set; }
        /// <summary>
        /// 用户在会议下的编码
        /// </summary>
        public string CodeUserInConference { get; set; }
        /// <summary>
        /// 用户部门
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 该用户是否属于会议参会人员
        /// </summary>
        public bool IsInConference { get; set; }
    }
}
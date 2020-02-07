using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Web;
using MCS.Library.WcfExtensions;
using SinoOcean.Seagull2.Framework.Contracts;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 预定会议室通用服务
    /// </summary>
    public class MeetingService
    {
        #region 根据Code查询预定详情
        /// <summary>
        /// 根据Code查询预定详情
        /// </summary>
        public static MeetingServiceContract GetByCode(string code)
        {
            MeetingServiceContract _MeetingServiceContract;
            string serviceUrl = ConfigurationManager.AppSettings["LaunchMeetingService"];
            var endpoint = new EndpointAddress(serviceUrl);
            using (var factory = new WfClientChannelFactory<ILaunchMeeting>(endpoint))
            {
                ILaunchMeeting service = factory.CreateChannel();
                _MeetingServiceContract = service.GetMeetingData(code);
            }
            return _MeetingServiceContract;
        }
        #endregion

        #region 获取会议所有人员Code列表
        /// <summary>
        /// 获取会议所有人员Code列表
        /// </summary>
        public static List<string> GetPeople(MeetingServiceContract model)
        {
            var userList = new List<string>();
            // --- 创建人
            if (!string.IsNullOrEmpty(model.creatorID) && !userList.Contains(model.creatorID))
            {
                userList.Add(model.creatorID);
            }
            // --- 主持人
            if (!string.IsNullOrEmpty(model.ModeratorID) && !userList.Contains(model.ModeratorID))
            {
                userList.Add(model.ModeratorID);
            }
            // --- 服务人
            if (!string.IsNullOrEmpty(model.ServerCode) && !userList.Contains(model.ServerCode))
            {
                userList.Add(model.ServerCode);
            }
            // --- 参会人
            if (!string.IsNullOrEmpty(model.MeetingMenCodeList))
            {
                foreach (string code in model.MeetingMenCodeList.Split(';'))
                {
                    if (!string.IsNullOrEmpty(code) && !userList.Contains(code))
                    {
                        userList.Add(code);
                    }
                }
            }
            return userList;
        }
        #endregion
    }
}
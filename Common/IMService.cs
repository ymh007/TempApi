using System;
using System.Configuration;
using System.Reflection;
using Grpc.Core;
using log4net;
using Newtonsoft.Json;
using YuanXin.Framework.PushService.Greeter.Models;
using YuanXin.IM.Group.Greeter;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// IM 服务
    /// </summary>
    public class IMService
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string host = ConfigurationManager.AppSettings["IM_Host"];
        private static int port = Convert.ToInt32(ConfigurationManager.AppSettings["IM_Port"]);

        #region 获取IM移动办公AppId
        /// <summary>
        /// 获取IM移动办公AppId
        /// </summary>
        public static long GetMobileOfficeAppId()
        {
            try
            {
                string sql = @"SELECT [ID] FROM [OAuth].[ApplicationInfo] WHERE [DisplayName] = '远薪移动办公'";
                SqlDbHelper db = new SqlDbHelper();
                object result = db.ExecuteScalar(sql);
                if (result != null && result != DBNull.Value)
                {
                    long appId;
                    if (long.TryParse(result.ToString(), out appId))
                    {
                        return appId;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Log.WriteLog("获取IM移动办公APPID失败：" + e.Message);
                return 0;
            }
        }
        #endregion

        #region 获取IM用户Id
        /// <summary>
        /// 获取IM用户Id
        /// </summary>
        /// <param name="code">海鸥二用户编码</param>
        public static long GetUserId(string code)
        {
            try
            {
                string sql = @"SELECT [Id] FROM [OAuth].[Seagull2Relation] WHERE [UserId] = '" + code + "'";
                SqlDbHelper db = new SqlDbHelper();
                object result = db.ExecuteScalar(sql);
                if (result != null && result != DBNull.Value)
                {
                    long userId;
                    if (long.TryParse(result.ToString(), out userId))
                    {
                        return userId;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                Log.WriteLog("获取IM用户Id失败：" + e.Message);
                return 0;
            }
        }
        #endregion

        #region 创建群组
        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="groupId">群Id</param>
        /// <param name="groupName">群名称</param>
        /// <param name="groupPhoto">群头像</param>
        /// <param name="masterId">管理员Id</param>
        /// <param name="memberIds">成员Id列表</param>
        /// <param name="isSuper">是否超级群</param>
        /// <returns>群组Id</returns>
        public static bool CreateGroup(out int groupId, string groupName, string groupPhoto, long masterId, long[] memberIds, bool isSuper)
        {
            try
            {
                var channel = new Channel(host, port, ChannelCredentials.Insecure);
                var client = new Service.ServiceClient(channel);
                var create = new Create();
                create.AppId = GetMobileOfficeAppId();
                create.GroupName = groupName;
                create.GroupPhoto = groupPhoto;
                create.Announcement = "无";
                create.MasterId = masterId.ToString();
                create.Members = string.Join(",", memberIds);
                create.IsSuper = isSuper;
                var result = client.CreateGroup(create);
                if (result.GroupId > 0)
                {
                    groupId = result.GroupId;
                    log.Info("成功创建IM群组A：" + JsonConvert.SerializeObject(create));
                    log.Info("成功创建IM群组B：" + JsonConvert.SerializeObject(result));
                    return true;
                }
                log.Error("创建IM群组失败：" + JsonConvert.SerializeObject(result));
                groupId = 0;
                return false;
            }
            catch (Exception e)
            {
                log.Error("创建IM群组失败：" + JsonConvert.SerializeObject(e));
                groupId = 0;
                return false;
            }
        }
        #endregion

        #region 群组添加成员
        /// <summary>
        /// 群组添加成员
        /// </summary>
        /// <param name="groupId">群组id</param>
        /// <param name="memberId">成员id</param>
        /// <returns></returns>
        public static bool AddMember(int groupId = -1, long[] memberId = null)
        {
            try
            {
                if (groupId == -1 || memberId == null) return false;
                var channel = new Channel(host, port, ChannelCredentials.Insecure);
                var client = new Service.ServiceClient(channel);
                var meber = new Member();
                meber.AppId = GetMobileOfficeAppId();
                meber.GroupId = groupId;
                meber.Members = string.Join(",", memberId);
                var add = client.AddMember(meber);
                return add.Status;
            }
            catch (Exception e)
            {
                log.Error("添加群成员失败：" + e);
                return false;
            }
        }
        #endregion

        #region 添加或编辑群组公告
        /// <summary>
        /// 添加或编辑会议公告
        /// </summary>
        /// <param name="groupId">群组ID</param>
        /// <param name="announcement">公告内容</param>
        /// <returns></returns>
        public static bool UpdateAnnouncement(int groupId, string announcement)
        {
            try
            {
                if (groupId == 0) return false;
                var channel = new Channel(host, port, ChannelCredentials.Insecure);
                var client = new Service.ServiceClient(channel);
                var updateNotice = new UpdateNotice();
                updateNotice.GroupId = groupId;
                updateNotice.Announcement = announcement;
                var notice = client.UpdateAnnouncement(updateNotice);
                return notice.Status;
            }
            catch (Exception e)
            {
                Log.WriteLog(e.ToString());
                return false;
            }
        }
        #endregion

        #region 发送群消息
        /// <summary>
        /// 发送群消息
        /// </summary>
        public static bool PostGroupMessage(int groupId, string msg, int sender)
        {
            try
            {
                var channel = new Channel(host, port, ChannelCredentials.Insecure);
                var client = new Service.ServiceClient(channel);
                var gm = new GroupMessage();
                gm.AppId = GetMobileOfficeAppId();
                gm.GroupId = groupId;
                gm.Message = JsonConvert.SerializeObject(new ImContext()
                {
                    text = msg
                });
                gm.Sender = sender;
                var add = client.PostGroupMessage(gm);
                return add.Status;
            }
            catch (Exception e)
            {
                log.Error("发送群消息异常：" + e);
                return false;
            }
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.IM;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.Models.IM;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 会议企业会话数据适配器
    /// </summary>
    public class ConferenceCompanyDialogAdapter : BaseAdapter<ConferenceCompanyDialog, ConferenceCompanyDialogCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static ConferenceCompanyDialogAdapter Instance = new ConferenceCompanyDialogAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 构造
        /// </summary>
        public ConferenceCompanyDialogAdapter()
        {
            BaseConnectionStr = ConnectionString;
        }

        /// <summary>
        /// 根据会议编码查询会话列表
        /// </summary>
        public ConferenceCompanyDialogCollection LoadByConferenceID(string conferenceID)
        {
            return Load(m => m.AppendItem("ConferenceID", conferenceID));
        }
    }

    /// <summary>
    /// 会议会话帮助类
    /// </summary>
    public class ConferenceDialogHelp
    {
        #region 创建会议企业会话
        /// <summary>
        /// 创建会议企业会话
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="userCode">海鸥二用户编码</param>
        public static void CreateConferenceDialog(string conferenceID, string conferenceName, string userCode)
        {
            bool isCreateWxGroup = Convert.ToBoolean(ConfigurationManager.AppSettings["IsCreateWxGroup"]);

            //会话类型集合
            DialogTypeCollection dtColl = DialogTypeAdapter.Instance.GetTColl();

            //会话内容类型集合
            DialogContentTypeCollection dctColl = DialogContentTypeAdapter.Instance.GetTColl();

            //会议企业会话集合
            ConferenceCompanyDialogCollection ccdColl = new ConferenceCompanyDialogCollection();

            dtColl.ForEach(dt =>
            {
                dctColl.ForEach(dct =>
                {
                    string id = Guid.NewGuid().ToString("N");
                    ConferenceCompanyDialog ccd = new ConferenceCompanyDialog()
                    {
                        ID = id,
                        DialogCode = id,
                        ConferenceID = conferenceID,
                        DialogContentTypeID = dct.ID,
                        DialogTypeID = dt.ID
                    };
                    ccdColl.Add(ccd);
                });
            });

            ccdColl.ForEach(ccd =>
            {
                DialogContentType dctModel = dctColl.Find(dct => dct.ID == ccd.DialogContentTypeID);
                //群组名称
                string dialogName = conferenceName.Substring(0, conferenceName.Length > 10 ? 10 : conferenceName.Length) + "_" + dctModel.Name;

                bool result = false;

                // 微信
                if (ccd.DialogTypeName == "WeiXin" && isCreateWxGroup)
                {
                    string userlist = ConfigurationManager.AppSettings["ConferenceCompanyDialogUser"];
                    result = WXService.CreateGroup(ccd.DialogCode, dialogName, userlist.Split(',')[0], userlist.Split(','));
                }

                // IM
                else if (ccd.DialogTypeName == "IM")
                {
                    var masterId = IMService.GetUserId(userCode);
                    var memberIds = new long[] { masterId };
                    int groupId;
                    result = IMService.CreateGroup(out groupId, dialogName, string.Empty, masterId, memberIds, false);
                    if (result)
                    {
                        ccd.DialogCode = groupId.ToString();
                    }
                }

                if (result)
                {
                    //添加数据库数据
                    ConferenceCompanyDialogAdapter.Instance.Update(ccd);
                }
            });
        }
        #endregion
    }

    /// <summary>
    /// 更新工作人员会话数据
    /// </summary>
    public class WorkerDialogHelp
    {
        #region 更新所有工作人员会议企业会话数据
        /// <summary>
        /// 更新所有工作人员会议企业会话数据(必须把该方法放在更新数据库方法之前)
        /// </summary>
        /// <param name="workerColl">工作人员集合</param>
        /// <param name="conferenceID">会议编码</param>
        public static void WorkerCollDialog(WorkerCollection workerColl, string conferenceID)
        {
            //操作人userid（loginName）--暂取会话管理人
            string op_user = ConfigurationManager.AppSettings["ConferenceCompanyDialogUser"].Split(',')[0];
            //key:会话ID，value:需添加登陆用户集合
            Dictionary<string, List<string>> addDicUserList = new Dictionary<string, List<string>>();
            //key:会话ID，value:需移除登陆用户集合
            Dictionary<string, List<string>> delDicUserList = new Dictionary<string, List<string>>();

            ConferenceCompanyDialogCollection diaColl = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conferenceID);
            diaColl.ForEach(dia =>
            {
                string key = dia.ID;
                List<string> addUserList = new List<string>();
                List<string> delUserList = new List<string>();

                WorkerCollection oldWorkerColl = WorkerAdapter.Instance.LoadByConferenceID(conferenceID);
                oldWorkerColl.ForEach(old =>
                {
                    //工作人员类型需和会议会话内容类型一致
                    if (old.UserID != "" /*&& old.WorkerTypeID == dia.DialogContentTypeID*/)
                    {
                        ContactsModel oldWorkerContacts = ContactsAdapter.Instance.LoadByMail(old.Email);
                        delUserList.Add(oldWorkerContacts.Logon_Name);
                    }
                });

                workerColl.ForEach(worker =>
                {
                    //工作人员类型需和会议会话内容类型一致
                    //if (worker.WorkerTypeID == dia.DialogContentTypeID)
                    //{
                        ContactsModel newWorkerContacts = ContactsAdapter.Instance.LoadByMail(worker.Email);
                        if (newWorkerContacts != null)
                        {
                            addUserList.Add(newWorkerContacts.Logon_Name);
                        }
                    //}
                });
                addDicUserList.Add(key, addUserList);
                delDicUserList.Add(key, delUserList);
            });

            foreach (string key in addDicUserList.Keys)
            {
                string[] adduser = addDicUserList[key].ToArray();
                string[] deluser = delDicUserList[key].ToArray();
                //微信企业号
                if (diaColl.Find(dia => dia.ID == key).DialogTypeName == "WeiXin")
                {
                    WXService.UpdateGroup(chatid: key, op_user: op_user, add_user_list: adduser, del_user_list: deluser);
                }
                //IM
                else if (diaColl.Find(dia => dia.ID == key).DialogTypeName == "IM")
                {

                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 现场服务发送群组消息
    /// </summary>
    public class SiteServiceDialogHelp
    {
        #region 现场服务发送群组消息
        /// <summary>
        /// 现场服务发送群组消息
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="userID">参会人编码</param>
        /// <param name="siteServiceID">现场服务编码</param>
        public static void SendMessage(string conferenceID, string userID, string siteServiceID)
        {
            // 获取会议下的所有会话
            ConferenceCompanyDialogCollection diaColl = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conferenceID);
            if (diaColl.Count < 1)
            {
                throw new Exception("该会议下没有任何企业会话！");
            }

            // 获取现场服务信息
            SiteServiceModel siteModel = SiteServiceAdapter.Instance.LoadByID(siteServiceID);
            if (siteModel == null)
            {
                throw new Exception("没有获取到现场服务信息！");
            }

            // 获取参会人信息
            AttendeeModel attModel = AttendeeAdapter.Instance.LoadByAttendeeID(userID, conferenceID);
            if (attModel == null)
            {
                throw new Exception("该会议下没有找到参会人编码为:" + userID + "的参会人！");
            }

            // 某某某（座位：二排1号，手机：13288888888）申请：饮用水。备注：无。
            string content = @"{0}（座位：{1}，手机：{2}）申请：{3}。备注：{4}。";
            content = string.Format(content,
                attModel.Name,
                attModel.SeatAddress,
                attModel.MobilePhone,
                siteModel.SiteServiceTypeName,
                siteModel.Remarks.Trim().Length < 1 ? "无" : siteModel.Remarks);

            diaColl.ForEach(dia =>
            {
                // 微信
                if (dia.DialogTypeName == "WeiXin" && dia.DialogContentTypeName == "现场服务")
                {
                    //操作人userid（loginName）--暂取会话管理人
                    string op_user = ConfigurationManager.AppSettings["ConferenceCompanyDialogUser"].Split(',')[0];
                    if (op_user.IsEmptyOrNull())
                    {
                        throw new Exception("请先配置会话管理人员");
                    }
                    WXService.SendMsg("group", dia.DialogCode, op_user, "text", content);
                }
                // IM
                else if (dia.DialogTypeName == "IM" && dia.DialogContentTypeName == "现场服务")
                {
                    int groupId = Convert.ToInt32(dia.DialogCode);
                    GroupInfoModel model = GroupInfoAdapter.Instance.GetGroupInfoByID(groupId);
                    if (model != null)
                    {
                        IMService.PostGroupMessage(Convert.ToInt32(dia.DialogCode), content, model.Master);
                    }
                }
            });
        }
        #endregion
    }

    /// <summary>
    /// 预定车辆发送群组消息
    /// </summary>
    public class VehicleBookingDialogHelp
    {
        #region 预定车辆发送群组消息
        /// <summary>
        /// 预定车辆发送群组消息
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="userID">参会人编码</param>
        /// <param name="vehicleBookingID">预定车辆编码</param>
        public static void SendMessage(string conferenceID, string userID, string vehicleBookingID)
        {
            // 获取会议下的所有会话
            ConferenceCompanyDialogCollection diaColl = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conferenceID);
            if (diaColl.Count < 1)
            {
                throw new Exception("该会议下没有任何企业会话！");
            }

            // 获取车辆预定信息
            VehicleBookingModel vbookModel = VehicleBookingAdapter.Instance.LoadByID(vehicleBookingID);
            if (vbookModel == null)
            {
                throw new Exception("没有获取到车辆预定信息！");
            }

            // 获取参会人信息
            AttendeeModel attModel = AttendeeAdapter.Instance.LoadByAttendeeID(userID, conferenceID);
            if (attModel == null)
            {
                throw new Exception("该会议下没有找到参会人编码为:" + userID + "的参会人！");
            }

            // 某某某（座位：二排1号，手机：13288888888）申请于2017-07-07 15:36从西安到北京。备注：无。
            string content = @"{0}（座位：{1}，手机：{2}）申请于{3}从{4}到{5}。备注：{6}。";
            content = string.Format(content,
                attModel.Name,
                attModel.SeatAddress,
                attModel.MobilePhone,
                vbookModel.ReserveTime.ToString("yyyy-MM-dd HH:mm:ss"),
                vbookModel.BeginPlace,
                vbookModel.EndPlace,
                vbookModel.Remark.Trim().Length < 1 ? "无" : vbookModel.Remark);

            diaColl.ForEach(dia =>
            {
                // 微信
                if (dia.DialogTypeName == "WeiXin" && dia.DialogContentTypeName == "预定车辆")
                {
                    //操作人userid（loginName）--暂取会话管理人
                    string op_user = ConfigurationManager.AppSettings["ConferenceCompanyDialogUser"].Split(',')[0];
                    if (op_user.IsEmptyOrNull())
                    {
                        throw new Exception("请先配置会话管理人员");
                    }
                    WXService.SendMsg("group", dia.DialogCode, op_user, "text", content);
                }

                // IM
                else if (dia.DialogTypeName == "IM" && dia.DialogContentTypeName == "预定车辆")
                {
                    int groupId = Convert.ToInt32(dia.DialogCode);
                    GroupInfoModel model = GroupInfoAdapter.Instance.GetGroupInfoByID(groupId);
                    if (model != null)
                    {
                        IMService.PostGroupMessage(Convert.ToInt32(dia.DialogCode), content, model.Master);
                    }
                }
            });
        }
        #endregion
    }

    /// <summary>
    /// 群公告
    /// </summary>
    public class Announcement
    {
        #region 添加或编辑会议公告
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
                return IMService.UpdateAnnouncement(groupId, announcement);
            }
            catch (Exception)
            {
                return false;
                throw new Exception("操作异常");
            }

        }
        #endregion
    }

    /// <summary>
    /// 会议交流推送群组消息
    /// </summary>
    public class MeetOfChange
    {
        #region 会议交流群组推送消息
        /// <summary>
        /// 会议交流群组推送消息
        /// </summary>
        /// <param name="conferenceId">会议编码</param>
        /// <param name="attendeeId">参会人编码</param>
        /// <param name="noticId">会议通告编码</param>
        public static void sendMeetingMessage(string conferenceId, string attendeeId, string noticId)
        {
            // 获取会议下的所有会话
            ConferenceCompanyDialogCollection diaColl = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conferenceId);
            if (diaColl.Count < 1)
            {
                throw new Exception("该会议下没有任何企业会话！");
            }

            var conference = ConferenceAdapter.Instance.GetConferenceByConferenceId(conferenceId);
            if (conference == null)
            {
                return;
            }
            var attendee = AttendeeAdapter.Instance.LoadByAttendeeID(attendeeId, conferenceId);
            if (attendee == null)
            {
                return;
            }
            string content = @"欢迎加入会议：{0}_会议交流群组，{1}（座位：{2}，手机：{3}）";
            content = string.Format(content, conference.Name, attendee.Name, attendee.SeatAddress, attendee.MobilePhone);
            var dialogs = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(conference.ID);
            foreach (var dialog in dialogs)
            {
                if (dialog.DialogTypeName == "IM")
                {

                }
                if (dialog.DialogTypeName == "WeiXin")
                {

                }
            }

            List<int> list = SeatsAdapter.Instance.GetGroupIdByConferenceId(conferenceId);
            GroupInfoAdapter adapter = new GroupInfoAdapter();
            GroupInfoModel model = adapter.GetGroupInfoByID(list[2]);
            IMService.PostGroupMessage(list.Last(), content, model.Master);
        }
        #endregion
    }
}
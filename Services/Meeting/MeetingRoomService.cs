using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;
using MCS.Library.SOA.DataObjects;
using SinoOcean.Seagull2.Framework.MasterData;
using SinoOcean.Seagull2.SendExpressBroker;
using SOS2.Meeting.MobileSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Seagull2.YuanXin.AppApi.Controllers.MeetingController;
using MD = SinoOcean.Seagull2.Framework.MasterData;
using TD = SinoOcean.Seagull2.TransactionData.Meeting;

namespace Seagull2.YuanXin.AppApi.Services
{
    public class MeetingRoomService : MD.MeetingRoomAdapter
    {
        private static object SequenceLock = new object();
        public static readonly SOS2.Meeting.MobileSite.Adapter.RoomBookingAdapter Instance = new SOS2.Meeting.MobileSite.Adapter.RoomBookingAdapter();
        private static ExchangeServiceBinding esb;
        private static object thisLock = new object();
        public static MeetingRoomService roomInstance = new MeetingRoomService();
        private enum MessageTypes
        {
            Mail,
            SMS
        }

        /// <summary>
        /// 对集合按照空闲时间进行排序
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static MeetingRoomCollection OrderByColl(MeetingRoomCollection mrColl)
        {
            //List<MeetingRoom> roomList = mrColl.OrderBy(mr => mr.Remark).ToList();
            mrColl.Sort((m, n) =>
            {
                if (Convert.ToDouble(m.Remark) > Convert.ToDouble(n.Remark))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });
            MeetingRoomCollection resultColl = new MeetingRoomCollection();

            foreach (var room in mrColl)
            {
                try
                {
                    if (room.Remark == "0")
                    {
                        room.Remark = "空闲中";
                    }
                    else if (room.Remark == "99999")
                    {
                        room.Remark = "忙碌中";
                    }
                    else
                    {
                        room.Remark = "空闲" + Convert.ToDouble(room.Remark) + "小时";
                    }
                    room.Meeting_Equipment.ForEach(me =>
                    {
                        if (me.MeetingSupplyTypeCode == MeetingSupplyTypeEnum.Flower)
                        {
                            me.MeetingCode = "zhuohua.png";
                        }
                        else if (me.MeetingSupplyTypeCode == MeetingSupplyTypeEnum.Fruit)
                        {
                            me.MeetingCode = "shuiguo.png";
                        }
                        else if (me.MeetingSupplyTypeCode == MeetingSupplyTypeEnum.Phone)
                        {
                            me.MeetingCode = "dianhua.png";
                        }
                        else if (me.MeetingSupplyTypeCode == MeetingSupplyTypeEnum.Projector)
                        {
                            me.MeetingCode = "touying.png";
                        }
                        else if (me.MeetingSupplyTypeCode == MeetingSupplyTypeEnum.Video)
                        {
                            me.MeetingCode = "shipin.png";
                        }
                    });
                }
                catch (Exception e)
                {
                    Log.WriteLog(e.Message);
                    Log.WriteLog(e.StackTrace);
                    throw e;
                }

                resultColl.Add(room);
            };
            return resultColl;
        }

        /// <summary>
        /// 保存会议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static HelpClass SaveMeeting(RoomBookingInputModel model, IUser user, HelpClass returnResult)
        {
            returnResult.obj1 = true;

            model.ContactPersonCode = user.ID;//会议联系人是当前登录人
            TD.Meetings meeting = RoomBookingInputModel.CopyFrom(model, user);
            meeting.ValidStatus = true;
            meeting.MeetingMen.ForEach(meetMen =>
            {
                meetMen.Creator = user;
            });

            MD.MeetingRoom Room = MD.MeetingRoomAdapter.Instance.Load(model.RoomGUID);
            model.UnitPrice = Room.UnitPrice;
            model.HidResult = decimal.Parse((model.EndTime - model.StartTime).TotalMinutes + "") / 60;
            model.MeetingLoacation = Room.Location + "-" + Room.CnName;
            model.RoomName = Room.CnName;

            computeTotal(model, meeting, Room);//计算价钱与超额判断

            if (model.MeetingCode.IsNullOrWhiteSpace())
            {

                int meetingRoomState = TD.MeetingAdapter.Instance.IsExists(model.RoomGUID, meeting.StartTime.ToString(), meeting.EndTime.ToString(), meeting.ID);

                if (meetingRoomState == 0)
                {
                    meeting = SendMessageAndEmail(meeting, model, user, Room);

                    if ((model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()) && meeting.IsSendEmail) || !model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()))
                    {
                        //避免同时预定会议室
                        lock (SequenceLock)
                        {
                            updateAndInsert(meeting, user, model);
                        }
                        returnResult.obj1 = true;
                        returnResult.obj3 = meeting;
                    }
                    else
                    {
                        returnResult.obj1 = false;
                        returnResult.obj2 = "可能由于网络原因，没有发送邮件，请您再次点击预订！";
                        //modelState.AddModelError("Default", "可能由于网络原因，没有发送邮件，请您再次保存！");
                        //return false;
                    }
                }
                else
                {
                    returnResult.obj1 = false;
                    returnResult.obj2 = "该时间段的会议室已被预定，请重新选择时间段！";
                    //modelState.AddModelError("Default", "该时间段的会议室已被预定，请重新选择时间段");
                    //return false;
                }
            }
            else
            {
                int meetingRoomState = TD.MeetingAdapter.Instance.IsExists(model.RoomGUID, meeting.StartTime.ToString(), meeting.EndTime.ToString(), meeting.ID);

                if (meetingRoomState > 0)
                {
                    returnResult.obj1 = false;
                    returnResult.obj2 = "该时间段的会议室已被预定，请重新选择时间段！";
                    //modelState.AddModelError("Default", "该时间段的会议室已被预定，请重新选择时间段");
                    //return false;

                }
                else
                {
                    meeting = SendMessageAndEmail(meeting, model, user, Room);
                    if ((model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()) && meeting.IsSendEmail) || !model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()))
                    {
                        updateAndInsert(meeting, user, model);
                        returnResult.obj1 = true;
                        returnResult.obj3 = meeting;
                        //return true;
                    }
                    else
                    {
                        returnResult.obj1 = false;
                        returnResult.obj2 = "可能由于网络原因，没有发送邮件，请您再次点击预订！";
                        //modelState.AddModelError("Default", "可能由于网络原因，没有发送邮件，请您再次保存！");
                        //return false;
                    }
                }
            }
            return returnResult;
        }


        #region 辅助方法

        /// <summary>
        /// 计算价钱与超额判断
        /// </summary>
        /// <param name="model"></param>
        /// <param name="meeting"></param>
        /// <param name="Room"></param>
        private static void computeTotal(RoomBookingInputModel model, TD.Meetings meeting, MD.MeetingRoom Room)
        {
            decimal dayOfWeekDiscount = 0;
            decimal dayOfWorkDiscount = 0;
            //decimal dayOfSpecialDiscount = 0;

            #region 计算总价钱
            decimal noWorkHours = 0;
            //decimal workHoursTotal = 0;
            //decimal spMeetingHours = 0;
            long countday = 0;//总天数
            long weekday = 0;//工作日  
            DateTime currentStartWorkDate = DateTime.MinValue;
            DateTime currentEndWorkDate = DateTime.MinValue;

            decimal sWeekEndHours = 0;//当开始日期是周末的时候的小时
            decimal eWeekEndHours = 0;//当结束日期是周末的时候的小时

            foreach (var dis in Room.MeetingRoomDiscount)
            {
                switch (dis.DiscountType)
                {
                    case MD.MeetingDiscountEnum.DayOfWeek:
                        if (dis.ValidStatus)
                        {
                            dayOfWeekDiscount = dis.Discount / 10;
                            if (dayOfWeekDiscount <= 0)
                            {
                                dayOfWeekDiscount = 1;
                            }
                        }
                        break;
                    case MD.MeetingDiscountEnum.DayOfWork:
                        if (dis.ValidStatus)
                        {
                            dayOfWorkDiscount = dis.Discount / 10;
                            if (dayOfWorkDiscount <= 0)
                            {
                                dayOfWorkDiscount = 1;
                            }
                        }

                        DateTime currentDate = DateTime.Now.Date;
                        currentStartWorkDate = currentDate.AddHours(dis.StartTime.Hour).AddMinutes(dis.StartTime.Minute);
                        currentEndWorkDate = currentDate.AddHours(dis.EndTime.Hour).AddMinutes(dis.EndTime.Minute);
                        //每天工作时间 9
                        var dayOfWorkHours = (currentDate.AddHours(dis.EndTime.Hour).AddMinutes(dis.EndTime.Minute)) -
                                             (currentDate.AddHours(dis.StartTime.Hour).AddMinutes(dis.StartTime.Minute));
                        //每日非工作时间 15
                        var nodayOWworkHours = 24 - (dayOfWorkHours).TotalHours;
                        DateTime fromTime = meeting.StartTime.Date.AddDays(-1);
                        DateTime toTime = meeting.EndTime.Date;//.AddDays(1);
                        TimeSpan timeSpan = toTime.Subtract(fromTime);
                        countday = timeSpan.Days;//获取两个日期间的总天数  

                        //循环用来扣除总天数中的双休日  
                        for (int i = 0; i < countday; i++)
                        {
                            DateTime tempdt = fromTime.AddDays(i + 1);
                            if (tempdt.DayOfWeek != DayOfWeek.Saturday && tempdt.DayOfWeek != DayOfWeek.Sunday)
                            {
                                weekday++;
                            }
                        }
                        var days = weekday - (countday - weekday);
                        if (weekday >= 2)
                        {
                            days = weekday - 2;
                        }


                        //会议预定当天非工作时间
                        TimeSpan beginSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date;
                        DateTime StartWorkTime = meeting.StartTime.Date.AddHours(dis.StartTime.Hour).AddMinutes(dis.StartTime.Minute);//预定会议当天工作开始时间
                        DateTime EndWorkTime = meeting.StartTime.Date.AddHours(dis.EndTime.Hour).AddMinutes(dis.EndTime.Minute);//预定会议当天工作结束时间
                        //会议开始时间小于上班时间
                        if (meeting.StartTime < StartWorkTime && meeting.EndTime > EndWorkTime)
                        {
                            beginSurplus = (StartWorkTime - meeting.StartTime) + (meeting.StartTime.Date.AddDays(1) - EndWorkTime);
                        }

                        if (meeting.StartTime < StartWorkTime && meeting.EndTime <= EndWorkTime)
                        {
                            beginSurplus = StartWorkTime - meeting.StartTime;
                        }

                        if (meeting.StartTime >= StartWorkTime && meeting.EndTime > EndWorkTime)
                        {
                            beginSurplus = meeting.StartTime.Date.AddDays(1) - meeting.StartTime;
                        }

                        if (meeting.StartTime >= StartWorkTime && meeting.EndTime <= EndWorkTime)
                        {
                            beginSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date;
                        }


                        if (meeting.StartTime.DayOfWeek == DayOfWeek.Saturday || meeting.StartTime.DayOfWeek == DayOfWeek.Sunday)
                        {
                            beginSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date;
                            sWeekEndHours = Convert.ToDecimal((meeting.StartTime.Date.AddDays(1) - meeting.StartTime).TotalHours);
                        }

                        //会议结束当天非工作时间
                        var overDate = meeting.EndTime.Date;

                        //会议结束当天的非工作时间
                        TimeSpan endSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date; ;
                        //会议结束当天的工作开始时间和结束时间
                        DateTime eStartWorkTime = meeting.EndTime.Date.AddHours(dis.StartTime.Hour).AddMinutes(dis.StartTime.Minute);
                        DateTime eEndWorkTime = meeting.EndTime.Date.AddHours(dis.EndTime.Hour).AddMinutes(dis.EndTime.Minute);

                        if (meeting.EndTime > eEndWorkTime)
                        {
                            endSurplus = (eStartWorkTime - overDate) + (meeting.EndTime - eEndWorkTime);
                        }
                        else
                        {
                            endSurplus = eStartWorkTime - overDate;
                        }

                        if (meeting.EndTime.DayOfWeek == DayOfWeek.Saturday || meeting.EndTime.DayOfWeek == DayOfWeek.Sunday)
                        {
                            endSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date;
                            eWeekEndHours = Convert.ToDecimal((meeting.EndTime - meeting.EndTime.Date).TotalHours);
                        }

                        //非工作时间总和（不包含周末）
                        if (days <= 0)
                        {
                            noWorkHours = Convert.ToDecimal(beginSurplus.TotalHours + endSurplus.TotalHours);
                        }
                        else
                        {
                            if (meeting.EndTime > meeting.StartTime.Date.AddDays(1) || meeting.EndTime > eEndWorkTime)
                            {
                                noWorkHours = Convert.ToDecimal(Convert.ToDouble(days) * nodayOWworkHours + beginSurplus.TotalHours + endSurplus.TotalHours);
                            }
                            else
                            {
                                noWorkHours = Convert.ToDecimal(beginSurplus.TotalHours + endSurplus.TotalHours);
                            }

                        }

                        if (meeting.StartTime.Date == meeting.EndTime.Date)
                        {
                            if (meeting.StartTime.DayOfWeek == DayOfWeek.Saturday || meeting.StartTime.DayOfWeek == DayOfWeek.Sunday)
                            {
                                noWorkHours = 0;
                                sWeekEndHours = 0;
                                eWeekEndHours = 0;
                            }
                            else
                            {
                                if (meeting.StartTime < StartWorkTime)
                                {
                                    beginSurplus = StartWorkTime - meeting.StartTime;
                                }
                                else
                                {
                                    beginSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date;
                                }
                                if (meeting.EndTime > EndWorkTime)
                                {
                                    endSurplus = meeting.EndTime - EndWorkTime;
                                }
                                else
                                {
                                    endSurplus = DateTime.MinValue.Date - DateTime.MinValue.Date;
                                }
                                noWorkHours = Convert.ToDecimal(beginSurplus.TotalHours + endSurplus.TotalHours);
                            }
                        }
                        break;
                }
            }
            decimal weekendHours = 0;
            if (countday > (countday - weekday))
            {
                weekendHours = (countday - weekday) * 24;
                if (sWeekEndHours > 0)
                {
                    weekendHours = weekendHours - 24;
                }
                if (eWeekEndHours > 0)
                {
                    weekendHours = weekendHours - 24;
                }
            }

            weekendHours = weekendHours + sWeekEndHours + eWeekEndHours;
            if (meeting.StartTime.Date == meeting.EndTime.Date)
            {
                if (meeting.StartTime.DayOfWeek == DayOfWeek.Saturday || meeting.StartTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendHours = model.HidResult;
                }
            }

            var workHours = model.HidResult - weekendHours - noWorkHours;
            var lastTotal = meeting.Total;//修改会议钱的会议总额
            meeting.Total = (workHours * model.UnitPrice) +
                            (weekendHours * model.UnitPrice * dayOfWeekDiscount) +
                            (noWorkHours * model.UnitPrice * dayOfWorkDiscount);

            #endregion

            #region 预算是否超额判断
            /*
             * 1、根据成本中心code获取当前年该成本中心已花费总额（包含预定会议室花费和会议室超额罚款）
             * 2、根据成本中心code获取该成本中心预算余额
            */
            decimal spentTotal = 0;//已花费总额
            var meetingCollection = TD.MeetingAdapter.Instance.LoadDataByYear(DateTime.Now.Year.ToString(), meeting.CostCenterCode);
            if (meetingCollection != null && meetingCollection.Count != 0)
            {
                spentTotal += meetingCollection.Sum(item => item.Total);
            }

            var overtimeFineCollection = TD.MeetingsOverTimeFineAdapter.Instance.LoadDataByYear(DateTime.Now.Year.ToString(), meeting.CostCenterCode);
            if (null != overtimeFineCollection && overtimeFineCollection.Count != 0)
            {
                spentTotal += overtimeFineCollection.Sum(item => item.OverTimeFine);
            }

            var meetingsBudget = TD.MeetingsBudgetAdapter.Instance.LoadDataByYear(DateTime.Now.Year.ToString(), meeting.CostCenterCode).FirstOrDefault();
            if (null != meetingsBudget)
            {
                meetingsBudget.BudgetBalance = meetingsBudget.BudgetBalance - meeting.Total + lastTotal;
                meetingsBudget.LastClearingTime = DateTime.Now;
                TD.MeetingsBudgetAdapter.Instance.Update(meetingsBudget);
            }
            #endregion
        }
        private static TD.Meetings SendMessageAndEmail(TD.Meetings meeting, RoomBookingInputModel model, IUser user, MD.MeetingRoom room)
        {
            List<IUser> users = CommonOperation.getSendUser(meeting);

            if (model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()))
            {
                string body = CommonOperation.CreateMailContent(meeting, new OguUser(model.ModeratorCode).DisplayName, new OguUser(model.MeetingMenArr[0]).DisplayName, new OguUser(model.ContactPersonCode).DisplayName, user.DisplayName, model.Content, room);

                string IsHand = System.Configuration.ConfigurationManager.AppSettings["IsHand"];

                if (IsHand == "true")
                {
                    List<string> IsUser = new List<string>();

                    IsUser.Add("lixh@yuanxin2015.com");

                    IsUser.Add("v-wannian@sinooceanland.com");

                    MailMessage mmMessage = SOS2.Meeting.MobileSite.Adapter.RoomBookingAdapter.CreateMeetingRequest(meeting.StartTime, meeting.EndTime,
                    meeting.Subject, model.Content, model.MeetingLoacation, meeting.Moderator.DisplayName, "lixh@yuanxin2015.com", IsUser);


                    string[] strDt = meeting.EndTime.ToString().Split('/');

                    string fileName = strDt[0] + strDt[1] + ".xml";

                    //CreateLog(mmMessage, _bodyText, _typeText, _bodyHtml, _typeHtml, _bodyCalendar, meeting.StartTime.ToString() + meeting.EndTime.ToString() + roomLocation.Text + DateTime.Now.ToString(), fileName);

                    //smtp.Send(mmMessage);//20110420这句代码，加上下边这句代码就发两遍了
                    //给本人差日程 20110419解决自己发的会议邀请，自己的日历看不到日程的问题
                    Instance.ExchangeOperation("lixh@yuanxin2015.com");
                    meeting = Instance.CreateAppointment(esb, meeting, "lixh@yuanxin2015.com", model.MeetingLoacation, model.Content, IsUser);
                }
                else
                {
                    MailMessage mmMessage = SOS2.Meeting.MobileSite.Adapter.RoomBookingAdapter.CreateMeetingRequest(meeting.StartTime, meeting.EndTime,
                    meeting.Subject, model.Content, model.MeetingLoacation, meeting.Moderator.DisplayName, new OguUser(model.ContactPersonCode).Email, CommonOperation.GenerateReceiver(users, MessageTypes.Mail.ToString()));


                    string[] strDt = meeting.EndTime.ToString().Split('/');

                    string fileName = strDt[0] + strDt[1] + ".xml";

                    try
                    {
                        //CreateLog(mmMessage, _bodyText, _typeText, _bodyHtml, _typeHtml, _bodyCalendar, meeting.StartTime.ToString() + meeting.EndTime.ToString() + roomLocation.Text + DateTime.Now.ToString(), fileName);

                        //smtp.Send(mmMessage);//20110420这句代码，加上下边这句代码就发两遍了
                        //给本人差日程 20110419解决自己发的会议邀请，自己的日历看不到日程的问题
                        Instance.ExchangeOperation(new OguUser(model.ContactPersonCode).Email);
                        meeting = Instance.CreateAppointment(esb, meeting, new OguUser(model.ContactPersonCode).Email, model.MeetingLoacation, model.Content, CommonOperation.GenerateReceiver(users, MessageTypes.Mail.ToString()));
                    }
                    catch (Exception ex)
                    {
                        //WriteError(ex.Message + "v-wanggf发送邮件");
                    }
                }
            }

            //如果发送短信   0为短信，1为邮件
            if (model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Message).ToString()))
            {
                if ((model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()) && meeting.IsSendEmail) || !model.RemindMethodArr.Contains(((int)MD.MeetingRemindWayEnum.Email).ToString()))
                {
                    try
                    {
                        string IsHand = System.Configuration.ConfigurationManager.AppSettings["IsHand"];

                        if (IsHand == "true")
                        {
                            List<string> phone = new List<string>();
                            phone.Add("15637057302");
                            phone.Add("13911721091");
                            SMSSender.Send(phone, "会议通知", string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                                meeting.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                meeting.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                model.MeetingLoacation,
                                                meeting.Subject));
                        }
                        else
                        {
                            SMSSender.Send(CommonOperation.GenerateReceiver(users, MessageTypes.SMS.ToString()), "会议通知", string.Format("您 {0}至{1}在{2}有主题为“{3}”的会议，请准时参加。",
                                                meeting.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                                meeting.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                                model.MeetingLoacation,
                                                meeting.Subject));
                        }
                    }
                    catch (Exception ex)
                    {
                        //WriteError(ex.Message + "短信发送");
                    }
                }
            }

            return meeting;
        }

        private static void updateAndInsert(TD.Meetings meeting, IUser user, RoomBookingInputModel bookingModel)
        {
            //会议议题插入或更新
            TD.MeetingTopic meetingtopic = new TD.MeetingTopic();
            meeting.MeetingTopics.Clear();

            meetingtopic.Code = Guid.NewGuid().ToString();
            meetingtopic.CnName = bookingModel.Content;
            meetingtopic.MeetingsCode = meeting.ID;
            meetingtopic.Creator = user.ID;
            meetingtopic.CreateTime = DateTime.Now;
            meetingtopic.Reportor = user.DisplayName;

            meeting.MeetingTopics.Add(meetingtopic);

            //会议提醒方式插入或更新
            meeting.MeetingsRemindWay.Clear();
            for (int i = 0; i <= bookingModel.RemindMethodArr.Count() - 1; i++)
            {
                TD.Meetings_RemindWay meeting_RemindWay = new TD.Meetings_RemindWay();
                meeting_RemindWay.RemindWayCode = (MD.MeetingRemindWayEnum)Convert.ToInt32(bookingModel.RemindMethodArr[i]);
                meeting_RemindWay.MeetingsCode = meeting.ID;
                meeting_RemindWay.Creator =user;
                meeting_RemindWay.CreateTime = DateTime.Now;

                meeting.MeetingsRemindWay.Add(meeting_RemindWay);
                //meeting.AddRemindWayCode(bookingModel.RemindMethodArr[i]);
                //meeting.MeetingsRemindWay[i].Creator = user;
            }

            //会议和会议室的关系
            meeting.MeetingRooms.Clear();

            TD.MeetingRoomUseRecord meetingRoomUseRecord = new TD.MeetingRoomUseRecord();
            meetingRoomUseRecord.MeetingsCode = meeting.ID;
            meetingRoomUseRecord.MeetingRoomCode = bookingModel.RoomGUID;
            meetingRoomUseRecord.MeetingRoomName = bookingModel.RoomName;
            meetingRoomUseRecord.Creator = user;
            meetingRoomUseRecord.CreateTime = DateTime.Now;
            meeting.MeetingRooms.Add(meetingRoomUseRecord);


            //meeting.AddRoomUseRecord(bookingModel.RoomGUID, bookingModel.RoomName);


            meeting.MeetingRooms.ForEach(mr =>
            {
                if (mr.MeetingsCode == meeting.Code)
                {
                    mr.Creator = user;
                }
            });

            //会议所使用的设备
            meeting.meeting_meetingSupplyTypes.Clear();

            for (int i = 0; i <= bookingModel.EquipmentArr.Count() - 1; i++)
            {
                TD.MeetingEquipment meetingEquipment = new TD.MeetingEquipment();

                meetingEquipment.MeetingCode = meeting.ID;
                meetingEquipment.Creator = user;
                meetingEquipment.CreateTime = DateTime.Now;
                meetingEquipment.MeetingSupplyTypeCode = (MD.MeetingSupplyTypeEnum)Convert.ToInt32(bookingModel.EquipmentArr[i]);
                meeting.meeting_meetingSupplyTypes.Add(meetingEquipment);
            }

            lock (thisLock)
            {
                //会议插入或更新
                TD.MeetingAdapter.Instance.Save(meeting);
            }
        }

        protected static bool CheckCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        internal NetworkCredential GetNetCS(string username, string password, string domain)
        {
            return new NetworkCredential(username, password, domain);
        }
        #endregion
    }
    [ORTableMapping("Secretary.MeetingRoom")]
    [Serializable]
    [XElementSerializable]
    public class yxMeetingRoom : MD.MeetingRoom
    {
        /// <summary>
        /// 会议室被占用时间（分钟）
        /// </summary>
        [ORFieldMapping("mtRoomTime")]
        public double mtRoomTime { get; set; }
    }
    [Serializable]
    [XElementSerializable]
    public class yxMeetingRoomCollection : EditableDataObjectCollectionBase<yxMeetingRoom>
    {

    }
    public class yxMeetingRoomAdatper : TypeEntityBAdapterBase<yxMeetingRoom, yxMeetingRoomCollection>
    {
        public static readonly yxMeetingRoomAdatper Instance = new yxMeetingRoomAdatper();

        protected override string GetConnectionName()
        {
            return "SecretarialService";
        }

        public yxMeetingRoomCollection SearchMyRoomByPageIndexAndTime(string departmentCode, int pageIndex, int pageSize, DateTime startTime, DateTime endTime)
        {
            yxMeetingRoomCollection mrColl = new yxMeetingRoomCollection();
            try
            {
                string sTime = startTime.ToString("yyyy-MM-dd HH:mm:ss");
                string eTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
                string sql = string.Format(@"
                SELECT * FROM (
	                SELECT *,ROW_NUMBER() OVER (ORDER BY C.mtRoomTime) AS rowNum FROM (
		                SELECT (
			                SELECT SUM(B.myTime) FROM (
				                SELECT DATEDIFF(minute,A.myStartTime,A.myEndTime) myTime FROM (
					                SELECT CASE WHEN DATEDIFF(MINUTE,StartTime,'{0}')>0 THEN '{1}' ELSE StartTime END
                                    myStartTime,CASE WHEN DATEDIFF(MINUTE,EndTime,'{2}')>0 THEN EndTime ELSE '{3}' END					
                                    myEndTime FROM SecretaryAdministration.Secretary.Meetings WHERE Code IN (
						                SELECT MeetingsCode FROM SecretaryAdministration.Secretary.MeetingRoomUseRecord WHERE MeetingRoomCode=mr.Code
					                ) AND ((StartTime<='{4}' AND EndTime>='{5}') OR (StartTime<='{6}'					
                                    AND	EndTime>='{7}') OR (StartTime>='{8}' AND EndTime<='{9}'))
				                ) A
			                ) B
		                ) AS mtRoomTime,* FROM SubjectDB.Secretary.MeetingRoom mr WHERE OrganizationID='{10}' AND			
                        mr.VersionEndTime IS NULL AND mr.IsOuter=0 AND mr.UseStateCode!={11}
	                ) AS C
                ) AS D WHERE D.rowNum BETWEEN {12} AND {13}
                ", sTime, sTime, eTime, eTime, sTime, sTime, eTime, eTime, sTime, eTime, departmentCode, Convert.ToString((int)MD.MeetingUseStateEnum.Unusable), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
                //MeetingRoomCollection mrColl = MeetingRoomService.roomInstance.QueryData(sql);
                mrColl = yxMeetingRoomAdatper.Instance.QueryData(sql);
            }
            catch (Exception e)
            {
                throw e;
            }
            return mrColl;
        }
    }
}
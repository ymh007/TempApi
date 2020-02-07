using Seagull2.YuanXin.AppApi.Adapter.ScheduleManage;
using Seagull2.YuanXin.AppApi.Models.ScheduleManage;
using System;
using System.Web.Http;
using System.Linq;
using Seagull2.Core.Models;
using System.Transactions;
using MCS.Library.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using MCS.Library.OGUPermission;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using System.Data;
using Seagull2.YuanXin.AppApi.Adapter.Sign;
using Seagull2.YuanXin.AppApi.Domain.Sign;
using Seagull2.YuanXin.AppApi.ViewsModel.ScheduleManage;
using Seagull2.YuanXin.AppApi.Adapter.Task;
using Seagull2.YuanXin.AppApi.Models.TaskManage;
using Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon;
using Seagull2.YuanXin.AppApi.Adapter.Conference;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 日程管理 api
    /// </summary>
    public class ScheduleManageController : ApiController
    {
        private string getCaledar = "calendar/search";
        private string createMeeting = "appointment/create";
        private string updateMeeting = "appointment/update";
        private string deleteCalendar = "calendar/delete";// 删除日历事件
        private string viewMeeting = "calendar/view";
        //private string cancelMeeting = "appointment/cancel";// 取消会议
        private string deleteMeeting = "appointment/delete";// 删除会议
        private string respondMeeting = "appointment/respond";//响应会议
        private string unreadEmail = "mail/unread";//未读邮件数量
        private const string strStartTime = " 00:00", strEndTime = " 23:55";
        private const string moringStart = " 08:30", moringEnd = " 11:30";
        private const string afternoonStart = " 01:00", afternoonEnd = " 05:30";
        private List<CalendarItemModel> outlookdataTemp; //过程数据


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
                    throw new Exception("获取用户失败,请重新登陆");
                }
            }
        }
        /// <summary>
        /// 海鸥二编码
        /// </summary>
        public string DisplayName
        {
            get
            {
                try
                {
                    return ((Seagull2Identity)User.Identity).DisplayName;
                }
                catch (Exception e)
                {
                    throw new Exception("获取用户失败,请重新登陆");
                }
            }
        }

        private string Week(DayOfWeek dayOfWeek)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(dayOfWeek)];
            return week;
        }


        #region    老版本
        /// <summary>
        /// 获取日程  flag  1 日历视图  2 日程视图
        /// 日程类型 : 未跨天数据 1  原始数据 0  拆分数据  2 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSchedule(string time, int flag)
        {
            var result = ControllerService.Run(() =>
            {
                List<object> days = new List<object>();
                ScheduleModelCollection temp = new ScheduleModelCollection();
                DateTime startDate = DateTime.Parse(time.Substring(0, 4) + "-" + time.Substring(4, 2) + "-01");
                DateTime endDate = LastDayOfMounth(startDate);
                // 获取 outlook 数据
                OutToLocalCroessData(false, startDate, endDate, temp);
                List<ScheduleModel> tempOrder = temp.OrderBy(o => o.StartTime).ToList();
                tempOrder.GroupBy(g => g.GroupKey).ToList().ForEach(p =>
                {
                    var day = new
                    {
                        date = p.Key,
                        weekday = Week(DateTime.Parse(p.Key).DayOfWeek),
                        data = new List<object>()
                    };
                    bool isOver = false;
                    p.ToList().ForEach(f =>
                    {

                        if (f.StartTime.ToString("yyyy-MM-dd") == f.EndTime.ToString("yyyy-MM-dd") && f.StartTime.ToString("HH:mm") == "00:00" && f.EndTime.ToString("HH:mm") == "23:55" && f.ScheduleType == 2)
                        {
                            isOver = true;
                        }
                        var d = new
                        {
                            key = f.Code,
                            scheduleCode = f.ScheduleCode,
                            startTime = f.StartTime.ToString("yyyy-MM-dd HH:mm"),
                            endTime = f.EndTime.ToString("yyyy-MM-dd HH:mm"),
                            title = f.Title,
                            reminderTime = f.ReminderTime,
                            flag = f.ScheduleType,
                            address = f.Place,
                            introduction = f.Content,
                            isOver,
                            f.IsMeeting,
                            f.IsCancelled,
                            f.OutlookId,
                            isOutlook = f.IsOutlook,
                            people = new List<object> { }
                        };
                        day.data.Add(d);
                    });
                    days.Add(day);
                });
                return days;
            });
            return this.Ok(result);
        }


        /// <summary>
        /// 获取日程  flag  1 日历视图  2 日程视图
        /// 日程类型 : 未跨天数据 1  原始数据 0  拆分数据  2 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetScheduleList(string time, int flag)
        {
            var result = ControllerService.Run(() =>
            {
                List<object> days = new List<object>();
                ScheduleModelCollection temp = new ScheduleModelCollection();
                DateTime startDate = DateTime.Parse(time.Substring(0, 4) + "-" + time.Substring(4, 2) + "-01");
                DateTime endDate = LastDayOfMounth(startDate);
                OutToLocalCroessData(true, startDate, endDate, temp);
                List<ScheduleModel> tempOrder = temp.OrderBy(o => o.StartTime).ToList();
                tempOrder.GroupBy(g => g.GroupKey).ToList().ForEach(p =>
                {
                    List<object> isOverList = new List<object>();
                    List<object> notOverList = new List<object>();
                    var day = new
                    {
                        date = p.Key,
                        weekday = Week(DateTime.Parse(p.Key).DayOfWeek),
                        data = new List<object>()
                    };
                    string startTime = "", endTime = "";
                    DateTime rstartTime, rendTime;
                    p.ToList().ForEach(f =>
                    {
                        bool isOver = false;
                        rstartTime = f.StartTime;
                        rendTime = f.EndTime;
                        startTime = $"{f.StartTime.Month}月{f.StartTime.Day}日 {f.StartTime.ToString("HH:mm")}";
                        endTime = $"{f.EndTime.Month}月{f.EndTime.Day}日 {f.EndTime.ToString("HH:mm")}";
                        if (f.ScheduleType == 2)// 跨天的数据
                        {
                            isOver = true;
                            var t1 = outlookdataTemp.Find(x => x.Id == f.OutlookId);
                            if (t1 != null)
                            {
                                rstartTime = ConvertLongToDateTime(t1.Start);
                                rendTime = ConvertLongToDateTime(t1.End);
                                if (t1.IsAllDayEvent)
                                {
                                    rendTime = DateTime.Parse(rendTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + strEndTime);
                                }
                                startTime = $"{rstartTime.Month}月{rstartTime.Day}日 {rstartTime.ToString("HH:mm")}";
                                endTime = $"{rendTime.Month}月{rendTime.Day}日 {rendTime.ToString("HH:mm")}";
                            }
                        }
                        else
                        {
                            if (f.IsOutlook)
                            {
                                isOver = rstartTime.ToString("yyyy-MM-dd") != rendTime.ToString("yyyy-MM-dd");
                            }
                        }
                        var d = new
                        {
                            key = f.Code,
                            scheduleCode = f.ScheduleCode,
                            rstartTime = rstartTime.ToString("yyyy-MM-dd HH:mm"),
                            rendTime = rendTime.ToString("yyyy-MM-dd HH:mm"),
                            startTime = startTime,
                            endTime = endTime,
                            title = f.Title,
                            reminderTime = f.ReminderTime,
                            flag = f.ScheduleType,
                            address = f.Place,
                            introduction = f.Content,
                            isOver,
                            f.IsMeeting,
                            f.IsFromMe,
                            f.IsCancelled,
                            f.OutlookId,
                            isOutlook = f.IsOutlook,
                            people = new List<object>()
                        };
                        if (!f.IsOutlook)
                        {
                            ScheduleParticipantsCollection sps = ScheduleParticipantsAdapter.Instance.Load(ss => ss.AppendItem("ScheduleCode", f.ScheduleCode));
                            if (sps.Count == 0)
                            {
                                d.people.Add(new { });
                            }
                            else
                            {
                                sps.ForEach(s =>
                                {
                                    d.people.Add(new
                                    {
                                        id = s.PersonnelCode,
                                        name = s.Name,
                                        department = s.Department
                                    });
                                });
                            }
                        }
                        else
                        {
                            d.people.Add(new { });
                        }
                        if (d.isOver)
                        {
                            isOverList.Add(d);
                        }
                        else
                        {
                            notOverList.Add(d);
                        }
                    });
                    isOverList.AddRange(notOverList);
                    day.data.AddRange(isOverList);
                    days.Add(day);
                });
                return days;
            });
            return this.Ok(result);
        }


        #endregion


        #region  新版本集成了多个模块数据

        /// <summary>
        ///  日程视图
        /// 日程类型 :     type // sign  task   leave  conference
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSchedule(string time)
        {
            var result = ControllerService.Run(() =>
            {
                ServiceUtility.SetCertificatePolicy();
                List<ParentResult> days = new List<ParentResult>();
                DateTime startDate = DateTime.Parse(time.Substring(0, 4) + "-" + time.Substring(4, 2) + "-01");
                DateTime endDate = LastDayOfMounth(startDate);
                PunchStatistical punchStatistical = GetPunchStatistical(startDate, endDate);
                ScheduleModelCollection temp = new ScheduleModelCollection();

                var outlookdata = Task.Run(() =>
                {
                    try
                    {
                        OutToLocalCroessData(false, startDate, endDate, temp);
                        List<ScheduleModel> tempOrder = temp.OrderBy(o => o.StartTime).ToList();
                        tempOrder.GroupBy(g => g.GroupKey).ToList().ForEach(p =>
                        {
                            ParentResult day = days.Find(x => x.date == p.Key);
                            if (day == null)
                            {
                                day = new ParentResult
                                {
                                    date = p.Key,
                                    weekday = Week(DateTime.Parse(p.Key).DayOfWeek),
                                    data = new List<ChildResult>()
                                };
                                days.Add(day);
                            }
                            bool isOver = false;
                            p.ToList().ForEach(f =>
                            {

                                if (f.StartTime.ToString("yyyy-MM-dd") == f.EndTime.ToString("yyyy-MM-dd") && f.StartTime.ToString("HH:mm") == "00:00" && f.EndTime.ToString("HH:mm") == "23:55" && f.ScheduleType == 2)
                                {
                                    isOver = true;
                                }
                                var d = new ChildResult
                                {
                                    key = f.Code,
                                    type = "bookMeeting",
                                    scheduleCode = f.ScheduleCode,
                                    startTime = f.StartTime.ToString("yyyy-MM-dd HH:mm"),
                                    endTime = f.EndTime.ToString("yyyy-MM-dd HH:mm"),
                                    title = f.Title,
                                    reminderTime = (int)f.ReminderTime,
                                    flag = f.ScheduleType,
                                    address = f.Place,
                                    introduction = f.Content,
                                    isOver = isOver,
                                    isMeeting = f.IsMeeting,
                                    isCancelled = f.IsCancelled,
                                    outlookId = f.OutlookId,
                                    isOutlook = f.IsOutlook,
                                    people = new List<object> { }
                                };
                                day.data.Add(d);
                            });

                        });
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString());
                    }

                });

                var punch = Task.Run(() =>
                {
                    try
                    {
                        string time0 = "", time1 = "";
                        var punchTime = PunchRemindSettingAdapter.Instance.GetRemindTime(CurrentUserCode);
                        punchTime.ForEach(f =>
                        {
                            if (f.Type == 0)
                            {
                                time0 = f.RemindTime;
                            }
                            if (f.Type == 1)
                            {
                                time1 = f.RemindTime;
                            }
                        });
                        if (time0 != "" || time1 != "")
                        {
                            punchStatistical._WorkDateList.ForEach(f =>
                            {
                                ParentResult day = days.Find(x => x.date == f.ToString("yyyy-MM-dd"));
                                if (day == null)
                                {
                                    day = new ParentResult()
                                    {
                                        date = f.ToString("yyyy-MM-dd"),
                                        weekday = Week(f.DayOfWeek),
                                        data = new List<ChildResult>()
                                    };
                                    days.Add(day);
                                }
                                if (time0 != "")
                                {
                                    day.data.Add(new ChildResult()
                                    {

                                        type = "sign",
                                        startTime = day.date + " " + time0,
                                        endTime = DateTime.Parse(day.date + " " + time0).AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
                                        title = "上班打卡",
                                    });
                                }
                                if (time1 != "")
                                {
                                    day.data.Add(new ChildResult()
                                    {
                                        type = "sign",
                                        startTime = day.date + " " + time1,
                                        endTime = DateTime.Parse(day.date + " " + time1).AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
                                        title = "下班打卡",
                                    });
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString());
                    }

                });

                var task = Task.Run(() =>
                {
                    try
                    {
                        TaskCollection tasks = TaskApapter.Instance.GetDeadLine(CurrentUserCode, startDate, endDate);
                        tasks.ForEach(f =>
                        {
                            ParentResult day = days.Find(x => x.date == f.Deadline.ToString("yyyy-MM-dd"));
                            if (day == null)
                            {
                                day = new ParentResult()
                                {
                                    date = f.Deadline.ToString("yyyy-MM-dd"),
                                    weekday = Week(f.Deadline.DayOfWeek),
                                    data = new List<ChildResult>()
                                };
                                days.Add(day);
                            }
                            day.data.Add(new ChildResult()
                            {
                                creator = f.Creator,
                                scheduleCode = f.Code,
                                key = f.Code,
                                type = "task",
                                startTime = f.Deadline.ToString("yyyy-MM-dd HH:mm"),
                                endTime = f.Deadline.AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
                                title = f.TitleContent + " 任务即将截止",
                            });

                        });
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString());
                    }
                });

                var leave = Task.Run(() =>
                {
                    try
                    {
                        string apiUrl = ConfigurationManager.AppSettings["GetLevaveInfo"] + $"?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";
                        LeaveInfo leaveInfo = null;
                        using (HttpClient http = new HttpClient())
                        {
                            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Request.Headers.Authorization.Scheme, Request.Headers.Authorization.Parameter);
                            try
                            {
                                string json = http.GetStringAsync(apiUrl).Result;
                                leaveInfo = JsonConvert.DeserializeObject<LeaveInfo>(json);
                            }
                            catch (Exception ex)
                            {
                                leaveInfo = new LeaveInfo() { data = new List<Leave>() };
                                leaveInfo.message = ex.Message;
                            }
                        } 
                        leaveInfo.data.ForEach(p =>
                        {
                            if (p.daysCount < 1)
                            {
                                ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                if (day == null)
                                {
                                    day = new ParentResult()
                                    {
                                        date = p.startDate.ToString("yyyy-MM-dd"),
                                        weekday = Week(p.startDate.DayOfWeek),
                                        data = new List<ChildResult>()
                                    };
                                    days.Add(day);
                                }
                                day.data.Add(new ChildResult()
                                {
                                    type = "leave",
                                    startTime = p.startDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringStart : afternoonStart),
                                    endTime = p.endDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringEnd : afternoonEnd),
                                    title = p.attendenceItemName,
                                });
                            }
                            if (p.daysCount == 1)
                            {
                                if (p.startTypeName == "上午")
                                {
                                    ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                    if (day == null)
                                    {
                                        day = new ParentResult()
                                        {
                                            date = p.startDate.ToString("yyyy-MM-dd"),
                                            weekday = Week(p.startDate.DayOfWeek),
                                            data = new List<ChildResult>()
                                        };
                                        days.Add(day);
                                    }
                                    day.data.Add(new ChildResult()
                                    {
                                        type = "leave",
                                        startTime = p.startDate.ToString("yyyy-MM-dd") + moringStart,
                                        endTime = p.endDate.ToString("yyyy-MM-dd") + afternoonEnd,
                                        title = p.attendenceItemName,
                                    });
                                }
                                else
                                {
                                    ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                    if (day == null)
                                    {
                                        day = new ParentResult()
                                        {
                                            date = p.startDate.ToString("yyyy-MM-dd"),
                                            weekday = Week(p.startDate.DayOfWeek),
                                            data = new List<ChildResult>()
                                        };
                                        days.Add(day);
                                    }
                                    day.data.Add(new ChildResult()
                                    {
                                        type = "leave",
                                        startTime = p.startDate.ToString("yyyy-MM-dd") + afternoonStart,
                                        endTime = p.startDate.ToString("yyyy-MM-dd") + afternoonEnd,
                                        title = p.attendenceItemName,
                                    });
                                    ParentResult day1 = days.Find(x => x.date == p.endDate.ToString("yyyy-MM-dd"));
                                    if (day1 == null)
                                    {
                                        day1 = new ParentResult()
                                        {
                                            date = p.endDate.ToString("yyyy-MM-dd"),
                                            weekday = Week(p.endDate.DayOfWeek),
                                            data = new List<ChildResult>()
                                        };
                                        days.Add(day1);
                                    }
                                    day1.data.Add(new ChildResult()
                                    {
                                        type = "leave",
                                        startTime = p.endDate.ToString("yyyy-MM-dd") + moringStart,
                                        endTime = p.endDate.ToString("yyyy-MM-dd") + moringEnd,
                                        title = p.attendenceItemName,
                                    });

                                }
                            }
                            if (p.daysCount > 1)
                            {
                                for (var i = p.startDate; i <= p.endDate; i = i.AddDays(1)) //1.5
                                {
                                    if (punchStatistical._WorkDateList.Exists(e => e == i))
                                    {
                                        if (i == p.startDate)
                                        {
                                            ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                            if (day == null)
                                            {
                                                day = new ParentResult()
                                                {
                                                    date = p.startDate.ToString("yyyy-MM-dd"),
                                                    weekday = Week(p.startDate.DayOfWeek),
                                                    data = new List<ChildResult>()
                                                };
                                                days.Add(day);
                                            }
                                            day.data.Add(new ChildResult()
                                            {
                                                type = "leave",
                                                startTime = p.startDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringStart : afternoonStart),
                                                endTime = p.startDate.ToString("yyyy-MM-dd") + afternoonEnd,
                                                title = p.attendenceItemName,
                                            });
                                        }
                                        else if (i == p.endDate)
                                        {
                                            ParentResult day = days.Find(x => x.date == p.endDate.ToString("yyyy-MM-dd"));
                                            if (day == null)
                                            {
                                                day = new ParentResult()
                                                {
                                                    date = p.endDate.ToString("yyyy-MM-dd"),
                                                    weekday = Week(p.endDate.DayOfWeek),
                                                    data = new List<ChildResult>()
                                                };
                                                days.Add(day);
                                            }
                                            day.data.Add(new ChildResult()
                                            {
                                                type = "leave",
                                                startTime = p.endDate.ToString("yyyy-MM-dd") + moringStart,
                                                endTime = p.endDate.ToString("yyyy-MM-dd") + (p.endTypeName == "上午" ? moringEnd : afternoonEnd),
                                                title = p.attendenceItemName,
                                            });
                                        }
                                        else
                                        {
                                            ParentResult day = days.Find(x => x.date == i.ToString("yyyy-MM-dd"));
                                            if (day == null)
                                            {
                                                day = new ParentResult()
                                                {
                                                    date = i.ToString("yyyy-MM-dd"),
                                                    weekday = Week(i.DayOfWeek),
                                                    data = new List<ChildResult>()
                                                };
                                                days.Add(day);
                                            }
                                            day.data.Add(new ChildResult()
                                            {
                                                type = "leave",
                                                startTime = i.ToString("yyyy-MM-dd") + moringStart,
                                                endTime = i.ToString("yyyy-MM-dd") + afternoonEnd,
                                                title = p.attendenceItemName,
                                            });
                                        }
                                    }
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString());
                    }

                });

                var conferenc = Task.Run(() =>
                {
                    try
                    {
                        var conferences = ConferenceAdapter.Instance.GetMyConference(CurrentUserCode, startDate, endDate);
                        conferences.ForEach(f =>
                        {
                            bool isCoressDay = f.BeginDate.ToString("yyyy-MM-dd") != f.EndDate.ToString("yyyy-MM-dd"); // 跨天  
                            if (isCoressDay)
                            {
                                for (var i = f.BeginDate; i <= f.EndDate; i = i.AddDays(1))
                                {

                                    if (i == f.BeginDate)
                                    {
                                        ParentResult day = days.Find(x => x.date == f.BeginDate.ToString("yyyy-MM-dd"));
                                        if (day == null)
                                        {
                                            day = new ParentResult()
                                            {
                                                date = f.BeginDate.ToString("yyyy-MM-dd"),
                                                weekday = Week(f.BeginDate.DayOfWeek),
                                                data = new List<ChildResult>()
                                            };
                                            days.Add(day);
                                        }
                                        day.data.Add(new ChildResult()
                                        {
                                            key = f.ID,
                                            scheduleCode = f.ID,
                                            type = "conference",
                                            startTime = f.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                                            endTime = f.BeginDate.ToString("yyyy-MM-dd") + strEndTime,
                                            title = f.Name,

                                        });
                                    }
                                    else if (i.ToString("yyyy-MM-dd") == f.EndDate.ToString("yyyy-MM-dd"))
                                    {
                                        ParentResult day = days.Find(x => x.date == f.EndDate.ToString("yyyy-MM-dd"));
                                        if (day == null)
                                        {
                                            day = new ParentResult()
                                            {
                                                date = f.EndDate.ToString("yyyy-MM-dd"),
                                                weekday = Week(f.EndDate.DayOfWeek),
                                                data = new List<ChildResult>()
                                            };
                                            days.Add(day);
                                        }
                                        day.data.Add(new ChildResult()
                                        {
                                            key = f.ID,
                                            scheduleCode = f.ID,
                                            type = "conference",
                                            startTime = f.EndDate.ToString("yyyy-MM-dd") + strStartTime,
                                            endTime = f.EndDate.ToString("yyyy-MM-dd HH:mm"),
                                            title = f.Name,

                                        });
                                    }
                                    else
                                    {
                                        ParentResult day = days.Find(x => x.date == i.ToString("yyyy-MM-dd"));
                                        if (day == null)
                                        {
                                            day = new ParentResult()
                                            {
                                                date = i.ToString("yyyy-MM-dd"),
                                                weekday = Week(i.DayOfWeek),
                                                data = new List<ChildResult>()
                                            };
                                            days.Add(day);
                                        }
                                        day.data.Add(new ChildResult()
                                        {
                                            key = f.ID,
                                            scheduleCode = f.ID,
                                            type = "conference",
                                            startTime = i.ToString("yyyy-MM-dd") + strStartTime,
                                            endTime = i.ToString("yyyy-MM-dd") + strEndTime,
                                            title = f.Name,

                                        });
                                    }

                                }
                            }
                            else
                            {
                                ParentResult day = days.Find(x => x.date == f.BeginDate.ToString("yyyy-MM-dd"));
                                if (day == null)
                                {
                                    day = new ParentResult()
                                    {
                                        date = f.BeginDate.ToString("yyyy-MM-dd"),
                                        weekday = Week(f.BeginDate.DayOfWeek),
                                        data = new List<ChildResult>()
                                    };
                                    days.Add(day);
                                }
                                day.data.Add(new ChildResult()
                                {
                                    key = f.ID,
                                    scheduleCode = f.ID,
                                    type = "conference",
                                    startTime = f.BeginDate.ToString("yyyy-MM-dd HH:mm"),
                                    endTime = f.EndDate.ToString("yyyy-MM-dd HH:mm"),
                                    title = f.Name,

                                });
                            }
                        });
                    }
                    catch (Exception ex) { Log.WriteLog(ex.ToString()); }


                });

                Task.WaitAll(outlookdata, punch, task, leave, conferenc);
                return days;
            });
            return this.Ok(result);
        }

        /// <summary>
        /// 日历视图 
        /// 日程类型 :    type // sign  task   leave  conference
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetScheduleList(string time)
        {
            var result = ControllerService.Run(() =>
            {
                ServiceUtility.SetCertificatePolicy();
                List<ParentResult> days = new List<ParentResult>();
                DateTime startDate = DateTime.Parse(time.Substring(0, 4) + "-" + time.Substring(4, 2) + "-01");
                DateTime endDate = LastDayOfMounth(startDate);
                ScheduleModelCollection temp = new ScheduleModelCollection();
                PunchStatistical punchStatistical = GetPunchStatistical(startDate, endDate);
                var outlookdata = Task.Run(() =>
                {
                    try
                    {
                        OutToLocalCroessData(true, startDate, endDate, temp);
                        temp.GroupBy(g => g.GroupKey).ToList().ForEach(p =>
                        {
                            ParentResult day = days.Find(x => x.date == p.Key);
                            if (day == null)
                            {
                                day = new ParentResult
                                {
                                    date = p.Key,
                                    weekday = Week(DateTime.Parse(p.Key).DayOfWeek),
                                    data = new List<ChildResult>()
                                };
                                days.Add(day);
                            }
                            string startTime = "", endTime = "";
                            DateTime rstartTime, rendTime;
                            p.ToList().ForEach(f =>
                            {
                                bool isOver = false;
                                rstartTime = f.StartTime;
                                rendTime = f.EndTime;
                                startTime = $"{f.StartTime.Month}月{f.StartTime.Day}日 {f.StartTime.ToString("HH:mm")}";
                                endTime = $"{f.EndTime.Month}月{f.EndTime.Day}日 {f.EndTime.ToString("HH:mm")}";
                                if (f.ScheduleType == 2)// 跨天的数据
                                {
                                    isOver = true;
                                    var t1 = outlookdataTemp.Find(x => x.Id == f.OutlookId);
                                    if (t1 != null)
                                    {
                                        rstartTime = ConvertLongToDateTime(t1.Start);
                                        rendTime = ConvertLongToDateTime(t1.End);
                                        if (t1.IsAllDayEvent)
                                        {
                                            rendTime = DateTime.Parse(rendTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + strEndTime);
                                        }
                                        startTime = $"{rstartTime.Month}月{rstartTime.Day}日 {rstartTime.ToString("HH:mm")}";
                                        endTime = $"{rendTime.Month}月{rendTime.Day}日 {rendTime.ToString("HH:mm")}";
                                    }
                                }
                                else
                                {
                                    isOver = rstartTime.ToString("yyyy-MM-dd") != rendTime.ToString("yyyy-MM-dd");
                                }
                                var d = new ChildResult
                                {
                                    key = f.Code,
                                    scheduleCode = f.ScheduleCode,
                                    rstartTime = rstartTime.ToString("yyyy-MM-dd HH:mm"),
                                    rendTime = rendTime.ToString("yyyy-MM-dd HH:mm"),
                                    startTime = startTime,
                                    endTime = endTime,
                                    title = f.Title,
                                    reminderTime = (int)f.ReminderTime,
                                    flag = f.ScheduleType,
                                    address = f.Place,
                                    introduction = f.Content,
                                    isOver = isOver,
                                    isMeeting = f.IsMeeting,
                                    IsFromMe = f.IsFromMe,
                                    isCancelled = f.IsCancelled,
                                    outlookId = f.OutlookId,
                                    isOutlook = f.IsOutlook,
                                    people = new List<object>()
                                };
                                if (f.IsOutlook)
                                {
                                    d.people.Add(new { });
                                }
                                day.data.Add(d);
                            });
                        });
                    }
                    catch (Exception ex) { Log.WriteLog(ex.ToString()); }

                });
                var punch = Task.Run(() =>
                {
                    try
                    {
                        string time0 = "", time1 = "";
                        var punchTime = PunchRemindSettingAdapter.Instance.GetRemindTime(CurrentUserCode);
                        
                        punchTime.ForEach(f =>
                        {
                            
                            if (f.Type == 0)
                            {
                                time0 = f.RemindTime;
                            }
                            if (f.Type == 1)
                            {
                                time1 = f.RemindTime;
                            }
                        });
                        if (time0 != "" || time1 != "")
                        {
                            punchStatistical._WorkDateList.ForEach(f =>
                            {
                                
                                ParentResult day = days.Find(x => x.date == f.ToString("yyyy-MM-dd"));
                                if (day == null)
                                {
                                    day = new ParentResult()
                                    {
                                        date = f.ToString("yyyy-MM-dd"),
                                        weekday = Week(f.DayOfWeek),
                                        data = new List<ChildResult>()
                                    };
                                    days.Add(day);
                                }
                                if (time0 != "")
                                {
                                    day.data.Add(new ChildResult()
                                    {

                                        type = "sign",
                                        startTime = day.date + " " + time0,
                                        endTime = DateTime.Parse(day.date + " " + time0).AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
                                        title = "上班打卡",
                                    });
                                }
                                if (time1 != "")
                                {
                                    day.data.Add(new ChildResult()
                                    {
                                        type = "sign",
                                        startTime = day.date + " " + time1,
                                        endTime = DateTime.Parse(day.date + " " + time1).AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
                                        title = "下班打卡",
                                    });
                                }
                            });
                        }
                    }
                    catch (Exception ex) { Log.WriteLog(ex.ToString()); }

                });
                var task = Task.Run(() =>
                {
                    try
                    {
                        TaskCollection tasks = TaskApapter.Instance.GetDeadLine(CurrentUserCode, startDate, endDate);
                        tasks.ForEach(f =>
                        {
                            ParentResult day = days.Find(x => x.date == f.Deadline.ToString("yyyy-MM-dd"));
                            if (day == null)
                            {
                                day = new ParentResult()
                                {
                                    date = f.Deadline.ToString("yyyy-MM-dd"),
                                    weekday = Week(f.Deadline.DayOfWeek),
                                    data = new List<ChildResult>()
                                };
                                days.Add(day);
                            }
                            day.data.Add(new ChildResult()
                            {
                                creator = f.Creator,
                                scheduleCode = f.Code,
                                key = f.Code,
                                type = "task",
                                startTime = f.Deadline.ToString("yyyy-MM-dd HH:mm"),
                                endTime = f.Deadline.AddMinutes(30).ToString("yyyy-MM-dd HH:mm"),
                                title = f.TitleContent + " 任务即将截止",
                            });

                        });
                    }
                    catch (Exception ex) { Log.WriteLog(ex.ToString()); }


                });
                var leave = Task.Run(() =>
                {
                    try
                    {
                        string apiUrl = ConfigurationManager.AppSettings["GetLevaveInfo"] + $"?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";
                        LeaveInfo leaveInfo = null;
                        using (HttpClient http = new HttpClient())
                        {
                            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Request.Headers.Authorization.Scheme, Request.Headers.Authorization.Parameter);
                            try
                            {
                                string json = http.GetStringAsync(apiUrl).Result;
                                leaveInfo = JsonConvert.DeserializeObject<LeaveInfo>(json);
                            }
                            catch (Exception ex)
                            {
                                leaveInfo = new LeaveInfo() { data = new List<Leave>() };
                                leaveInfo.message = ex.Message;
                            }
                        }
                        leaveInfo.data.ForEach(p =>
                        {
                            bool isOver = p.startDate.ToString("yyyy-MM-dd") != p.endDate.ToString("yyyy-MM-dd");
                            if (p.daysCount < 1)
                            {
                                ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                if (day == null)
                                {
                                    day = new ParentResult()
                                    {
                                        date = p.startDate.ToString("yyyy-MM-dd"),
                                        weekday = Week(p.startDate.DayOfWeek),
                                        data = new List<ChildResult>()
                                    };
                                    days.Add(day);
                                }
                                day.data.Add(new ChildResult()
                                {
                                    type = "leave",
                                    startTime = p.startDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringStart : afternoonStart),
                                    endTime = p.endDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringEnd : afternoonEnd),
                                    title = p.attendenceItemName,
                                    isOver = isOver,
                                });
                            }
                            if (p.daysCount == 1)
                            {
                                if (p.startTypeName == "上午")
                                {
                                    ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                    if (day == null)
                                    {
                                        day = new ParentResult()
                                        {
                                            date = p.startDate.ToString("yyyy-MM-dd"),
                                            weekday = Week(p.startDate.DayOfWeek),
                                            data = new List<ChildResult>()
                                        };
                                        days.Add(day);
                                    }
                                    day.data.Add(new ChildResult()
                                    {
                                        type = "leave",
                                        startTime = p.startDate.ToString("yyyy-MM-dd") + moringStart,
                                        endTime = p.endDate.ToString("yyyy-MM-dd") + afternoonEnd,
                                        title = p.attendenceItemName,
                                        isOver = isOver,
                                    });
                                }
                                else
                                {
                                    ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                    if (day == null)
                                    {
                                        day = new ParentResult()
                                        {
                                            date = p.startDate.ToString("yyyy-MM-dd"),
                                            weekday = Week(p.startDate.DayOfWeek),
                                            data = new List<ChildResult>()
                                        };
                                        days.Add(day);
                                    }
                                    day.data.Add(new ChildResult()
                                    {
                                        type = "leave",
                                        startTime = p.startDate.ToString("yyyy-MM-dd") + afternoonStart,
                                        endTime = p.endDate.ToString("yyyy-MM-dd") + moringEnd,
                                        title = p.attendenceItemName,
                                        isOver = isOver,
                                    });
                                    ParentResult day1 = days.Find(x => x.date == p.endDate.ToString("yyyy-MM-dd"));
                                    if (day1 == null)
                                    {
                                        day1 = new ParentResult()
                                        {
                                            date = p.endDate.ToString("yyyy-MM-dd"),
                                            weekday = Week(p.endDate.DayOfWeek),
                                            data = new List<ChildResult>()
                                        };
                                        days.Add(day1);
                                    }
                                    day1.data.Add(new ChildResult()
                                    {
                                        type = "leave",
                                        startTime = p.endDate.ToString("yyyy-MM-dd") + moringStart,
                                        endTime = p.endDate.ToString("yyyy-MM-dd") + moringEnd,
                                        title = p.attendenceItemName,
                                        isOver = isOver,
                                    });

                                }
                            }
                            if (p.daysCount > 1)
                            {
                                for (var i = p.startDate; i <= p.endDate; i = i.AddDays(1)) //1.5
                                {
                                    if (punchStatistical._WorkDateList.Exists(e => e == i))
                                    {
                                        if (i == p.startDate)
                                        {
                                            ParentResult day = days.Find(x => x.date == p.startDate.ToString("yyyy-MM-dd"));
                                            if (day == null)
                                            {
                                                day = new ParentResult()
                                                {
                                                    date = p.startDate.ToString("yyyy-MM-dd"),
                                                    weekday = Week(p.startDate.DayOfWeek),
                                                    data = new List<ChildResult>()
                                                };
                                                days.Add(day);
                                            }
                                            day.data.Add(new ChildResult()
                                            {
                                                type = "leave",
                                                startTime = p.startDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringStart : afternoonStart),
                                                endTime = p.endDate.ToString("yyyy-MM-dd") + (p.endTypeName == "上午" ? moringEnd : afternoonEnd),
                                                title = p.attendenceItemName,
                                                isOver = true,
                                            });
                                        }
                                        else if (i == p.endDate)
                                        {
                                            ParentResult day = days.Find(x => x.date == p.endDate.ToString("yyyy-MM-dd"));
                                            if (day == null)
                                            {
                                                days.Add(new ParentResult()
                                                {
                                                    date = p.endDate.ToString("yyyy-MM-dd"),
                                                    weekday = Week(p.endDate.DayOfWeek),
                                                    data = new List<ChildResult>()
                                                });
                                                days.Add(day);
                                            }
                                            day.data.Add(new ChildResult()
                                            {
                                                type = "leave",
                                                startTime = p.startDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringStart : afternoonStart),
                                                endTime = p.endDate.ToString("yyyy-MM-dd") + (p.endTypeName == "上午" ? moringEnd : afternoonEnd),
                                                title = p.attendenceItemName,
                                                isOver = true,
                                            });
                                        }
                                        else
                                        {
                                            ParentResult day = days.Find(x => x.date == i.ToString("yyyy-MM-dd"));
                                            if (day == null)
                                            {
                                                day = new ParentResult()
                                                {
                                                    date = i.ToString("yyyy-MM-dd"),
                                                    weekday = Week(i.DayOfWeek),
                                                    data = new List<ChildResult>()
                                                };
                                                days.Add(day);
                                            }
                                            day.data.Add(new ChildResult()
                                            {
                                                type = "leave",
                                                startTime = p.startDate.ToString("yyyy-MM-dd") + (p.startTypeName == "上午" ? moringStart : afternoonStart),
                                                endTime = p.endDate.ToString("yyyy-MM-dd") + (p.endTypeName == "上午" ? moringEnd : afternoonEnd),
                                                title = p.attendenceItemName,
                                                isOver = true,
                                            });
                                        }
                                    }
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex.ToString());
                    }

                });
                var conferenc = Task.Run(() =>
                {
                    try
                    {
                        var conferences = ConferenceAdapter.Instance.GetMyConference(CurrentUserCode, startDate, endDate);
                        conferences.ForEach(f =>
                        {
                            string startStr = f.BeginDate.ToString("yyyy-MM-dd HH:mm");
                            string endStr = f.EndDate.ToString("yyyy-MM-dd HH:mm");
                            bool isCoressDay = f.BeginDate.ToString("yyyy-MM-dd") != f.EndDate.ToString("yyyy-MM-dd"); // 跨天  
                            if (isCoressDay)
                            {
                                
                                for (var i = f.BeginDate; i <= f.EndDate; i = i.AddDays(1))
                                {

                                    if (i == f.BeginDate)
                                    {
                                        ParentResult day = days.Find(x => x.date == f.BeginDate.ToString("yyyy-MM-dd"));
                                        if (day == null)
                                        {
                                            day = new ParentResult()
                                            {
                                                date = f.BeginDate.ToString("yyyy-MM-dd"),
                                                weekday = Week(f.BeginDate.DayOfWeek),
                                                data = new List<ChildResult>()
                                            };
                                            days.Add(day);
                                        }
                                        day.data.Add(new ChildResult()
                                        {
                                            city = f.City,
                                            key = f.ID,
                                            scheduleCode = f.ID,
                                            type = "conference",
                                            startTime = startStr,
                                            endTime = endStr,
                                            title = f.Name,
                                            isOver = true,

                                        });
                                    }
                                    else if (i.ToString("yyyy-MM-dd") == f.EndDate.ToString("yyyy-MM-dd"))
                                    {
                                        ParentResult day = days.Find(x => x.date == f.EndDate.ToString("yyyy-MM-dd"));
                                        if (day == null)
                                        {
                                            day = new ParentResult()
                                            {
                                                date = f.EndDate.ToString("yyyy-MM-dd"),
                                                weekday = Week(f.EndDate.DayOfWeek),
                                                data = new List<ChildResult>()
                                            };
                                            days.Add(day);
                                        }
                                        day.data.Add(new ChildResult()
                                        {
                                            city = f.City,
                                            key = f.ID,
                                            scheduleCode = f.ID,
                                            type = "conference",
                                            startTime = startStr,
                                            endTime = endStr,
                                            title = f.Name,
                                            isOver = true,
                                        });
                                    }
                                    else
                                    {
                                        ParentResult day = days.Find(x => x.date == i.ToString("yyyy-MM-dd"));
                                        if (day == null)
                                        {
                                            day = new ParentResult()
                                            {
                                                date = i.ToString("yyyy-MM-dd"),
                                                weekday = Week(i.DayOfWeek),
                                                data = new List<ChildResult>()
                                            };
                                            days.Add(day);
                                        }
                                        day.data.Add(new ChildResult()
                                        {
                                            city = f.City,
                                            key = f.ID,
                                            scheduleCode = f.ID,
                                            type = "conference",
                                            startTime = startStr,
                                            endTime = endStr,
                                            title = f.Name,
                                            isOver = true,
                                        });
                                    }

                                }
                            }
                            else
                            {
                                ParentResult day = days.Find(x => x.date == f.BeginDate.ToString("yyyy-MM-dd"));
                                if (day == null)
                                {
                                    day = new ParentResult()
                                    {
                                        date = f.BeginDate.ToString("yyyy-MM-dd"),
                                        weekday = Week(f.BeginDate.DayOfWeek),
                                        data = new List<ChildResult>()
                                    };
                                    days.Add(day);
                                }
                                day.data.Add(new ChildResult()
                                {
                                    city = f.City,
                                    key = f.ID,
                                    scheduleCode = f.ID,
                                    type = "conference",
                                    startTime = startStr,
                                    endTime = endStr,
                                    title = f.Name,
                                    isOver = false,
                                });
                            }
                        });
                    }
                    catch (Exception ex) { Log.WriteLog(ex.ToString()); }


                });

                Task.WaitAll(outlookdata, punch, task, leave, conferenc);
                days.ForEach(x =>
                {
                    if (x.data.Count > 1)
                    {
                        x.data =x.data.OrderByDescending(o => o.isOver).ToList();
                    }
                });
                return days.OrderBy(o => o.date).ToList();
            });
            return this.Ok(result);
        }

        #endregion



        /// <summary>
        /// 获取日程详情根据code
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetScheduleByCode(ParmMode parm)
        {
            var result = ControllerService.Run(() =>
            {
                scheduleDetail sd = new scheduleDetail() { selectPeopleArr = new List<object>() };
                var paramet = new
                {
                    Id = parm.Id
                };
                JsonMode<CalendarDetailModel> outlookdata = OperationOutlook<CalendarDetailModel>(viewMeeting, paramet).Result;
                if (!outlookdata.IsSuccess)
                {
                    throw new Exception(outlookdata.Message);
                }
                if (outlookdata.Data != null)
                {
                    sd.themeTitle = outlookdata.Data.Subject;
                    sd.key = outlookdata.Data.Id;
                    sd.scheduleCode = "";
                    sd.body = outlookdata.Data.Body;
                    sd.startValue = ConvertLongToDateTime(outlookdata.Data.Start).ToString("yyyy-MM-dd HH:mm");
                    sd.endValue = ConvertLongToDateTime(outlookdata.Data.End).ToString("yyyy-MM-dd HH:mm");
                    sd.typeIndex = CalcRemindType(outlookdata.Data);
                    sd.IsReminderSet = outlookdata.Data.IsReminderSet;
                    sd.ReminderDueBy = outlookdata.Data.ReminderDueBy;
                    sd.ReminderDueByStr = CalcRemindTime(outlookdata.Data);
                    sd.address = outlookdata.Data.Location;
                    sd.themeContent = outlookdata.Data.PreviewText == "undefined" ? "" : outlookdata.Data.PreviewText;
                    sd.isOutlook = true;
                    sd.IsFromMe = outlookdata.Data.IsFromMe;
                    if (outlookdata.Data.RequiredAttendees != null && outlookdata.Data.RequiredAttendees.Count > 0)
                    {
                        outlookdata.Data.RequiredAttendees.ForEach(x =>
                        {
                            sd.selectPeopleArr.Add(new
                            {
                                id = x.UserID,
                                name = x.Name,
                                department = "",
                                isUser = x.IsUser
                            });
                        });
                    }
                    else
                    {
                        sd.selectPeopleArr.Add(new
                        {
                            id = outlookdata.Data.Organizer.UserID,
                            name = DisplayName,
                            department = "",
                            isUser = true
                        });
                    }
                }
                else
                {
                    throw new Exception("该事件已被取消！");
                }
                return sd;
            });
            return this.Ok(result);
        }

        /// <summary>
        /// 更新日程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UpdateSchedule(ScheduleModel model)
        {
            var result = ControllerService.Run(() =>
            {
                bool isUpdate = false;
                if (!string.IsNullOrWhiteSpace(model.OutlookId))
                {
                    isUpdate = true;
                }
                JsonMode<AppointmentModel> outlook = CreateOutlookMeeting(model, isUpdate);
                return outlook;
            });
            return this.Ok(result);
        }


        /// <summary>
        /// 删除日程
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DelSchedule(string code, string oid = "")
        {
            var result = ControllerService.Run(() =>
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    ScheduleAdapter.Instance.Delete(p => p.AppendItem("ScheduleCode", code).AppendItem("Creator", CurrentUserCode));
                    ScheduleParticipantsAdapter.Instance.Delete(p => p.AppendItem("ScheduleCode", code));
                    scope.Complete();
                }
            });
            return this.Ok(result);
        }
        /// <summary>
        /// 删除日程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult DelSchedule(ParmMode pm)
        {
            var result = ControllerService.Run(() =>
            {
                JsonMode<AppointmentModel> oresult = null;
                if (!string.IsNullOrEmpty(pm.Id))
                {
                    var arg = new { Ids = new string[] { pm.Id } };
                    oresult = OperationOutlook<AppointmentModel>(deleteCalendar, arg).Result;
                }
                return oresult;
            });
            return this.Ok(result);
        }

        /// <summary>
        /// 响应会议
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult ResposeMeeting(ParmMode pm)
        {
            var result = ControllerService.Run(() =>
            {
                JsonMode<AppointmentModel> oresult = null;
                if (!string.IsNullOrEmpty(pm.Id))
                {
                    var arg = new
                    {
                        Id = pm.Id,
                        Type = pm.ResponsType,
                        SendResponse = pm.SendResponse
                    };
                    oresult = OperationOutlook<AppointmentModel>(respondMeeting, arg).Result;
                }
                return oresult;
            });
            return this.Ok(result);
        }


        /// <summary>
        /// 获取未读邮件数量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult UnreadEmail()
        {
            var result = ControllerService.Run(() =>
            {
                UnReadCountModel unredResult = new UnReadCountModel();
                JsonMode<UnReadCountModel> oresult = OperationOutlook<UnReadCountModel>(unreadEmail, null).Result;
                if (oresult.IsSuccess)
                {
                    unredResult = oresult.Data;
                }
                return unredResult;
            });
            return this.Ok(result);
        }


        private PunchStatistical GetPunchStatistical(DateTime s, DateTime e)
        {
            var authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
            return new PunchStatistical(s, e, authStr);
        }

        /// <summary>
        /// 加载日程数据的时候 把outlook 数据转换为本地数据格式  与本地数据合并
        /// </summary>
        /// <param name="localData"></param>
        /// <param name="time"></param>
        /// <param name="listData"> 是否是日历数据</param>
        /// <returns></returns>
        private ScheduleModelCollection OutToLocalCroessData(bool listData, DateTime s, DateTime e, ScheduleModelCollection localData)
        {
            DateTime start = s;
            var paramet = new { StartDate = start, EndDate = DateTime.Parse(e.ToString("yyyy-MM-dd") + " 23:59:59") };
            JsonMode<List<CalendarItemModel>> outlookdata = OperationOutlook<List<CalendarItemModel>>(getCaledar, paramet).Result;
            if (!outlookdata.IsSuccess)
            {
                Log.WriteLog(outlookdata.Message);
                return localData;
            }
            bool isCoressDay = false;
            int day = 0;
            string currentName = DisplayName;
            outlookdata.Data.ForEach(f =>
            {

                DateTime startTime = ConvertLongToDateTime(f.Start);
                DateTime endTime = ConvertLongToDateTime(f.End);
                if (f.IsAllDayEvent)
                {
                    endTime = DateTime.Parse(endTime.AddDays(-1).ToString("yyyy-MM-dd") + " " + strEndTime);
                }
                isCoressDay = startTime.ToString("yyyy-MM-dd") != endTime.ToString("yyyy-MM-dd"); // 跨天 
                {
                    if (isCoressDay)
                    {
                        day = (endTime - startTime).Days;
                        for (int i = 0; i <= day; i++)
                        {
                            DateTime tempstartTime = startTime, tempEndTime = endTime;
                            if (i == 0)
                            {
                                tempEndTime = DateTime.Parse(startTime.ToString("yyyy-MM-dd") + strEndTime);
                            }
                            else if (i == day)
                            {
                                tempstartTime = DateTime.Parse(endTime.ToString("yyyy-MM-dd") + strStartTime);
                            }
                            else
                            {
                                tempstartTime = tempstartTime.AddDays(i);
                                tempstartTime = DateTime.Parse(tempstartTime.ToString("yyyy-MM-dd") + strStartTime);
                                tempEndTime = DateTime.Parse(tempstartTime.ToString("yyyy-MM-dd") + strEndTime);
                            }

                            if (tempstartTime < paramet.StartDate || tempEndTime > paramet.EndDate)
                            {
                                continue;
                            }
                            localData.Add(new ScheduleModel()
                            {
                                Code = f.Id.Substring(f.Id.Length - 20) + i,
                                ScheduleCode = f.Id,
                                ScheduleType = 2,
                                Creator = CurrentUserCode,
                                Title = f.Subject,
                                StartTime = tempstartTime,
                                EndTime = tempEndTime,
                                Place = f.Location,
                                ReminderTime = CalcRemindType(f),
                                IsOutlook = true,
                                IsMeeting = f.IsMeeting,
                                IsCancelled = f.IsCancelled,
                                OutlookId = f.Id,
                                IsFromMe = f.Organizer.Name == currentName,
                                Content = f.Subject
                            });
                        }
                    }
                    else
                    {
                        localData.Add(new ScheduleModel()
                        {
                            Code = f.Id,
                            ScheduleCode = f.Id,
                            ScheduleType = 1,
                            Creator = CurrentUserCode,
                            Title = f.Subject,
                            StartTime = startTime,
                            EndTime = endTime,
                            Place = f.Location,
                            ReminderTime = CalcRemindType(f),
                            IsOutlook = true,
                            IsMeeting = f.IsMeeting,
                            IsCancelled = f.IsCancelled,
                            OutlookId = f.Id,
                            IsFromMe = f.Organizer.Name == currentName,
                            Content = f.Subject,
                        });
                    }
                }

            });
            outlookdataTemp = outlookdata.Data;
            return localData;
        }
        private ReminderTimeType CalcRemindType(CalendarItemModel calen)
        {
            if (!calen.IsReminderSet)
            {
                return ReminderTimeType.NoNotify;
            }
            else
            {
                int timeSpan = (int)ConvertLongToDateTime(calen.Start).Subtract(ConvertLongToDateTime(calen.ReminderDueBy)).TotalMinutes;
                switch (timeSpan)
                {
                    case 0:
                        return ReminderTimeType.EventBeginning;
                    case 15:
                        return ReminderTimeType.Beforehand15;
                    case 30:
                        return ReminderTimeType.Beforehand30;
                    case 60:
                        return ReminderTimeType.Beforehand60;
                    case 1440:
                        return ReminderTimeType.BeforehandOneDay;
                    default:
                        return ReminderTimeType.Other;
                }
            }
        }
        private TimeSpan CalcRemindTime(CalendarItemModel calen)
        {
            if (calen.IsReminderSet)
            {
                return ConvertLongToDateTime(calen.Start).Subtract(ConvertLongToDateTime(calen.ReminderDueBy));
            }
            else
            {
                return new TimeSpan();
            }
        }
        private DateTime ConvertLongToDateTime(long d)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(d + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }

        #region  操作outlook

        private async Task<JsonMode<T>> OperationOutlook<T>(string url, object args)
        {

            JsonMode<T> responsResult = new JsonMode<T>();
            string apiUrl = ConfigurationManager.AppSettings["OutlookApi"] + url;
            string apptoken = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
            IEnumerable<string> rsaPwd = new List<string>();
            if (!Request.Headers.TryGetValues("rsaPwd", out rsaPwd))
            {
                return responsResult;
            }
            string encryptio = rsaPwd.FirstOrDefault();
            string logonName = ((Seagull2Identity)User.Identity).LogonName;
            bool O365 = IsO365Email(logonName);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("apptoken", apptoken);
                http.DefaultRequestHeaders.Add("encryption", encryptio);
                http.DefaultRequestHeaders.Add("account", O365 ? (logonName + "@sinooceangroup.com") : logonName);
                http.DefaultRequestHeaders.Add("location", O365 ? "O365" : "");
                //ServiceUtility.SetCertificatePolicy();
                Task<HttpResponseMessage> response = http.PostAsJsonAsync(apiUrl, args);
                return JsonConvert.DeserializeObject<JsonMode<T>>(await response.Result.Content.ReadAsStringAsync());
            }
        }


        private bool IsO365Email(string logonName)
        {
            bool O365 = false;
            DataTable office365 = UsersInfoExtendAdapter.Instance.GetUserOfficeEmail(logonName);
            if (office365.Rows.Count > 0)
            {
                if (office365.Rows[0]["HomeMDB"] != null && office365.Rows[0]["HomeMDB"] != DBNull.Value)
                {
                    O365 = false;
                }
                else
                {
                    O365 = true;
                }
            }
            return O365;
        }



        private JsonMode<AppointmentModel> CreateOutlookMeeting(ScheduleModel model, bool isUpdate)
        {
            bool isReminderSet = false;
            int reminderMinutesBeforeStart = 0;
            if (model.ReminderTime != ReminderTimeType.NoNotify)
            {
                isReminderSet = true;
                switch (model.ReminderTime)
                {
                    case ReminderTimeType.Beforehand15:
                        reminderMinutesBeforeStart = 15;
                        break;
                    case ReminderTimeType.Beforehand30:
                        reminderMinutesBeforeStart = 30;
                        break;
                    case ReminderTimeType.Beforehand60:
                        reminderMinutesBeforeStart = 60;
                        break;
                    case ReminderTimeType.BeforehandOneDay:
                        reminderMinutesBeforeStart = 1440;
                        break;
                    case ReminderTimeType.Other:
                        reminderMinutesBeforeStart = (int)model.StartTime.Subtract(ConvertLongToDateTime(model.ReminderDueBy)).TotalMinutes;
                        break;
                }
            }
            else
            {
                isReminderSet = false;
            }
            var parm = new
            {
                Id = model.OutlookId,
                Subject = model.Title,
                Body = model.Content,
                Location = model.Place,
                Start = model.StartTime,
                End = model.EndTime,
                IsAllDayEvent = false,
                IsReminderSet = isReminderSet,
                ReminderMinutesBeforeStart = reminderMinutesBeforeStart,
                RequiredAttendees = new List<AttendeeWithIdModel>()
            };
            if (model.Participants.Count > 1)
            {
                var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, model.Participants.ToList().ConvertAll(t => t.PersonnelCode).ToArray()).ToList();
                users.ForEach(f =>
                {
                    parm.RequiredAttendees.Add(new AttendeeWithIdModel()
                    {
                        Name = f.DisplayName,
                        Email = f.Email
                    });
                });
            }
            if (isUpdate)
            {
                return OperationOutlook<AppointmentModel>(updateMeeting, parm).Result;
            }
            else
            {
                return OperationOutlook<AppointmentModel>(createMeeting, parm).Result;
            }
        }

        private async Task<JsonMode<T>> DeleteMeeting<T>(string oid)
        {
            if (string.IsNullOrEmpty(oid)) return null;
            var Ids = new List<object>();
            Ids.Add(new { id = oid });
            var parm = new
            {
                Ids = new string[] { oid }
            };
            //return OperationOutlook<AppointmentModel>(deleteMeeting, parm).Result;
            var user = ((Seagull2Identity)User.Identity);
            string apiUrl = ConfigurationManager.AppSettings["OutlookApi"] + deleteMeeting;
            string encryptio = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
            IEnumerable<string> rsaPwd = new List<string>();
            Request.Headers.TryGetValues("rsaPwd", out rsaPwd);
            string apptoken = rsaPwd.FirstOrDefault();
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("apptoken", apptoken);
                http.DefaultRequestHeaders.Add("encryption", encryptio);
                http.DefaultRequestHeaders.Add("account", user.LogonName);
                http.DefaultRequestHeaders.Add("", JsonConvert.SerializeObject(parm));
                StringContent context = new StringContent(JsonConvert.SerializeObject(parm))
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                };

                HttpResponseMessage response = http.PostAsync(apiUrl, context).Result;
                return JsonConvert.DeserializeObject<JsonMode<T>>(await response.Content.ReadAsStringAsync());
            }

        }


        /// <summary>
        /// 获取某月最后一天
        /// </summary>
        DateTime LastDayOfMounth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }
        #endregion
    }
    class scheduleDetail
    {
        public string themeTitle { get; set; }
        public string key { get; set; }
        public string scheduleCode { get; set; }
        public string startValue { get; set; }
        public string endValue { get; set; }
        public ReminderTimeType typeIndex { get; set; }
        public string address { get; set; }
        public string themeContent { get; set; }
        public string body { get; set; }
        public bool isOutlook { get; set; }
        public List<object> selectPeopleArr { get; set; }
        public bool IsReminderSet { get; set; }
        public long ReminderDueBy { get; set; }
        public TimeSpan ReminderDueByStr { get; set; }
        public bool IsFromMe { get; set; }
    }
    class JsonMode<T>
    {
        public bool IsSuccess { get; set; }

        public T Data { get; set; }

        public string Message { get; set; }
    }
    class CalendarItemModel
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public bool IsAllDayEvent { get; set; }
        public bool IsReminderSet { get; set; }
        public long ReminderDueBy { get; set; }
        public bool IsDraft { get; set; }
        public bool IsMeeting { get; set; } //是否为会议（否为约会）
        public int Importance { get; set; }
        public string Location { get; set; }
        public bool IsCancelled { get; set; }
        public AttendeeWithIdModel Organizer { get; set; }//主持人（等同于发件人）
    }
    class CalendarDetailModel : CalendarItemModel
    {
        public int MyResponseType { get; set; }//响应状态
        public bool IsResponseRequested { get; set; }
        public string Body { get; set; }
        public string PreviewText { get; set; }
        public bool IsFromMe { get; set; }//是否自己发送（创建）
        public List<AttendeeWithIdModel> RequiredAttendees { get; set; }//参会人
    }
    class AttendeeWithIdModel
    {
        public string UMID { get; set; }
        public string stringName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int? ResponseType { get; set; }
        public bool IsUser { get; set; }//是否为用户：1＝用户，0＝群组
        public string Id { get; set; }
        public string UserID { get; set; }
    }
    class AppointmentModel
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
    public class ParmMode
    {
        public bool isOutlook { get; set; }
        public string Id { get; set; }
        public string code { get; set; }
        /// <summary>
        /// 响应类型 1＝接受  2＝待定 3＝谢绝
        /// </summary>
        public int ResponsType { get; set; }

        public bool SendResponse { get; set; } //是否发送响应
    }

    public class UnReadCountModel
    {

        public int TotalCount { get; set; }

        public int UnreadCount { get; set; }
    }
}
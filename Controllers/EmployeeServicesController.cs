using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using MCS.Library.OGUPermission;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Sign;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Sign;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.EmployeeService;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 员工考勤API控制器
    /// </summary>
    public class EmployeeServicesController : ControllerBase
    {
        #region 查询原始打卡记录
        /// <summary>
        /// 查询记录入口
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> PunchList()
        {
            int page = int.Parse(HttpContext.Current.Request["page"]);

            var startTime = HttpContext.Current.Request["queryMonth"];

            string start;
            string end;
            if (!String.IsNullOrEmpty(startTime))
            {
                start = DateTime.Parse(startTime).ToString("yyyy-MM-dd");
                end = FirstDayNextMonth(DateTime.Parse(startTime));
            }
            else
            {
                start = FirstDayOfMonth(DateTime.Now);
                end = FirstDayNextMonth(DateTime.Now);
            }

            var dataCount = ComparePunchAdapater.Instance.QueryDataCount(((Seagull2Identity)User.Identity).Id, start, end);

            var queryNum = dataCount.Count() - (5 * page);


            var data = ComparePunchAdapater.Instance.LoadData(queryNum, ((Seagull2Identity)User.Identity).Id, start, end);

            List<Punch> list = new List<Punch>();
            foreach (var item in data)
            {
                list.Add(new Punch
                {
                    SignType = item.PunchType.ToString(),
                    SignTime = item.CreateTime.ToString("HH:mm"),
                    CreatTime = item.CreateTime.ToString("MM-dd"),
                    SignAddress = item.MapUrl
                });
            }

            return Ok(list);
        }
        #endregion

        #region 查询打卡记录（出勤详细情况）
        /// <summary>
        /// 查询打卡记录（出勤详细情况）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> QueryComparePurchList()
        {
            string startTime;
            string endTime;
            string sort;

            int page1 = int.Parse(HttpContext.Current.Request["page1"]);

            string start = HttpContext.Current.Request["queryMonth"];

            if (!String.IsNullOrEmpty(start))
            {
                startTime = FirstDayOfMonth(DateTime.Parse(start));
                endTime = FirstDayNextMonth(DateTime.Parse(start));
            }
            else
            {
                startTime = FirstDayOfMonth(DateTime.Now);
                endTime = FirstDayNextMonth(DateTime.Now);
            }

            sort = "ASC";
            var dataAM = ComparePunchAdapater.Instance.LoadCompareList(sort, ((Seagull2Identity)User.Identity).Id, startTime, endTime);

            sort = "DESC";
            var dataPM = ComparePunchAdapater.Instance.LoadCompareList(sort, ((Seagull2Identity)User.Identity).Id, startTime, endTime);


            var amPunchList = AMPunch(dataAM, dataPM);

            var totalNum = dataPM.Count();

            var endNum = totalNum - 5;

            //List<ComparePunch> endResult = new List<ComparePunch>();

            var endResult = amPunchList.Skip(page1 * 5).Take(5).ToList();

            return Ok(endResult.OrderByDescending(p => p.CreatTime));
        }
        #endregion

        #region 当月考勤信息
        /// <summary>
        /// 当月考勤信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<IHttpActionResult> QuerySignOfMonth()
        {
            string startTime;
            string endTime;

            string start = HttpContext.Current.Request["date"];

            if (!String.IsNullOrEmpty(start))
            {
                startTime = FirstDayOfMonth(DateTime.Parse(start));
                endTime = FirstDayNextMonth(DateTime.Parse(start));
            }
            else
            {
                startTime = FirstDayOfMonth(DateTime.Now);
                endTime = FirstDayNextMonth(DateTime.Now);
            }
            List<PunchMonth> apList = new List<PunchMonth>();
            var amList = PunchMonthAdapter.Instance.LoadDataOfMonth(((Seagull2Identity)User.Identity).Id, startTime, endTime, "ASC");
            var pmList = PunchMonthAdapter.Instance.LoadDataOfMonth(((Seagull2Identity)User.Identity).Id, startTime, endTime, "DESC");
            apList.AddRange(amList);
            apList.AddRange(pmList);
            apList = apList.OrderBy(s => s.CreateTime).ToList();
            var allList = GetSignin(apList);
            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList { State = true, Message = "操作成功！", Data = allList }));
        }

        /// <summary>
        /// 当月考勤信息业务逻辑判断
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public List<PunchMonthView> GetSignin(List<PunchMonth> pList)
        {
            try
            {
                List<PunchMonthView> list = new List<PunchMonthView>();

                var standerMessage = StandardPunchAdapater.Instance.LoadStandardList();
                DateTime date = Convert.ToDateTime(HttpContext.Current.Request["date"]); //获取接口返回的日期date
                DateTime mounthFirstDay = FirstDayOfMounth(date);  //返回日期date某月的第一天
                DateTime mounthLastDay = LastDayOfMounth(date);    //返回日期date某月的最后一天
                DateTime dtStand = new DateTime();
                DateTime dtEnd = new DateTime();
                StandardPunchModel standardPunch = new StandardPunchModel();
                var onTime = standardPunch.OnTime;
                var offTime = standardPunch.OffTime;
                //循环参数date当前月份的第一天和最后一天
                for (DateTime i = mounthFirstDay; i <= mounthLastDay; i = i.AddDays(1))
                {

                    var listOneDay = pList.FindAll(f => f.CreateTime.Date == i.Date);
                    var IsNormal = true;
                    if (listOneDay.Count > 0)
                    {
                        //am
                        var amData = listOneDay.Find(f => f.CreateTime.Hour < 12);
                        Info am = null;
                        if (amData != null)
                        {
                            am = new Info();
                            //获取am打卡标准时间
                            foreach (var m in standerMessage)
                            {
                                if (amData.StandardPunchCode == m.Code)
                                {
                                    onTime = m.OnTime;
                                    dtStand = Convert.ToDateTime(i.Date.ToString("yyyy-MM-dd") + " " + onTime);
                                    break;
                                }
                            }
                            am.IsNormal = true;
                            //判断是否迟到
                            if (amData.CreateTime > dtStand)
                            {
                                am.IsNormal = false;
                            }
                            am.Time = amData.CreateTime.ToString("HH:mm");
                            am.Address = amData.Address;
                            am.MapUrl = amData.MapUrl;
                        }

                        //pm
                        var pmData = listOneDay.Find(f => f.CreateTime.Hour >= 12);
                        Info pm = null;
                        if (pmData != null)
                        {
                            pm = new Info();
                            //获取pm打卡标准时间
                            foreach (var m in standerMessage)
                            {
                                if (pmData.StandardPunchCode == m.Code)
                                {
                                    offTime = m.OffTime;
                                    dtEnd = Convert.ToDateTime(i.Date.ToString("yyyy-MM-dd") + " " + offTime);
                                    break;
                                }
                            }
                            pm.IsNormal = true;
                            //判断是否早退
                            if (pmData.CreateTime < dtEnd)
                            {
                                pm.IsNormal = false;
                            }
                            pm.Time = pmData.CreateTime.ToString("HH:mm");
                            pm.Address = pmData.Address;
                            pm.MapUrl = pmData.MapUrl;
                        }

                        //判断该天打卡是否正常
                        IsNormal = am != null && am.IsNormal && pm != null && pm.IsNormal ? true : false;

                        list.Add(new PunchMonthView()
                        {

                            Date = i.Date.ToString("yyyy-MM-dd"),
                            IsNormal = IsNormal,
                            AM = am,
                            PM = pm
                        });
                    }
                    else
                    {
                        list.Add(new PunchMonthView()
                        {
                            Date = i.Date.ToString("yyyy-MM-dd"),
                            IsNormal = false,
                            AM = null,
                            PM = null
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                Log.WriteLog(ex.StackTrace);
                throw ex;
            }
        }
        /// <summary>
        /// 获取一个月份的第一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public DateTime FirstDayOfMounth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day);
        }

        /// <summary>
        /// 返回某月的最后一天
        /// </summary>
        /// <param name="datetimre"></param>
        /// <returns></returns>
        public DateTime LastDayOfMounth(DateTime datetimre)
        {
            return datetimre.AddDays(1 - datetimre.Day).AddMonths(1).AddDays(-1);
        }

        #endregion

        #region 分析是否打卡、是否迟到早退、以及迟到早退多长时间

        #region 判断是否打卡
        /// <summary>
        /// 判断是否打卡
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool IsInTimeInterval(DateTime time, DateTime startTime, DateTime endTime)
        {
            //判断时间段开始时间是否小于时间段结束时间，如果不是就交换
            if (startTime > endTime)
            {
                DateTime tempTime = startTime;
                startTime = endTime;
                endTime = tempTime;
            }

            //获取以公元元年元旦日时间为基础的新判断时间
            DateTime newTime = new DateTime();
            newTime = newTime.AddHours(time.Hour);
            newTime = newTime.AddMinutes(time.Minute);
            newTime = newTime.AddSeconds(time.Second);

            //获取以公元元年元旦日时间为基础的区间开始时间
            DateTime newStartTime = new DateTime();
            newStartTime = newStartTime.AddHours(startTime.Hour);
            newStartTime = newStartTime.AddMinutes(startTime.Minute);
            newStartTime = newStartTime.AddSeconds(startTime.Second);

            //获取以公元元年元旦日时间为基础的区间结束时间
            DateTime newEndTime = new DateTime();
            if (startTime.Hour > endTime.Hour)
            {
                newEndTime = newEndTime.AddDays(1);
            }
            newEndTime = newEndTime.AddHours(endTime.Hour);
            newEndTime = newEndTime.AddMinutes(endTime.Minute);
            newEndTime = newEndTime.AddSeconds(endTime.Second);

            if (newTime > newStartTime && newTime < newEndTime)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 判断是否迟到早退
        /// <summary>
        /// 计算是否迟到
        /// </summary>
        /// <param name="dateBegin"></param>
        /// <param name="dateEnd"></param>
        /// <returns>true 代表迟到</returns>
        public bool ExecDateDiff(DateTime dateBegin, DateTime dateEnd)
        {
            bool puch = false;
            if (dateBegin > dateEnd)
            {
                puch = true;
            }
            else
            {
                puch = false;
            }

            return puch;
        }
        #endregion

        #region 计算迟到早退多长时间（按分钟计算）
        /// <summary>
        /// 计算迟到早退多长时间（按分钟计算）
        /// </summary>
        /// <param name="DateTime1"></param>
        /// <param name="DateTime2"></param>
        /// <returns></returns>
        public int DateDiff(DateTime startTime, DateTime endTime)
        {
            int dateDiff;
            TimeSpan ts1 = new TimeSpan(startTime.Hour, startTime.Minute, startTime.Second);
            TimeSpan ts2 = new TimeSpan(endTime.Hour, endTime.Minute, endTime.Second);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            int hours = ts.Hours, minutes = ts.Minutes, seconds = ts.Seconds;
            if (ts.Hours < 10)
            {
                hours = 0 + ts.Hours;
            }
            if (ts.Minutes < 10)
            {
                minutes = 0 + ts.Minutes;
            }
            if (ts.Seconds < 10)
            {
                seconds = 0 + ts.Seconds;
            }
            dateDiff = hours * 60 + minutes;
            return dateDiff;
        }
        #endregion

        #endregion

        #region 上午 下午 打卡情况统计
        /// <summary>
        /// 上午 下午 打卡情况统计
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataPM"></param>
        /// <returns></returns>
        public List<ComparePunch> AMPunch(List<EmployeeServicesModel> data, List<EmployeeServicesModel> dataPM)
        {
            List<ComparePunch> list = new List<ComparePunch>();

            var standerMessage = StandardPunchAdapater.Instance.LoadStandardList();

            var onTime = "";
            var offTime = "";

            //上午打卡情况统计
            foreach (var item in data)
            {
                var timeCode = item.StandardPunchCode;
                for (int i = 0; i < standerMessage.Count; i++)
                {
                    if (timeCode == standerMessage[i].Code)
                    {
                        onTime = standerMessage[i].OnTime;

                    }
                }

                //1.0判断上午是否出勤
                var AM = IsInTimeInterval(item.CreateTime, Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd 00:00:00")), Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd 12:00:00")));

                //2.0、判断是否迟到
                var cludeTime = TimeInclude(Convert.ToDateTime(item.CreateTime), Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd 00:00:00")), Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd") + " " + onTime));


                if (AM)
                {

                    //判断是否在晚上十二点和早上八点半之间
                    if (cludeTime == 1)
                    {
                        list.Add(new ComparePunch
                        {
                            CreatTime = item.CreateTime.ToString("MM-dd"),
                            AMSignTime = item.CreateTime.ToString("HH:mm"),
                            standerPunchCode = item.StandardPunchCode
                        });
                    }
                    else
                    {
                        var bl = DateDiff(item.CreateTime, Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd " + onTime)));
                        if (bl >= 1 && bl < 2)
                        {
                            //int lateMinutes = DateDiff(item.CreateTime, Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd " + onTime)));
                            list.Add(new ComparePunch
                            {
                                CreatTime = item.CreateTime.ToString("MM-dd"),
                                AMSignTime = "迟到" + "1" + "分钟",
                                standerPunchCode = item.StandardPunchCode
                            });


                        }
                        else
                        {

                            int lateMinutes = DateDiff(item.CreateTime, Convert.ToDateTime(item.CreateTime.ToString("yyyy-MM-dd " + onTime)));
                            list.Add(new ComparePunch
                            {
                                CreatTime = item.CreateTime.ToString("MM-dd"),
                                AMSignTime = "迟到" + lateMinutes.ToString() + "分钟",
                                standerPunchCode = item.StandardPunchCode
                            });
                        }

                    }

                }
                else
                {
                    list.Add(new ComparePunch
                    {
                        CreatTime = item.CreateTime.ToString("MM-dd"),
                        AMSignTime = "未打卡",
                        standerPunchCode = item.StandardPunchCode
                    });
                }
            }

            //下午打卡情况统计
            for (int i = 0; i < list.Count; i++)

            {
                var pmCode = list[i].standerPunchCode;
                for (int x = 0; x < standerMessage.Count; x++)
                {
                    if (pmCode == standerMessage[x].Code)
                    {
                        offTime = standerMessage[x].OffTime;
                    }
                }

                var cludeTimePM = TimeInclude(Convert.ToDateTime(dataPM[i].CreateTime), Convert.ToDateTime(dataPM[i].CreateTime.ToString("yyyy-MM-dd") + " " + offTime), Convert.ToDateTime(dataPM[i].CreateTime.ToString("yyyy-MM-dd 23:59:59")));





                var PM = IsInTimeInterval(dataPM[i].CreateTime, Convert.ToDateTime(dataPM[i].CreateTime.ToString("yyyy-MM-dd 12:00:00")), Convert.ToDateTime(dataPM[i].CreateTime.ToString("yyyy-MM-dd 23:59:59")));
                if (PM)  //下午已经出勤
                {
                    if (cludeTimePM == 1)//如果打卡时间在17:30----23:59之间  没有早退
                    {
                        list[i].PMSignTime = dataPM[i].CreateTime.ToString("HH:mm");
                    }
                    else
                    {
                        //2.0 判断下午是否早退
                        var b2 = DateDiff(dataPM[i].CreateTime, Convert.ToDateTime(dataPM[i].CreateTime.ToString("yyyy-MM-dd " + offTime)));
                        if (b2 >= 1 && b2 < 2)
                        {

                            list[i].PMSignTime = "早退" + "1" + "分钟";

                        }
                        else
                        {
                            int beforeMinutes = DateDiff(Convert.ToDateTime(dataPM[i].CreateTime), Convert.ToDateTime(dataPM[i].CreateTime.ToString("yyyy-MM-dd " + offTime)));
                            list[i].PMSignTime = "早退" + (beforeMinutes + 1).ToString() + "分钟";
                        }
                    }
                }
                else   //没有打卡
                {
                    list[i].PMSignTime = "未打卡";
                }
            }


            return list;
        }
        #endregion

        #region 取得某月的第一天和最后一天
        /// <summary>
        /// 取得某月的第一天
        /// </summary>
        /// <param name="datetime">要取得月份第一天的时间</param>
        /// <returns></returns>
        private string FirstDayOfMonth(DateTime datetime)
        {
            var time = datetime.AddDays(1 - datetime.Day);
            return time.ToString("yyyy-MM-dd");
        }
        private string FirstDayNextMonth(DateTime datetime)
        {
            var time = datetime.AddMonths(1);
            //var time = datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);

            return time.ToString("yyyy-MM-01");
        }
        #endregion

        #region 查询最近六个月
        /// <summary>
        /// 查询最近六个月
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult QueryDateList()
        {
            List<string> listItem = new List<string>();
            for (DateTime date = DateTime.Now; date > DateTime.Now.AddMonths(-6); date = date.AddMonths(-1))
            {
                listItem.Add(date.ToString("yyyy-MM"));
            }
            return Ok(listItem);
        }


        public async Task<IHttpActionResult> LoadToDayEmpoly()
        {
            string startTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
            string endTime = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
            EmployeeServicesCollection employeeServiceCollection = EmployeeServicesAdapter.Instance.LoadEmployTodayAll(((Seagull2Identity)User.Identity).Id, startTime, endTime);
            EmployeeServicesCollection listEmployeeService = new EmployeeServicesCollection();
            EmployeeServicesModel employeeServices = new EmployeeServicesModel();
            if (employeeServiceCollection.Count > 0)
            {
                if (employeeServiceCollection.Count < 2)
                {
                    employeeServices = EmployeeServicesAdapter.Instance.LoadEmployess(((Seagull2Identity)User.Identity).Id, startTime, endTime, employeeServiceCollection.Count);
                    listEmployeeService.Add(employeeServices);
                }
                else
                {
                    employeeServices = EmployeeServicesAdapter.Instance.LoadEmployess(((Seagull2Identity)User.Identity).Id, startTime, endTime, 0);
                    listEmployeeService.Add(employeeServices);

                    EmployeeServicesModel employeeServicesLast = EmployeeServicesAdapter.Instance.LoadEmployess(((Seagull2Identity)User.Identity).Id, startTime, endTime, employeeServiceCollection.Count);
                    listEmployeeService.Add(employeeServicesLast);
                }
            }
            return Ok(listEmployeeService);
        }


        #endregion

        #region 已经注释---- 根据城市查询目标经纬度 查询XML文件配置信息
        /// <summary>
        /// 根据城市查询目标经纬度
        /// </summary>
        /// <returns></returns>

        //public async Task<IHttpActionResult> GetLocationMessage()
        //{
        //    var city = HttpContext.Current.Request["city"];
        //    List<double> list = new List<double>();
        //    var xmlWorkMessage = ComparePunchAdapater.Instance.QueryWorkMessages();
        //    double lat = 0.00;
        //    double lng = 0.00;

        //    foreach (var item in xmlWorkMessage)
        //    {
        //        if (item.Id == city)
        //        {
        //            lat = item.Lat;
        //            lng = item.Lng;
        //        }
        //    }
        //    list.Add(lat);
        //    list.Add(lng);
        //    return Ok(list);
        //}
        #endregion

        #region 查询所有的标准地理位置信息
        /// <summary>
        /// 查询所有的标准地理位置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> LoadStanderMessage()
        {
            var standerMessage = StandardPunchAdapater.Instance.LoadStandardList();
            return Ok(standerMessage);
        }
        #endregion

        static int nowDistance = 1;

        #region 员工打卡记录
        /// <summary>
        /// 员工打卡记录
        /// </summary>
        [HttpPost]
        public IHttpActionResult Sign(SaveViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ((Seagull2Identity)User.Identity).Id)[0];
                var timeLine = ConfigAppSetting.SignTimeLine.Split(',');
                var timeLine1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[0]);
                var timeLine2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[1]);

                var punchType = PunchType(timeLine1, timeLine2, out DateTime punchDate);

                List<EmployHelpViewModel> list = new List<EmployHelpViewModel>();
                if (String.IsNullOrEmpty(model.Address) || String.IsNullOrEmpty(model.Lat) || String.IsNullOrEmpty(model.Lng))
                {
                    throw new Exception("参数不能为空");
                }
                //取系统打卡地点
                var standerMessage = StandardPunchAdapater.Instance.LoadStandardList();
                if (standerMessage.Count < 1)
                {
                    throw new Exception("暂未设置打卡地点");
                }
                //取用户当前位置与系统所有打卡地点1公里内的集合
                foreach (StandardPunchModel item in standerMessage)
                {
                    double distances = GetDistance(Convert.ToDouble(model.Lat), Convert.ToDouble(model.Lng), Convert.ToDouble(item.Lat), Convert.ToDouble(item.Lng));
                    //过滤距离打卡地点1公里外的数据
                    if (distances < 1)
                    {
                        list.Add(new EmployHelpViewModel
                        {
                            Distances = distances,
                            StanderCode = item.Code,
                            OnTime = item.OnTime,
                            OffTime = item.OffTime
                        });
                    }
                }
                if (list.Count < 1)
                {
                    throw new Exception("未在打卡范围");
                }
                //取出距离系统规定打卡地点最小的对象
                var employHelp = list.OrderBy(m => m.Distances).FirstOrDefault();

                var _model = new EmployeeServicesModel();
                _model.Code = Guid.NewGuid().ToString();
                _model.CnName = user.DisplayName;
                _model.EnName = user.LogOnName;
                _model.OrganizationCode = user.Parent.ID;
                _model.FullPath = user.FullPath.Remove(user.FullPath.LastIndexOf("\\"));
                _model.PunchDate = punchDate;
                _model.PunchType = punchType;
                _model.MapUrl = model.Address;
                _model.Lat = model.Lat;
                _model.Lng = model.Lng;
                _model.StandardPunchCode = employHelp.StanderCode;
                if (punchType == 0)
                {
                    // 判断是否迟到
                    _model.IsLate = IsLate(employHelp.OnTime, out int minute);
                    if (_model.IsLate == true)
                    {
                        _model.Minute = minute;
                    }
                }
                if (punchType == 1)
                {
                    // 判断是否早退
                    _model.IsEarly = IsEarly(employHelp.OffTime, out int minute, timeLine1);
                    if (_model.IsEarly == true)
                    {
                        _model.Minute = minute;
                    }
                }
                _model.IsUnusual = false;
                _model.Creator = user.ID;
                _model.CreateTime = DateTime.Now;
                _model.IsValid = true;
                EmployeeServicesAdapter.Instance.Update(_model);
            });
            return Ok(result);
        }
        #endregion

        /// <summary>
        /// 增加打卡记录
        /// </summary>
        /// <param name="lng">伟度</param>
        /// <param name="lat">经度</param>
        /// <param name="mapUrl"></param>
        /// <param name="standerCode"></param>
        [HttpGet, HttpPost]
        public async Task<IHttpActionResult> GetGreatCircleDistance(string lng, string lat, string mapUrl, string standerCode)
        {
            byte result = 0;
            try
            {
                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ((Seagull2Identity)User.Identity).Id)[0];
                var timeLine = ConfigAppSetting.SignTimeLine.Split(',');
                var timeLine1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[0]);
                var timeLine2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[1]);

                var punchType = PunchType(timeLine1, timeLine2, out DateTime punchDate);

                var standerMessage = StandardPunchAdapater.Instance.LoadStandardList();//拿出定义区域
                if (standerMessage.Count < 1)
                {
                    Log.WriteLog("无有效的打卡地点");
                    return await Task.FromResult<IHttpActionResult>(Ok(result));
                }
                double distances1 = double.MaxValue;
                string onTime = "";
                string offTime = "";
                foreach (StandardPunchModel item in standerMessage)
                {
                    double distances = GetDistance(Convert.ToDouble(lat), Convert.ToDouble(lng), Convert.ToDouble(item.Lat), Convert.ToDouble(item.Lng));
                    if (distances1 > distances)
                    {
                        distances1 = distances;
                        standerCode = item.Code;
                        onTime = item.OnTime;
                        offTime = item.OffTime;
                    }
                }

                if (distances1 < nowDistance)
                {
                    if (!String.IsNullOrEmpty(mapUrl) && !String.IsNullOrEmpty(lat) && !String.IsNullOrEmpty(lng))
                    {
                        var _model = new EmployeeServicesModel();
                        _model.Code = Guid.NewGuid().ToString();
                        _model.CnName = user.DisplayName;
                        _model.EnName = user.LogOnName;
                        _model.OrganizationCode = user.Parent.ID;
                        _model.FullPath = user.FullPath.Remove(user.FullPath.LastIndexOf("\\"));
                        _model.PunchDate = punchDate;
                        _model.PunchType = punchType;
                        _model.MapUrl = mapUrl;
                        _model.Lat = lat;
                        _model.Lng = lng;
                        _model.StandardPunchCode = standerCode;
                        if (punchType == 0)
                        {
                            // 判断是否迟到
                            _model.IsLate = IsLate(onTime, out int minute);
                            if (_model.IsLate == true)
                            {
                                _model.Minute = minute;
                            }
                        }
                        if (punchType == 1)
                        {
                            // 判断是否早退
                            _model.IsEarly = IsEarly(offTime, out int minute, timeLine1);
                            if (_model.IsEarly == true)
                            {
                                _model.Minute = minute;
                            }
                        }
                        _model.IsUnusual = false;
                        _model.Creator = user.ID;
                        _model.CreateTime = DateTime.Now;
                        _model.IsValid = true;
                        EmployeeServicesAdapter.Instance.Update(_model);
                        result = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("考勤报错信息1：" + ex.Message);
                Log.WriteLog("考勤报错堆栈2：" + ex.StackTrace);
            }
            return await Task.FromResult<IHttpActionResult>(Ok(result));
        }


        #region 判断是否迟到或者早退
        /// <summary>
        /// /判断是否迟到或者早退
        /// </summary>
        /// <param name="time">目标时间</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        public int TimeInclude(DateTime time, DateTime startTime, DateTime endTime)
        {
            int flage = 0;
            if (time > startTime && time < endTime)
            {
                flage = 1;
            }
            else
            {
                flage = 0;
            }
            return flage;
        }
        #endregion

        private const double EARTH_RADIUS = 6378.137; //地球半径
        private static double Rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 计算周边一千米
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);

            double radLat2 = Rad(lat2);

            double a = radLat1 - radLat2;

            double b = Rad(lng1) - Rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
            Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));

            s = s * EARTH_RADIUS;

            s = Math.Round(s * 10000) / 10000;

            return s;
        }

        #region 后台管理页面
        /// <summary>
        /// 员工考勤列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="searchTime"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult EmployeeAttendanceList(int pageIndex, string searchTime = "", string userName = "", string signAddress = "", string signStartDate = "", string signEndDate = "", string fullPathName = "")
        {
            ViewPageBase<List<EmployeeAttendanceViewModel>> list = new ViewPageBase<List<EmployeeAttendanceViewModel>>();
            ControllerHelp.SelectAction(() =>
            {
                var time = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(searchTime))
                {
                    time = Convert.ToDateTime(searchTime);
                }
                list = EmployeeAttendanceViewModelAdapter.Instance.GetEmployeeAttendanceViewByPage(pageIndex, time, userName, signAddress, signStartDate, signEndDate, fullPathName);
                list.Data1 = StandardPunchAdapater.Instance.LoadStandardList();   //获取考勤地点集合
            });
            return Ok(list);
        }
        /// <summary>
        /// 员工考勤导出成EXCEL
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForEmployeeAttendance(string userName = "", string signAddress = "", string signStartDate = "", string signEndDate = "", string fullPathName = "")
        {

            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                List<EmployeeAttendanceViewModel> list = EmployeeAttendanceViewModelAdapter.Instance.GetAllEmployeeAttendance(userName, signAddress, signStartDate, signEndDate, fullPathName);

                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"打卡人","CnName" },
                    {"组织机构","FullPath" },
                    {"打卡日期","CreateDate" },
                    {"上/下午","PunchTypeStr" },
                    {"打卡时间","CreateTimeStr" },
                    {"打卡地点","Address" }
                    };
                result = ExcelHelp<EmployeeAttendanceViewModel, List<EmployeeAttendanceViewModel>>.ExportExcelData(dicColl, list, "EmployeeAttendance");

            });
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "导出应用管理-员工考勤-打卡记录");
            return result;
        }
        /// <summary>
        /// 员工考勤_统计导出
        /// </summary>
        /// <param name="attendanceMonth">考勤月份</param>
        /// <param name="IsFilterWeek">是否过滤周末</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForEmployeeStatistics(string attendanceMonth = "", bool isFilterWeek = false)
        {

            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                List<EmployeeStatisticsViewModel> list = EmployeeStatisticsViewModelAdapter.Instance.GetAllEmployeeAttendance(attendanceMonth);

                int daysCurrentMonth = DateTime.DaysInMonth(Convert.ToDateTime(attendanceMonth).Year, Convert.ToDateTime(attendanceMonth).Month);
                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"打卡人","DisplayName" },
                    {"登录名","LOGON_NAME" },
                    {"组织机构","FullPath" }
                };
                for (var i = 1; i <= daysCurrentMonth; i++)
                {
                    if (isFilterWeek)
                    {
                        if (Convert.ToDateTime(attendanceMonth + "-" + i.ToString()).DayOfWeek != DayOfWeek.Sunday && Convert.ToDateTime(attendanceMonth + "-" + i.ToString()).DayOfWeek != DayOfWeek.Saturday)
                        {
                            dicColl.Add(i.ToString(), "Day" + i);
                        }
                    }
                    else
                    {
                        dicColl.Add(i.ToString(), "Day" + i);
                    }
                };

                result = ExcelHelp<EmployeeStatisticsViewModel, List<EmployeeStatisticsViewModel>>.ExportExcelData(dicColl, list, "EmployeeStatistics");
            });
            return result;
        }










        #endregion

        #region 保存打卡地点信息
        /// <summary>
        /// 保存打卡地点信息
        /// </summary>
        /// <param name="standard"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult EditStanderd(StandardViewModel standard)
        {
            var user = (Seagull2Identity)User.Identity;
            if (string.IsNullOrWhiteSpace(standard.Code))
            {
                //新增
                StandardPunchAdapater.Instance.Update(new StandardPunchModel
                {
                    Code = Guid.NewGuid().ToString(),
                    OnTime = standard.OnTime,
                    OffTime = standard.OffTime,
                    Lng = standard.Lng,
                    Lat = standard.Lat,
                    Address = standard.Address,
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    Modifier = user.Id,
                    ModifyTime = DateTime.Now,
                    ValidStatus = true,
                    IsValid = standard.IsValid

                });
            }
            else
            {
                //修改后保存
                var find = StandardPunchAdapater.Instance.Load(p => { p.AppendItem("Code", standard.Code); }).SingleOrDefault();
                if (find == null)
                {
                    return Ok(new BaseView()
                    {
                        State = false,
                        Message = "数据不存在于数据库"
                    });
                }
                StandardPunchAdapater.Instance.Update(new StandardPunchModel
                {
                    Code = standard.Code,
                    OnTime = standard.OnTime,
                    OffTime = standard.OffTime,
                    Lng = standard.Lng,
                    Lat = standard.Lat,
                    Address = standard.Address,
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    Modifier = user.Id,
                    ModifyTime = DateTime.Now,
                    ValidStatus = standard.ValidStatus,
                    IsValid = standard.IsValid
                });
            }
            StandardPunchAdapater.Instance.Dispose();
            ControllerService.UploadLog(user.Id, "操作了应用管理-考勤打卡-" +  "打卡地点");
            return Ok(new BaseView
            {
                State = true,
                Message = "保存成功！"
            });
        }
        #endregion

        #region 启用 / 禁用打卡地点
        /// <summary>
        /// 启用 / 禁用打卡地点
        /// </summary>
        [HttpPost]
        public IHttpActionResult UpdateIsPublicState()
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                //ture--启用，false--禁用
                bool isPublic = Convert.ToBoolean(baseRequest.Form["IsPublic"]);
                string standerdCode = baseRequest.Form["StanderCode"];

                var conferencemodel = StandardPunchAdapater.Instance.LoadByCode(standerdCode);
                conferencemodel.IsValid = isPublic;
                StandardPunchAdapater.Instance.Update(conferencemodel);
            });
            return Ok(result);
        }
        #endregion

        #region 根据code获取打卡地点详情信息
        /// <summary>
        /// 根据code获取打卡地点详情信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult SelectStanderdByCode(string code)
        {
            StandardPunchModel model = new StandardPunchModel();
            ControllerHelp.SelectAction(() =>
            {
                model = StandardPunchAdapater.Instance.LoadByCode(code);
            });

            return Ok(model);
        }
        #endregion

        #region 根据打卡地点名称查询打卡地点信息
        /// <summary>
        /// 根据打卡地点名称查询打卡地点信息
        /// </summary>
        [HttpGet]
        public IHttpActionResult SelectStanderdByName(string name)
        {
            StandardPunchCollection modelColl = new StandardPunchCollection();
            if (!string.IsNullOrEmpty(name) && name != "null")
            {
                ControllerHelp.SelectAction(() =>
                {
                    modelColl = StandardPunchAdapater.Instance.Load(m => m.AppendItem("Address", "%" + name + "%", "LIKE"));
                });
            }
            else
            {
                ControllerHelp.SelectAction(() =>
                {
                    modelColl = StandardPunchAdapater.Instance.Load(m => m.AppendItem("1", "1"));
                });

            }
            return Ok(modelColl);
        }
        #endregion

        #region 根据组织机构名称获取所有打卡地点
        /// <summary>
        /// 根据组织机构名称获取所有打卡地点
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="searchTime">查询时间</param>
        /// <param name="signAddress">地点名称</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetStanderdByAddressList(int pageIndex, string searchTime = "", string signAddress = "")
        {
            ViewPageBase<List<StandardViewModel>> list = new ViewPageBase<List<StandardViewModel>>();
            ControllerHelp.SelectAction(() =>
            {
                list = StandardViewModelAdapter.Instance.GetStandardDataViewByPage(pageIndex, DateTime.Now, signAddress);
            });

            return Ok(list);
        }
        #endregion

        #region 私有方法

        /// <summary>
        /// 判断打卡类型
        /// </summary>
        /// <param name="timeLine1">时间线1</param>
        /// <param name="timeLine2">时间线2</param>
        /// <param name="PunchDate">打卡日期</param>
        /// <returns>打卡类型 0=上班、1=下班</returns>
        int PunchType(DateTime timeLine1, DateTime timeLine2, out DateTime PunchDate)
        {
            var hour = DateTime.Now.Hour;

            if (timeLine1.Hour <= hour && hour < timeLine2.Hour)
            {
                PunchDate = DateTime.Now;
                return 0;// 上班
            }
            else
            {
                PunchDate = DateTime.Now;
                if (0 <= hour && hour < timeLine1.Hour)
                {
                    PunchDate = DateTime.Now.AddDays(-1);
                }
                return 1;// 下班
            }
        }
        /// <summary>
        /// 是否迟到
        /// </summary>
        /// <param name="onTime">上班时间</param>
        /// <param name="minute">迟到分钟数</param>
        /// <returns>是否迟到</returns>
        bool IsLate(string onTime, out int minute)
        {
            var time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + onTime);// 上班时间
            if (DateTime.Now <= time)
            {
                minute = 0;
                return false;
            }
            minute = (int)(DateTime.Now - time).TotalMinutes;
            return true;
        }
        /// <summary>
        /// 是否早退
        /// </summary>
        /// <param name="offTime">下班时间</param>
        /// <param name="minute">早退分钟数</param>
        /// <param name="timeLineOff">签退截止时间线</param>
        /// <returns>是否早退</returns>
        bool IsEarly(string offTime, out int minute, DateTime timeLineOff)
        {
            var time = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + offTime);// 下班时间
                                                                                               // 当天签退
            if (DateTime.Now >= time)
            {
                minute = 0;
                return false;
            }
            // 次日凌晨签退
            if (DateTime.Now <= timeLineOff)
            {
                minute = 0;
                return false;
            }
            minute = (int)(time - DateTime.Now).TotalMinutes;
            return true;
        }

        #endregion
    }
}
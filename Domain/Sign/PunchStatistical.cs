using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Domain.Sign
{
    /// <summary>
    /// 打卡统计
    /// </summary>
    public class PunchStatistical
    {
        /// <summary>
        /// 打卡统计（初始化打卡工作日）
        /// </summary>
        /// <param name="date1">开始时间</param>
        /// <param name="date2">结束时间</param>
        /// <param name="authorizationStr">用户身份认证字符串</param>
        /// <param name="userCode">当前用户的Code</param>
        public PunchStatistical(DateTime date1, DateTime date2, string authorizationStr, string userCode)
        {
            GetWorkDateList(date1, date2, authorizationStr);
            _CurrentUserCode = userCode;
            _SearchTime1 = date1;
            _SearchTime2 = date2;
        }

        public PunchStatistical(DateTime date1, DateTime date2, string authorizationStr)
        {
            GetWorkDateList(date1, date2, authorizationStr);
        }
        /// <summary>
        /// 获取或设置指定时间段内的工作日列表
        /// </summary>
        public List<DateTime> _WorkDateList;

       
        /// <summary>
        /// 当前用户的编码
        /// </summary>
        string _CurrentUserCode;

        /// <summary>
        /// 查询开始时间
        /// </summary>
        DateTime _SearchTime1;

        /// <summary>
        /// 查询结束时间
        /// </summary>
        DateTime _SearchTime2;

        /// <summary>
        /// 工作日字符串
        /// </summary>
        string _WorkDays
        {
            get
            {
                return string.Join(",", _WorkDateList.Select(w => w.ToString("yyyy-MM-dd")));
            }
        }

        /// <summary>
        /// 数据统计
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="code">编码</param>
        public object GetStatistical(PunchStatisticalEnum type, string code)
        {
            switch (type)
            {
                case PunchStatisticalEnum.AllManage:
                    {
                        var dt1 = Adapter.PunchManagement.PunchManagementAdapter.Instance.GetList(_CurrentUserCode);
                        var dt2 = Adapter.Sign.EmployeeServicesAdapter.Instance.GetPunchStatistical(PunchStatisticalDataEnum.SettingUserCode.ToString(), _CurrentUserCode, _WorkDays);

                        dt1.PrimaryKey = new DataColumn[] { dt1.Columns["Name"] };
                        dt2.PrimaryKey = new DataColumn[] { dt2.Columns["Name"] };

                        dt1.Merge(dt2);

                        var view = new List<ViewsModel.Sign.SignStatisticsPlusViewModel>();
                        foreach (DataRow dr in dt1.Rows)
                        {
                            var model = new ViewsModel.Sign.SignStatisticsPlusViewModel
                            {
                                Code = dr["Code"].ToString(),
                                Name = dr["Name"].ToString(),
                                Type = "Management",
                                IsCustomize = (int)dr["Type"] == 1 ? true : false,
                                UserCount = 0,
                                AvgHour = "--",
                                LateCount = "--",
                                EarlyCount = "--",
                                UnusualCount = "--",
                                NoSignCount = "--",
                                DefaultCode = dr["Default"].ToString().Split('|')[0],
                                DefaultType = dr["Default"].ToString().Split('|')[1],
                                Creator = dr["Creator"].ToString(),
                            };
                            if (dr["UserCount"] != DBNull.Value)
                            {
                                model.UserCount = (int)dr["UserCount"];
                            }
                            if (dr["AvgMinute"] != DBNull.Value)
                            {
                                var minute = Convert.ToDouble(dr["AvgMinute"]);
                                if (minute > 0)
                                {
                                    model.AvgHour = $"{(minute / 60).ToString("0.0")}";
                                }
                            }
                            if (dr["LateCount"] != DBNull.Value)
                            {
                                model.LateCount = dr["LateCount"].ToString();
                            }
                            if (dr["EarlyCount"] != DBNull.Value)
                            {
                                model.EarlyCount = dr["EarlyCount"].ToString();
                            }
                            if (dr["UnusualCount"] != DBNull.Value)
                            {
                                model.UnusualCount = dr["UnusualCount"].ToString();
                            }
                            if (dr["SignCount"] != DBNull.Value)
                            {
                                var dayCount = _WorkDateList.Count;
                                var signCount = Convert.ToInt32(dr["SignCount"]);
                                model.NoSignCount = (dayCount * model.UserCount * 2 - signCount).ToString();
                            }
                            else
                            {
                                var dayCount = _WorkDateList.Count;
                                model.NoSignCount = (dayCount * model.UserCount * 2).ToString();
                            }
                            view.Add(model);
                        }
                        return view;
                    }
                case PunchStatisticalEnum.AllHuman:
                    {
                        var dt1 = Adapter.PunchManagement.PunchManagementAdapter.Instance.GetListByManagerCode(_CurrentUserCode);
                        var dt2 = Adapter.Sign.EmployeeServicesAdapter.Instance.GetPunchStatistical(PunchStatisticalDataEnum.ManagerUserCode.ToString(), _CurrentUserCode, _WorkDays);

                        dt1.PrimaryKey = new DataColumn[] { dt1.Columns["Name"] };
                        dt2.PrimaryKey = new DataColumn[] { dt2.Columns["Name"] };

                        dt1.Merge(dt2);

                        var view = new List<ViewsModel.Sign.SignStatisticsPlusViewModel>();
                        foreach (DataRow dr in dt1.Rows)
                        {
                            var model = new ViewsModel.Sign.SignStatisticsPlusViewModel
                            {
                                Code = dr["Code"].ToString(),
                                Name = dr["Name"].ToString(),
                                Type = "Management",
                                IsCustomize = (int)dr["Type"] == 1 ? true : false,
                                UserCount = 0,
                                AvgHour = "--",
                                LateCount = "--",
                                EarlyCount = "--",
                                UnusualCount = "--",
                                NoSignCount = "--",
                                DefaultCode = dr["Default"].ToString().Split('|')[0],
                                DefaultType = dr["Default"].ToString().Split('|')[1],
                            };
                            if (dr["UserCount"] != DBNull.Value)
                            {
                                model.UserCount = (int)dr["UserCount"];
                            }
                            if (dr["AvgMinute"] != DBNull.Value)
                            {
                                var minute = Convert.ToDouble(dr["AvgMinute"]);
                                if (minute > 0)
                                {
                                    model.AvgHour = $"{(minute / 60).ToString("0.0")}";
                                }
                            }
                            if (dr["LateCount"] != DBNull.Value)
                            {
                                model.LateCount = dr["LateCount"].ToString();
                            }
                            if (dr["EarlyCount"] != DBNull.Value)
                            {
                                model.EarlyCount = dr["EarlyCount"].ToString();
                            }
                            if (dr["UnusualCount"] != DBNull.Value)
                            {
                                model.UnusualCount = dr["UnusualCount"].ToString();
                            }
                            if (dr["SignCount"] != DBNull.Value)
                            {
                                var dayCount = _WorkDateList.Count;
                                var signCount = Convert.ToInt32(dr["SignCount"]);
                                model.NoSignCount = (dayCount * model.UserCount * 2 - signCount).ToString();
                            }
                            else
                            {
                                var dayCount = _WorkDateList.Count;
                                model.NoSignCount = (dayCount * model.UserCount * 2).ToString();
                            }
                            view.Add(model);
                        }
                        return view;
                    }
                case PunchStatisticalEnum.ManagementCode:
                    {
                        var dt1 = Adapter.PunchManagement.PunchDepartmentAdapter.Instance.GetListByManagementCode(code);
                        var dt2 = Adapter.Sign.EmployeeServicesAdapter.Instance.GetPunchStatistical(PunchStatisticalDataEnum.ManagementCode.ToString(), code, _WorkDays);

                        dt1.PrimaryKey = new DataColumn[] { dt1.Columns["Name"] };
                        dt2.PrimaryKey = new DataColumn[] { dt2.Columns["Name"] };

                        dt1.Merge(dt2);

                        var view = new List<ViewsModel.Sign.SignStatisticsPlusViewModel>();
                        foreach (DataRow dr in dt1.Rows)
                        {
                            var model = new ViewsModel.Sign.SignStatisticsPlusViewModel
                            {
                                Code = dr["ConcatCode"].ToString(),
                                Name = dr["Name"].ToString(),
                                Type = dr["Type"].ToString() == "Organizations" ? "Organization" : dr["Type"].ToString() == "Users" ? "User" : "",
                                IsCustomize = false,
                                UserCount = 0,
                                AvgHour = "--",
                                LateCount = "--",
                                EarlyCount = "--",
                                UnusualCount = "--",
                                NoSignCount = "--",
                            };
                            if (dr["UserCount"] != DBNull.Value)
                            {
                                model.UserCount = (int)dr["UserCount"];
                            }
                            if (dr["AvgMinute"] != DBNull.Value)
                            {
                                var minute = Convert.ToDouble(dr["AvgMinute"]);
                                if (minute > 0)
                                {
                                    model.AvgHour = $"{(minute / 60).ToString("0.0")}";
                                }
                            }
                            if (dr["LateCount"] != DBNull.Value)
                            {
                                model.LateCount = dr["LateCount"].ToString();
                            }
                            if (dr["EarlyCount"] != DBNull.Value)
                            {
                                model.EarlyCount = dr["EarlyCount"].ToString();
                            }
                            if (dr["UnusualCount"] != DBNull.Value)
                            {
                                model.UnusualCount = dr["UnusualCount"].ToString();
                            }
                            if (dr["SignCount"] != DBNull.Value)
                            {
                                var dayCount = _WorkDateList.Count;
                                var signCount = Convert.ToInt32(dr["SignCount"]);
                                model.NoSignCount = (dayCount * model.UserCount * 2 - signCount).ToString();
                            }
                            else
                            {
                                var dayCount = _WorkDateList.Count;
                                model.NoSignCount = (dayCount * model.UserCount * 2).ToString();
                            }
                            view.Add(model);
                        }
                        return view;
                    }
                case PunchStatisticalEnum.OrganizationCode:
                    {
                        var dt1 = Adapter.AddressBook.ContactsAdapter.Instance.GetListByOrganizationCode(code);
                        var dt2 = Adapter.Sign.EmployeeServicesAdapter.Instance.GetPunchStatistical(PunchStatisticalDataEnum.OrganizationCode.ToString(), code, _WorkDays);

                        dt1.PrimaryKey = new DataColumn[] { dt1.Columns["Name"] };
                        dt2.PrimaryKey = new DataColumn[] { dt2.Columns["Name"] };

                        dt1.Merge(dt2);

                        var view = new List<ViewsModel.Sign.SignStatisticsPlusViewModel>();
                        foreach (DataRow dr in dt1.Rows)
                        {
                            var model = new ViewsModel.Sign.SignStatisticsPlusViewModel
                            {
                                Code = dr["ObjectID"].ToString(),
                                Name = dr["DisplayName"].ToString(),
                                Type = dr["SchemaType"].ToString() == "Organizations" ? "Organization" : dr["SchemaType"].ToString() == "Users" ? "User" : "",
                                IsCustomize = false,
                                UserCount = 0,
                                AvgHour = "--",
                                LateCount = "--",
                                EarlyCount = "--",
                                UnusualCount = "--",
                                NoSignCount = "--",
                            };
                            if (dr["UserCount"] != DBNull.Value)
                            {
                                model.UserCount = (int)dr["UserCount"];
                            }
                            if (dr["AvgMinute"] != DBNull.Value)
                            {
                                var minute = Convert.ToDouble(dr["AvgMinute"]);
                                if (minute > 0)
                                {
                                    model.AvgHour = $"{(minute / 60).ToString("0.0")}";
                                }
                            }
                            if (dr["LateCount"] != DBNull.Value)
                            {
                                model.LateCount = dr["LateCount"].ToString();
                            }
                            if (dr["EarlyCount"] != DBNull.Value)
                            {
                                model.EarlyCount = dr["EarlyCount"].ToString();
                            }
                            if (dr["UnusualCount"] != DBNull.Value)
                            {
                                model.UnusualCount = dr["UnusualCount"].ToString();
                            }
                            if (dr["SignCount"] != DBNull.Value)
                            {
                                var dayCount = _WorkDateList.Count;
                                var signCount = Convert.ToInt32(dr["SignCount"]);
                                model.NoSignCount = (dayCount * model.UserCount * 2 - signCount).ToString();
                            }
                            else
                            {
                                var dayCount = _WorkDateList.Count;
                                model.NoSignCount = (dayCount * model.UserCount * 2).ToString();
                            }
                            view.Add(model);
                        }
                        return view;
                    }
                case PunchStatisticalEnum.UserCode:
                    {
                        var userInfo = Adapter.AddressBook.ContactsAdapter.Instance.LoadUserByCode(code);
                        var _list = new List<ViewsModel.Sign.SignDetailViewModel>();
                        var dt = Adapter.Sign.EmployeeServicesAdapter.Instance.GetListByDetail(code, _SearchTime1, _SearchTime2);
                        foreach (DataRow dr in dt.Rows)
                        {
                            var item = new ViewsModel.Sign.SignDetailViewModel();
                            item.EnName = userInfo.Logon_Name;
                            item.CnName = userInfo.DisplayName;
                            item.OrganizationName = userInfo.FullPath;
                            item.Address = dr["Address1"] + ((dr["Address1"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["Address1"].ToString()) || dr["Address2"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["Address2"].ToString())) ? "" : "；") + dr["Address2"];
                            item.PunchDate = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                            item.PunchTime1 = (dr["CreateTime1"] == DBNull.Value || dr["IsValid1"] == DBNull.Value || !Convert.ToBoolean(dr["IsValid1"])) ? "--" : Convert.ToDateTime(dr["CreateTime1"]).ToString("HH:mm");
                            item.PunchTime2 = (dr["CreateTime2"] == DBNull.Value || dr["IsValid2"] == DBNull.Value || !Convert.ToBoolean(dr["IsValid2"])) ? "--" : Convert.ToDateTime(dr["CreateTime2"]).ToString("HH:mm");
                            item.WorkHour = "--";
                            if (dr["Minute"] != DBNull.Value)
                            {
                                var minute = Convert.ToDouble(dr["Minute"]);
                                if (minute > 0)
                                {
                                    item.WorkHour = $"{(minute / 60).ToString("0.0")}";
                                }
                            }
                            item.IsLate = "--";
                            if (dr["IsLate"] != DBNull.Value)
                            {
                                item.IsLate = Convert.ToInt16(dr["IsLate"]).ToString();
                            }
                            item.IsEarly = "--";
                            if (dr["IsEarly"] != DBNull.Value)
                            {
                                item.IsEarly = Convert.ToInt16(dr["IsEarly"]).ToString();
                            }
                            item.NoSign = dr["NoSign"].ToString();
                            item.UnusualType = dr["UnusualType1"] + ((dr["UnusualType1"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualType1"].ToString()) || dr["UnusualType2"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualType2"].ToString())) ? "" : "；") + dr["UnusualType2"];
                            item.UnusualDesc = dr["UnusualDesc1"] + ((dr["UnusualDesc1"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualDesc1"].ToString()) || dr["UnusualDesc2"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualDesc2"].ToString())) ? "" : "；") + dr["UnusualDesc2"];
                            _list.Add(item);
                        }
                        return _list;
                    }
            }
            return null;
        }

        /// <summary>
        /// 打卡记录
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="code">编码</param>
        public object GetRecord(PunchStatisticalEnum type, string code)
        {
            var dt = Adapter.Sign.EmployeeServicesAdapter.Instance.GetPunchRecord(type.ToString(), code, _SearchTime1, _SearchTime2);
            if (dt == null) throw new Exception("暂无明细数据！");
            var _list = new List<ViewsModel.Sign.SignDetailViewModel>();
            foreach (DataRow dr in dt.Rows)
            {
                var item = new ViewsModel.Sign.SignDetailViewModel();
                item.EnName = dr["EnName"].ToString();
                item.CnName = dr["CnName"].ToString();
                item.OrganizationName = string.Empty;
                item.Address = dr["Address1"] + ((dr["Address1"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["Address1"].ToString()) || dr["Address2"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["Address2"].ToString())) ? "" : "；") + dr["Address2"];
                item.PunchDate = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                item.PunchTime1 = (dr["CreateTime1"] == DBNull.Value || dr["IsValid1"] == DBNull.Value || !Convert.ToBoolean(dr["IsValid1"])) ? "--" : Convert.ToDateTime(dr["CreateTime1"]).ToString("HH:mm");
                item.PunchTime2 = (dr["CreateTime2"] == DBNull.Value || dr["IsValid2"] == DBNull.Value || !Convert.ToBoolean(dr["IsValid2"])) ? "--" : Convert.ToDateTime(dr["CreateTime2"]).ToString("HH:mm");
                item.WorkHour = "--";
                if (dr["Minute"] != DBNull.Value)
                {
                    var minute = Convert.ToDouble(dr["Minute"]);
                    if (minute > 0)
                    {
                        item.WorkHour = $"{(minute / 60).ToString("0.0")}";
                    }
                }
                item.IsLate = "--";
                if (dr["IsLate"] != DBNull.Value)
                {
                    item.IsLate = Convert.ToInt16(dr["IsLate"]).ToString();
                }
                item.IsEarly = "--";
                if (dr["IsEarly"] != DBNull.Value)
                {
                    item.IsEarly = Convert.ToInt16(dr["IsEarly"]).ToString();
                }
                item.NoSign = dr["NoSign"].ToString();
                item.UnusualType = dr["UnusualType1"] + ((dr["UnusualType1"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualType1"].ToString()) || dr["UnusualType2"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualType2"].ToString())) ? "" : "；") + dr["UnusualType2"];
                item.UnusualDesc = dr["UnusualDesc1"] + ((dr["UnusualDesc1"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualDesc1"].ToString()) || dr["UnusualDesc2"] == DBNull.Value || string.IsNullOrWhiteSpace(dr["UnusualDesc2"].ToString())) ? "" : "；") + dr["UnusualDesc2"];
                _list.Add(item);
            }
            return _list;
        }

        #region private

        /// <summary>
        /// 获取指定时间段内的工作日，调用远洋企业日历
        /// </summary>
        void GetWorkDateList(DateTime date1, DateTime date2, string authorizationStr)
        {
            _WorkDateList = new List<DateTime>();
            try
            {
                var ApiUrl = System.Configuration.ConfigurationManager.AppSettings["OceanCalendarApiUrl"];
                var json = HttpService.Get($"{ApiUrl}{date1.Year}", authorizationStr).Result;
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<GetWorkDateListModel>(json);
                for (var i = date1; i <= date2; i = i.AddDays(1))
                {
                    var date = response.Data.Where(w => w.StartDate <= i && i <= w.EndDate).FirstOrDefault();
                    if (date == null)
                    {
                        if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                        {
                            _WorkDateList.Add(i);
                        }
                    }
                    else
                    {
                        if (date.IsWorkDay)
                        {
                            _WorkDateList.Add(i);
                        }
                    }
                }
                
            }
            catch  
            {
                
            }
        }




        #endregion private
    }

    class GetWorkDateListModel
    {
        public bool State { set; get; }
        public string Message { set; get; }

        public List<GetWorkDateListSubModel> Data { set; get; }
    }

    class GetWorkDateListSubModel
    {
        public string DateTypeName { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime EndDate { set; get; }
        public bool IsWorkDay { set; get; }
    }

    /// <summary>
    /// 打卡统计类型
    /// </summary>
    public enum PunchStatisticalEnum
    {
        /// <summary>
        /// 所有打卡管理单元（后台）
        /// </summary>
        AllManage,
        /// <summary>
        /// 所有打卡管理单元（人力）
        /// </summary>
        AllHuman,
        /// <summary>
        /// 根据打卡管理单元编码
        /// </summary>
        ManagementCode,
        /// <summary>
        /// 根据组织机构编码
        /// </summary>
        OrganizationCode,
        /// <summary>
        /// 根据人员编码
        /// </summary>
        UserCode,
    }

    /// <summary>
    /// 打卡统计数据类型
    /// </summary>
    enum PunchStatisticalDataEnum
    {
        /// <summary>
        /// 根据配置人员编码统计各个打卡单元的打卡信息
        /// </summary>
        SettingUserCode,
        /// <summary>
        /// 根据管理人员编码统计各个打卡单元的打卡信息
        /// </summary>
        ManagerUserCode,
        /// <summary>
        /// 根据打卡单元编码统计该打卡单元下的各个组织或员工的打卡信息
        /// </summary>
        ManagementCode,
        /// <summary>
        /// 根据组织机构编码统计该组织机构下的各个子组织或直属员工的打卡信息
        /// </summary>
        OrganizationCode,
        /// <summary>
        /// 根据用户编码统计该用户的打卡详情
        /// </summary>
        UserCode,
    }
}
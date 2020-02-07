using MCS.Library.OGUPermission;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.Sign;
using Seagull2.YuanXin.AppApi.Domain.Sign;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Extensions;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.Models.Sign;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Sign;
using Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 打卡控制器
    /// </summary>
    public class SignController : ApiController
    {

        /// <summary>
        /// 缓存工作日数据
        /// </summary>
        static PunchStatistical domain;

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
                    IsValid = standard.IsValid
                });
            }
            StandardPunchAdapater.Instance.Dispose();
            return Ok(new BaseView
            {
                State = true,
                Message = "保存成功！"
            });
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

        #region 获取打卡地点列表
        /// <summary>
        /// 获取打卡地点列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="signAddress">地点名称</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetStandardByAddressList(int pageIndex, string signAddress = "")
        {
            ViewPageBase<List<StandardViewModel>> list = new ViewPageBase<List<StandardViewModel>>();
            ControllerHelp.SelectAction(() =>
            {
                list = StandardViewModelAdapter.Instance.GetStandardDataViewByPage(pageIndex, DateTime.Now, signAddress);
            });

            return Ok(list);
        }
        #endregion

        #region 删除打卡地点
        /// <summary>
        /// 删除打卡地点
        /// </summary>
        [HttpGet]
        public IHttpActionResult DeleteStandard(string code)
        {
            var result = ControllerService.Run(() =>
            {
                StandardPunchAdapater.Instance.Delete(w => w.AppendItem("Code", code));
            });
            return Ok(result);
        }
        #endregion

        /// <summary>
        /// 查询考勤统计信息
        /// </summary>
        [HttpPost]
        public IHttpActionResult GetList(List<string> codeList, DateTime date1, DateTime date2, string parentCode = "")
        {
            var result = ControllerService.Run(() =>
            {
                var _nav = new List<BaseViewCodeName>();
                var _organ = new List<SignStatisticsViewModel>();
                var _users = new List<SignStatisticsViewModel>();
                var _detail = new List<SignDetailViewModel>();

                var orgCodeList = new List<string>();

                var orgaList = new ContactsCollection();
                var userList = new ContactsCollection();

                if (string.IsNullOrWhiteSpace(parentCode))
                {
                    if (codeList == null || codeList.Count < 1)
                    {
                        throw new Exception("请传入权限信息！");
                    }
                    // 无导航

                    // 组织编码列表
                    orgaList = ContactsAdapter.Instance.LoadListByUserCodeList(codeList);
                    orgaList.ForEach(item =>
                    {
                        orgCodeList.Add(item.ObjectID);
                    });
                    if (orgCodeList.Count < 1)
                    {
                        throw new Exception("传入的权限信息有误！");
                    }
                    // 无人员列表
                }
                else
                {
                    // 导航
                    ContactsAdapter.Instance.LoadPathListForParent(parentCode).ForEach(item =>
                    {
                        _nav.Add(new BaseViewCodeName { Code = item.ObjectID, Name = item.DisplayName });
                    });
                    if (_nav.Count > 0)
                    {
                        _nav.RemoveAt(0);
                    }
                    if (codeList != null && codeList.Count > 0) // 根据权限过滤导航
                    {
                        var index = 0;
                        var flag = false;
                        _nav.ForEach(n =>
                        {
                            var find = codeList.Find(f => f == n.Code);
                            if (find != null)
                            {
                                index = _nav.IndexOf(n);
                                flag = true;
                                return;
                            }
                        });
                        if (flag == false)
                        {
                            throw new Exception("您暂无权限查看此信息！");
                        }
                        _nav.RemoveRange(0, index);
                    }
                    // 组织编码列表
                    orgaList = ContactsAdapter.Instance.LoadOrganizations(parentCode);
                    orgaList.ForEach(item =>
                    {
                        orgCodeList.Add(item.ObjectID);
                    });
                    // 组织下的人员列表
                    userList = ContactsAdapter.Instance.LoadUsers(parentCode);
                }

                // 按组织统计
                if (orgCodeList.Count > 0)
                {
                    _organ = SignStatisticsByOrganizations(orgCodeList, date1, date2, orgaList);
                }

                // 按人员统计
                if (userList.Count > 0 && !string.IsNullOrWhiteSpace(parentCode))
                {
                    _users = SignStatisticsByUsers(parentCode, date1, date2, userList);
                }

                // 人员明细
                var userInfo = ContactsAdapter.Instance.LoadUserByCode(parentCode);
                if (userInfo != null && !string.IsNullOrWhiteSpace(parentCode))
                {
                    _detail = SignUserDetail(parentCode, date1, date2, userInfo);
                }

                return new
                {
                    Nav = _nav,
                    Organization = _organ,
                    Users = _users,
                    Detail = _detail
                };
            });
            return Ok(result);
        }

        /// <summary>
        /// 查询考勤统计信息 Plus
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetPunchStatistical(DateTime date1, DateTime date2, Domain.Sign.PunchStatisticalEnum type, string code = "")
        {
            return Ok(ControllerService.Run(() =>
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                if (date1.Year != date2.Year)
                {
                    throw new Exception("请勿跨年查询！");
                }

                var domain = new Domain.Sign.PunchStatistical(date1, date2, authStr, userCode);

                var dt = domain.GetStatistical(type, code);

                return dt;
            }));
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        [HttpPost]
        public HttpResponseMessage ExportExcel(List<string> codeList, DateTime date1, DateTime date2, string parentCode = "")
        {
            try
            {
                var _organ = new List<SignStatisticsViewModel>();
                var _users = new List<SignStatisticsViewModel>();
                var _detail = new List<SignDetailViewModel>();

                var orgCodeList = new List<string>();

                var orgaList = new ContactsCollection();
                var userList = new ContactsCollection();

                if (string.IsNullOrWhiteSpace(parentCode))
                {
                    if (codeList == null || codeList.Count < 1)
                    {
                        throw new Exception("请传入权限信息！");
                    }

                    // 组织编码列表
                    orgaList = ContactsAdapter.Instance.LoadListByUserCodeList(codeList);
                    orgaList.ForEach(item =>
                    {
                        orgCodeList.Add(item.ObjectID);
                    });
                    if (orgCodeList.Count < 1)
                    {
                        throw new Exception("传入的权限信息有误！");
                    }
                    // 无人员列表
                }
                else
                {
                    // 组织编码列表
                    orgaList = ContactsAdapter.Instance.LoadOrganizations(parentCode);
                    orgaList.ForEach(item =>
                    {
                        orgCodeList.Add(item.ObjectID);
                    });
                    // 组织下的人员列表
                    userList = ContactsAdapter.Instance.LoadUsers(parentCode);
                }

                var excel = new ExportExcelService();

                // 按组织统计
                if (orgCodeList.Count > 0)
                {
                    _organ = SignStatisticsByOrganizations(orgCodeList, date1, date2, orgaList);
                    var column = new Dictionary<string, string>() {
                        {"组织名称","Name" },
                        {"日平均工作时长（小时）","AvgHour" },
                        {"迟到次数","LateCount" },
                        {"早退次数","EarlyCount" },
                        {"未签次数","NoSignCount" },
                        {"异常次数","UnusualCount" }
                    };
                    excel.CreateSheetForSign(date1, date2, "考勤单元统计", column, _organ);
                }

                // 按人员统计
                if (userList.Count > 0 && !string.IsNullOrWhiteSpace(parentCode))
                {
                    _users = SignStatisticsByUsers(parentCode, date1, date2, userList);
                    var column = new Dictionary<string, string>() {
                        {"人员名称","Name" },
                        {"日平均工作时长（小时）","AvgHour" },
                        {"迟到次数","LateCount" },
                        {"早退次数","EarlyCount" },
                        {"未签次数","NoSignCount" },
                        {"异常次数","UnusualCount" }
                    };
                    excel.CreateSheetForSign(date1, date2, "个人统计", column, _users);
                }

                // 人员明细
                var userInfo = ContactsAdapter.Instance.LoadUserByCode(parentCode);
                if (userInfo != null && !string.IsNullOrWhiteSpace(parentCode))
                {
                    _detail = SignUserDetail(parentCode, date1, date2, userInfo);
                    var column = new Dictionary<string, string>() {
                        {"人员名称","CnName" },
                        {"域帐号","EnName" },
                        {"打卡地点","Address" },
                        {"日期","PunchDate" },
                        {"上班打卡时间","PunchTime1" },
                        {"下班打卡时间","PunchTime2" },
                        {"工作时长（小时）","WorkHour" },
                        {"迟到","IsLate" },
                        {"早退","IsEarly" },
                        {"未签","NoSign" },
                        {"异常原因","UnusualType" },
                        {"异常说明","UnusualDesc" }
                    };
                    excel.CreateSheetForSign(date1, date2, "个人明细", column, _detail);
                }

                var file = excel.GetFileStream();

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(file);
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "员工考勤统计.xls";
                return result;
            }
            catch (Exception e)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                result.Content = new StringContent(e.Message);
                return result;
            }
        }

        /// <summary>
        /// 导出Excel Plus  可以导出未签到的数据
        /// </summary>
        [HttpGet]
        public HttpResponseMessage ExportPunchStatistical(DateTime date1, DateTime date2, Domain.Sign.PunchStatisticalEnum type, string code = "")
        {
            try
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                if (date1.Year != date2.Year)
                {
                    throw new Exception("请勿跨年查询！");
                }

                var domain = new Domain.Sign.PunchStatistical(date1, date2, authStr, userCode);

                var dt = domain.GetStatistical(type, code);

                var excel = new ExportExcelService();

                // 统计
                if (type == Domain.Sign.PunchStatisticalEnum.AllManage ||
                    type == Domain.Sign.PunchStatisticalEnum.AllHuman ||
                    type == Domain.Sign.PunchStatisticalEnum.ManagementCode ||
                    type == Domain.Sign.PunchStatisticalEnum.OrganizationCode)
                {
                    var _statis = (List<SignStatisticsPlusViewModel>)dt;
                    var column = new Dictionary<string, string>() {
                        {"组织名称","Name" },
                        {"人数", "UserCount"},
                        {"日平均工作时长（小时）","AvgHour" },
                        {"迟到次数","LateCount" },
                        {"早退次数","EarlyCount" },
                        {"未签次数","NoSignCount" },
                        {"异常次数","UnusualCount" }
                    };
                    excel.CreateSheetForSign(date1, date2, "考勤单元统计", column, _statis);
                }

                // 人员明细
                if (type == Domain.Sign.PunchStatisticalEnum.UserCode)
                {
                    var _detail = (List<SignDetailViewModel>)dt;
                    var column = new Dictionary<string, string>() {
                        {"人员名称","CnName" },
                        {"域帐号","EnName" },
                        {"打卡地点","Address" },
                        {"日期","PunchDate" },
                        {"上班打卡时间","PunchTime1" },
                        {"下班打卡时间","PunchTime2" },
                        {"工作时长（小时）","WorkHour" },
                        {"迟到","IsLate" },
                        {"早退","IsEarly" },
                        {"未签","NoSign" },
                        {"异常原因","UnusualType" },
                        {"异常说明","UnusualDesc" }
                    };
                    excel.CreateSheetForSign(date1, date2, "打卡明细", column, _detail);
                }

                var file = excel.GetFileStream();

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(file);
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "员工考勤统计.xls";
                ControllerService.UploadLog(userCode, "导出应用管理-考勤打卡-员工考勤-打卡数据");
                return result;
            }
            catch (Exception e)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                result.Content = new StringContent(e.Message);
                return result;
            }
        }

        /// <summary>
        /// 导出Excel Plus 打卡明细
        /// </summary>
        [HttpGet]
        public HttpResponseMessage ExportPunchRecord(DateTime date1, DateTime date2, Domain.Sign.PunchStatisticalEnum type, string code)
        {
            try
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";

                if (date1.Year != date2.Year)
                {
                    throw new Exception("请勿跨年查询！");
                }

                var domain = new Domain.Sign.PunchStatistical(date1, date2, authStr, userCode);

                var dt = domain.GetRecord(type, code);
                var _detail = (List<SignDetailViewModel>)dt;
                if (_detail.Count < 1)
                {
                    throw new Exception("暂无明细数据！");
                }

                var excel = new ExportExcelService();

                var column = new Dictionary<string, string>() {
                    {"人员名称","CnName" },
                    {"域帐号","EnName" },
                    {"打卡地点","Address" },
                    {"日期","PunchDate" },
                    {"上班打卡时间","PunchTime1" },
                    {"下班打卡时间","PunchTime2" },
                    {"工作时长（小时）","WorkHour" },
                    {"迟到","IsLate" },
                    {"早退","IsEarly" },
                    {"未签","NoSign" },
                    {"异常原因","UnusualType" },
                    {"异常说明","UnusualDesc" }
                };
                excel.CreateSheetForSign(date1, date2, "打卡明细", column, _detail);

                var file = excel.GetFileStream();

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(file);
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.ms-excel");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "员工考勤统计.xls";
                ControllerService.UploadLog(userCode, "导出应用管理-考勤打卡-员工考勤-打卡数据明细");
                return result;
            }
            catch (Exception e)
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                result.Content = new StringContent(e.Message);
                return result;
            }
        }

        /// <summary>
        /// 获取搜索选项列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetSearchList(Domain.Sign.PunchStatisticalEnum type, string keyword)
        {
            return Ok(ControllerService.Run(() =>
            {
                var userCode = ((Seagull2Identity)User.Identity).Id;

                var ds = new DataSet();

                if (type == Domain.Sign.PunchStatisticalEnum.AllManage)
                {
                    ds = Adapter.PunchManagement.PunchManagementAdapter.Instance.SearchUserListByKeyword(Domain.Sign.PunchStatisticalDataEnum.SettingUserCode.ToString(), userCode, keyword);
                }
                if (type == Domain.Sign.PunchStatisticalEnum.AllHuman)
                {
                    ds = Adapter.PunchManagement.PunchManagementAdapter.Instance.SearchUserListByKeyword(Domain.Sign.PunchStatisticalDataEnum.ManagerUserCode.ToString(), userCode, keyword);
                }

                var view = new List<GetSearchListViewModel>();
                // 打卡管理单元搜索结果
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    view.Add(new GetSearchListViewModel
                    {
                        Type = "Management",
                        Code = dr["Code"].ToString(),
                        Name = dr["Name"].ToString(),
                    });
                }
                // 用户搜索结果
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    view.Add(new GetSearchListViewModel
                    {
                        Type = "User",
                        Code = dr["ObjectID"].ToString(),
                        Name = dr["DisplayName"].ToString(),
                    });
                }
                // 搜索一个用户信息
                var users = ContactsAdapter.Instance.LoadUserByKeyword(keyword);
                if (users.Count > 0)
                {
                    // 取当前用户的权限信息
                    var auth = Adapter.Sys_Menu.Sys_UserAdapter.Instance.GetModelByUserCode(userCode);
                    if (auth != null && auth.IsEnabled)
                    {
                        // 用户所在的所有打卡管理单元
                        var manages = Adapter.PunchManagement.PunchManagementAdapter.Instance.GetListByUserCode(users[0].ObjectID);

                        var newManages = new List<Models.PunchManagement.PunchManagementModel>();

                        // 后台发出的请求
                        if (type == Domain.Sign.PunchStatisticalEnum.AllManage)
                        {
                            // 超管 返回所有打卡管理单元  ****************************************
                            if (auth.Super || auth.IsPunchSuper)
                            {
                                newManages = manages.ToList();
                            }

                            // 分管 返回自己创建的打卡管理单元  ****************************************
                            else
                            {
                                newManages = manages.Where(w => w.Creator == userCode).ToList();
                            }
                        }

                        // 员工考勤单页发出的请求
                        if (type == Domain.Sign.PunchStatisticalEnum.AllHuman)
                        {
                            // ****************************************  考勤员 返回自己有考勤权限的打卡管理单元  ****************************************
                            var myManages = DataConvertHelper<Models.PunchManagement.PunchManagementModel>.ConvertToList(Adapter.PunchManagement.PunchManagementAdapter.Instance.GetListByManagerCode(userCode));

                            newManages = myManages.Where(w => manages.Find(f => f.Code == w.Code) != null).ToList();
                        }

                        newManages.ForEach(manage =>
                        {
                            // 取打卡管理单元下的人员或部门列表
                            var departs = Adapter.PunchManagement.PunchDepartmentAdapter.Instance.GetList(manage.Code);
                            departs.ForEach(depart =>
                            {
                                if (depart.Type == "Users")
                                {
                                    if (depart.ConcatCode == users[0].ObjectID)
                                    {
                                        view.Add(new GetSearchListViewModel
                                        {
                                            Type = "Management",
                                            Code = manage.Code,
                                            Name = manage.Name
                                        });
                                    }
                                }
                                else
                                {
                                    users.ForEach(user =>
                                    {
                                        // 通过部门编码判断该部门下是否存在该用户
                                        var _departs = ContactsAdapter.Instance.GetParentDepartListByDepartCode(user.ParentID);
                                        var _depart = _departs.Find(w => w.ObjectID == depart.ConcatCode);
                                        if (_depart != null)
                                        {
                                            var arr = user.FullPath.Split('\\');
                                            if (arr.Length >= 2)
                                            {
                                                view.Add(new GetSearchListViewModel
                                                {
                                                    Type = "Organization",
                                                    Code = user.ParentID,
                                                    Name = arr[arr.Length - 2]
                                                });
                                            }
                                        }
                                    });
                                }
                            });
                        });
                    }
                }
                // 去重
                view = view.Where((x, i) => view.FindIndex(z => z.Code == x.Code) == i).ToList();

                return view;
            }));
        }


        // ---------- APP ----------

        #region APP

        /// <summary>
        /// 打卡
        /// </summary>
        [HttpPost]
        public IHttpActionResult Punch(SignViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(post.Address) || string.IsNullOrWhiteSpace(post.Lat) || string.IsNullOrWhiteSpace(post.Lng))
                {
                    throw new Exception("打卡地点及经纬度参数不能为空。");
                }
                var timeLine = ConfigAppSetting.SignTimeLine.Split(',');
                var timeLine1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[0]);
                var timeLine2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[1]);
                var punchType = PunchType(timeLine1, timeLine2, out DateTime punchDate);
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var isPunch = EmployeeServicesAdapter.Instance.IsPunchToday(userCode);
                if (post.IsQuickPunch)  // 对极速打卡的一些判断
                {
                    // 用户是否开启了极速打卡
                    var setting = PunchQuickSettingAdapter.Instance.GetModelByUserCode(userCode);
                    if (setting == null || !setting.IsEnable)
                    {
                        throw new Exception("用户暂未开启极速打卡！");
                    }
                    // 是否在打卡时间范围
                    var date = DateTime.Now;
                    var date1 = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " 06:30:00");
                    var date2 = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " 12:00:00");
                    var isAllow = date >= date1 && date <= date2;
                    if (!isAllow)
                    {
                        throw new Exception("不在极速打卡时间范围！");
                    }
                    // 用户是否已经签到 
                    if (isPunch)
                    {
                        throw new Exception("用户已经打过卡了！");
                    }
                }
                // 用户是否已经签到  
                if (isPunch && punchType == 0)
                {
                    return new Temp
                    {
                        isRedirect = false,
                        msg = "已打上班卡，若要更新打卡信息，请在“我的考勤-更新打卡”操作"
                    };
                }
                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCode)[0];
                // 正常
                if (!post.IsUnusual)
                {
                    Models.PunchManagement.PunchManagementModel punchManagementModel = null;
                    // 组织机构全路径(由下到上)，遍历取到上下班时间等配置信息
                    var paths = ContactsAdapter.Instance.LoadPathListForParent(user.ID).OrderByDescending(o => o.GlobalSort).ToList();
                    if (paths.Count < 1)
                    {
                        throw new Exception("组织机构信息错误。");
                    }
                    var flag = true;
                    var index = 0;
                    while (flag)
                    {
                        var path = paths[index];
                        var managements = Adapter.PunchManagement.PunchManagementAdapter.Instance.GetPunchManagementListByContactCode(path.ObjectID);
                        if (managements.Count > 0)
                        {
                            punchManagementModel = managements.First();
                            flag = false;
                        }
                        index++;
                        if (index > paths.Count - 1)
                        {
                            flag = false;
                        }
                    }
                    if (punchManagementModel == null)
                    {
                        throw new Exception("没有获取到上下班时间配置信息，请联系管理员。");
                    }
                    if (string.IsNullOrWhiteSpace(punchManagementModel.OnTime) || string.IsNullOrWhiteSpace(punchManagementModel.OffTime) || punchManagementModel.PunchArea == 0)
                    {
                        throw new Exception($"打卡管理单元配置信息错误，请联系管理员[{punchManagementModel.Name}]。");
                    }

                    // 获取距离当前经纬度距离最近的打卡地点
                    var standards = StandardPunchAdapater.Instance.GetMinSingle(Convert.ToDecimal(post.Lng), Convert.ToDecimal(post.Lat), punchManagementModel.PunchArea);
                    if (standards.Count < 1)
                    {
                        throw new Exception("[NotInRange]不在打卡范围。");
                    }

                    var _model = new EmployeeServicesModel();
                    _model.Code = Guid.NewGuid().ToString();
                    _model.CnName = user.DisplayName;
                    _model.EnName = user.LogOnName;
                    _model.OrganizationCode = user.Parent.ID;
                    _model.FullPath = user.FullPath;
                    _model.PunchDate = punchDate;
                    _model.PunchType = punchType;
                    _model.MapUrl = post.Address;
                    _model.Lat = post.Lat;
                    _model.Lng = post.Lng;
                    _model.StandardPunchCode = standards[0].Code;
                    if (punchType == 0)
                    {
                        // 判断是否迟到
                        _model.IsLate = IsLate(punchManagementModel.OnTime, out int minute);
                        if (_model.IsLate == true)
                        {
                            _model.Minute = minute;
                        }
                    }
                    if (punchType == 1)
                    {
                        // 判断是否早退
                        _model.IsEarly = IsEarly(punchManagementModel.OffTime, out int minute, timeLine1);
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
                }
                // 异常
                if (post.IsUnusual)
                {
                    EmployeeServicesAdapter.Instance.Update(new EmployeeServicesModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        CnName = user.DisplayName,
                        EnName = user.LogOnName,
                        OrganizationCode = user.Parent.ID,
                        FullPath = user.FullPath,
                        PunchDate = punchDate,
                        PunchType = punchType,
                        MapUrl = post.Address,
                        Lat = post.Lat,
                        Lng = post.Lng,
                        IsUnusual = true,
                        UnusualType = post.UnusualType,
                        UnusualDesc = post.UnusualDesc,
                        Creator = user.ID,
                        CreateTime = DateTime.Now,
                        IsValid = true
                    });
                }

                return new Temp
                {
                    isRedirect = true,
                    CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                };
            });
            return Ok(result);
        }


        /// <summary>
        /// 打卡
        /// </summary>
        [HttpPost]
        public IHttpActionResult Punchold(SignViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(post.Address) || string.IsNullOrWhiteSpace(post.Lat) || string.IsNullOrWhiteSpace(post.Lng))
                {
                    throw new Exception("打卡地点及经纬度参数不能为空。");
                }

                var userCode = ((Seagull2Identity)User.Identity).Id;

                if (post.IsQuickPunch)  // 对极速打卡的一些判断
                {
                    // 用户是否开启了极速打卡
                    var setting = PunchQuickSettingAdapter.Instance.GetModelByUserCode(userCode);
                    if (setting == null || !setting.IsEnable)
                    {
                        throw new Exception("用户暂未开启极速打卡！");
                    }
                    // 是否在打卡时间范围
                    var date = DateTime.Now;
                    var date1 = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " 06:30:00");
                    var date2 = Convert.ToDateTime(date.ToString("yyyy-MM-dd") + " 12:00:00");
                    var isAllow = date >= date1 && date <= date2;
                    if (!isAllow)
                    {
                        throw new Exception("不在极速打卡时间范围！");
                    }
                    // 用户是否已经签到
                    var isPunch = EmployeeServicesAdapter.Instance.IsPunchToday(userCode);
                    if (isPunch)
                    {
                        throw new Exception("用户已经打过卡了！");
                    }
                }

                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, userCode)[0];
                var timeLine = ConfigAppSetting.SignTimeLine.Split(',');
                var timeLine1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[0]);
                var timeLine2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[1]);

                var punchType = PunchType(timeLine1, timeLine2, out DateTime punchDate);

                // 正常
                if (!post.IsUnusual)
                {
                    Models.PunchManagement.PunchManagementModel punchManagementModel = null;

                    // 组织机构全路径(由下到上)，遍历取到上下班时间等配置信息
                    var paths = ContactsAdapter.Instance.LoadPathListForParent(user.ID).OrderByDescending(o => o.GlobalSort).ToList();
                    if (paths.Count < 1)
                    {
                        throw new Exception("组织机构信息错误。");
                    }
                    var flag = true;
                    var index = 0;
                    while (flag)
                    {
                        var path = paths[index];
                        var managements = Adapter.PunchManagement.PunchManagementAdapter.Instance.GetPunchManagementListByContactCode(path.ObjectID);
                        if (managements.Count > 0)
                        {
                            punchManagementModel = managements.First();
                            flag = false;
                        }
                        index++;
                        if (index > paths.Count - 1)
                        {
                            flag = false;
                        }
                    }
                    if (punchManagementModel == null)
                    {
                        throw new Exception("没有获取到上下班时间配置信息，请联系管理员。");
                    }
                    if (string.IsNullOrWhiteSpace(punchManagementModel.OnTime) || string.IsNullOrWhiteSpace(punchManagementModel.OffTime) || punchManagementModel.PunchArea == 0)
                    {
                        throw new Exception($"打卡管理单元配置信息错误，请联系管理员[{punchManagementModel.Name}]。");
                    }

                    // 获取距离当前经纬度距离最近的打卡地点
                    var standards = StandardPunchAdapater.Instance.GetMinSingle(Convert.ToDecimal(post.Lng), Convert.ToDecimal(post.Lat), punchManagementModel.PunchArea);
                    if (standards.Count < 1)
                    {
                        throw new Exception("[NotInRange]不在打卡范围。");
                    }

                    var _model = new EmployeeServicesModel();
                    _model.Code = Guid.NewGuid().ToString();
                    _model.CnName = user.DisplayName;
                    _model.EnName = user.LogOnName;
                    _model.OrganizationCode = user.Parent.ID;
                    _model.FullPath = user.FullPath;
                    _model.PunchDate = punchDate;
                    _model.PunchType = punchType;
                    _model.MapUrl = post.Address;
                    _model.Lat = post.Lat;
                    _model.Lng = post.Lng;
                    _model.StandardPunchCode = standards[0].Code;
                    if (punchType == 0)
                    {
                        // 判断是否迟到
                        _model.IsLate = IsLate(punchManagementModel.OnTime, out int minute);
                        if (_model.IsLate == true)
                        {
                            _model.Minute = minute;
                        }
                    }
                    if (punchType == 1)
                    {
                        // 判断是否早退
                        _model.IsEarly = IsEarly(punchManagementModel.OffTime, out int minute, timeLine1);
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
                }
                // 异常
                if (post.IsUnusual)
                {
                    EmployeeServicesAdapter.Instance.Update(new EmployeeServicesModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        CnName = user.DisplayName,
                        EnName = user.LogOnName,
                        OrganizationCode = user.Parent.ID,
                        FullPath = user.FullPath,
                        PunchDate = punchDate,
                        PunchType = punchType,
                        MapUrl = post.Address,
                        Lat = post.Lat,
                        Lng = post.Lng,
                        IsUnusual = true,
                        UnusualType = post.UnusualType,
                        UnusualDesc = post.UnusualDesc,
                        Creator = user.ID,
                        CreateTime = DateTime.Now,
                        IsValid = true
                    });
                }
                return new
                {
                    CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                };
            });
            return Ok(result);
        }


        /// <summary>
        /// 更新打卡
        /// </summary>
        [HttpPost]
        public IHttpActionResult UpdatePunch(SignViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                
                var userCode = ((Seagull2Identity)User.Identity).Id;
                var oldMode = EmployeeServicesAdapter.Instance.Load(w => w.AppendItem("Code", post.Code)).FirstOrDefault();
                if (oldMode != null)
                {
                    DateTime isPunchDate = DateTime.Parse(oldMode.PunchDate.ToString("yyyy-MM-dd") + " 12:00:00");
                    if (DateTime.Now > isPunchDate) { throw new Exception("已过更新打卡时间"); }
                    var timeLine = ConfigAppSetting.SignTimeLine.Split(',');
                    var timeLine1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[0]);
                    var timeLine2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[1]);
                    var punchType = PunchType(timeLine1, timeLine2, out DateTime punchDate);
                    oldMode.Modifier = userCode;
                    oldMode.ModifyTime = DateTime.Now;
                    oldMode.PunchDate = punchDate;
                    oldMode.CreateTime = DateTime.Now;
                    oldMode.MapUrl = post.Address;
                    oldMode.Lat = post.Lat;
                    oldMode.Lng = post.Lng;
                    oldMode.IsUnusual = post.IsUnusual;
                    if (oldMode.IsUnusual)
                    {
                        oldMode.UnusualType = post.UnusualType;
                        oldMode.UnusualDesc = post.UnusualDesc;
                    }
                    else {
                        oldMode.UnusualType ="";
                        oldMode.UnusualDesc = "";
                    }

                    EmployeeServicesAdapter.Instance.Update(oldMode);
                }
                else
                {
                    throw new Exception("没有找到打卡信息！");
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 判断当前打卡人是否在范围内
        /// </summary>
        [HttpPost]
        public IHttpActionResult IsInRange(SignViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                bool isInRange = true;
                if (string.IsNullOrWhiteSpace(post.Lat) || string.IsNullOrWhiteSpace(post.Lng))
                {
                    throw new Exception("经纬度参数不能为空。");
                }
                var userCode = ((Seagull2Identity)User.Identity).Id;
                Models.PunchManagement.PunchManagementModel punchManagementModel = null;
                // 组织机构全路径(由下到上)，遍历取到上下班时间等配置信息
                var paths = ContactsAdapter.Instance.LoadPathListForParent(userCode).OrderByDescending(o => o.GlobalSort).ToList();
                if (paths.Count < 1)
                {
                    throw new Exception("组织机构信息错误。");
                }
                var flag = true;
                var index = 0;
                while (flag)
                {
                    var path = paths[index];
                    var managements = Adapter.PunchManagement.PunchManagementAdapter.Instance.GetPunchManagementListByContactCode(path.ObjectID);
                    if (managements.Count > 0)
                    {
                        punchManagementModel = managements.First();
                        flag = false;
                    }
                    index++;
                    if (index > paths.Count - 1)
                    {
                        flag = false;
                    }
                }
                if (punchManagementModel == null)
                {
                    throw new Exception("没有获取到上下班时间配置信息，请联系管理员。");
                }
                if (string.IsNullOrWhiteSpace(punchManagementModel.OnTime) || string.IsNullOrWhiteSpace(punchManagementModel.OffTime) || punchManagementModel.PunchArea == 0)
                {
                    throw new Exception($"打卡管理单元配置信息错误，请联系管理员[{punchManagementModel.Name}]。");
                }
                // 获取距离当前经纬度距离最近的打卡地点
                var standards = StandardPunchAdapater.Instance.GetMinSingle(Convert.ToDecimal(post.Lng), Convert.ToDecimal(post.Lat), punchManagementModel.PunchArea);
                if (standards.Count < 1)
                {
                    isInRange = false;
                }
                return isInRange;
            });
            return Ok(result);
        }



        /// <summary>
        /// 打卡 - 测试
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult PunchTest(SignViewModel post, string userName)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(post.Address) || string.IsNullOrWhiteSpace(post.Lat) || string.IsNullOrWhiteSpace(post.Lng))
                {
                    throw new Exception("打卡地点及经纬度参数不能为空。");
                }

                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, userName)[0];
                var timeLine = ConfigAppSetting.SignTimeLine.Split(',');
                var timeLine1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[0]);
                var timeLine2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + timeLine[1]);

                var punchType = PunchType(timeLine1, timeLine2, out DateTime punchDate);

                // 正常
                if (!post.IsUnusual)
                {
                    var standerdList = StandardPunchAdapater.Instance.LoadStandardList();
                    if (standerdList.Count < 1)
                    {
                        throw new Exception("没有获取到有效的打卡地点，请联系管理员。");
                    }

                    StandardPunchModel standardModel = null;
                    var distanceFlag = double.MaxValue;
                    foreach (StandardPunchModel item in standerdList)
                    {
                        var distance = CommonService.Distance(Convert.ToDouble(post.Lat), Convert.ToDouble(post.Lng), Convert.ToDouble(item.Lat), Convert.ToDouble(item.Lng));
                        if (distance < distanceFlag)
                        {
                            standardModel = item;
                            distanceFlag = distance;
                        }
                    }

                    if (distanceFlag >= 1)
                    {
                        throw new Exception("[NotInRange]请移步打卡地点方圆1公里范围以内打卡。");
                    }

                    var _model = new EmployeeServicesModel();
                    _model.Code = Guid.NewGuid().ToString();
                    _model.CnName = user.DisplayName;
                    _model.EnName = user.LogOnName;
                    _model.OrganizationCode = user.Parent.ID;
                    _model.FullPath = user.FullPath;
                    _model.PunchDate = punchDate;
                    _model.PunchType = punchType;
                    _model.MapUrl = post.Address;
                    _model.Lat = post.Lat;
                    _model.Lng = post.Lng;
                    _model.StandardPunchCode = standardModel.Code;
                    if (punchType == 0)
                    {
                        // 判断是否迟到
                        _model.IsLate = IsLate(standardModel.OnTime, out int minute);
                        if (_model.IsLate == true)
                        {
                            _model.Minute = minute;
                        }
                    }
                    if (punchType == 1)
                    {
                        // 判断是否早退
                        _model.IsEarly = IsEarly(standardModel.OffTime, out int minute, timeLine1);
                        if (_model.IsEarly == true)
                        {
                            _model.Minute = minute;
                        }
                    }
                    _model.IsUnusual = false;
                    _model.Creator = user.ID;
                    _model.CreateTime = DateTime.Now;
                    _model.IsValid = true;
                    //EmployeeServicesAdapter.Instance.Update(_model);
                }
                // 异常
                if (post.IsUnusual)
                {
                    //EmployeeServicesAdapter.Instance.Update(new EmployeeServicesModel()
                    //{
                    //    Code = Guid.NewGuid().ToString(),
                    //    CnName = user.DisplayName,
                    //    EnName = user.LogOnName,
                    //    OrganizationCode = user.Parent.ID,
                    //    FullPath = user.FullPath,
                    //    PunchDate = punchDate,
                    //    PunchType = punchType,
                    //    MapUrl = post.Address,
                    //    Lat = post.Lat,
                    //    Lng = post.Lng,
                    //    IsUnusual = true,
                    //    UnusualType = post.UnusualType,
                    //    UnusualDesc = post.UnusualDesc,
                    //    Creator = user.ID,
                    //    CreateTime = DateTime.Now,
                    //    IsValid = true
                    //});
                }
            });
            return Ok(result);
        }
         
        /// <summary>
        /// 查询当月考勤记录
        ///获取个人请假记录   打卡完成马上执行的接口
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetResultByMonth(DateTime month)
        {
            var result = ControllerService.Run(() =>
            {

                var day1 = CommonService.FirstDayOfMounth(month);     //当月的第一天
                var day2 = CommonService.LastDayOfMounth(month);      //当月最后一天 
                var user = (Seagull2Identity)User.Identity;
                var data = EmployeeServicesAdapter.Instance.GetResultByMonth(user.Id, month);
                var view = new List<SignResultByMonthViewModel>();
                for (var day = day1; day <= day2; day = day.AddDays(1))
                {
                    var curr = day.ToString("yyyy-MM-dd");
                    var row1 = data.Select("[PunchDate]='" + curr + "' AND [PunchType]=0");
                    var row2 = data.Select("[PunchDate]='" + curr + "' AND [PunchType]=1");
                    view.Add(new SignResultByMonthViewModel()
                    {
                        Day = curr,
                        AM = BuildData(row1),
                        PM = BuildData(row2)
                    });
                }
                return view;
            });
            return Ok(result);
        }

        /// <summary>
        /// 按月统计考勤记录  迟到 早退 未打卡  个人统计页面
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetResultByMonthv1(DateTime month)
        {
            var result = ControllerService.Run(() =>
            {
                List<UserReport> urs = new List<UserReport>();
                var user = (Seagull2Identity)User.Identity;
                DateTime start = CommonService.FirstDayOfMounth(month);     //当月的第一天
                DateTime end = CommonService.LastDayOfMounth(month);      //当月最后一天
                if (month.Month == DateTime.Now.Month)
                {
                    end = DateTime.Now;
                }
                var data = EmployeeServicesAdapter.Instance.GetResultByMonth(user.Id, month);
                string authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
                PunchStatistical punchStatistical = new PunchStatistical(start, end, authStr); ;//企业日历数据
                string curr = "";
                punchStatistical._WorkDateList.ForEach(f =>
                {
                    curr = f.ToString("yyyy-MM-dd");
                    var row1 = data.Select("[PunchDate]='" + curr + "' AND [PunchType]=0");
                    var row2 = data.Select("[PunchDate]='" + curr + "' AND [PunchType]=1");
                    var temp = new SignResultByMonthViewModel()
                    {
                        Day = curr,
                        AM = BuildData(row1),
                        PM = BuildData(row2)
                    };

                    if (temp.AM.IsPunch)
                    {
                        if (temp.AM.IsUnusual == true)
                        {
                            UserReport amur = new UserReport();
                            amur.pdate = curr;
                            amur.ptime = temp.AM.PunchTime;
                            amur.wd = Week(f.DayOfWeek);
                            if (string.IsNullOrEmpty(temp.AM.UnusualType))
                            {
                                amur.ut = "迟到";
                                amur.desc = "无";
                                urs.Add(amur);
                            }
                            else
                            {
                                amur.ut = temp.AM.UnusualType;
                                amur.desc = temp.AM.UnusualDesc;
                                urs.Add(amur);
                            }
                        }
                        else
                        {
                            if (temp.AM.IsRegular == true)
                            {
                                UserReport amur = new UserReport();
                                amur.pdate = curr;
                                amur.ptime = temp.AM.PunchTime;
                                amur.wd = Week(f.DayOfWeek);
                                amur.ut = "迟到";
                                amur.desc = "无";
                                urs.Add(amur);
                            }
                        }
                    }

                    if (temp.PM.IsPunch)
                    {
                        if (temp.PM.IsUnusual == true)
                        {
                            UserReport pmur = new UserReport();
                            pmur.pdate = curr;
                            pmur.ptime = temp.PM.PunchTime;
                            pmur.wd = Week(f.DayOfWeek);
                            if (string.IsNullOrEmpty(temp.PM.UnusualType))
                            {
                                pmur.ut = "早退";
                                pmur.desc = "无";
                                urs.Add(pmur);
                            }
                            else
                            {
                                pmur.ut = temp.PM.UnusualType;
                                pmur.desc = temp.PM.UnusualDesc;
                                urs.Add(pmur);
                            }
                        }
                        else
                        {
                            if (temp.PM.IsRegular == true)
                            {
                                UserReport amur = new UserReport();
                                amur.pdate = curr;
                                amur.ptime = temp.PM.PunchTime;
                                amur.wd = Week(f.DayOfWeek);
                                amur.ut = "早退";
                                amur.desc = "无";
                                urs.Add(amur);
                            }
                        }
                    }

                    UserReport ur = new UserReport();
                    ur.ut = "未打卡";
                    ur.pdate = curr;
                    ur.wd = Week(f.DayOfWeek);
                    ur.desc = "无";
                    if (!temp.AM.IsPunch && temp.PM.IsPunch)
                    {
                        ur.ptime = "未签到";
                        urs.Add(ur);
                    }
                    if (temp.AM.IsPunch && !temp.PM.IsPunch)
                    {
                        ur.ptime = "未签退";
                        urs.Add(ur);
                    }
                    if (!temp.AM.IsPunch && !temp.PM.IsPunch)
                    {
                        ur.ptime = "未签到 ,未签退";
                        urs.Add(ur);
                    }
                });
                List<object> pdata = new List<object>();
                urs.GroupBy(g => g.ut).ToList().ForEach(f =>
                {
                    pdata.Add(new
                    {
                        untype = f.Key,
                        data = f.ToList()
                    });
                });
                return pdata;
            });
            return Ok(result);

        }

        /// <summary>
        /// 获取当前登录人的请休假信息  某个时间段内的所有 工作日去除节假日
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="user"></param>
        /// <param name="authStr"></param>
        /// <param name="signData"></param>
        /// <returns></returns>
        private List<PunchInfoModel> GetCurrentUserLeaveInfo(DataTable signData, DateTime startDate, DateTime endDate, string user, string authStr)
        {
            string apiUrl = System.Configuration.ConfigurationManager.AppSettings["GetLevaveInfo"] + $"?startDate={startDate.ToString("yyyy-MM-dd")}&endDate={endDate.ToString("yyyy-MM-dd")}";
            //ServiceUtility.SetCertificatePolicy();// 临时删除掉
            LeaveInfo leaveInfo = null;
            List<PunchInfoModel> merageData = new List<PunchInfoModel>();
            using (HttpClient http = new HttpClient())
            {
                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Request.Headers.Authorization.Scheme, Request.Headers.Authorization.Parameter);
                try
                {
                    string json = http.GetStringAsync(apiUrl).Result;
                    leaveInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<LeaveInfo>(json);
                }
                catch (Exception ex)
                {
                    leaveInfo = new LeaveInfo();
                    leaveInfo.data = new List<Leave>();
                    leaveInfo.message = ex.Message;
                }
            }
            if (leaveInfo.data.Count > 0)
            {
                PunchStatistical punchStatistical = GetWorkDay(startDate, DateTime.Now, authStr);//企业日历数据

                leaveInfo.data.ForEach(p =>
                {
                    if (p.daysCount < 1)
                    {
                        PunchInfoModel half = new PunchInfoModel();
                        half.punchDate = p.startDate;
                        half.summary = p.summary;
                        half.attendenceItemName = p.attendenceItemName;
                        if (p.startTypeName == "上午")
                        {
                            half.AM = true;
                        }
                        else
                        {
                            half.PM = true;
                        }
                        merageData.Add(half);
                    }
                    if (p.daysCount == 1)
                    {
                        PunchInfoModel day = new PunchInfoModel();
                        day.punchDate = p.startDate;
                        day.summary = p.summary;
                        day.attendenceItemName = p.attendenceItemName;
                        if (p.startTypeName == "上午")
                        {
                            day.AM = true;
                            day.PM = true;
                            merageData.Add(day);
                        }
                        else
                        {
                            day.PM = true;
                            merageData.Add(day);
                            merageData.Add(new PunchInfoModel()
                            {
                                punchDate = p.endDate,
                                summary = p.summary,
                                attendenceItemName = p.attendenceItemName,
                                AM = true
                            });
                        }
                    }
                    if (p.daysCount > 1)
                    {
                        for (var i = p.startDate; i <= p.endDate; i = i.AddDays(1)) //1.5
                        {
                            if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday && punchStatistical._WorkDateList.Exists(e => e == i))
                            {
                                PunchInfoModel day = new PunchInfoModel()
                                {
                                    punchDate = i,
                                    summary = p.summary,
                                    attendenceItemName = p.attendenceItemName,
                                    AM = true,
                                    PM = true
                                };
                                if (i == p.startDate)
                                {
                                    if (p.startTypeName == "下午")
                                    {
                                        day.AM = false;
                                    }
                                }
                                if (i == p.endDate)
                                {
                                    if (p.endTypeName == "上午")
                                    {
                                        day.PM = false;
                                    }
                                }
                                merageData.Add(day);
                            }
                        }
                    }

                });

                if (merageData.Count > 0)
                {
                    var currentUser = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, user)[0];
                    merageData.ForEach(f =>
                    {
                        EmployeeServicesModel updateData = new EmployeeServicesModel()
                        {
                            CnName = currentUser.DisplayName,
                            EnName = currentUser.LogOnName,
                            OrganizationCode = currentUser.Parent.ID,
                            FullPath = currentUser.FullPath,
                            PunchDate = f.punchDate,
                            IsUnusual = true,
                            UnusualType = "请休假",
                            UnusualDesc = f.summary,
                            Creator = currentUser.ID,
                            CreateTime = DateTime.Now,
                            DescStatus = 2,
                            IsValid = true
                        };
                        if (f.AM)
                        {
                            DataRow[] drs = signData.Select("[PunchDate]='" + f.punchDate.ToString("yyyy-MM-dd") + "' AND [PunchType]=0");
                            updateData.PunchType = 0;
                            if (drs.Length < 1)
                            {
                                updateData.Code = Guid.NewGuid().ToString();
                                EmployeeServicesAdapter.Instance.Update(updateData);
                            }
                            else
                            {
                                //if (drs[0]["IsUnusual"] != DBNull.Value)
                                //{
                                //    temp_IsUnusual = (bool)drs[0]["IsUnusual"];
                                //}
                                if (drs[0]["ModifyTime"] == DBNull.Value)
                                {
                                    updateData.Code = drs[0]["Code"].ToString();
                                    EmployeeServicesAdapter.Instance.Update(updateData);
                                }
                            }
                        }
                        if (f.PM)
                        {
                            DataRow[] drs = signData.Select("[PunchDate]='" + f.punchDate.ToString("yyyy-MM-dd") + "' AND [PunchType]=1");
                            updateData.PunchType = 1;
                            if (drs.Length < 1)
                            {
                                updateData.Code = Guid.NewGuid().ToString();
                                EmployeeServicesAdapter.Instance.Update(updateData);
                            }
                            else
                            {
                                //if (drs[0]["IsUnusual"] != DBNull.Value)
                                //{
                                //    temp_IsUnusual = (bool)drs[0]["IsUnusual"];
                                //}
                                if (drs[0]["ModifyTime"] == DBNull.Value)
                                {
                                    updateData.Code = drs[0]["Code"].ToString();
                                    EmployeeServicesAdapter.Instance.Update(updateData);
                                }
                            }
                        }
                    });
                }

            }
            return merageData;
        }

        /// <summary>
        /// 更新异常信息
        /// </summary>
        [HttpPost]
        public IHttpActionResult UpdateUnusual(UpdateUnusualViewModel post)
        {
            var result = ControllerService.Run(() =>
            {
                var user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ((Seagull2Identity)User.Identity).Id)[0];

                var list = EmployeeServicesAdapter.Instance.GetByPunchDateType(user.ID, post.PunchDate, post.PunchType);
                if (string.IsNullOrWhiteSpace(post.Code) && list.Count < 1)
                {
                    // 添加
                    EmployeeServicesAdapter.Instance.Update(new EmployeeServicesModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        CnName = user.DisplayName,
                        EnName = user.LogOnName,
                        OrganizationCode = user.Parent.ID,
                        FullPath = user.FullPath,
                        PunchDate = Convert.ToDateTime(post.PunchDate),
                        PunchType = post.PunchType,
                        IsUnusual = true,
                        UnusualType = post.UnusualType,
                        UnusualDesc = post.UnusualDesc,
                        Creator = user.ID,
                        CreateTime = DateTime.Now,
                        IsValid = false,
                        DescStatus = 1
                    });
                }
                else
                {
                    // 修改
                    var model = EmployeeServicesAdapter.Instance.GetByCode(post.Code, user.ID, post.PunchDate, post.PunchType);
                    if (model == null)
                    {
                        throw new Exception("当前没有签到记录。");
                    }
                    model.UnusualType = post.UnusualType;
                    model.UnusualDesc = post.UnusualDesc;
                    model.Modifier = user.ID;
                    model.ModifyTime = DateTime.Now;
                    model.DescStatus = 1;
                    EmployeeServicesAdapter.Instance.Update(model);
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 获取用户的打卡提醒设置
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserPunchRemindSetting()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var list = PunchRemindSettingAdapter.Instance.GetListByUser(user.Id);
                if (list.Count >= 2)
                {
                    return list;
                }
                else
                {
                    var model = new PunchRemindSettingModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        Monday = true,
                        Tuesday = true,
                        Wednesday = true,
                        Thursday = true,
                        Friday = true,
                        Saturday = false,
                        Sunday = false,
                        RemindTime = "08:00",
                        Type = 0,
                        IsEnable = false,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    PunchRemindSettingAdapter.Instance.Update(model);
                    model.Code = Guid.NewGuid().ToString();
                    model.RemindTime = "18:00";
                    model.Type = 1;
                    PunchRemindSettingAdapter.Instance.Update(model);
                    list = PunchRemindSettingAdapter.Instance.GetListByUser(user.Id);
                    return list;
                }
            });
            return Ok(result);
        }

        /// <summary>
        /// 保存用户的打卡提醒设置
        /// </summary>
        [HttpPost]
        public IHttpActionResult SaveUserPunchRemindSetting(List<PunchRemindSettingModel> list)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var updateCount = 0;

                list.ForEach(model =>
                {
                    if (model.Type != 0 && model.Type != 1)
                    {
                        return;
                    }
                    if (!System.Text.RegularExpressions.Regex.IsMatch(model.RemindTime, @"^\d{2}:\d{2}$"))// 时间格式 08:00
                    {
                        return;
                    }
                    var dbModel = PunchRemindSettingAdapter.Instance.Load(w => w.AppendItem("Type", model.Type).AppendItem("Creator", user.Id)).FirstOrDefault();
                    RedisManager rm = new RedisManager(ConfigAppSetting.RedisConfName);
                    if (dbModel != null)
                    {
                        model.Code = dbModel.Code;
                        model.Creator = dbModel.Creator;
                        model.CreateTime = dbModel.CreateTime;
                        model.Modifier = user.Id;
                        model.ModifyTime = DateTime.Now;
                        model.ValidStatus = dbModel.ValidStatus;
                        PunchRemindSettingAdapter.Instance.Update(model);
                        updateCount++;
                    }
                    else
                    {
                        model.Code = Guid.NewGuid().ToString();
                        model.Creator = user.Id;
                        model.CreateTime = DateTime.Now;
                        model.ValidStatus = true;
                        PunchRemindSettingAdapter.Instance.Update(model);
                        updateCount++;
                    }
                    string key = EnumMessageTitle.Signin.ToString() + "_" + model.Code;
                    if (model.IsEnable)
                    {
                        string endStr = PunchRemindSettingAdapter.Instance.CalcEndTime(model);
                        rm.DeleteKey(key);
                        if (endStr.Length > 0)
                        {
                            DateTime endDate = DateTime.Parse(endStr + " " + model.RemindTime);
                            TimeSpan expireSpan = endDate.Subtract(DateTime.Now);
                            rm.StringSet(key, model.Creator, expireSpan);
                        }
                    }
                    else
                    {
                        rm.DeleteKey(key);
                    }
                });

                return updateCount;
            });
            return Ok(result);
        }


        /// <summary>
        /// 获取用户急速打卡设置
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserPunchQuickSetting()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                var setting = PunchQuickSettingAdapter.Instance.GetModelByUserCode(user.Id);
                var isEnable = false;
                if (setting != null)
                {
                    isEnable = setting.IsEnable;
                }

                return new
                {
                    IsEnable = isEnable,
                };
            });
            return Ok(result);
        }

        /// <summary>
        /// 保存用户急速打卡设置
        /// </summary>
        [HttpPost]
        public IHttpActionResult SaveUserPunchQuickSetting(dynamic post)
        {
            var result = ControllerService.Run(() =>
            {
                var isEnable = Convert.ToBoolean(post.IsEnable);

                var user = (Seagull2Identity)User.Identity;

                var setting = PunchQuickSettingAdapter.Instance.GetModelByUserCode(user.Id);

                if (setting == null)
                {
                    PunchQuickSettingAdapter.Instance.Update(new PunchQuickSettingModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        IsEnable = isEnable,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    });
                }
                else
                {
                    setting.IsEnable = isEnable;
                    setting.Modifier = user.Id;
                    setting.ModifyTime = DateTime.Now;
                    PunchQuickSettingAdapter.Instance.Update(setting);
                }
            });
            return Ok(result);
        }

        #endregion

         
        #region 私有方法
        private string Week(DayOfWeek dayOfWeek)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(dayOfWeek)];
            return week;
        }
        /// <summary>
        /// 去除了节假日的 工作日
        /// </summary>
        PunchStatistical GetWorkDay(DateTime date1, DateTime date2, string authorizationStr)
        {
            if (domain != null && domain._WorkDateList.Count > 0)
                return domain;
            else
            {
                domain = new PunchStatistical(date1, date2, authorizationStr);
                return domain;
            }
        }


        /// <summary>
        /// 判断打卡类型
        /// </summary>
        /// <param name="timeLine1">时间线1</param>
        /// <param name="timeLine2">时间线2</param>
        /// <param name="punchDate">打卡日期</param>
        /// <returns>打卡类型 0=上班、1=下班</returns>
        int PunchType(DateTime timeLine1, DateTime timeLine2, out DateTime punchDate)
        {
            var hour = DateTime.Now.Hour;

            if (timeLine1.Hour <= hour && hour < timeLine2.Hour)
            {
                punchDate = DateTime.Now;
                return 0;// 上班
            }
            else
            {
                punchDate = DateTime.Now;
                if (0 <= hour && hour < timeLine1.Hour)
                {
                    punchDate = DateTime.Now.AddDays(-1);
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

        /// <summary>
        /// 获取两个日期之间的工作日天数
        /// </summary>
        int GetWorkDayCount(DateTime dateTime1, DateTime dateTime2)
        {
            var count = 0;
            for (var i = dateTime1; i <= dateTime2; i = i.AddDays(1))
            {
                if (i.DayOfWeek != DayOfWeek.Saturday && i.DayOfWeek != DayOfWeek.Sunday)
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 构建当前考勤信息
        /// </summary>
        SignResultByMonthOfDayViewModel BuildData(System.Data.DataRow[] drs)
        {
            var model = new SignResultByMonthOfDayViewModel();
            if (drs.Length < 1)
            {
                model.IsPunch = false;
                return model;
            }
            var dr = drs[0];

            model.IsPunch = true;
            model.Code = dr["Code"].ToString();
            model.StandardAddress = dr["Address"].ToString();
            model.PunchAddress = dr["MapUrl"].ToString();
            model.PunchTime = Convert.ToDateTime(dr["CreateTime"]).ToString("HH:mm");
            model.IsRegular = false;
            var punchType = Convert.ToInt32(dr["PunchType"]);
            if (punchType == 0)
            {
                if (dr["IsLate"] != null && dr["IsLate"] != DBNull.Value)
                {
                    model.IsRegular = (bool)dr["IsLate"];
                }
            }
            if (punchType == 1)
            {
                if (dr["IsEarly"] != null && dr["IsEarly"] != DBNull.Value)
                {
                    model.IsRegular = (bool)dr["IsEarly"];
                }
            }
            model.IsUnusual = false;
            if (dr["IsUnusual"] != null && dr["IsUnusual"] != DBNull.Value)
            {
                model.IsUnusual = (bool)dr["IsUnusual"];
            }
            model.UnusualType = dr["UnusualType"].ToString();
            model.UnusualDesc = dr["UnusualDesc"].ToString();
            model.IsValid = (bool)dr["IsValid"];
            if (dr["DescStatus"] != null && dr["DescStatus"] != DBNull.Value)
            {
                model.DescStatus = (int)dr["DescStatus"];
            }
            return model;
        }
        /// <summary>
        /// 根据组织统计
        /// </summary>
        /// <param name="orgCodeList">要统计的组织编码列表</param>
        /// <param name="date1">开始日期</param>
        /// <param name="date2">结束日期</param>
        /// <param name="orgaList">组织列表</param>
        List<SignStatisticsViewModel> SignStatisticsByOrganizations(List<string> orgCodeList, DateTime date1, DateTime date2, ContactsCollection orgaList)
        {
            var _list = new List<SignStatisticsViewModel>();
            var dt = EmployeeServicesAdapter.Instance.GetListByOrganization(string.Join(",", orgCodeList), date1, date2);
            var count = GetWorkDayCount(date1, date2) * 2;
            foreach (var ogu in orgaList)
            {
                var model = new SignStatisticsViewModel()
                {
                    Code = ogu.ObjectID,
                    Name = ogu.DisplayName,
                    AvgHour = "--",
                    LateCount = "--",
                    EarlyCount = "--",
                    UnusualCount = "--",
                    NoSignCount = "--",
                    UserCount = 0,
                };
                var drs = dt.Select($"[Code]='{model.Code}'");
                if (drs.Length > 0)
                {
                    var dr = drs[0];
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
                    var userCount = ContactsAdapter.Instance.GetUserCountByPId1(model.Code);
                    if (userCount > 0)
                    {
                        model.UserCount = userCount;
                        if (dr["SignCount"] != DBNull.Value)
                        {
                            var signCount = Convert.ToInt32(dr["SignCount"]);
                            model.NoSignCount = (count * userCount * 2 - signCount).ToString();
                        }
                        else
                        {
                            model.NoSignCount = (count * userCount * 2).ToString();
                        }
                    }
                }
                _list.Add(model);
            }
            return _list;
        }
        /// <summary>
        /// 根据人员统计
        /// </summary>
        /// <param name="parentCode">父级部门编码</param>
        /// <param name="date1">开始日期</param>
        /// <param name="date2">结束日期</param>
        /// <param name="userList">人员列表</param>
        List<SignStatisticsViewModel> SignStatisticsByUsers(string parentCode, DateTime date1, DateTime date2, ContactsCollection userList)
        {
            var _list = new List<SignStatisticsViewModel>();
            var dt = EmployeeServicesAdapter.Instance.GetListByUser(parentCode, date1, date2);
            var count = GetWorkDayCount(date1, date2) * 2;
            userList.ForEach(user =>
            {
                var item = new SignStatisticsViewModel()
                {
                    Code = user.ObjectID,
                    Name = user.DisplayName,
                    OrganizationName = user.FullPath,
                    AvgHour = "--",
                    LateCount = "--",
                    EarlyCount = "--",
                    UnusualCount = "--",
                    NoSignCount = count.ToString()
                };
                var drs = dt.Select($"[Code] = '{user.ObjectID}'");
                if (drs.Length > 0)
                {
                    var dr = drs[0];
                    if (dr["AvgMinute"] != DBNull.Value)
                    {
                        var minute = Convert.ToDouble(dr["AvgMinute"]);
                        if (minute > 0)
                        {
                            item.AvgHour = $"{(minute / 60).ToString("0.0")}";
                        }
                    }
                    if (dr["LateCount"] != DBNull.Value)
                    {
                        item.LateCount = dr["LateCount"].ToString();
                    }
                    if (dr["EarlyCount"] != DBNull.Value)
                    {
                        item.EarlyCount = dr["EarlyCount"].ToString();
                    }
                    if (dr["UnusualCount"] != DBNull.Value)
                    {
                        item.UnusualCount = dr["UnusualCount"].ToString();
                    }
                    item.NoSignCount = (count - Convert.ToInt32(dr["SignCount"])).ToString();
                }
                _list.Add(item);
            });
            return _list;
        }
        /// <summary>
        /// 统计个人打卡明细
        /// </summary>
        /// <param name="parentCode">人员编码</param>
        /// <param name="date1">开始日期</param>
        /// <param name="date2">结束日期</param>
        /// <param name="userInfo">人员信息</param>
        List<SignDetailViewModel> SignUserDetail(string parentCode, DateTime date1, DateTime date2, ContactsModel userInfo)
        {
            var _list = new List<SignDetailViewModel>();
            var dt = EmployeeServicesAdapter.Instance.GetListByDetail(parentCode, date1, date2);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                var item = new SignDetailViewModel();
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
        #endregion

        #region  定时每月 1号 推送上个月打卡异常 并且没有填写异常说明

        /// <summary>
        /// 推送上个月打卡异常并且没有填写原因的数据  定时任务调用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult PushPunchException()
        {
            var result = ControllerService.Run(() =>
            {
                List<DateTime> _WorkDateList = new List<DateTime>();
                BaseView data = new BaseView();
                string startDate = CommonService.FirstDayOfMounth(DateTime.Now.AddMonths(-1)).ToString("yyyy-MM-dd");
                string endDate = CommonService.LastDayOfMounth(DateTime.Now.AddMonths(-1)).ToString("yyyy-MM-dd");
                var authStr = $"{Request.Headers.Authorization.Scheme} {Request.Headers.Authorization.Parameter}";
                var currdomain = new Domain.Sign.PunchStatistical(CommonService.FirstDayOfMounth(DateTime.Now.AddMonths(-1)), CommonService.LastDayOfMounth(DateTime.Now.AddMonths(-1)), authStr);
                int worlDays = currdomain._WorkDateList.Count;
                DataTable dt = EmployeeServicesAdapter.Instance.GetPunchException(worlDays, endDate, startDate);
                List<string> ids = new List<string>();
                Models.Message.MessageCollection models = new Models.Message.MessageCollection();
                if (dt.Rows.Count > 0)
                {
                    #region 发送打卡消息
                    string usercode = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Creator"] != null)
                        {
                            usercode = row["Creator"].ToString();
                            ids.Add(usercode);
                            models.Add(new Models.Message.MessageModel
                            {
                                Code = Guid.NewGuid().ToString(),
                                MeetingCode = "",
                                MessageContent = "小伙伴上月打卡存在异常，今天是补写异常说明的最后一天，要及时填写哦～",
                                MessageStatusCode = EnumMessageStatus.New,
                                MessageTypeCode = "0",
                                MessageTitleCode = EnumMessageTitle.SigninException,
                                ModuleType = EnumMessageModuleType.SigninException.ToString(),
                                Creator = usercode,
                                CreatorName = "",
                                ReceivePersonCode = usercode,
                                ReceivePersonName = "",
                                ReceivePersonMeetingTypeCode = string.Empty,
                                OverdueTime = DateTime.Now.AddDays(2),
                                ValidStatus = true,
                                CreateTime = DateTime.Now,
                            });
                        }
                    }

                    MessageAdapter.Instance.MessageBatchInsert(models);
                    #endregion
                    #region 推送消息
                    var model = new PushService.Model()
                    {
                        BusinessDesc = "打卡异常提醒",
                        Title = "打卡补写提醒",
                        Content = "小伙伴上个月打卡存在异常，今天是补写异常说明的最后一天，要及时填写哦～",
                        Extras = new PushService.ModelExtras()
                        {
                            action = "abnormalPunchCard",
                            bags = ""
                        },
                        SendType = PushService.SendType.Person,
                        Ids = string.Join(",", ids)
                    };
                    PushService.Push(model, out string pushResult);
                    #endregion
                    data.Data = ids.Count;
                    data.Message = pushResult;
                }
                else
                    data.Message = "未找到打卡异常人员";
                return data;
            });
            return this.Ok(result);
        }

        #endregion 

    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System.Net.Http;
using log4net;
using System.Reflection;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 用户行为日志控制器
    /// </summary>
    public class UserBehaviorController : ApiController
    {
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 添加用户行为日志
        /// <summary>
        /// 添加用户行为日志
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> Add(UserBehaviorLogViewModel obj)
        {
            //获取用户信息
            var user = Adapter.AddressBook.ContactsAdapter.Instance.LoadByCode(((Seagull2Identity)User.Identity).Id);
            if (user == null)
            {
                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "获取用户信息失败：没有获取到用户信息。"
                }));
            }

            //添加日志数据
            var newModel = new UserBehaviorLogModel();
            try
            {
                //查询模块编号
                string moduleCode;
                var module = UserBehaviorModuleAdapter.Instance.Load(p => { p.AppendItem("Name", obj.Module); });
                if (module.Count < 1)
                {
                    moduleCode = Guid.NewGuid().ToString();
                    UserBehaviorModuleAdapter.Instance.Update(new UserBehaviorModuleModel()
                    {
                        Code = moduleCode,
                        Name = obj.Module,
                        Creator = user.ObjectID,
                        CreateTime = DateTime.Now,
                        ValidStatus = true
                    });
                }
                else
                {
                    moduleCode = module[0].Code;
                }

                //添加日志
                newModel = new UserBehaviorLogModel()
                {
                    UserCode = user.ObjectID,
                    UserAccount = user.Logon_Name,
                    UserName = user.DisplayName,
                    DepartmentCode = user.ParentID,
                    DepartmentName = user.FullPath.Substring(0, user.FullPath.LastIndexOf('\\')),
                    ModuleCode = moduleCode,
                    Name = obj.Name,
                    TimeStart = obj.ConvertTime(obj.TimeStart),
                    TimeEnd = obj.ConvertTime(obj.TimeEnd),
                    Creator = user.ObjectID,
                    CreateTime = DateTime.Now,
                    ValidStatus = true
                };
                UserBehaviorLogAdapter.Instance.Add(newModel);

                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
                {
                    State = true,
                    Message = "添加成功！"
                }));
            }
            catch (Exception e)
            {
                Log.WriteLog("Error - 添加用户行为日志API：" + e.Message);
                Log.WriteLog(Newtonsoft.Json.JsonConvert.SerializeObject(newModel));
                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "添加信息失败：" + e.Message
                }));
            }
        }
        #endregion

        //后台接口

        #region 获取统计列表
        /// <summary>
        /// 获取统计列表
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetStatistical(dynamic post)
        {
            int pageSize = Convert.ToInt32(post.pageSize);
            int pageIndex = Convert.ToInt32(post.pageIndex);
            string module = post.module;
            DateTime dateStart = Convert.ToDateTime(post.dateStart + " 00:00:00");
            DateTime dateEnd = Convert.ToDateTime(post.dateEnd + " 23:59:59");

            int totalCount = UserBehaviorLogAdapter.Instance.Statistical(module, dateStart, dateEnd);

            DataTable pageData = UserBehaviorLogAdapter.Instance.Statistical(module, dateStart, dateEnd, pageSize, pageIndex);

            List<dynamic> listData = new List<dynamic>();
            foreach (DataRow dr in pageData.Rows)
            {
                dynamic item = new ExpandoObject();
                item.ModuleCode = dr["Code"];
                item.ModuleName = dr["Name"];
                item.ViewCount = dr["ViewCount"];
                listData.Add(item);
            }

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList()
            {
                State = true,
                Message = "获取成功！",
                Data = new BaseViewPage<dynamic>()
                {
                    DataCount = totalCount,
                    PageCount = totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0),
                    PageData = listData
                }
            }));
        }
        #endregion

        #region 获取统计详情
        /// <summary>
        /// 获取统计详情
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> GetStatisticalInfo(dynamic post)
        {
            try
            {
                int pageSize = Convert.ToInt32(post.pageSize);
                int pageIndex = Convert.ToInt32(post.pageIndex);
                string moduleCode = post.moduleCode;
                DateTime dateStart = Convert.ToDateTime(post.dateStart + " 00:00:00");
                DateTime dateEnd = Convert.ToDateTime(post.dateEnd + " 23:59:59");

                int totalCount = UserBehaviorLogAdapter.Instance.StatisticalInfo(moduleCode, dateStart, dateEnd);

                DataTable pageData = UserBehaviorLogAdapter.Instance.StatisticalInfo(moduleCode, dateStart, dateEnd, pageSize, pageIndex);

                List<dynamic> listData = new List<dynamic>();
                foreach (DataRow dr in pageData.Rows)
                {
                    dynamic item = new ExpandoObject();
                    item.UserName = dr["UserName"];
                    item.DepartmentName = dr["DepartmentName"];
                    item.Name = dr["Name"];
                    item.TimeStart = Convert.ToDateTime(dr["TimeStart"]);
                    item.TimeEnd = Convert.ToDateTime(dr["TimeEnd"]);
                    item.UseTime = dr["UseTime"];
                    listData.Add(item);
                }

                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList()
                {
                    State = true,
                    Message = "获取成功！",
                    Data = new BaseViewPage<dynamic>()
                    {
                        DataCount = totalCount,
                        PageCount = totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0),
                        PageData = listData
                    }
                }));
            }
            catch (Exception e)
            {
                log.Error("获取用户行为分析统计详情报错");
                log.Error(e);
                return Task.FromResult<IHttpActionResult>(Ok(new BaseView() { State = false, Message = e.Message }));
            }
        }
        #endregion

        #region 单模块统计详情导出成Excel -- PC
        /// <summary>
        /// 统计详情导出成Excel -- PC
        /// </summary>
        /// <param name="moduleCode">模块ID</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="dateStart">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForStatisticalInfo(string moduleCode, string moduleName, string dateStart, string dateEnd)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            DateTime dateStartTime = Convert.ToDateTime(dateStart + " 00:00:00");
            DateTime dateEndsTime = Convert.ToDateTime(dateEnd + " 23:59:59");
            DataTable table = UserBehaviorLogAdapter.Instance.ExportStatisticalInfo(moduleCode, dateStartTime, dateEndsTime);
            IList<UserBehaviorLogModel> list = DataConvertHelper<UserBehaviorLogModel>.ConvertToList(table);
            ControllerHelp.RunAction(() =>
            {
                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"访问者","UserName" },
                    {"部门","DepartmentName" },
                    {"名称","Name" },
                    {"访问时间","TimeStart" },
                    { "用时(ms)","UseTime"}
                };
                result = ExcelHelp<UserBehaviorLogModel, List<UserBehaviorLogModel>>.ExportExcelData(dicColl, list.ToList<UserBehaviorLogModel>(), moduleName + "模块统计分析表");
            });
            return result;
        }
        #endregion

        #region 全部模块统计导出Excel --PC
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForALL(string dateStart, string dateEnd)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            DateTime dateStartTime = Convert.ToDateTime(dateStart + " 00:00:00");
            DateTime dateEndsTime = Convert.ToDateTime(dateEnd + " 23:59:59");
            DataTable table = UserBehaviorLogAdapter.Instance.ExportAllByDate(dateStartTime, dateEndsTime);
            IList<UserBehaviorLogModel> list = DataConvertHelper<UserBehaviorLogModel>.ConvertToList(table);
            ControllerHelp.RunAction(() =>
            {
                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"访问者","UserName" },
                    {"部门","DepartmentName" },
                    {"名称","Name" },
                    {"访问时间","TimeStart" },
                    { "用时(ms)","UseTime"},
                    { "模块名称","ModuleName"}
                };
                result = ExcelHelp<UserBehaviorLogModel, List<UserBehaviorLogModel>>.ExportExcelData(dicColl, list.ToList<UserBehaviorLogModel>(), "用户行为统计分析表");
            });
            return result;
        }
        #endregion

        #region 导出Excel - 搜索多个模块
        /// <summary>
        /// 导出Excel - 搜索多个模块
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage ExportExcelForSearch(string moduleName, string dateStart, string dateEnd)
        {
            HttpResponseMessage result = new HttpResponseMessage();

            var module = UserBehaviorModuleAdapter.Instance.Load(w => w.AppendItem("Code", moduleName));
            if (module.Count < 1)
            {
                result.Content = new StringContent("模块编码错误！");
            }

            var dateStartTime = Convert.ToDateTime(dateStart + " 00:00:00");
            var dateEndsTime = Convert.ToDateTime(dateEnd + " 23:59:59");
            DataTable table = UserBehaviorLogAdapter.Instance.ExportStatisticalInfo(moduleName, dateStartTime, dateEndsTime);
            IList<UserBehaviorLogModel> list = DataConvertHelper<UserBehaviorLogModel>.ConvertToList(table);
            ControllerHelp.RunAction(() =>
            {
                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"访问者","UserName" },
                    {"部门","DepartmentName" },
                    {"名称","Name" },
                    {"访问时间","TimeStart" },
                };
                result = ExcelHelp<UserBehaviorLogModel, List<UserBehaviorLogModel>>.ExportExcelData(dicColl, list.ToList<UserBehaviorLogModel>(), moduleName + "模块统计分析表");
            });
            return result;
        }
        #endregion

        #region 导出Excel -- 单个模块
        /// <summary>
        /// 导出Excel - 单个模块
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ExportExcelForModule(string moduleCode, string moduleName, string dateStart, string dateEnd)
        {
            HttpResponseMessage result = new HttpResponseMessage();

            var module = UserBehaviorModuleAdapter.Instance.Load(w => w.AppendItem("Code", moduleCode));
            if (module.Count < 1)
            {
                result.Content = new StringContent("模块编码错误！");
            }

            var dateStartTime = Convert.ToDateTime(dateStart + " 00:00:00");
            var dateEndsTime = Convert.ToDateTime(dateEnd + " 23:59:59");
            DataTable table = UserBehaviorLogAdapter.Instance.ExportStatisticalInfo(moduleCode, dateStartTime, dateEndsTime);
            IList<UserBehaviorLogModel> list = DataConvertHelper<UserBehaviorLogModel>.ConvertToList(table);
            ControllerHelp.RunAction(() =>
            {
                Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"访问者","UserName" },
                    {"部门","DepartmentName" },
                    {"名称","Name" },
                    {"访问时间","TimeStart" },
                };
                result = ExcelHelp<UserBehaviorLogModel, List<UserBehaviorLogModel>>.ExportExcelData(dicColl, list.ToList<UserBehaviorLogModel>(), moduleName + "模块统计分析表");
            });
            return result;
        }
        #endregion
        
        #region 获取统计图表数据
        /// <summary>
        /// 获取统计图表数据
        /// </summary>
        [HttpGet]
        public Task<IHttpActionResult> GetModuleViewCount()
        {
            DataTable dt = UserBehaviorLogAdapter.Instance.GetModuleViewCount();

            List<dynamic> listData = new List<dynamic>();
            foreach (DataRow dr in dt.Rows)
            {
                dynamic item = new ExpandoObject();
                item.Name = dr["Name"];
                item.Value = dr["ViewCount"];
                listData.Add(item);
            }

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList()
            {
                State = true,
                Message = "获取成功！",
                Data = listData
            }));
        }
        #endregion
    }
}
using MCS.Library.Office.OpenXml.Excel;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 班车路线API控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 查询班车信息列表--PC/APP
        /// </summary>
        /// <returns></returns>
        [Route("GetBusRouteList")]
        [HttpGet]
        public IHttpActionResult GetBusRouteList(int pageIndex, string conferenceID, string searchTime = "")
        {
            ViewPageBase<BusRouteModelCollection> list = new ViewPageBase<BusRouteModelCollection>();
            ControllerHelp.SelectAction(() =>
            {
                list = BusRouteModelAdapter.Instance.GetConferenceModelListByPage(pageIndex, DateTime.Now.AddHours(1), conferenceID);
            });
            return Ok(list);
        }
        /// <summary>
        /// 查询单个班车信息
        /// </summary>
        /// <returns></returns>
        [Route("GetBusRoute")]
        [HttpGet]
        public IHttpActionResult GetBusRoute(string id)
        {
            BusRouteModel model = new BusRouteModel();
            ControllerHelp.SelectAction(() =>
            {
                model = BusRouteModelAdapter.Instance.GetTByID(id);
            });

            return Ok(model);
        }
        /// <summary>
        /// 增加班车信息--PC
        /// </summary>
        /// <returns></returns>
        [Route("AddBusRoute")] 
        [HttpPost]
        public IHttpActionResult AddBusRoute(BusRouteModel model)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.Creator = CurrentUserCode;
                model.CreateTime = DateTime.Now;
                model.ValidStatus = true;
                BusRouteModelAdapter.Instance.AddOrUpdateT(model);
            });
            return Ok(result);
        }
        /// <summary>
        /// 删除班车信息
        /// </summary>
        /// <returns></returns>
        [Route("DelBusRoute")]
        [HttpPost]
        public IHttpActionResult DelBusRoute(string id)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                BusRouteModelAdapter.Instance.DelTById(id);
            });
            return Ok(result);
        }
        /// <summary>
        /// 修改班车信息
        /// </summary>
        /// <returns></returns>
        [Route("UpdateBusRoute")]
        [HttpPost]
        public IHttpActionResult UpdateBusRoute(BusRouteModel model)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                model.CreateTime = DateTime.Now;
                model.Creator = CurrentUserCode;
                model.ValidStatus = true;
                BusRouteModelAdapter.Instance.Update(model);
            });
            return Ok(result);
        }

        /// <summary>
        /// 编辑班车信息--PC
        /// </summary>
        /// <returns></returns>
        [Route("EditBusRoute")]
        [HttpPost]
        public IHttpActionResult EditBusRoute(BusRouteModel model)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                if (string.IsNullOrEmpty(model.ID))
                {
                    model.ID = Guid.NewGuid().ToString("N");
                    model.Creator = CurrentUserCode;
                    model.CreateTime = DateTime.Now;
                    model.ValidStatus = true;
                    BusRouteModelAdapter.Instance.AddOrUpdateT(model);
                }
                else {
                    model.CreateTime = DateTime.Now;
                    model.Creator = CurrentUserCode;
                    model.ValidStatus = true;
                    BusRouteModelAdapter.Instance.Update(model);
                }

              
            });
            return Ok(result);
        }
        /// <summary>
        /// 班车信息模板导入--PC
        /// </summary>
        /// <returns></returns>
        [Route("ImportExcelDataForBusRoute")]
        [HttpPost]
        public IHttpActionResult ImportExcelDataForBusRoute()
        {
            BusRouteModelCollection modelColl = new BusRouteModelCollection();

            string conferenceID = baseRequest.Form["ConferenceID"].ToString();
            ControllerHelp.RunAction(() =>
            {
                //删除会议编码下的所有班车路线
                BusRouteModelAdapter.Instance.DelBusRouteByConferenceID(conferenceID);

                //将新班车路线数据添加进数据库
                DataTable dataTable = ExcelHelp<BusRouteModel, List<BusRouteModel>>.GetExcelData("BusRoute");
                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];
                    BusRouteModel model = new BusRouteModel
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Creator = CurrentUserCode,
                        ValidStatus = true,
                        CreateTime = DateTime.Now,
                        ConferenceID = conferenceID,

                        BeginPlace = row["出发地"].ToString(),
                        EndPlace = row["目的地"].ToString(),
                        ContactsName = row["对接人"].ToString(),
                        ContactsPhone = row["对接人电话"].ToString(),
                        DepartDate = Convert.ToDateTime(row["发车时间"]),
                        Title = row["路线标题"].ToString()
                    };
                    modelColl.Add(model);
                }
                BusRouteModelAdapter.Instance.AddBusRouteModelCollection(modelColl);
            });

            ViewPageBase<BusRouteModelCollection> view = new ViewPageBase<BusRouteModelCollection>()
            {
                State = true,
                dataList = modelColl,
                PageCount = modelColl.Count / ViewPageBase<BusRouteModelCollection>.PageSize + (modelColl.Count % ViewPageBase<BusRouteModelCollection>.PageSize > 0 ? 1 : 0)
            };

            return Ok(view);
        }
        /// <summary>
        /// 班车数据导出成EXCEL
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ExportExcelDataForBusRoute")]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForBusRoute(string conferenceID)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(conferenceID,false);

                    BusRouteModelCollection busColl = BusRouteModelAdapter.Instance.Load(m => m.AppendItem("ConferenceID", conferenceID));

                    Dictionary<string, string> dicColl = new Dictionary<string, string>() {
                    {"路线标题","Title" },
                    {"发车时间","DepartDate" },
                    {"出发地","BeginPlace" },
                    {"目的地","EndPlace" },
                    {"对接人","ContactsName" },
                    {"对接人电话","ContactsPhone" }
                    };
                    result = ExcelHelp<BusRouteModel, List<BusRouteModel>>.ExportExcelData(dicColl, busColl.ToList(), "BusRoute");

            });
            return result;
        }
    }
}

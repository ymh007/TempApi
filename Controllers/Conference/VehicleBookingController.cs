using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.Models;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Adapter.Conference;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //车辆预定控制器
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 查询车辆预定列表
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        [Route("GetVehicleBookingList")]
        [HttpGet]
        public IHttpActionResult GetVehicleBookingList(int pageIndex, string ConferenceID, DateTime searchTime, string BeginPlace = "", string EndPlace = "", string ReserveTime = "")
        {
            ViewPageBase<List<VehicleBookingListModel>> list = new ViewPageBase<List<VehicleBookingListModel>>();

            ControllerHelp.RunAction(() =>
            {
                ViewModelBase result = ControllerHelp.RunAction(() =>
                {
                    list = VehicleBookingListModelAdapter.Instance.GetVehicleBookingListByPage(pageIndex, ConferenceID, BeginPlace, EndPlace, (ReserveTime == "" || ReserveTime == "null") ? DateTime.MinValue : Convert.ToDateTime(ReserveTime), searchTime, "");
                });
            });
            return Ok(list);
        }
        /// <summary>
        /// 查询我的预定列表--APP
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        [Route("GetMyVehicleBookingList")]
        [HttpGet]
        public IHttpActionResult GetMyVehicleBookingList(int pageIndex, string ConferenceID, DateTime searchTime)
        {
            ViewPageBase<List<VehicleBookingListModel>> list = new ViewPageBase<List<VehicleBookingListModel>>();

            ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(ConferenceID);
                if (userInfo.IsInConference)
                {
                    list = VehicleBookingListModelAdapter.Instance.GetVehicleBookingListByPage(pageIndex, ConferenceID, "", "", DateTime.MinValue, searchTime, userInfo.CodeUserInConference);
                }
            });

            return Ok(list);
        }
        /// <summary>
        /// 新增车辆预定--APP
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [Route("AddVehicleBooking")]
        [HttpPost]
        public IHttpActionResult AddVehicleBooking(VehicleBookingModel vm)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                UserInfo userInfo = IsInConferenceCurrentUser(vm.ConferenceID, true);
                if (userInfo.IsInConference)
                {
                    vm.ID = Guid.NewGuid().ToString();
                    vm.ValidStatus = true;
                    vm.CreateTime = DateTime.Now;
                    vm.AttendeeID = userInfo.CodeUserInConference;
                    vm.Remark = vm.Remark;

                    VehicleBookingAdapter.Instance.AddVehicleBooking(vm);

                    // 消息推送
                    VehicleBookingDialogHelp.SendMessage(vm.ConferenceID, userInfo.CodeSeagull, vm.ID);
                }
            });

            return Ok(result);
        }
        /// <summary>
        /// 删除车辆预定
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [Route("DelVehicleBooking")]
        [HttpPost]
        public IHttpActionResult DelVehicleBooking(string id)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
             {
                 VehicleBookingAdapter.Instance.DelTById(id);
             });

            return Ok(result);

        }
    }
}
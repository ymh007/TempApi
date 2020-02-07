using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using MCS.Library.Data;
using Newtonsoft.Json.Linq;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 座位控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        #region 查询我的座位号--APP
        /// <summary>
        /// 查询我的座位号--APP
        /// </summary>
        /// <param name="conferenceID">会议ID</param>
        [Route("GetUserSeat")]
        [HttpGet]
        public IHttpActionResult GetUserSeat(string conferenceID)
        {
            ConferenceModel conference = ConferenceAdapter.Instance.GetConferenceByConferenceId(conferenceID);
            if (conference == null)
            {
                return Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "会议编号错误！"
                });
            }

            UserInfo userInfo = IsInConferenceCurrentUser(conferenceID, false);
            if (userInfo == null || userInfo.IsInConference == false)
            {
                return Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "您不属于参会人，无法查看座位信息！"
                });
            }

            SeatsModel seatModel = SeatsAdapter.Instance.GetUserSeatModel(userInfo.CodeUserInConference, conferenceID);
            if (seatModel == null)
            {
                return Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "暂时没有您的座位信息！"
                });
            }

            return Ok(new ViewModelBaseList()
            {
                State = true,
                Message = "success.",
                Data = new
                {
                    SeatAddress = seatModel.SeatAddress,
                    SeatImage = string.Empty,
                    UserHeadUrl = UserHeadPhotoService.GetUserHeadPhoto(((Seagull2Identity)User.Identity).Id)
                }
            });
        }
        #endregion

        #region 根据会议编码获取座位列表
        /// <summary>
        /// 根据会议编码获取座位列表
        /// </summary>
        [Route("GetUserSeatList")]
        [HttpGet]
        public IHttpActionResult GetUserSeatList(int pageIndex, string ConferenceID, DateTime searchTime)
        {
            ViewPageBase<List<SeatsViewListModel>> list = new ViewPageBase<List<SeatsViewListModel>>();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                list = SeatsViewListModelAdapter.Instance.GetUserSeatListByPage(pageIndex, ConferenceID, searchTime);
            });
            return Ok(list);
        }
        #endregion

        #region 更新用户座位--PC
        /// <summary>
        /// 更新用户座位--PC
        /// </summary>
        [Route("UpdateUserSeat")]
        [HttpPost]
        public IHttpActionResult UpdateUserSeat(SeatsViewUpdateModel model)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (model.SeatColl.Count > 0 && model.ConferenceID != null)
                {
                    vm.State = SeatsAdapter.Instance.Update(model.SeatColl, model.ConferenceID);
                }
            });
            return Ok(vm);
        }
        #endregion

        #region 根据ID删除用户座位--PC
        /// <summary>
        /// 根据ID删除用户座位--PC
        /// </summary>
        [Route("DelUserSeat")]
        [HttpPost]
        public IHttpActionResult DelUserSeat(SeatsModel sm)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (sm.ID != null)
                {
                    vm.State = SeatsAdapter.Instance.DelUserSeat(sm.ID);
                }
                else
                {
                    vm.State = false;
                    vm.Message = "编码不能为空！";
                }
            });
            return Ok(vm);
        }
        #endregion

        #region 根据会议编码获取禁用的座位列表--PC
        /// <summary>
        /// 根据会议编码获取禁用的座位列表
        /// </summary>
        /// <param name="ConferenceID">会议编码</param>
        /// <returns></returns>
        [Route("GetDisableSeatList")]
        [HttpGet]
        public IHttpActionResult GetDisableSeatList(string ConferenceID)
        {
            List<DisableSeatsViewModel> list = new List<DisableSeatsViewModel>();
            ControllerHelp.SelectAction(() =>
            {
                list = DisableSeatsViewListModelAdapter.Instance.GetDisableSeatsModels(ConferenceID);
            });
            return Ok(list);
        }

        #endregion

        #region 禁用座位信息--PC
        /// <summary>
        /// 把座位设为禁用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateDisableSeat")]
        [HttpPost]
        public IHttpActionResult UpdateDisableSeat(DisableSeatsViewModel model)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(model.SeatAddress) && !string.IsNullOrEmpty(model.ConferenceID))
                {

                    //判断当前会议是否已有预订的座位，如果有则不允许再禁用任何座位
                    bool IsHasSeat = SeatsAdapter.Instance.GetHasSeatModelByID(model.ConferenceID);
                    if (!IsHasSeat)
                    {
                        SeatsModel seatModel = SeatsAdapter.Instance.GetSeatModelByID(model.ConferenceID, model.SeatAddress);
                        if (seatModel != null)
                        {
                            vm.State = false;
                            vm.Message = "该座位已被预订，请先移除预订后再禁用！";
                        }
                        else
                        {
                            var user = (Seagull2Identity)User.Identity;
                            var DisableSeats = new DisableSeatsModel
                            {
                                ID = Guid.NewGuid().ToString(),
                                ConferenceID = model.ConferenceID,
                                SeatAddress = model.SeatAddress,
                                Creator = user.Id,
                                CreateTime = DateTime.Now,
                                ValidStatus = true
                            };
                            DisableSeatsAdapter.Instance.Update(DisableSeats);
                            vm.State = true;
                        }
                    }
                    else
                    {
                        vm.State = false;
                        vm.Message = "当前会议已有被预订的座位，不允许禁用座位！";
                    }

                }
                else
                {
                    vm.State = false;
                    vm.Message = "座位号和会议编码不能为空！";
                }
            });
            return Ok(vm);
        }

        #endregion

        #region 启用所有被禁用的座位信息--PC
        /// <summary>
        /// 启用所有被禁用的座位信息
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        [Route("DelDisableSeatList")]
        [HttpPost]
        public IHttpActionResult DelDisableSeatList(string conferenceID)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(conferenceID))
                {
                    //判断当前会议是否已有预订的座位，如果有则不允许再禁用任何座位
                    bool IsHasSeat = SeatsAdapter.Instance.GetHasSeatModelByID(conferenceID);
                    if (!IsHasSeat)
                    {
                        vm.State = DisableSeatsAdapter.Instance.DelHasDisableSeats(conferenceID);
                    }
                    else
                    {
                        vm.State = false;
                        vm.Message = "当前会议已存在被预订的座位，不允许再做禁用相关操作！";
                    }

                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空！";
                }
            });
            return Ok(vm);
        }
        #endregion

        #region 禁用整行或者整列座位--PC
        /// <summary>
        /// 禁用整行或者整列座位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateDisableRowSeats")]
        [HttpPost]
        public IHttpActionResult UpdateDisableRowSeats(DisableRowSeatsViewModel model)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(model.SeatRows) && !string.IsNullOrEmpty(model.SeatColumns) && !string.IsNullOrEmpty(model.ConferenceID) && !string.IsNullOrEmpty(model.flag) && !string.IsNullOrEmpty(model.NowSeatRow))
                {
                    //判断当前会议是否已有预订的座位，如果有则不允许再禁用任何座位
                    bool IsHasSeat = SeatsAdapter.Instance.GetHasSeatModelByID(model.ConferenceID);
                    if (!IsHasSeat)
                    {
                        //判断当前要禁用的行或列有无已被预订的座位
                        bool IsDisSeat = SeatsAdapter.Instance.GetDisSeatModelByID(model.ConferenceID, model.NowSeatRow, model.flag);
                        if (!IsDisSeat)
                        {
                            int SeatRows = Convert.ToInt32(model.SeatRows);//总行数
                            int SeatColumns = Convert.ToInt32(model.SeatColumns);//总列数
                            List<DisableSeatsModel> ListDisSeat = new List<DisableSeatsModel>();
                            if (model.flag == "1")
                            {
                                //禁用行
                                for (int i = 1; i < SeatColumns + 1; i++)
                                {
                                    string seatno = model.NowSeatRow + "-" + i;//座位实际对应的位置坐标编号
                                                                               //根据座位实际对应的坐标编号和会议编码判断当前座位是否已被禁用
                                    DisableSeatsModel Disseat = DisableSeatsAdapter.Instance.GetDisSeatById(model.ConferenceID, seatno);
                                    if (Disseat == null)
                                    {
                                        //说明未被禁用
                                        var user = (Seagull2Identity)User.Identity;
                                        var DisableSeats = new DisableSeatsModel
                                        {
                                            ID = Guid.NewGuid().ToString(),
                                            ConferenceID = model.ConferenceID,
                                            SeatAddress = seatno,
                                            Creator = user.Id,
                                            CreateTime = DateTime.Now,
                                            ValidStatus = true
                                        };
                                        ListDisSeat.Add(DisableSeats);
                                    }
                                }
                                DisableSeatsAdapter.Instance.UpdateSeatColl(ListDisSeat);
                                vm.State = true;
                            }
                            else if (model.flag == "2")
                            {
                                //禁用列
                                for (int i = 1; i < SeatRows + 1; i++)
                                {
                                    string seatno = i + "-" + model.NowSeatRow;//座位实际对应的位置坐标编号
                                                                               //根据座位坐标编号和会议编码判断当前座位是否已被禁用
                                    DisableSeatsModel Disseat = DisableSeatsAdapter.Instance.GetDisSeatById(model.ConferenceID, seatno);
                                    if (Disseat == null)
                                    {
                                        //说明未被禁用
                                        var user = (Seagull2Identity)User.Identity;
                                        var DisableSeats = new DisableSeatsModel
                                        {
                                            ID = Guid.NewGuid().ToString(),
                                            ConferenceID = model.ConferenceID,
                                            SeatAddress = seatno,
                                            Creator = user.Id,
                                            CreateTime = DateTime.Now,
                                            ValidStatus = true
                                        };
                                        ListDisSeat.Add(DisableSeats);
                                    }
                                }
                                DisableSeatsAdapter.Instance.UpdateSeatColl(ListDisSeat);
                                vm.State = true;
                            }
                        }
                        else
                        {
                            vm.State = false;
                            if (model.flag == "1")
                            {
                                vm.Message = "当前行已有预订的座位，请先移除预订的座位后再禁用！";
                            }
                            else
                            {
                                vm.Message = "当前列已有预订的座位，请先移除预订的座位后再禁用！";
                            }

                        }
                    }
                    else
                    {
                        vm.State = false;
                        vm.Message = "当前会议已有被预订的座位，不允许禁用座位！";
                    }

                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码和座位行数和列数以及当前要禁用的行或列的编号不能为空！";
                }
            });
            return Ok(vm);
        }
        #endregion

        #region 初始化座位分布图
        /// <summary>
        /// 初始化座位分布图
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        [Route("InitializationList")]
        [HttpPost]
        public IHttpActionResult InitializationList(string conferenceID, int seatType)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(conferenceID))
                {
                    vm.State = SeatsAdapter.Instance.RemoveHasSeatList(conferenceID, seatType);
                    if (vm.State)
                    {
                        vm.Message = "初始化成功！";
                    }
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空！";
                }
            });

            return Ok(vm);
        }
        #endregion

        #region  更新宴会桌 桌号

        /// <summary>
        /// 更新宴会桌 某一卓的人员的桌号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("UpdateDesckNum")]
        [HttpPost]
        public IHttpActionResult UpdateDesckNum([FromBody]JObject model)
        {
            string conferenceID = (string)model.SelectToken("conferenceID");
            string seatAddress = (string)model.SelectToken("seatAddress");
            string seatType = (string)model.SelectToken("seatType");
            string newSeatAddress = (string)model.SelectToken("newSeatAddress");

            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(conferenceID) && !string.IsNullOrEmpty(newSeatAddress))
                {
                    vm.State = SeatsAdapter.Instance.UpdateDeskNum(conferenceID, seatAddress, seatType, newSeatAddress);
                    if (vm.State)
                    {
                        vm.Message = "更新成功！";
                    }
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空！";
                }
            });

            return Ok(vm);
        }

        #endregion

        #region  设置宴请人员座位

        /// <summary>
        /// 设置宴请人员座位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("SetEntertainHallSeatNum")]
        [HttpPost]
        public IHttpActionResult SetEntertainHallSeatNum([FromBody]JObject model)
        {

            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                string conferenceID = (string)model.SelectToken("ConferenceID");
                string desktopNum = (string)model.SelectToken("desktopNum");
                if (!string.IsNullOrEmpty(conferenceID) && !string.IsNullOrEmpty(desktopNum))
                {
                    JArray broadcastJArray = (JArray)model.SelectToken("viewList");
                    SeatsCollection seats = new SeatsCollection();
                    SeatsCollection oldSeats = SeatsAdapter.Instance.GetEntertainHallSeatByCID(conferenceID, 2);
                    foreach (JObject vl in broadcastJArray)
                    {
                        var temp = new SeatsModel()
                        {
                            ConferenceID = conferenceID,
                            AttendeeID = (string)vl.SelectToken("id"),
                            SeatAddress = desktopNum,
                            SeatType = 2,
                            ValidStatus = true,
                            CreateTime = DateTime.Now
                        };
                        if (oldSeats != null)
                        {
                            var old = oldSeats.Find(f => f.AttendeeID == temp.AttendeeID);
                            temp.ID = old != null ? old.ID : "";
                        }
                        if (!string.IsNullOrEmpty(temp.ID))
                        {
                            seats.Add(temp);
                        }
                    }
                    SeatsAdapter.Instance.UpdateSeatColl(seats);
                    vm.State = true;
                    vm.Message = "ok！";
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空！";
                }
            });

            return Ok(vm);
        }

        #endregion



        #region 会议 交换会议座位

        /// <summary>
        /// 交换座位
        /// </summary>
        /// <param name="id">会议id</param>
        /// <param name="attid1">要交换的人员id</param>
        /// <param name="attid2">被交换人员id</param>
        /// <returns></returns>
        [Route("ExchangeSeat")]
        [HttpGet]
        public IHttpActionResult ExchangeSeat(string id, string attid1, string attid2)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(attid1) && !string.IsNullOrEmpty(attid2))
                {
                    SeatsModel s1 = SeatsAdapter.Instance.Load(p => p.AppendItem("ConferenceID", id).AppendItem("AttendeeID", attid1)).FirstOrDefault();
                    SeatsModel s2 = SeatsAdapter.Instance.Load(p => p.AppendItem("ConferenceID", id).AppendItem("AttendeeID", attid2)).FirstOrDefault();
                    if (s1 != null && s2 != null)
                    {
                        s1.AttendeeID = attid2;
                        s2.AttendeeID = attid1;
                        using (TransactionScope ts = TransactionScopeFactory.Create())
                        {
                            SeatsAdapter.Instance.Update(s1);
                            SeatsAdapter.Instance.Update(s2);
                            ts.Complete();
                        }
                        vm.State = true;
                        vm.Message = "ok！";
                    }
                }
                else
                {
                    vm.State = false;
                    vm.Message = "交换人不能为空！";
                }
            });

            return Ok(vm);
        }

        #endregion

        #region 会议 添加座位，座位右移

        /// <summary>
        /// 会议座位右移
        /// </summary>
        /// <param name="id">会议id</param>
        /// <param name="currentSeatNo">要移动的座位号</param>
        /// <returns></returns>
        [Route("AddSeatMoveRight")]
        [HttpGet]
        public IHttpActionResult AddSeatMoveRight(string id, string currentSeatNo)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(currentSeatNo))
                {
                    ConferenceModel confer = ConferenceAdapter.Instance.Load(p => p.AppendItem("ID", id)).FirstOrDefault();
                    int row = 0, colum = 0, attRow = 0, attCloum = 0, seatFlow = 0;
                    bool isAddRow = SeatsAdapter.Instance.GetSeatModelByID(id, confer.Layout) == null ? false : true;
                    if (confer.Layout != null && confer.Layout.Split('-').Length > 1)
                    {
                        row = int.Parse(confer.Layout.Split('-')[0]);
                        colum = int.Parse(confer.Layout.Split('-')[1]);
                    }
                    else
                    {
                        vm.Message = "请添加会议布局！";
                    }
                    if (currentSeatNo.Split('-').Length > 1)
                    {
                        attRow = int.Parse(currentSeatNo.Split('-')[0]);
                        attCloum = int.Parse(currentSeatNo.Split('-')[1]);
                        if (attRow == 1)
                        {
                            seatFlow = attRow * attCloum;
                        }
                        else
                        {
                            seatFlow = ((attRow - 1) * colum) + attCloum;
                        }

                    }
                    else
                    {
                        vm.Message = "请选择要移动的座位！";
                    }

                    SeatsCollection oldSeats = SeatsAdapter.Instance.GetEntertainHallSeatByCID(id, 1);
                    SeatsCollection needUpdate = new SeatsCollection();
                    int flag = 0;
                    string seatCoordinate = "", temp_seatAddress = "";
                    for (int i = 1; i <= row; i++)
                    {
                        for (int x = 1; x <= colum; x++)
                        {
                            flag++;
                            if (flag >= seatFlow)
                            {
                                seatCoordinate = i + "-" + x;
                                SeatsModel temp = oldSeats.Find(f => f.SeatCoordinate == seatCoordinate);
                                if (temp != null)
                                {
                                    if (x == colum)
                                    {
                                        temp_seatAddress = (i + 1) + "-" + 1;
                                    }
                                    else
                                    {
                                        temp_seatAddress = i + "-" + (x + 1);
                                    }
                                    needUpdate.Add(new SeatsModel()
                                    {
                                        ID = temp.ID,
                                        ConferenceID = temp.ConferenceID,
                                        AttendeeID = temp.AttendeeID,
                                        SeatAddress = temp_seatAddress,
                                        SeatCoordinate = temp_seatAddress,
                                        SeatType = temp.SeatType,
                                        Creator = temp.Creator,
                                        CreateTime = temp.CreateTime,
                                        ValidStatus = temp.ValidStatus
                                    });
                                }
                            }
                        }


                    }
                    SeatsAdapter.Instance.UpdateSeatColl(needUpdate);
                    if (isAddRow)
                    {
                        row = isAddRow ? row + 1 : row; //没有预定不用改变会议座位布局
                        confer.Layout = row + "-" + colum;
                        ConferenceAdapter.Instance.Update(confer);
                    }
                    vm.Data1 = needUpdate.Count;
                    vm.State = true;
                    vm.Message = "ok！";
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空或当前移动座位号不能为空！";
                }
            });
            return Ok(vm);
        }

        #endregion

        #region 会议 添加座位，座位下移

        /// <summary>
        /// 会议座位右移
        /// </summary>
        /// <param name="id">会议id</param>
        /// <param name="currentSeatNo">要移动的座位号</param>
        /// <returns></returns>
        [Route("AddSeatMoveDown")]
        [HttpGet]

        public IHttpActionResult AddSeatMoveDown(string id, string currentSeatNo)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(currentSeatNo))
                {
                    ConferenceModel confer = ConferenceAdapter.Instance.Load(p => p.AppendItem("ID", id)).FirstOrDefault();
                    int row = 0, colum = 0, attRow = 0, attCloum = 0;
                    if (confer.Layout != null && confer.Layout.Split('-').Length > 1)
                    {
                        row = int.Parse(confer.Layout.Split('-')[0]);
                        colum = int.Parse(confer.Layout.Split('-')[1]);
                    }
                    else
                    {
                        vm.Message = "请添加会议布局！";
                    }
                    if (currentSeatNo.Split('-').Length > 1)
                    {
                        attRow = int.Parse(currentSeatNo.Split('-')[0]);
                        attCloum = int.Parse(currentSeatNo.Split('-')[1]);
                    }
                    else
                    {
                        vm.Message = "请选择要移动的座位！";
                    }
                    bool isAddRow = SeatsAdapter.Instance.GetSeatModelByID(id, row + "-" + attCloum) == null ? false : true;
                    SeatsCollection needUpdate = new SeatsCollection();
                    string seatCoordinate = "", temp_seatAddress = "";
                    for (int i = 1; i <= row; i++)
                    {
                        if (i >= attRow)
                        {
                            seatCoordinate = i + "-" + attCloum;
                            var temp = SeatsAdapter.Instance.GetSeatModelByID(id, seatCoordinate);
                            if (temp != null)
                            {
                                temp_seatAddress = (i + 1) + "-" + attCloum;
                                temp.SeatAddress = temp_seatAddress;
                                temp.SeatCoordinate = temp_seatAddress;
                                needUpdate.Add(temp);
                            }
                        }
                    }
                    SeatsAdapter.Instance.UpdateSeatColl(needUpdate);
                    if (isAddRow)
                    {
                        row = isAddRow ? row + 1 : row; //没有预定不用改变会议座位布局
                        confer.Layout = row + "-" + colum;
                        ConferenceAdapter.Instance.Update(confer);
                    }
                    vm.Data1 = needUpdate;
                    vm.State = true;
                    vm.Message = "ok！";
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空或当前移动座位号不能为空！";
                }
            });
            return Ok(vm);
        }

        #endregion

        #region  会议 删除座位，座位左移

        /// <summary>
        /// 删除座位，座位左移
        /// </summary>
        /// <param name="id">会议id</param>
        /// <param name="currentSeatNo">要移动的座位号</param>
        /// <returns></returns>
        [Route("DeleteSeatMoveLeft")]
        [HttpGet]

        public IHttpActionResult DeleteSeatMoveLeft(string id, string currentSeatNo)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(currentSeatNo))
                {
                    ConferenceModel confer = ConferenceAdapter.Instance.Load(p => p.AppendItem("ID", id)).FirstOrDefault();
                    int row = 0, colum = 0, attRow = 0, attCloum = 0, seatFlow = 0;
                    if (confer.Layout != null && confer.Layout.Split('-').Length > 1)
                    {
                        row = int.Parse(confer.Layout.Split('-')[0]);
                        colum = int.Parse(confer.Layout.Split('-')[1]);
                    }
                    else
                    {
                        vm.Message = "请添加会议布局！";
                    }
                    if (currentSeatNo.Split('-').Length > 1)
                    {
                        attRow = int.Parse(currentSeatNo.Split('-')[0]);
                        attCloum = int.Parse(currentSeatNo.Split('-')[1]);
                        if (attRow == 1)
                        {
                            seatFlow = attRow * attCloum;
                        }
                        else
                        {
                            seatFlow = ((attRow - 1) * colum) + attCloum;
                        }

                    }
                    else
                    {
                        vm.Message = "请选择要移动的座位！";
                    }

                    SeatsCollection oldSeats = SeatsAdapter.Instance.GetEntertainHallSeatByCID(id, 1);
                    SeatsCollection needUpdate = new SeatsCollection();
                    int flag = 0;
                    string seatCoordinate = "", temp_seatAddress = "", temp_seatCoordinate = "";
                    for (int i = 1; i <= row; i++)
                    {
                        for (int x = 1; x <= colum; x++)
                        {
                            flag++;
                            if (flag >= seatFlow)
                            {
                                seatCoordinate = i + "-" + x;
                                SeatsModel temp = oldSeats.Find(f => f.SeatCoordinate == seatCoordinate);
                                if (temp != null)
                                {

                                    if (i == 1 && x == 1)
                                    {
                                        temp_seatAddress = "";
                                        temp_seatCoordinate = temp.SeatCoordinate;
                                    }
                                    else if (seatCoordinate == currentSeatNo)
                                    {
                                        temp_seatAddress = "";
                                        temp_seatCoordinate = temp.SeatCoordinate;
                                    }
                                    else if (x == 1)
                                    {
                                        temp_seatAddress = (i - 1) + "-" + colum;
                                        temp_seatCoordinate = (i - 1) + "-" + colum;
                                    }
                                    else
                                    {
                                        temp_seatAddress = i + "-" + (x - 1);
                                        temp_seatCoordinate = i + "-" + (x - 1);
                                    }
                                    needUpdate.Add(new SeatsModel()
                                    {
                                        ID = temp.ID,
                                        ConferenceID = temp.ConferenceID,
                                        AttendeeID = temp.AttendeeID,
                                        SeatAddress = temp_seatAddress,
                                        SeatCoordinate = temp_seatCoordinate,
                                        SeatType = temp.SeatType,
                                        Creator = temp.Creator,
                                        CreateTime = temp.CreateTime,
                                        ValidStatus = temp.ValidStatus
                                    });
                                }
                            }
                        }

                    }
                    SeatsAdapter.Instance.UpdateSeatColl(needUpdate);
                    vm.Data1 = needUpdate.Count;
                    vm.State = true;
                    vm.Message = "ok！";
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空或当前移动座位号不能为空！";
                }
            });
            return Ok(vm);
        }

        #endregion

        #region 会议 删除座位，座位上移

        /// <summary>
        ///  删除座位，座位上移
        /// </summary>
        /// <param name="id">会议id</param>
        /// <param name="currentSeatNo">要移动的座位号</param>
        /// <returns></returns>
        [Route("DeleteSeatMoveTop")]
        [HttpGet]

        public IHttpActionResult DeleteSeatMoveTop(string id, string currentSeatNo)
        {
            SeatsViewModel vm = new SeatsViewModel();
            ControllerHelp.RunAction(() =>
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(currentSeatNo))
                {
                    ConferenceModel confer = ConferenceAdapter.Instance.Load(p => p.AppendItem("ID", id)).FirstOrDefault();
                    int row = 0, colum = 0, attRow = 0, attCloum = 0;
                    if (confer.Layout != null && confer.Layout.Split('-').Length > 1)
                    {
                        row = int.Parse(confer.Layout.Split('-')[0]);
                        colum = int.Parse(confer.Layout.Split('-')[1]);
                    }
                    else
                    {
                        vm.Message = "请添加会议布局！";
                    }
                    if (currentSeatNo.Split('-').Length > 1)
                    {
                        attRow = int.Parse(currentSeatNo.Split('-')[0]);
                        attCloum = int.Parse(currentSeatNo.Split('-')[1]);
                    }
                    else
                    {
                        vm.Message = "请选择要移动的座位！";
                    }
                    bool isAddRow = SeatsAdapter.Instance.GetSeatModelByID(id, row + "-" + attCloum) == null ? false : true;
                    SeatsCollection needUpdate = new SeatsCollection();
                    string seatCoordinate = "", temp_seatAddress = "", temp_seatCoordinate = "";
                    for (int i = 1; i <= row; i++)
                    {
                        if (i >= attRow)
                        {

                            seatCoordinate = i + "-" + attCloum;
                            var temp = SeatsAdapter.Instance.GetSeatModelByID(id, seatCoordinate);
                            if (temp != null)
                            {
                                if (i == 1 && attCloum == 1)
                                {
                                    temp_seatAddress = "";
                                    temp_seatCoordinate = temp.SeatCoordinate;
                                }
                                else if (seatCoordinate == currentSeatNo)
                                {
                                    temp_seatAddress = "";
                                    temp_seatCoordinate = temp.SeatCoordinate;
                                }
                                else
                                {
                                    temp_seatAddress = (i - 1) + "-" + attCloum;
                                    temp_seatCoordinate = (i - 1) + "-" + attCloum;
                                }
                                temp.SeatAddress = temp_seatAddress;
                                temp.SeatCoordinate = temp_seatCoordinate;
                                needUpdate.Add(temp);
                            }
                        }
                    }
                    SeatsAdapter.Instance.UpdateSeatColl(needUpdate);
                    vm.Data1 = needUpdate.Count;
                    vm.State = true;
                    vm.Message = "ok！";
                }
                else
                {
                    vm.State = false;
                    vm.Message = "会议编码不能为空或当前移动座位号不能为空！";
                }
            });
            return Ok(vm);
        }

        #endregion
    }
}
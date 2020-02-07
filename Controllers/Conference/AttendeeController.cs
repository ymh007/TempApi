using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AddressBook;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.AddressBook;
using Seagull2.YuanXin.AppApi.ViewsModel.Conference;

namespace Seagull2.YuanXin.AppApi.Controllers
{

    /// <summary>
    /// 参会人控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        #region 根据ID查询查询参会人
        /// <summary>
        /// 根据ID查询查询参会人
        /// </summary>
        /// <param name="attendeeID">参会人编码</param>
        [Route("GetAttendeeListByConferenceID")]
        [HttpGet]
        public IHttpActionResult GetAttendeeListByConferenceID(string attendeeID)
        {
            AttendeeModel attModel = AttendeeAdapter.Instance.LoadByID(attendeeID);
            return Ok(attModel);
        }
        #endregion

        #region 查询参会人列表
        /// <summary>
        /// 查询参会人列表
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="searchTime">首页搜索时间</param>
        /// <param name="attendeeName">参会人姓名</param>
        /// <param name="isJoinPrize">是否加入抽奖</param>
        [Route("GetAttendeeListByConferenceID")]
        [HttpGet]
        public IHttpActionResult GetAttendeeListByConferenceID(string conferenceID, int pageIndex, string searchTime = "", string attendeeName = "", bool isJoinPrize = false, int attendeeType = 1)
        {
            ViewPageBase<List<AttendeeList>> attendeeList = new ViewPageBase<List<AttendeeList>>();
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                attendeeList = AttendeeListAdapter.Instance.GetAttendeeListView(conferenceID, attendeeType, pageIndex, DateTime.Now.AddHours(1), attendeeName, isJoinPrize);
            });
            return Ok(attendeeList);
        }
        #endregion

        #region 查询某一个会议下面 参会人列表 全部
        /// <summary>
        /// 查询某一个会议下面 参会人列表 全部
        /// </summary>
        /// <param name="conferenceID">会议id</param>
        /// <param name="attendeeType">会议类型</param>
        /// <returns></returns>
        [Route("GetAttendeeAllConferenceID")]
        [HttpPost]
        public IHttpActionResult GetAttendeeAllConferenceID(AttendeeView parment)
        {
            var result = ControllerService.Run(() =>
            {
               return  AttendeeAdapter.Instance.GetAttendeeCollectionByConference(parment.ConferenceID, parment.AttendeeType,parment.Name);
            });
            return Ok(result);
        }
        #endregion




        #region 清空参会人员列表
        /// <summary>
        /// 清空参会人员列表
        /// </summary>
        [HttpGet]
        [Route("ClearAttendee")]
        public IHttpActionResult ClearAttendee(string id, int AttendeeType=1)
        {

            var result = ControllerService.Run(() =>
            {
                AttendeeAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("ConferenceID", id);
                    m.AppendItem("AttendeeType", AttendeeType);
                });
                SeatsAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("ConferenceID", id);
                    m.AppendItem("SeatType", AttendeeType);
                });
            });
            return Ok(result);
        }
        #endregion

        #region 手动添加参会人员列表
        /// <summary>
        /// 手动添加参会人员列表
        /// </summary>
        [HttpPost]
        [Route("AddAttendeeList")]
        public IHttpActionResult AddAttendeeList(List<UserInfoViewModel> userList,string ConferenceID,int AttendeeType)
        {

            var result = ControllerService.Run(() =>
            {
                userList.ForEach(item => {
                    //获取当前存入的人员
                    AttendeeModel attendee = AttendeeAdapter.Instance.GetByEmailAndConferenceID(item.Email.Trim(), ConferenceID, AttendeeType);
                    ContactsModel contacts = ContactsAdapter.Instance.LoadByMail(item.Email);
                    if (attendee==null) {
                        //证明该参会人员不是该参会人员

                        attendee = new AttendeeModel()
                        {
                            ID = Guid.NewGuid().ToString(),
                            AttendeeID = contacts == null ? "" : contacts.ObjectID,
                            ConferenceID = ConferenceID,
                            AttendeeType = AttendeeType,
                            Name = item.DisplayName,
                            Email = item.Email.Trim(),
                            MobilePhone = contacts == null ? "" : contacts.MP,
                            PhotoAddress = contacts == null ? "" : UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID),
                            OrganizationID = contacts == null ? "" : contacts.ObjectID,
                            OrganizationStructure = item.FullPath,
                            Creator = CurrentUserCode,
                            CreateTime = DateTime.Now,
                            ValidStatus = true,
                            CityName = "",
                            IsJoinPrize = false
                        };

                    }

                    SeatsModel seat = SeatsAdapter.Instance.GetUserSeatModel(attendee.ID, ConferenceID);
                    if (seat == null)
                    {
                        seat = new SeatsModel()
                        {
                            ID = Guid.NewGuid().ToString(),
                            ConferenceID = ConferenceID,
                            AttendeeID = attendee.ID,
                            SeatAddress = "",
                            SeatType =AttendeeType,
                            Creator = CurrentUserCode,
                            CreateTime = DateTime.Now,
                            ValidStatus = true
                        };
                    }

                    //参会人入库
                    AttendeeAdapter.Instance.Update(attendee);
                    //座位信息入库
                    SeatsAdapter.Instance.Update(seat);


                });


            });
            return Ok(result);
        }
        #endregion

        #region 删除人员
        /// <summary>
        /// 删除人员
        /// </summary>
        [HttpGet]
        [Route("DeleteAttendee")]
        public IHttpActionResult DeleteAttendee(string id, int AttendeeType)
        {

            var result = ControllerService.Run(() =>
            {
                AttendeeAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("ID", id);
                    m.AppendItem("AttendeeType", AttendeeType);
                });


                SeatsAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("AttendeeID", id);
                    m.AppendItem("SeatType", AttendeeType);
                });
            });
            return Ok(result);
        }
        #endregion

        #region 添加座位
        /// <summary>
        /// 添加座位列表
        /// </summary>
        [HttpPost]
        [Route("AddAttendeeSeatList")]
        public IHttpActionResult AddAttendeeSeatList(List<AttendeeModel> userList, string ConferenceID,int Row,int Line, int AttendeeType = 1)
        {

            var result = ControllerService.Run(() =>
            {
                //清空改会议类型的行也排座位的人员（）

                SeatsAdapter.Instance.ClearLikeSeat( ConferenceID, Row.ToString());
                var num = 0;
                var seatColl = new SeatsCollection();
                userList.ForEach(item => {
                    //获取当前存入的人员
                    num++;
                var seatPeople = SeatsAdapter.Instance.Load(m =>
                {
                    m.AppendItem("[AttendeeID]", item.ID);
                    m.AppendItem("[SeatType]", AttendeeType);
                }).FirstOrDefault();
                    if (seatPeople!=null) {
                        seatPeople.SeatAddress = item.SeatAddress1;
                        seatPeople.SeatCoordinate = item.SeatAddress1;
                        //seatPeople.SeatAddress = Row + "-" + num;
                        //seatPeople.SeatCoordinate = Row + "-" + num;
                        //seatColl.Add(seatPeople);
                        SeatsAdapter.Instance.Update(seatPeople);
                    }
                });
               // SeatsAdapter.Instance.SeatPersonInsert(seatColl);

            });
            return Ok(result);
        }
        #endregion

        #region 该行已排座位和未排座位的集合
        /// <summary>
        ///该行已排座位和未排座位的集合
        /// </summary>
        [HttpGet]
      
        public IHttpActionResult HasSeatAndNoSeat( string ConferenceID, int Row, string where = "", int AttendeeType = 1)
        {

            var result = ControllerService.Run(() =>
            {
              
            var list=SeatsAdapter.Instance.GetHasRowAndNoSeat(ConferenceID, Row,where);

                return list;

            });
            return Ok(result);
        }
        #endregion

        #region 查询会议编码获取所有参会人
        /// <summary>
        /// 查询会议编码获取所有参会人
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="isJoinPrize">是否只获取加入抽奖名单的参会人</param>
        [Route("GetAllAttendeeByConferenceID")]
        [HttpGet]
        public IHttpActionResult GetAllAttendeeByConferenceID(string conferenceID, bool isJoinPrize = false, bool isForAppWeb = false, int attendeeType = 1)
        {
            List<AttendeeList> attList = new List<AttendeeList>();
            ControllerHelp.SelectAction(() =>
            {
                attList = AttendeeListAdapter.Instance.GetAllAttendeeList(conferenceID, isJoinPrize, isForAppWeb);
            });
            return Ok(attList);
        }
        #endregion

        #region 查询会议编码获取所有已选座位参会人
        /// <summary>
        /// 查询会议编码获取所有已选座位参会人
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        [Route("GetAllHasSeatAttendeeByConferenceID")]
        [HttpGet]
        public IHttpActionResult GetAllHasSeatAttendeeByConferenceID(string conferenceID, int AttendeeType = 1)
        {
            DataTable attList = new DataTable();
            ControllerHelp.SelectAction(() =>
            {
                attList = AttendeeListAdapter.Instance.GetAllHasSeatAttendeeList(conferenceID, AttendeeType);
            });

            ConferenceModel model = new ConferenceModel();
            ControllerHelp.SelectAction(() =>
            {
                model = ConferenceAdapter.Instance.GetTByID(conferenceID);
            });
            var not_att = AttendeeListAdapter.Instance.GetAllNoChooseSeatAttendeeList(conferenceID, "", AttendeeType).Count;
            dynamic result = new
            {
                NotAtten = not_att,
                AlreadyAtten = attList.Rows.Count,
                AttendaList = attList,
                LayOut = model.Layout
            };
            return Ok(result);
        }


        #endregion

        #region 查询会议编码获取所有已选座位参会人 带模糊查询的
        /// <summary>
        /// 查询会议编码获取所有已选座位参会人
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        [Route("SearchHasSeatAttendee")]
        [HttpGet]
        public IHttpActionResult SearchHasSeatAttendee(string conferenceID, int AttendeeType = 1, string key = "")
        {
            DataTable attList = new DataTable();
            ControllerHelp.SelectAction(() =>
            {
                attList = AttendeeListAdapter.Instance.GetAllHasSeatAttendeeList(conferenceID, AttendeeType, key);
            });
            dynamic result = new
            {
                AttendaList = attList,
            };
            return Ok(result);
        }


        #endregion

        #region 查询所有已选座位宴请人
        /// <summary>
        /// 查询会议编码获取所有已选座位宴请人
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        [Route("GetAllHasSeatAttendeeByEntertainHall")]
        [HttpGet]
        public IHttpActionResult GetAllHasSeatAttendeeByEntertainHall(string conferenceID, int AttendeeType = 2)
        {
            DataTable attList = new DataTable();
            ControllerHelp.SelectAction(() =>
            {
                attList = AttendeeListAdapter.Instance.GetAllHasSeatAttendeeList(conferenceID, AttendeeType);
            });

            var not_att = AttendeeListAdapter.Instance.GetAllNoChooseSeatAttendeeList(conferenceID, "", AttendeeType);
            List<dynamic> DestopAndPersons = new List<dynamic>();
            var query = from t in attList.AsEnumerable()
                        group t by new { t1 = t.Field<string>("SeatAddress") } into m
                        select new { SeatAddress = m.Key.t1, persons = m.AsEnumerable() };
            query.OrderBy(o => o.SeatAddress);
            int sortNum = 0;
            query.ToList().ForEach(q =>
            {
                List<AttendeeList> list = new List<AttendeeList>();
                q.persons.ToList().ForEach(f =>
                {
                    list.Add(new AttendeeList()
                    {
                        ID = f["ID"].ToString(),
                        Name = f["Name"].ToString(),
                        Email = f["Email"].ToString(),
                        AttendeeID = f["AttendeeID"].ToString(),
                        SeatCoordinate = f["SeatCoordinate"].ToString(),
                        OrganizationStructure=f["OrganizationStructure"].ToString(),
                    });
                });
                int.TryParse(q.SeatAddress, out sortNum);
                var temp = new
                {
                    sort = sortNum,
                    desktopNum = q.SeatAddress,
                    persons = list
                };
                DestopAndPersons.Add(temp);
            });
            dynamic result = new
            {
                not_attCount = not_att.Count,
                NotAtten = not_att,
                AlreadyAtten = attList.Rows.Count,
                AttendaList = DestopAndPersons.OrderBy(o => o.sort).ToList(),
            };
            return Ok(result);
        }


        #endregion

        #region 查询会议编码获取所有未选座位参会人
        /// <summary>
        /// 查询会议编码获取所有未选座位参会人
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="userName">人员姓名</param>
        /// <param name="AttendeeType">人员类型</param>
        [Route("GetAllNoChooseSeatAttendeeByConferenceID")]
        [HttpGet]
        public IHttpActionResult GetAllNoChooseSeatAttendeeByConferenceID(string conferenceID, string userName = "", int attendeeType = 1)
        {
            List<AttendeeList> attList = new List<AttendeeList>();
            ControllerHelp.SelectAction(() =>
            {
                attList = AttendeeListAdapter.Instance.GetAllNoChooseSeatAttendeeList(conferenceID, userName, attendeeType);
            });
            return Ok(attList);
        }
        #endregion

        #region 将人员和座位绑定
        /// <summary>
        /// 更新座位信息
        /// </summary>
        [Route("UpdateSeat")]
        [HttpPost]
        public IHttpActionResult UpdateSeat(AttendeeSeatView post)
        {
            ViewModelBase result = new ViewModelBase();
            string Data1 = "";
            result = ControllerHelp.RunAction(() =>
            {
                post.SeatType = post.SeatType == 0 ? 1 : 1;
                //座位信息
                SeatsModel seat = SeatsAdapter.Instance.GetUserSeatModel(post.AttendeeID, post.ConferenceID, post.SeatType);
                if (seat.SeatAddress != "")
                {
                    Data1 = "已选择";
                }
                else
                {
                    //判断当前座位是否已被禁用
                    DisableSeatsModel Disseat = DisableSeatsAdapter.Instance.GetDisSeatById(post.ConferenceID, post.SeatCoordinateNo);
                    if (Disseat == null)
                    {
                        seat.SeatAddress = post.SeatNo;
                        seat.SeatCoordinate = post.SeatCoordinateNo;
                        seat.AttendeeID = post.AttendeeID;
                        //座位信息入库
                        SeatsAdapter.Instance.Update(seat);
                    }
                    else
                    {
                        result.Data1 = "已禁用";
                    }

                }
            });
            if (Data1 == "已选择")
            {
                result.Data1 = Data1;
            }
            return Ok(result);
        }
        #endregion

        #region 将人员移除座位
        /// <summary>
        /// 更新座位信息
        /// </summary>
        [Route("RemoveSeat")]
        [HttpPost]
        public IHttpActionResult RemoveSeat(AttendeeSeatView post)
        {
            ViewModelBase result = new ViewModelBase();
            if (string.IsNullOrEmpty(post.AttendeeID)) throw new Exception("参会人不能为空");
            result = ControllerHelp.RunAction(() =>
            {
                //座位信息
                SeatsModel seat = SeatsAdapter.Instance.GetUserSeatModel(post.AttendeeID, post.ConferenceID, post.SeatType);
                seat.SeatAddress = "";
                seat.SeatCoordinate = "";
                //座位移除
                SeatsAdapter.Instance.Update(seat);
            });
            return Ok(result);
        }
        #endregion

        #region 判断当前会议是否存在该人员
        /// <summary>
        /// 判断当前会议是否存在该人员
        /// </summary>
        /// <param name="attHelp">参会人实体</param>
        [Route("IsContainsAttendee")]
        [HttpPost]
        public bool IsContainsAttendee(AttendeeView attHelp)
        {
            AttendeeModel attModel = AttendeeAdapter.Instance.IsContainsAttendee(attHelp.Email, attHelp.ConferenceID);
            if (attModel == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 将参会人置为无效/有效--PC
        /// <summary>
        /// 将参会人置为无效/有效--PC
        /// </summary>
        /// <param name="id">参会人id</param>
        /// <param name="isDel">true：置为无效，false：置为有效</param>
        [Route("DelAttendee")]
        [HttpGet]
        public IHttpActionResult DelOrUnDelAttendee(string id, bool isDel = true)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                AttendeeAdapter.Instance.UpdateStatus(id, isDel == true ? false : true);
                if (isDel)
                {
                    //将该参会人员的座位号字段置为空
                    var attModel = AttendeeAdapter.Instance.LoadByID(id);
                    if (attModel != null)
                    {
                        var seatModel = SeatsAdapter.Instance.Load(
                            m => m.AppendItem("AttendeeID", attModel.ID).AppendItem("ConferenceID", attModel.ConferenceID)).SingleOrDefault();
                        if (seatModel != null)
                        {
                            seatModel.SeatAddress = "";
                            SeatsAdapter.Instance.Update(seatModel);
                        }
                    }
                }
            });
            return Ok(result);
        }
        #endregion

        #region 增加或更新参会人--PC
        /// <summary>
        /// 增加或更新参会人--PC
        /// 最新需求：手动添加不校验海鸥二邮箱，如果通讯录找不到该人，则默认为非海鸥二人员
        /// </summary>
        [Route("AddOrUpdateAttendee")]
        [HttpPost]
        public IHttpActionResult AddOrUpdateAttendee(AttendeeView post)
        {
            ViewModelBase result = new ViewModelBase();

            result = ControllerHelp.RunAction(() =>
            {
                if (post.Email.IsEmptyOrNull())
                {
                    throw new Exception("邮箱不能为空");
                }
                //根据邮箱和会议编码去找参会人
                AttendeeModel attendee = AttendeeAdapter.Instance.GetByEmailAndConferenceID(post.Email.Trim(), post.ConferenceID, post.AttendeeType);
                //根据邮箱获取通讯录信息
                ContactsModel contacts = ContactsAdapter.Instance.LoadByMail(post.Email);

                //已经存在参会人
                if (attendee != null)
                {

                    attendee.Name = post.Name;
                    attendee.Email = post.Email.Trim();
                    attendee.MobilePhone = contacts == null ? "" : contacts.MP;
                    attendee.CityName = post.CityName;
                    attendee.AttendeeType = post.AttendeeType;
                    attendee.PhotoAddress = contacts == null ? "" : UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID);
                }
                //不存在
                else
                {
                    attendee = new AttendeeModel()
                    {
                        ID = string.IsNullOrEmpty(post.AttendeeID) ? Guid.NewGuid().ToString() : post.AttendeeID,
                        AttendeeID = contacts == null ? "" : contacts.ObjectID,
                        ConferenceID = post.ConferenceID,
                        AttendeeType = post.AttendeeType,
                        Name = post.Name,
                        Email = post.Email.Trim(),
                        MobilePhone = contacts == null ? "" : contacts.MP,
                        PhotoAddress = contacts == null ? "" : UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID),
                        OrganizationID = contacts == null ? "" : contacts.ObjectID,
                        OrganizationStructure = contacts == null ? "" : contacts.FullPath,
                        Creator = CurrentUserCode,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                        CityName = post.CityName,
                        IsJoinPrize = false
                    };
                }

                //座位信息
                SeatsModel seat = SeatsAdapter.Instance.GetUserSeatModel(attendee.ID, post.ConferenceID);
                if (seat == null)
                {
                    seat = new SeatsModel()
                    {
                        ID = Guid.NewGuid().ToString(),
                        ConferenceID = post.ConferenceID,
                        AttendeeID = attendee.ID,
                        SeatAddress = "",
                        SeatType = post.AttendeeType,
                        Creator = CurrentUserCode,
                        CreateTime = DateTime.Now,
                        ValidStatus = true
                    };
                }
                else
                {
                    seat.SeatAddress = post.AttendeeSeat;
                    seat.SeatType = post.AttendeeType;
                }

                //参会人入库
                AttendeeAdapter.Instance.Update(attendee);
                //座位信息入库
                SeatsAdapter.Instance.Update(seat);

                //将人员加入IM群组
                if (!string.IsNullOrWhiteSpace(attendee.AttendeeID))
                {
                    try
                    {
                        var dialogs = ConferenceCompanyDialogAdapter.Instance.LoadByConferenceID(post.ConferenceID);
                        foreach (var dialog in dialogs)
                        {
                            if (dialog.DialogTypeName == "IM")
                            {
                                var groupId = Convert.ToInt32(dialog.DialogCode);
                                var userId = IMService.GetUserId(attendee.AttendeeID);
                                IMService.AddMember(groupId, new long[] { userId });
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error(JsonConvert.SerializeObject(e));
                    }
                }
            });
            return Ok(result);
        }
        #endregion

        #region 参会人年会（参与/取消）抽奖
        /// <summary>
        /// 参会人年会（参与/取消）抽奖
        /// </summary>
        [HttpPost]
        [Route("UpdateIsJoinPrize")]
        public IHttpActionResult UpdateIsJoinPrize()
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                //ture--发布上线，false--下线
                bool IsJoinPrize = Convert.ToBoolean(baseRequest.Form["IsJoinPrize"]);
                string attendeeId = baseRequest.Form["AttendeeId"];

                AttendeeModel attModel = AttendeeAdapter.Instance.LoadByID(attendeeId);
                attModel.IsJoinPrize = IsJoinPrize;
                AttendeeAdapter.Instance.Update(attModel);
            });
            return Ok(result);
        }
        #endregion

        #region 获取会议下参会人城市的名称的集合和议程签到数量
        /// <summary>
        /// 获取会议下参会人城市的名称的集合和议程签到数量
        /// </summary>
        [HttpGet]
        [Route("GetAttendeeCityNameColl"), AllowAnonymous]
        public IHttpActionResult GetAttendeeCityNameColl(string conferenceID)
        {
            //获取城市列表
            List<string> cityNamelist = new List<string>();
            ControllerHelp.SelectAction(() =>
            {
                List<AttendeeList> attList = AttendeeListAdapter.Instance.GetAllAttendeeList(conferenceID, false, true);
                attList.ForEach(att =>
                {
                    if (!att.CityName.IsEmptyOrNull() && !cityNamelist.Contains(att.CityName))
                    {
                        cityNamelist.Add(att.CityName);
                    }
                });
            });
            //获取签到数量
            ConferenceAgendaCollection agendaColl = ConferenceAgendaAdapter.Instance.Load(m => m.AppendItem("ConferenceID", conferenceID));
            ConferenceAgendaModel needSignAgeda = null;
            DateTime now = DateTime.Now;
            foreach (var agenda in agendaColl)
            {
                if (agenda.NeedSign && now > agenda.SignBeginDate && now < agenda.SingEndDate)
                {
                    needSignAgeda = agenda;
                    break;
                }
            };
            //签到大图
            string backgroundImageUrl = string.Empty;
            var setting = SignInSettingAdapter.Instance.GetSignInSettingByConferenceID(conferenceID);
            if (setting != null)
            {
                backgroundImageUrl = ConfigurationManager.AppSettings["ConferenceImageDownLoadRootPath"] + "/" + setting.BackgroundImageName;
            }
            //获取会议地址
            //var address = ConferenceAdapter.Instance.Load(m => m.AppendItem("ID", conferenceID)).SingleOrDefault().Address;
            //int leng = address.IndexOf("市");
            //address = address.Substring(0, leng);
            var model = SignInSettingAdapter.Instance.Load(m => m.AppendItem("ConferenceID", conferenceID)).SingleOrDefault();
            dynamic result = new
            {
                MatchcodeSignCount = needSignAgeda == null ? 0 : SignInAdapter.Instance.SignInCount(needSignAgeda.ID),
                CityNamelist = cityNamelist,
                BackgroundImageUrl = backgroundImageUrl,
                Model = model
            };

            return Ok(result);
        }
        #endregion

        #region 参会人模板导入--PC
        /// <summary>
        /// 参会人模板导入--PC
        /// </summary>
        [Route("ImportExcelDataForBusRouteAttendee")]
        [HttpPost]
        public IHttpActionResult ImportExcelDataForBusRouteAttendee()
        {
            //需求场景：1-参会人导入只考虑：海鸥二人员
            //2-非海鸥二人员暂不允许导入，可以通过手动添加方式加入
            // 新加参会人类型  1 参会人  2 宴请人 其他逻辑不变
            //导入失败邮箱集合
            List<string> errorEmailList = new List<string>();

            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                string conferenceID = baseRequest.Form["ConferenceID"].ToString();
                int attendeeType = 1;
                int.TryParse(baseRequest.Form["AttendeeType"] == null ? "1" : baseRequest.Form["AttendeeType"].ToString(), out attendeeType);
                DataTable dataTable = ExcelHelp<AttendeeModel, List<AttendeeModel>>.GetExcelData("Attendee");

                #region 业务逻辑：批量导入只考虑新增和更新  查询条件要改变
                AttendeeCollection oldColl = AttendeeAdapter.Instance.GetAttendeeCollectionByConference(conferenceID, attendeeType);

                //参会人集合
                AttendeeCollection isNeedUpdateColl = new AttendeeCollection();
                //座位集合
                SeatsCollection isNeedUpdateSeatColl = new SeatsCollection();

                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow row = dataTable.Rows[i];

                    string SeatAddress = "";
                    string Attendaemail = row["邮箱"].ToString().Trim();

                    if (Attendaemail != "")
                    {
                        if (attendeeType == 2)
                        {
                            SeatAddress = row["桌号"].ToString().Trim();
                        }
                        if (attendeeType == 1)
                        {
                            SeatAddress = row["座位号"].ToString().Trim();
                        }
                        //参会人导入只考虑：海鸥二人员
                        ContactsModel contacts = ContactsAdapter.Instance.LoadByMail(Attendaemail);
                        if (contacts == null)
                        {
                            errorEmailList.Add((string)Attendaemail);
                        }
                        else
                        {
                            if (isNeedUpdateColl.Exists(aa => aa.Email.Trim() == row["邮箱"].ToString().Trim()) == false)
                            {
                                #region 准备需要更新的参会人信息

                                AttendeeModel oldAtt = oldColl.Find(old => old.Email == Attendaemail.Trim() && old.ConferenceID == conferenceID);
                                string attID = oldAtt == null ? Guid.NewGuid().ToString() : oldAtt.ID;
                                AttendeeModel model = new AttendeeModel
                                {
                                    ID = attID,
                                    Creator = CurrentUserCode,
                                    ValidStatus = true,
                                    CreateTime = DateTime.Now,
                                    ConferenceID = conferenceID,
                                    AttendeeType = attendeeType,
                                    Name = contacts.DisplayName.ToString(),
                                    Email = Attendaemail.Trim(),
                                    MobilePhone = contacts.MP.ToString(),
                                    OrganizationStructure = contacts.FullPath,
                                    PhotoAddress = UserHeadPhotoService.GetUserHeadPhoto(contacts.ObjectID),
                                    AttendeeID = contacts.ObjectID,
                                    OrganizationID = contacts.ParentID,
                                    CityName = row["城市"] == null ? "" : row["城市"].ToString()
                                };
                                isNeedUpdateColl.Add(model);
                                #endregion

                                #region 准备需要更新的座位信息
                                SeatsAdapter.Instance.DelUserSeat(conferenceID, model.ID, attendeeType);
                                SeatsModel seatModel = new SeatsModel()
                                {
                                    ID = Guid.NewGuid().ToString(),
                                    AttendeeID = model.ID,
                                    ConferenceID = conferenceID,
                                    SeatType = attendeeType,
                                    CreateTime = DateTime.Now,
                                    Creator = CurrentUserCode,
                                    SeatAddress = SeatAddress,
                                    SeatCoordinate = attendeeType == 2 ? "" : SeatAddress,
                                    ValidStatus = true
                                };
                                isNeedUpdateSeatColl.Add(seatModel);
                                #endregion
                            }
                        }
                    }
                }

                if (isNeedUpdateColl.Count > 0)
                {
                    Log.WriteLog("本次需要更新的人员集合：" + JsonConvert.SerializeObject(isNeedUpdateColl));
                    //参会人入库
                    AttendeeAdapter.Instance.UpdateAttendeeColl(isNeedUpdateColl);
                }
                if (isNeedUpdateSeatColl.Count > 0)
                {
                    Log.WriteLog("本次需要更新的人员座位集合：" + JsonConvert.SerializeObject(isNeedUpdateSeatColl));
                    //座位信息入库
                    SeatsAdapter.Instance.UpdateSeatColl(isNeedUpdateSeatColl);
                }
                #endregion
            });
            result.Data1 = errorEmailList;
            return Ok(result);
        }
        #endregion

        #region 参会人数据导出成EXCEL--PC
        /// <summary>
        /// 参会人数据导出成EXCEL--PC
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ExportExcelDataForAttendee(string conferenceID, int attendeeType = 1)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            ControllerHelp.RunAction(() =>
            {
                string exportFileName = attendeeType == 1 ? "Attendee" : "Entertain";
                AttendeeCollection atColl = AttendeeAdapter.Instance.GetAttendeeCollectionByConference(conferenceID, attendeeType);
                Dictionary<string, string> dicColl = null;
                if (attendeeType == 1)
                {
                    dicColl = new Dictionary<string, string>() {
                    {"姓名","Name" },  {"邮箱","Email" },
                    {"电话","MobilePhone" },
                    {"座位号","SeatAddress" },
                    {"城市","CityName" }  };
                }
                else
                {
                    dicColl = new Dictionary<string, string>() {
                    {"桌号","SeatAddress" },
                    { "姓名","Name" },
                    { "邮箱","Email" },
                    {"电话","MobilePhone" },
                    {"城市","CityName" }  };
                }

                result = ExcelHelp<AttendeeModel, List<AttendeeModel>>.ExportExcelData(dicColl, atColl.ToList(), exportFileName);
            });
            return result;
        }
        #endregion

        #region 根据会议编码查询所有有效的参会人员
        /// <summary>
        /// 根据会议编码查询所有有效的参会人员--为了某次的会议
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetAttendeeList(string conferenceID)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(conferenceID)) { throw new Exception("无效的参数conferenceID"); }
                AttendeeCollection coll = AttendeeAdapter.Instance.Load(m =>
                {
                    m.AppendItem("ConferenceID", conferenceID);
                    m.AppendItem("ValidStatus", true);
                });
                if (coll.Count == 0) { throw new Exception("无有效数据"); }
                List<SignAttendee> list = new List<SignAttendee>();
                coll.ForEach(m =>
                {
                    var item = new SignAttendee();
                    var departList = m.OrganizationStructure.Split('\\').ToList();
                    departList.RemoveRange(0, departList.Count - 3);
                    m.OrganizationStructure = string.Join("\\", departList.ToArray());
                    item.Code = m.AttendeeID;
                    item.Photo = UserHeadPhotoService.GetUserHeadPhoto(m.AttendeeID);
                    item.Name = m.Name;
                    item.Department = m.OrganizationStructure;
                    list.Add(item);
                });
                return list;
            });
            return Ok(result);
        }
        #endregion
    }



    #region 帮助类
    /// <summary>
    /// 参会人实体类
    /// </summary>
    public class AttendeeView
    {
        /// <summary>
        /// 参会人编码（修改）
        /// </summary>
        public string AttendeeID { get; set; }
        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 参会人座位
        /// </summary>
        public string AttendeeSeat { get; set; }
        /// <summary>
        /// 城市名称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 参会人类型
        /// </summary>
        public int AttendeeType { get; set; }
    }


    /// <summary>
    /// 座位人员实体类
    /// </summary>
    public class AttendeeSeatView
    {
        /// <summary>
        /// 参会人编码（修改）
        /// </summary>
        public string AttendeeID { get; set; }
        /// <summary>
        /// 会议编码
        /// </summary>
        public string ConferenceID { get; set; }
        /// <summary>
        /// 座位号
        /// </summary>
        public string SeatNo { get; set; }

        /// <summary>
        ///座位实际对应的分布图坐标编号
        /// </summary>
        public string SeatCoordinateNo { get; set; }

        /// <summary>
        /// 座位类型
        /// </summary>
        public int SeatType { get; set; }
    }



    #endregion
}




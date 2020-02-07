using MCS.Library.Data;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Activity;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Adapter.ShareFile;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.Models.ShareFile;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Conference;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 会议API控制器
    /// </summary>
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 会议图片模块名称
        /// </summary>
        string ShareFileModule = "ConferenceImage";

        #region 获取会议列表 - APP
        /// <summary>
        /// 获取会议列表 - APP
        /// </summary>
        [HttpGet]
        [Route("GetConferenceListForApp")]
        public IHttpActionResult GetConferenceListForApp(int pageIndex, string type)
        {
            var result = ControllerService.Run(() =>
            {
                var pageSize = 10;
                var user = (Seagull2Identity)User.Identity;
                var list = new ConferenceModelCollection();
                if (type == "my")
                {
                    list = ConferenceAdapter.Instance.GetList(pageSize, pageIndex, user.Id);
                }
                else if (type == "all")
                {
                    list = ConferenceAdapter.Instance.GetList(pageSize, pageIndex);
                }
                else
                {
                    throw new Exception("参数type的值不在允许范围内！");
                }
                var view = new List<ConferenceListAppViewModel>();
                list.ForEach(item =>
                {
                    view.Add(new ConferenceListAppViewModel()
                    {
                        Code = item.ID,
                        Name = item.Name,
                        Image = item.ShowImage,
                        StartDate = item.BeginDate.ToString("MM月dd日"),
                        EndDate = item.EndDate.ToString("MM月dd日"),
                        City = item.City
                    });
                });
                return view;
            });
            return Ok(result);
        }
        #endregion

        #region 获取会议详情 - APP
        /// <summary>
        /// 获取会议详情 - APP
        /// </summary>
        [HttpGet]
        [Route("GetConferenceDetailForApp")]
        public IHttpActionResult GetConferenceDetailForApp(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var conference = ConferenceAdapter.Instance.GetConferenceByConferenceId(code);

                string seatNumber_meeting = "", seatNumber_Entertain = "";
                List<string> CurrentDeskPersons = new List<string>();//当前桌子的人员集合
                var attendee = AttendeeAdapter.Instance.GetAttendeeByAttendaID(user.Id, conference.ID);
                if (attendee != null)
                {
                    if (attendee.Find(f => f.AttendeeType == 1) != null)
                    {
                        var seats = SeatsAdapter.Instance.GetUserSeatModel(attendee.Find(f => f.AttendeeType == 1).ID, conference.ID, 1);
                        if (seats != null)
                        {
                            seatNumber_meeting = seats.SeatAddress;
                        }
                    }

                    if (attendee.Find(f => f.AttendeeType == 2) != null)
                    {
                        var Entertain = SeatsAdapter.Instance.GetUserSeatModel(attendee.Find(f => f.AttendeeType == 2).ID, conference.ID, 2);
                        if (Entertain != null)
                        {
                            seatNumber_Entertain = Entertain.SeatAddress;
                        }
                        DataTable dt = AttendeeListAdapter.Instance.GetAllHasSeatAttendeeList(conference.ID, 2);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                if (dt.Rows[i]["Name"] != null && dt.Rows[i]["seatAddress"] != null)
                                {
                                    if (dt.Rows[i]["seatAddress"].ToString() == seatNumber_Entertain)
                                    {
                                        CurrentDeskPersons.Add(dt.Rows[i]["Name"].ToString());
                                    }
                                }
                            }
                        }
                    }

                }

                var images = new List<string>();
                ShareFileAdapter.Instance.GetList(ShareFileModule, code).ForEach(image =>
                {
                    images.Add(image.PathFull);
                });
                // 如果没有会议图片，默认使用封面图
                if (images.Count < 1)
                {
                    images.Add(conference.ShowImage);
                }

                var agenda = new Dictionary<string, List<ConferenceAgendaAppViewModel>>();
                var agendaGroups = ConferenceAgendaAdapter.Instance.LoadListbyCode(code).GroupBy(g => g.BeginDate.ToString("MM月dd日"));
                foreach (var agendaGroup in agendaGroups)
                {
                    var list = new List<ConferenceAgendaAppViewModel>();
                    foreach (var item in agendaGroup.OrderBy(o => o.BeginDate))
                    {
                        list.Add(new ConferenceAgendaAppViewModel()
                        {
                            Time = item.BeginDate.ToString("HH:mm"),
                            Title = item.Title
                        });
                    }
                    agenda.Add(agendaGroup.Key, list);
                }

                var worker = new List<ConferenceWorkerAppViewModel>();
                WorkerAdapter.Instance.LoadByConferenceID(code).ForEach(item =>
                {
                    worker.Add(new ConferenceWorkerAppViewModel()
                    {
                        Code = item.UserID,
                        Name = item.UserName,
                        Type = item.WorkerTypeName,
                        Phone = item.UserTelPhone
                    });
                });

                var bus = new List<ConferenceBusAppViewModel>();
                BusRouteModelAdapter.Instance.LoadByConferenceCode(code).ForEach(item =>
                {
                    bus.Add(new ConferenceBusAppViewModel()
                    {
                        Title = item.Title,
                        Date = item.DepartDate.ToString("MM月dd日"),
                        Time = item.DepartDate.ToString("HH:mm"),
                        Address = item.BeginPlace,
                        Phone = item.ContactsPhone
                    });
                });


                #region 根据当前登录人查询会议附件
                List<dynamic> materiallist = new List<dynamic>();
                MeetingMaterialCollection temp = MeetingMaterialAdapter.Instance.Load(p =>
                {
                    p.AppendItem("ConfereenceId", conference.ID);
                    p.AppendItem("MeetingScenes", 1);
                });
                if (temp != null && temp.Count > 0)
                {
                    temp.ForEach(f =>
                    {
                        var t = new
                        {
                            id = f.Code,
                            isShowAll = f.IsShowAll,
                            name = f.Name,
                            url = FileService.DownloadFile(f.ViewUrl)
                        };
                        if (f.ViewList.Count > 0)
                        {
                            if (f.ViewList.Find(x => x.PeopleId == user.Id) != null)
                            {
                                materiallist.Add(t);
                            }
                        }
                    });
                }
                #endregion

                var view = new ConferenceDetailAppViewModel()
                {
                    Code = conference.ID,
                    Name = conference.Name,
                    StartDate = conference.BeginDate.ToString("MM月dd日"),
                    EndDate = conference.EndDate.ToString("MM月dd日"),
                    Hotel = conference.Hotel,
                    Ballroom = conference.Ballroom,
                    SeatNumber = seatNumber_meeting,
                    EntertainHall = conference.EntertainHall,
                    E_SeatNumber = seatNumber_Entertain,
                    CurrentDeskList = CurrentDeskPersons,
                    Layout = conference.Layout,
                    Address = conference.Address,
                    Lat = conference.Latitude,
                    Lng = conference.Longitude,
                    Notice = conference.Notice,
                    Images = images,
                    Agenda = agenda,
                    Worker = worker,
                    Bus = bus,
                    MaterialList = materiallist
                };
                return view;
            });
            return Ok(result);
        }
        #endregion




        #region 获取会议列表--PC
        /// <summary>
        /// 获取会议列表--PC
        /// </summary>
        [HttpGet]
        [Route("GetConferenceListForPC")]
        public IHttpActionResult GetConferenceListForPC(int pageIndex, string name = "")
        {
            ViewPageBase<List<ConferenceViewModel>> list = new ViewPageBase<List<ConferenceViewModel>>();
            ControllerHelp.SelectAction(() =>
            {
                list = ConferenceViewAdapter.Instance.GetConferenceList(pageIndex, name);
            });
            return Ok(list);
        }
        #endregion
        #region 保存会议座位坐标
        /// <summary>
        /// 保存会议座位坐标
        /// </summary>
        [HttpGet]
        [Route("SaveLocation")]
        public IHttpActionResult SaveLocation(string id,string Layout)
        {

            var result = ControllerService.Run(() =>
            {
                var meetingByCode = ConferenceAdapter.Instance.Load(w => w.AppendItem("ID", id)).FirstOrDefault();
                if (meetingByCode == null)
                {
                    throw new Exception("编码错误!");
                }
                meetingByCode.Layout = Layout;
                ConferenceAdapter.Instance.Update(meetingByCode);


            });
            return Ok(result);
        }
        #endregion
        #region 保存协作者
        /// <summary>
        /// 保存协作者
        /// </summary>
        [HttpGet]
        [Route("SaveEnviteCollaboration")]
        public IHttpActionResult SaveEnviteCollaboration(string id ,string enviteCollaboration)
        {

            var result = ControllerService.Run(() =>
            {
                var enviteCollaborationByCode = ConferenceAdapter.Instance.Load(w => w.AppendItem("ID", id)).FirstOrDefault();
                if (enviteCollaborationByCode == null)
                {
                    throw new Exception("编码错误!");
                }
                enviteCollaborationByCode.EnviteCollaboration = enviteCollaboration;
                ConferenceAdapter.Instance.Update(enviteCollaborationByCode);


            });
            return Ok(result);
        }
        #endregion



        #region 获取会议详情--PC
        /// <summary>
        /// 获取会议详情
        /// </summary>
        [HttpGet]
        [Route("GetConferenceDetailForPC")]
        public IHttpActionResult GetConferenceDetailForPC(string id)
        {
            var result = ControllerService.Run(() =>
            {
                var model = ConferenceAdapter.Instance.GetModel(id);
                if (model == null)
                {
                    throw new Exception("会议编码错误！");
                }

                var images = ShareFileAdapter.Instance.GetList(ShareFileModule, id);
                var viewImages = new List<ConferenceEditImageViewModel>();
                images.ForEach(image =>
                {
                    viewImages.Add(new ConferenceEditImageViewModel()
                    {
                        Code = image.Code,
                        Url = image.PathFull,
                        Sort = image.Sort
                    });
                });

                return new ConferenceEditViewModel()
                {
                    Id = model.ID,
                    Name = model.Name,
                    Description = model.Description,
                    BeginDate = model.BeginDate,
                    EndDate = model.EndDate,
                    Layout = model.Layout,
                    Image = model.ShowImage,
                    City = model.City,
                    Hotel = model.Hotel,
                    Ballroom = model.Ballroom,
                    EntertainHall = model.EntertainHall,
                    Address = model.Address,
                    Longitude = model.Longitude,
                    Latitude = model.Latitude,
                    Notice = model.Notice,
                    Images = viewImages
                };
            });
            return Ok(result);
        }
        #endregion

        #region 编辑会议信息--PC
        /// <summary>
        /// 编辑会议信息--PC
        /// </summary>
        [HttpPost]
        [Route("EditConference")]
        public IHttpActionResult EditConference(ConferenceEditViewModel post)
        {
            var user = (Seagull2Identity)User.Identity;

            //上传图片
            var isImage = false;
            if (post.Image.StartsWith("data:image"))
            {
                post.Image = FileService.UploadFile(post.Image);
                isImage = true;
            }
            string conferenceCode;
            //添加
            if (string.IsNullOrWhiteSpace(post.Id))
            {
                conferenceCode = Guid.NewGuid().ToString();
                ConferenceAdapter.Instance.Update(new ConferenceModel()
                {
                    ID = conferenceCode,
                    Name = post.Name,
                    Description = post.Description,
                    BeginDate = post.BeginDate,
                    EndDate = post.EndDate,
                    Layout = post.Layout,
                    Image = isImage ? post.Image : string.Empty,
                    City = post.City,
                    Hotel = post.Hotel,
                    Ballroom = post.Ballroom,
                    EntertainHall = post.EntertainHall,
                    Address = post.Address,
                    Longitude = post.Longitude,
                    Latitude = post.Latitude,
                    Notice = post.Notice,
                    IsPublic = false,
                    Creator = user.Id,
                    CreateName=user.DisplayName,
                    EcreateName=user.LogonName,
                    CreateTime = DateTime.Now,
                    ValidStatus = true
                });
            }
            //修改
            else
            {
                var find = ConferenceAdapter.Instance.Load(p => { p.AppendItem("ID", post.Id); }).SingleOrDefault();
                if (find == null)
                {
                    return Ok(new BaseView()
                    {
                        State = false,
                        Message = "not find."
                    });
                }
                conferenceCode = post.Id;
                ConferenceAdapter.Instance.Update(new ConferenceModel()
                {
                    ID = post.Id,
                    Name = post.Name,
                    Description = post.Description,
                    BeginDate = post.BeginDate,
                    EndDate = post.EndDate,
                    Layout = post.Layout,
                    Image = isImage ? post.Image : find.Image,
                    City = post.City,
                    Hotel = post.Hotel,
                    Ballroom = post.Ballroom,
                    EntertainHall = post.EntertainHall,
                    Address = post.Address,
                    Longitude = post.Longitude,
                    Latitude = post.Latitude,
                    Notice = post.Notice,
                    IsPublic = find.IsPublic,
                    Creator = find.Creator,
                    CreateTime = find.CreateTime,
                    CreateName = user.DisplayName,
                    EcreateName = user.LogonName,
                    ValidStatus = true
                });
            }
            //会议图片
            Task.Run(() =>
            {
                ShareFileAdapter.Instance.GetList(ShareFileModule, conferenceCode).ForEach(image =>
                {
                    var find = post.Images.Find(f => f.Code == image.Code);
                    if (find == null)
                    {
                        ShareFileAdapter.Instance.Delete(image.Code);
                    }
                });

                var sort = 1;
                foreach (var image in post.Images)
                {
                    // 新增图片
                    if (image.Code.StartsWith("temp-image-") && image.Url.StartsWith("data:image"))
                    {
                        ShareFileAdapter.Instance.Update(new ShareFileModel()
                        {
                            Code = Guid.NewGuid().ToString(),
                            Module = ShareFileModule,
                            TargetCode = conferenceCode,
                            Type = "image",
                            Name = string.Empty,
                            Path = FileService.UploadFile(image.Url),
                            Sort = sort,
                            Creator = user.Id,
                            CreateTime = DateTime.Now,
                            Modifier = user.Id,
                            ModifyTime = DateTime.Now,
                            ValidStatus = true
                        });
                    }
                    // 修改图片
                    else
                    {
                        var model = ShareFileAdapter.Instance.GetModel(image.Code);
                        if (model == null)
                        {
                            continue;
                        }
                        model.Sort = sort;
                        model.Modifier = user.Id;
                        model.ModifyTime = DateTime.Now;
                        ShareFileAdapter.Instance.Update(model);
                    }
                    sort++;
                }
            });
            return Ok(new BaseView
            {
                State = true,
                Message = "保存成功！"
            });
        }
        #endregion

        #region 删除会议--PC
        /// <summary>
        /// 删除会议--PC
        /// </summary>
        [HttpGet]
        [Route("DelConferenceByID")]
        public IHttpActionResult DelConferenceByID(string id)
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    ConferenceAdapter.Instance.DelTById(id);
                    ShareFileAdapter.Instance.Delete(ShareFileModule, id);

                    //事务提交
                    scope.Complete();
                }
            });
            return Ok(result);
        }
        #endregion

        #region 发布 / 下线 ---------------------20180901后删除
        /// <summary>
        /// 发布 / 下线
        /// </summary>
        [HttpPost]
        [Route("UpdateIsPublic")]
        public IHttpActionResult UpdateIsPublic()
        {
            ViewModelBase result = ControllerHelp.RunAction(() =>
            {
                //ture--发布上线，false--下线
                bool isPublic = Convert.ToBoolean(baseRequest.Form["IsPublic"]);
                string conferenceID = baseRequest.Form["ConferenceID"];

                ConferenceModel conferencemodel = ConferenceAdapter.Instance.GetTByID(conferenceID);
                conferencemodel.IsPublic = isPublic;
                ConferenceAdapter.Instance.Update(conferencemodel);
            });
            return Ok(result);
        }
        #endregion

        #region 发布 / 下线
        /// <summary>
        /// 发布 / 下线
        /// </summary>
        [HttpPost]
        [Route("UpdateConferenceState")]
        public IHttpActionResult UpdateConferenceState(string conferenceId)
        {
            var result = ControllerService.Run(() =>
            {
                var model = ConferenceAdapter.Instance.GetTByID(conferenceId);
                model.IsPublic = !model.IsPublic;
                ConferenceAdapter.Instance.Update(model);
            });
            return Ok(result);
        }
        #endregion










        #region 获取会议列表--APP--20180625改版后弃用
        /// <summary>
        /// 获取会议列表--APP
        /// </summary>
        [HttpGet]
        [Route("GetConferenceListForAPP")]
        public IHttpActionResult GetConferenceListForAPP(int pageIndex, DateTime searchTime)
        {
            ViewPageBase<List<ConferenceViewModel>> list = new ViewPageBase<List<ConferenceViewModel>>();
            ControllerHelp.SelectAction(() =>
            {
                //APP测试用户-可查看全部会议
                bool isAppSearch = true;
                string[] appTestUser = ConfigurationManager.AppSettings["AppTestUser"].Split(',');
                if (appTestUser.Contains(User.Identity.Name))
                {
                    isAppSearch = false;
                }
                list = ConferenceViewAdapter.Instance.GetConferenceViewByPage(pageIndex, searchTime, isAppSearch);
            });
            return Ok(list);
        }
        #endregion

        #region 获取我的会议列表--APP-20180625改版后弃用
        /// <summary>
        /// 获取我的会议列表
        /// </summary>
        [HttpGet]
        [Route("GetMyConferenceListForPC")]
        public IHttpActionResult GetMyConferenceListForPC(int pageIndex, DateTime searchTime)
        {
            ViewPageBase<List<ConferenceViewModel>> list = new ViewPageBase<List<ConferenceViewModel>>();
            ControllerHelp.SelectAction(() =>
            {
                list = ConferenceViewAdapter.Instance.GetMyConferenceViewByPage(pageIndex, searchTime, CurrentUserCode);
            });
            return Ok(list);
        }
        #endregion







        #region 根据会议名称查询会议
        /// <summary>
        /// 根据会议名称查询会议
        /// </summary>
        [HttpGet]
        [Route("SelectConferenceByName")]
        public IHttpActionResult SelectConferenceByName(string name)
        {
            ConferenceModelCollection modelColl = new ConferenceModelCollection();
            ControllerHelp.SelectAction(() =>
            {
                modelColl = ConferenceAdapter.Instance.Load(m => m.AppendItem("Name", "%" + name + "%", "LIKE"));
            });

            return Ok(modelColl);
        }
        #endregion

        #region 内部测试使用
        /// <summary>
        /// 增加或修改会议--PC
        /// </summary>
        [HttpGet]
        [Route("TestConferenceDialog")]
        public IHttpActionResult TestConferenceDialog(bool isTest = true, string conferenceID = "", string conferenceName = "")
        {
            if (isTest)
            {
                conferenceID = Guid.NewGuid().ToString("N");
                conferenceName = "测试会议";
                ConferenceDialogHelp.CreateConferenceDialog(conferenceID, conferenceName, ((Seagull2Identity)User.Identity).Id);
            }
            else
            {
                conferenceID = conferenceID == "" ? "d49a7d4ebed140fa9996fbfe091b6e66" : conferenceID;
                conferenceName = conferenceName == "" ? "远洋集团2017经营管理年会" : conferenceName;
                ConferenceDialogHelp.CreateConferenceDialog(conferenceID, conferenceName, ((Seagull2Identity)User.Identity).Id);
            }

            return Ok(new ViewsModel.ViewModelBaseNull() { State = true, Message = "success." });
        }
        /// <summary>
        /// 添加工作人员--PC
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("TestWorkerDialog")]
        public IHttpActionResult TestWorkerDialog(string conferenceID)
        {
            ControllerHelp.SelectAction(() =>
            {
                string workerID = Guid.NewGuid().ToString("N");
                WorkerModel workModel = new WorkerModel() { ID = workerID };
                WorkerCollection wColl = WorkerAdapter.Instance.LoadByConferenceID(conferenceID);
                wColl.Add(workModel);

                WorkerDialogHelp.WorkerCollDialog(wColl, conferenceID);
                WorkerAdapter.Instance.Update(workModel);
            });
            return Ok();
        }
        /// <summary>
        /// 推送现场服务消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("TestSiteService")]
        public IHttpActionResult TestSiteService(string conferenceID, string testLoginName, string remark)
        {
            UserInfo user = IsInConferenceCurrentUser(conferenceID, false, testLoginName);
            SiteServiceCollection ssColl = SiteServiceAdapter.Instance.GetTColl();

            SiteServiceDialogHelp.SendMessage(conferenceID, user.CodeUserInConference, ssColl[0].ID);
            return Ok();
        }
        #endregion







        #region 查询会议地点
        /// <summary>
        /// 查询会议地点
        /// </summary>
        [HttpGet]
        [Route("GetConferenceLocation")]
        public IHttpActionResult GetConferenceLocation(string conferenceID)
        {
            ConferencePlaceView view = new ConferencePlaceView();
            ControllerHelp.SelectAction(() =>
            {
                view = ConferenceAdapter.Instance.GetConferenceModelListByPage(conferenceID);
            });
            return Ok(view);
        }
        #endregion

        #region 获取用户首页权限--APP
        /// <summary>
        /// 获取用户首页权限--APP
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="TestLoginName">调试时，可输入该参数</param>
        [Route("GetUserPermissionsIndexPage")]
        [HttpGet]
        public IHttpActionResult GetUserPermissionsIndexPage(string conferenceID, string TestLoginName = "")
        {
            //返回：用户是否属于该会议参会人；首页需要进行用户权限验证的菜单
            try
            {
                UserInfo userInfo = IsInConferenceCurrentUser(conferenceID, false, TestLoginName == "" ? "" : TestLoginName);

                log.Info("用户信息：" + JsonConvert.SerializeObject(userInfo));

                string[] needUserIndexMenuKey = ConfigurationManager.AppSettings["NeedUserIndexMenuKey"].Split(',');
                dynamic result = new
                {
                    IsInConference = userInfo.IsInConference,
                    NeedUserIndexMenuKey = needUserIndexMenuKey
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.WriteLog(e.Message);
                Log.WriteLog(e.StackTrace);
                throw e;
            }
        }
        #endregion

        #region 获取项目列表
        /// <summary>
        /// 获取项目列表
        /// </summary>
        [HttpGet]
        [Route("GetProjectList")]
        public IHttpActionResult GetProjectList(int pageIndex)
        {
            DateTime searchTime = DateTime.Now;
            ViewPageBase<List<Project>> list = new ViewPageBase<List<Project>>();
            ControllerHelp.SelectAction(() =>
            {
                list = ActivityAdapter.Instance.GetProjectViewByPage(pageIndex, searchTime);
            });
            return Ok(list);
        }
        #endregion

        #region APP意见反馈
        /// <summary>
        /// APP意见反馈
        /// </summary>
        /// <param name="formdata">作品表单实体</param>
        [HttpPost]
        [Route("Feedback")]
        public string Feedback(FeedbackForm formdata)
        {
            ZhongChouData.Models.Feedback feed = new ZhongChouData.Models.Feedback
            {
                Code = Guid.NewGuid().ToString(),
                Content = formdata.Content,
                Creator = ((Seagull2Identity)User.Identity).Id
            };

            using (TransactionScope scope = TransactionScopeFactory.Create())
            {
                ZhongChouData.Models.FeedbackAdapter.Instance.Update(feed);

                formdata.imgUrlList.ForEach(imgUrl =>
                {
                    AttachmentModel model = new AttachmentModel()
                    {
                        ID = Guid.NewGuid().ToString(),
                        CreateTime = DateTime.Now,
                        ResourceID = feed.Code,
                        ValidStatus = true,
                        FilePath = imgUrl
                    };
                    //附件入库&上传附件
                    AttachmentAdapter.Instance.Update(model);
                });

                scope.Complete();
            }
            return "OK";
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ImageHelp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageBase64Str"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        public static FileHelp SaveImage(string imageBase64Str, string imageType)
        {
            string newFileName = "";
            string FileLoadPath = "";
            try
            {
                byte[] bytes = Convert.FromBase64String(imageBase64Str);
                MemoryStream memStream = new MemoryStream(bytes);
                BinaryFormatter binFormatter = new BinaryFormatter();
                System.Drawing.Bitmap map = new Bitmap(memStream);
                Image image = (Image)map;

                string baseUrl = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ConferenceImageUploadRootPath"]);
                newFileName = Guid.NewGuid().ToString("N") + "." + imageType;

                string FileAllPath = baseUrl + "\\" + newFileName;
                image.Save(FileAllPath);
                FileLoadPath = ConfigurationManager.AppSettings["ConferenceImageDownLoadRootPath"] + "/" + newFileName;
            }
            catch (Exception e)
            {
                Log.WriteLog(string.Format(@"有图片上传失败：imageBase64Str={0}&imageType={1}", imageBase64Str, imageType));
                throw e;
            }
            return new FileHelp { NewFileName = newFileName, FileLoadPath = FileLoadPath };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FileHelp
    {
        /// <summary>
        /// 
        /// </summary>
        public string NewFileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FileLoadPath { get; set; }
    }
}
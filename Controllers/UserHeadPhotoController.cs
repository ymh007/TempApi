using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.Core.Models;
using Seagull2.Owin;
using Seagull2.Owin.File.Services;
using Seagull2.YuanXin.AppApi.Adapter.Message;
using Seagull2.YuanXin.AppApi.Adapter.UserHeadPhoto;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.UserHeadPhoto;
using Seagull2.YuanXin.AppApi.ViewsModel.UserHeadPhoto;
using SinoOcean.Seagull2.Framework.MasterData;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 用户头像控制器
    /// </summary>
    public class UserHeadPhotoController : ApiController
    {
        /// <summary>
        /// 日志
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IClientFileService _fileService = ContainerHelper.GetService<IClientFileService>(new DefaultClientFileService());
        #region 用户头像上传
        /// <summary>
        /// 用户头像上传
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(UserHeadPhotoSaveViewModel post)
        {
            if (post == null)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = "参数post为null."
                });
            }
            var user = ((Seagull2Identity)User.Identity);
            var model = new UserHeadPhotoModel
            {
                Code = Guid.NewGuid().ToString(),
                UserCode = user.Id,
                UserName = user.DisplayName,
                Url = post.Url,
                IsOperate = false,
                IsAudit = true,
                Creator = user.Id,
                CreateTime = DateTime.Now,
                ValidStatus = true
            };
            UserHeadPhotoAdapter.Instance.Update(model);

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "sucess."
            });
        }
        #endregion

        #region 审核用户头像
        /// <summary>
        /// 审核用户头像
        /// </summary>
        [HttpGet]
        public IHttpActionResult Examine(string code, bool state, string message = "")
        {
            var user = (Seagull2Identity)User.Identity;
            var model = UserHeadPhotoAdapter.Instance.Load(m => m.AppendItem("Code", code)).SingleOrDefault(); //查询是否存在头像
            var dbState = model.IsAudit;
            if (model != null)
            {
                if (state) //审核
                {
                    model.IsOperate = true;
                    model.IsAudit = true;
                }
                else     //驳回
                {
                    model.IsOperate = true;
                    model.IsAudit = false;
                }

                model.Operator = ((Seagull2Identity)User.Identity).Id;
                model.OperatorName = ((Seagull2Identity)User.Identity).DisplayName;
                model.OperateTime = DateTime.Now;
                model.Modifier = ((Seagull2Identity)User.Identity).Id;
                model.ModifyTime = DateTime.Now;
                UserHeadPhotoAdapter.Instance.Update(model);

                #region 推送服务、消息提醒
                if (dbState == true && model.IsAudit == false)
                {
                    // 推送服务
                    var pushResult = string.Empty;
                    var pushModel = new PushService.Model()
                    {
                        BusinessDesc = "审核用户头像",
                        Title = "头像驳回通知",
                        Content = "您的头像不符合规范,请您重新上传",
                        Extras = new PushService.ModelExtras()
                        {
                            action = "systemMessageReminder",
                            bags = model.UserCode
                        },
                        SendType = PushService.SendType.Person,
                        Ids = model.UserCode
                    };
                    bool isPush = PushService.Push(pushModel, out pushResult);
                    log.Info(string.Format("头像驳回推送状态:{0}，返回消息:{1}", isPush, pushResult));

                    // 消息提醒
                    MessageAdapter.Instance.Update(new MessageModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        //MeetingCode = string.Empty,
                        MessageContent = string.Format("您的头像不符合规范，请您重新上传。驳回原因：{0}", string.IsNullOrEmpty(message) ? "无" : message),
                        MessageStatusCode = EnumMessageStatus.New,
                        MessageTypeCode = "2",
                        MessageTitleCode = EnumMessageTitle.System,
                        Creator = user.Id,
                        CreatorName = user.DisplayName,
                        ReceivePersonCode = model.UserCode,
                        ReceivePersonName = model.UserName,
                        ReceivePersonMeetingTypeCode = string.Empty,
                        //OverdueTime = DateTime.Now,
                        ValidStatus = true,
                        CreateTime = DateTime.Now
                    });
                }
                #endregion
                ControllerService.UploadLog(user.Id, "审核工具-头像审核-用户头像");
                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "sucess."
                });
            }
            else
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = "未找到该用户头像."
                });
            }
        }
        #endregion

        #region 获取用户头像列表 - PC
        /// <summary>
        /// 获取用户头像 - PC
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="state">全部用户：-1 未审核：0 已审核：1</param>
        /// <param name="userName">用户名称</param>
        [HttpGet]
        public IHttpActionResult GetList(int pageIndex, int pageSize, int state, string userName = "")
        {
            var coll = new List<UserHeadPhotoModel>();
            var list = new List<UserHeadPhotoListViewModel>();
            int count = 0;
            switch (state)
            {
                case -1: //查询全部
                    coll = UserHeadPhotoAdapter.Instance.GetList(-1, pageIndex, pageSize, userName);
                    count = UserHeadPhotoAdapter.Instance.GetListForPC(-1, userName);
                    break;

                case 0: //查询待处理
                    coll = UserHeadPhotoAdapter.Instance.GetList(0, pageIndex, pageSize, userName);
                    count = UserHeadPhotoAdapter.Instance.GetListForPC(0, userName);
                    break;

                case 1: //查询已经处理
                    coll = UserHeadPhotoAdapter.Instance.GetList(1, pageIndex, pageSize, userName);
                    count = UserHeadPhotoAdapter.Instance.GetListForPC(1, userName);
                    break;

                case 2: //查询已经审核
                    coll = UserHeadPhotoAdapter.Instance.GetList(2, pageIndex, pageSize, userName);
                    count = UserHeadPhotoAdapter.Instance.GetListForPC(2, userName);
                    break;

                case 3: //查询审核未通过
                    coll = UserHeadPhotoAdapter.Instance.GetList(3, pageIndex, pageSize, userName);
                    count = UserHeadPhotoAdapter.Instance.GetListForPC(3, userName);
                    break;
                default:
                    break;
            }
            if (coll != null)
            {
                coll.ForEach(item =>
                {
                    var model = new UserHeadPhotoListViewModel
                    {
                        Code = item.Code,
                        UserCode = item.UserCode,
                        UserName = item.UserName,
                        Url = FileService.DownloadFile(item.Url),
                        Creator = item.Creator,
                        IsOperate = item.IsOperate,
                        IsAudit = item.IsAudit,
                        Operator = item.Operator ?? null,
                        OperatorName = item.OperatorName ?? null,
                        OperateTime = item.OperateTime == null ? null : item.OperateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    list.Add(model);
                });
            }
            return Ok(new ViewsModel.BaseViewPage()
            {
                DataCount = count,
                PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                PageData = list
            });
        }
        #endregion

        #region 获取用户头像设置
        /// <summary>
        /// 获取用户头像设置
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserHeadPhoto()
        {
            var user = (Seagull2Identity)User.Identity;
            var vmodel = new UserHeadPhotoLViewModel();
            var modelColl = UserHeadPhotoAdapter.Instance.Load(m => m.AppendItem("UserCode", user.Id)).OrderByDescending(m => m.CreateTime).ToList();
            if (modelColl.Count() > 0)
            {
                var audit = modelColl.Find(w => w.IsAudit == true);
                var first = modelColl[0];
                vmodel = new UserHeadPhotoLViewModel
                {
                    State = true,
                    Code = first.Code,
                    UserCode = first.UserCode,
                    UserName = first.UserName,
                    Url = audit != null ? FileService.DownloadFile(audit.Url) : string.Format(ConfigAppSetting.SignImgPath, user.Id),
                    IsOperate = first.IsOperate,
                    IsAudit = first.IsAudit
                };
            }
            else
            {
                vmodel = new UserHeadPhotoLViewModel
                {
                    State = false,
                    UserCode = user.Id,
                    UserName = user.DisplayName,
                    Url = string.Format(ConfigAppSetting.SignImgPath, user.Id)
                };
            }
            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "sucess.",
                Data = vmodel
            });
        }
        #endregion

        #region 下载用户头像
        /// <summary>
        /// 下载用户头像
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetImage(string userCode)
        {
            try
            {
                string url = "";
                List<UserHeadPhotoModel> list = UserHeadPhotoAdapter.Instance.Load(m => m.AppendItem("UserCode", userCode).AppendItem("IsAudit", true)).OrderByDescending(o => o.CreateTime).ToList();
                if (list.Count() > 0)
                {
                    url= string.Format(ConfigurationManager.AppSettings["FileDownloadByData"], list[0].Url);
                }
                else
                {
                    url = string.Format(ConfigAppSetting.SignImgPath, userCode);
                }
                var webClient = new WebClient()
                {
                    BaseAddress = url
                };
                var bytes = webClient.DownloadData(url);
                webClient.Dispose();
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                return response;
            }
            catch
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(System.IO.File.ReadAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}Images\\default_user_photo.png"))
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                return response;
            }
        }
        #endregion

        #region 下载用户头像（seagull2）
        /// <summary>
        /// 下载用户头像（seagull2）
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage Photo(string id)
        {
            string defaultPhotoPath = "Images\\default_user_photo.png";
            try
            {

                var employee = EmployeeAdapter.Instance.LoadEmployee(id);
                var fileInfo = _fileService.GetFilesByResourceId(employee.PhotoID, "HR_PHOTO");
                if (fileInfo.Count > 0)
                {
                    var stream = _fileService.GetFileStream(fileInfo.FirstOrDefault());
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(stream)
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                    return response;
                }
                if (employee != null)
                {
                    if (employee.GenderCode == "2" || employee.GenderCode == "02") // 女
                    {
                        defaultPhotoPath = "Images\\gender02.png";
                    }
                    else
                    {
                        defaultPhotoPath = "Images\\gender01.png";
                    }
                }
                var responseNo = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(System.IO.File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + defaultPhotoPath))
                };
                responseNo.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                return responseNo;
            }
            catch (Exception ex)
            {
                log.Error("获取头像失败:" + ex.ToString());
                var responseNo = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(System.IO.File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + defaultPhotoPath))
                };
                responseNo.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpg");
                return responseNo;
            }
        }


        #endregion
    }
}
using MCS.Library.Data;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using Seagull2.YuanXin.AppApi.Adapter.ShareFile;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    ///会议大屏效果控制器
    /// </summary>

    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 查询大屏效果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetConferenceBigScreenList")]
        public IHttpActionResult GetConferenceBigScreenList(string conferenceID)
        {
            var result = ControllerService.Run(() =>
            {
                ConferenceBigScreenModelCollection list = new ConferenceBigScreenModelCollection();
                list = ConferenceBigScreenAdapter.Instance.LoadAll(conferenceID);
                list.ForEach(f =>
                {
                    if (!f.IsSystem)
                    {
                        if (f.BgUrl.StartsWith("YuanXin-File://"))
                        {
                            string temp = FileService.DownloadFile(f.BgUrl);
                            f.BgUrl = temp;
                        }
                    }
                });
                return new
                {
                    comm = list.FindAll(f => f.IsSystem == true),
                    myScreen = list.FindAll(f => f.IsSystem == false)
                };
            });
            return Ok(result);
        }



        /// <summary>
        /// 删除我的大屏效果
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("DelMyBigScreen")]
        public IHttpActionResult DelMyBigScreen(string id)
        {
            var result = ControllerService.Run(() =>
            {
                if (!string.IsNullOrEmpty(id))
                {
                    ConferenceBigScreenAdapter.Instance.Delete(p => p.AppendItem("Code", id));
                }
            });
            return Ok(result);
        }


        /// <summary>
        /// 更新我的大屏效果
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateMyBigScreen")]
        public IHttpActionResult UpdateMyBigScreen(ConferenceBigScreenModel cbs)
        {
            var result = ControllerService.Run(() =>
            {
                if (string.IsNullOrEmpty(cbs.Code))
                {
                    cbs.Code = Guid.NewGuid().ToString();
                    cbs.ValidStatus = true;
                    cbs.CreateTime = DateTime.Now;
                    cbs.Creator = CurrentUser.CodeSeagull;
                }
                else
                {
                    cbs.ModifyTime = DateTime.Now;
                    cbs.Modifier = CurrentUser.CodeSeagull;
                    var screen = ConferenceBigScreenAdapter.Instance.Load(w => w.AppendItem("Code", cbs.Code)).FirstOrDefault();
                    cbs.Creator = screen.Creator;
                    cbs.CreateTime = screen.CreateTime;
                    cbs.BgUrl = screen.BgUrl;
                }
                if (cbs.Image.StartsWith("data:image"))
                {
                    cbs.BgUrl = FileService.UploadFile(cbs.Image);

                }

                ConferenceBigScreenAdapter.Instance.Update(cbs);
            });

            return Ok(result);
        }


        /// <summary>
        /// 查看某一个大屏效果
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ViewMyScreen")]
        public IHttpActionResult ViewMyScreen(string id)
        {
            var result = ControllerService.Run(() =>
            {
                ConferenceBigScreenModel myBig = null;
                AttendeeCollection photos = new AttendeeCollection();

                if (!string.IsNullOrEmpty(id))
                {
                    myBig = ConferenceBigScreenAdapter.Instance.Load(p => { p.AppendItem("Code", id); }).FirstOrDefault();
                    if (myBig != null)
                    {
                        if (myBig.IsSystem)
                        {
                            photos.Add(new AttendeeModel()
                            {
                                Email = "sysy@ssy.com",
                                PhotoAddress = $"{ConfigAppSetting.ApiPath}Images\\meeting\\sysPhoto.png"
                            });
                        }
                        else
                        {
                            if (myBig.BgUrl.StartsWith("YuanXin-File://"))
                            {
                                string temp = FileService.DownloadFile(myBig.BgUrl);
                                myBig.BgUrl = temp;
                            }
                            if (!string.IsNullOrEmpty(myBig.ConfereenceId))
                            {
                                photos = AttendeeAdapter.Instance.Load(p =>
                                {
                                    p.AppendItem("ValidStatus", true);
                                    p.AppendItem("ConferenceID", myBig.ConfereenceId);
                                    p.AppendItem("AttendeeType", 1);
                                });
                                photos.ForEach(d =>
                                {
                                    if (string.IsNullOrWhiteSpace(d.AttendeeID))
                                    {
                                        d.PhotoAddress = $"{ConfigAppSetting.ApiPath}Images/default_user_photo.png";
                                    }
                                    else
                                    {
                                        d.PhotoAddress = $"{ConfigAppSetting.ApiPath}UserHeadPhoto/GetImage?userCode={d.AttendeeID}";
                                    }

                                });
                            }
                        }
                    }
                }
                return new
                {
                    myBig,
                    photos
                };
            });

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("del")]
        public IHttpActionResult initSign()
        {
            var result = ControllerService.Run(() =>
            {
                return SignInAdapter.Instance.del();
            });
            return this.Ok(result);
        }

    }
}
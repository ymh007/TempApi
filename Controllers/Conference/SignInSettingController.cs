using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    //签到大屏控制器
    public partial class ConferenceController : ControllerBase
    {
        /// <summary>
        /// 添加或修改签到大屏
        /// </summary>
        /// <param name="singInSetting"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddOrUpdateSingInSetting")]
        public IHttpActionResult AddOrUpdateSingInSetting()
        {
            ControllerHelp.SelectAction(() =>
            {
                SignInSetting signInSetting = new SignInSetting();
                string SignInSettingID = baseRequest.Form["ID"];

                if (SignInSettingID == null || SignInSettingID == "")
                {
                    signInSetting = new SignInSetting()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        ConferenceID = baseRequest.Form["conferenceID"],
                        BackgroundColor = baseRequest.Form["backgroundColor"],
                        CityName = baseRequest.Form["cityName"],
                        IsDefaultBackgroundImage = Convert.ToBoolean(baseRequest.Form["isDefaultBackgroundImage"]),
                        IsDefaultSelectImageIndex = Convert.ToInt32(baseRequest.Form["isDefaultSelectImageIndex"]),
                        ValidStatus = true,
                        Creator = CurrentUserCode,
                        CreateTime = DateTime.Now
                    };
                    string newImageName = "";
                    string imageType = baseRequest.Form["backgroundImageNameType"];
                    //本项目图片默认存储到~/Images/目录下
                    string image = baseRequest.Form["backgroundImageName"];
                    newImageName = ImageHelp.SaveImage(image, imageType).NewFileName;
                    signInSetting.BackgroundImageName = newImageName;
                }
                else
                {
                    signInSetting = SignInSettingAdapter.Instance.GetSignInSettingById(SignInSettingID);
                    signInSetting.BackgroundColor = baseRequest.Form["backgroundColor"];
                    signInSetting.CityName = baseRequest.Form["cityName"];
                    signInSetting.IsDefaultBackgroundImage = Convert.ToBoolean(baseRequest.Form["isDefaultBackgroundImage"]);
                    signInSetting.IsDefaultSelectImageIndex = Convert.ToInt32(baseRequest.Form["isDefaultSelectImageIndex"]);
                    string imageRootUrl = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ConferenceImageUploadRootPath"]);
                    if (baseRequest.Form["backgroundImageNameType"] != "undefined" && baseRequest.Form["backgroundImageNameType"] != "")
                    {
                        string image = baseRequest.Form["backgroundImageName"];
                        if (!image.IsEmptyOrNull())
                        {
                            //File.Delete(imageRootUrl + "\\" + signInSetting.BackgroundImageName);
                            string newImageName = "";
                            string imageType = baseRequest.Form["backgroundImageNameType"];
                            //本项目图片默认存储到~/Images/目录下

                            newImageName = ImageHelp.SaveImage(image, imageType).NewFileName;
                            signInSetting.BackgroundImageName = newImageName;
                        }
                    }
                }
                SignInSettingAdapter.Instance.AddOrUpdateT(signInSetting);
            });

            return Ok(new ViewModelBase() { State = true });
        }

        /// <summary>
        /// 查找当前会议的签到大屏数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSingInSetting")]
        public IHttpActionResult GetSingInSetting(string conferenceID)
        {
            SignInSetting model = new SignInSetting();
            ControllerHelp.SelectAction(() =>
            {
                model = SignInSettingAdapter.Instance.GetSignInSettingByConferenceID(conferenceID);
            });

            return Ok(model);
        }



    }
}
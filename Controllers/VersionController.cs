using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Version;
using Seagull2.YuanXin.AppApi.Models.Version;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Version;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 版本管理 Controller
    /// </summary>
    public class VersionController : ApiController
    {
        #region 编辑系统升级记录 - PC
        /// <summary>
        /// 编辑系统升级记录 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult Edit(SystemUpdateRecordSaveViewModel model)
        {
            var user = (Seagull2Identity)User.Identity;

            //验证输入
            int version;
            var isVersion = int.TryParse(model.Version.Replace(".", ""), out version);
            if (!isVersion)
            {
                return Ok(new BaseView()
                {
                    State = false,
                    Message = "invalid version number."
                });
            }

            //添加
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                var info = new SystemUpdateRecordModel()
                {
                    AppId = ConfigAppSetting.AppId,
                    Version = model.Version,
                    Description = model.Description,
                    CreateTime = DateTime.Now,
                    Creator = user.Id,
                    ValidStatus = true
                };
                if (model.System == "all")
                {
                    // android
                    info.Code = Guid.NewGuid().ToString();
                    info.System = SystemUpdateRecordSystem.android.ToString();
                    info.DownLoadUrl = model.DownLoadUrlAndroid;
                    SystemUpdateRecordAdapter.Instance.Update(info);
                    // ios
                    info.Code = Guid.NewGuid().ToString();
                    info.System = SystemUpdateRecordSystem.ios.ToString();
                    info.DownLoadUrl = model.DownLoadUrlIos;
                    SystemUpdateRecordAdapter.Instance.Update(info);
                }
                else
                {
                    if (model.System == SystemUpdateRecordSystem.android.ToString())
                    {
                        info.Code = Guid.NewGuid().ToString();
                        info.System = SystemUpdateRecordSystem.android.ToString();
                        info.DownLoadUrl = model.DownLoadUrlAndroid;
                        SystemUpdateRecordAdapter.Instance.Update(info);
                    }
                    if (model.System == SystemUpdateRecordSystem.ios.ToString())
                    {
                        info.Code = Guid.NewGuid().ToString();
                        info.System = SystemUpdateRecordSystem.ios.ToString();
                        info.DownLoadUrl = model.DownLoadUrlIos;
                        SystemUpdateRecordAdapter.Instance.Update(info);
                    }
                }
            }
            //修改
            else
            {
                var find = SystemUpdateRecordAdapter.Instance.Load(w => { w.AppendItem("Code", model.Code); }).SingleOrDefault();
                if (find == null)
                {
                    return Ok(new BaseView()
                    {
                        State = false,
                        Message = "not find."
                    });
                }
                var info = new SystemUpdateRecordModel()
                {
                    Code = model.Code,
                    AppId = ConfigAppSetting.AppId,
                    System = model.System,
                    Version = model.Version,
                    Description = model.Description,
                    CreateTime = find.CreateTime,
                    Creator = find.Creator,
                    ModifyTime = DateTime.Now,
                    Modifier = user.Id,
                    ValidStatus = true
                };
                if (model.System == SystemUpdateRecordSystem.android.ToString())
                {
                    info.DownLoadUrl = model.DownLoadUrlAndroid;
                }
                if (model.System == SystemUpdateRecordSystem.ios.ToString())
                {
                    info.DownLoadUrl = model.DownLoadUrlIos;
                }
                SystemUpdateRecordAdapter.Instance.Update(info);
            }
            ControllerService.UploadLog(user.Id, "操作了工具-版本管理");
            return Ok(new BaseView
            {
                State = true,
                Message = "保存成功！"
            });
        }
        #endregion

        #region 获取系统升级记录列表 - PC
        /// <summary>
        /// 获取系统升级记录列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(int pageSize, int pageIndex, string system = "")
        {
            var dataCount = SystemUpdateRecordAdapter.Instance.GetList(ConfigAppSetting.AppId, system);
            var dataList = SystemUpdateRecordAdapter.Instance.GetList(pageSize, pageIndex, ConfigAppSetting.AppId, system);

            var view = new List<SystemUpdateRecordPCListViewModel>();
            dataList.ForEach(item =>
            {
                view.Add(new SystemUpdateRecordPCListViewModel()
                {
                    Code = item.Code,
                    System = item.System,
                    Version = item.Version,
                    Description = item.Description,
                    DownLoadUrl = item.DownLoadUrl,
                    CreateTime = item.CreateTime
                });
            });

            var data = new BaseViewPage()
            {
                DataCount = dataCount,
                PageCount = dataCount % pageSize == 0 ? dataCount / pageSize : dataCount / pageSize + 1,
                PageData = view
            };

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = data
            });
        }
        #endregion

        #region 获取系统升级记录详情 - PC
        /// <summary>
        /// 获取系统升级记录详情 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetModel(string code)
        {
            var find = SystemUpdateRecordAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
            if (find == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "not find."
                });
            }
            var model = new SystemUpdateRecordPCDetailViewModel
            {
                Code = find.Code,
                System = find.System,
                Version = find.Version,
                Description = find.Description,
                DownLoadUrl = find.DownLoadUrl
            };
            return Ok(new BaseView
            {
                State = true,
                Message = "success.",
                Data = model
            });
        }
        #endregion

        #region 删除系统升级记录 - PC
        /// <summary>
        /// 删除系统升级记录 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            var model = SystemUpdateRecordAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
            if (model == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "not find."
                });
            }
            SystemUpdateRecordAdapter.Instance.Delete(w => w.AppendItem("Code", code));
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了工具-版本管理");
            return Ok(new BaseView
            {
                State = true,
                Message = "删除成功"
            });
        }
        #endregion

        #region 获取最新版本信息 - APP
        /// <summary>
        /// 获取最新版本信息 - APP
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetNewVersionForApp(string system, string version)
        {
            var model = SystemUpdateRecordAdapter.Instance.GetNewVersion(ConfigAppSetting.AppId, system, version);
            if (model == null)
            {
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new
                    {
                        IsNew = true
                    }
                });
            }
            else
            {
                return Ok(new BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = new
                    {
                        IsNew = false,
                        NewVersionInfo = new SystemUpdateRecordAppNewViewModel()
                        {
                            Version = model.Version,
                            Description = model.Description,
                            DownLoadUrl = model.DownLoadUrl
                        }
                    }
                });
            }
        }
        #endregion
    }
}
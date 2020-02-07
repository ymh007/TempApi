using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.YuanXin.AppApi.Models.WorkRegionMenu;
using Seagull2.YuanXin.AppApi.Adapter.WorkRegionMenu;
using Seagull2.Core.Models;
using MCS.Library.OGUPermission;
using System.Data;
using Seagull2.YuanXin.AppApi.Models.Common;
using Newtonsoft.Json.Linq;
using Seagull2.YuanXin.AppApi.Adapter.Common;
using Seagull2.YuanXin.AppApi.Enum;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 工作通菜单 Controller
    /// </summary>
    public class WorkRegionMenuController : ApiController
    {
        #region 编辑菜单 - PC
        /// <summary>
        /// 编辑菜单 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult Edit(WorkRegionMenuModel model)
        {
            var result = ControllerService.Run(() =>
            {
                string currentUser = ((Seagull2Identity)User.Identity).Id;
                //判断APP版本类型
                if (string.IsNullOrWhiteSpace(model.VersionType))
                {
                    throw new Exception("版本类型不能为空！");
                }
                if (model.VersionType != "oldVersion" && model.VersionType != "newVersion")
                {
                    throw new Exception("版本类型错误！");
                }
                string relationCode = "";

                //上传图片
                var isChangeIco = false;
                if (model.IcoUrl.IndexOf("data:image") == 0)
                {
                    //上传图片
                    string icoUrl = FileService.UploadFile(model.IcoUrl);
                    model.IcoUrl = icoUrl;
                    isChangeIco = true;
                }

                //添加
                if (string.IsNullOrWhiteSpace(model.Code))
                {
                    model.Code = Guid.NewGuid().ToString();
                    model.Creator = currentUser;
                    model.CreateTime = DateTime.Now;
                    model.ValidStatus = true;
                    WorkRegionMenuAdapter.Instance.Update(model);
                    relationCode = model.Code;
                }
                //修改
                else
                {
                    var item = WorkRegionMenuAdapter.Instance.Load(p => { p.AppendItem("Code", model.Code); }).SingleOrDefault();
                    if (item == null)
                    {
                        throw new Exception("菜单编号错误！");
                    }
                    item.Name = model.Name;
                    item.Type = model.Type;
                    item.Event = model.Event;
                    if (isChangeIco)
                    {
                        item.IcoUrl = model.IcoUrl;
                    }
                    item.AppKey = model.AppKey;
                    item.CategoryId = model.CategoryId;
                    item.IsTop = model.IsTop;
                    item.IsEnable = model.IsEnable;
                    item.Sort = model.Sort;
                    item.RecommendIco = model.RecommendIco;
                    item.VersionType = model.VersionType;
                    item.Modifier = currentUser;
                    item.ModifyTime = DateTime.Now;
                    WorkRegionMenuAdapter.Instance.Update(item);
                    relationCode = item.Code;
                }
                if (model.IsSavePermission)
                {
                    PersonUnitAdapter.Instance.UpdatePersionUnit(model.PermissionData, relationCode, currentUser, (int)EnumPermissionCategory.WorkRegionMenu);
                }
                ControllerService.UploadLog(currentUser, "操作了工具-工作通菜单管理-工作通菜单管理");
            });
            return Ok(result);
        }
        #endregion

        #region 菜单 推荐图标的操作
        /// <summary>
        /// 添加图标
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult AddRecommend(AppModuleVisible model)
        {
            var result = ControllerService.Run(() =>
            {
                string icoUrl = "";
                if (model.module.IndexOf("data:image") == 0)
                {
                    icoUrl = FileService.UploadFile(model.module);
                }
                string creator = ((Seagull2Identity)User.Identity).Id;
                string code = Guid.NewGuid().ToString();
                int row = WorkRegionMenuAdapter.Instance.AddRecommend(code, icoUrl, 0, creator);
                if (row != 0)
                {
                    return new
                    {
                        Code = code,
                        RecommendIco = icoUrl,
                        RecommendIco_ = FileService.DownloadFile(icoUrl),
                        RecommendType = 0,
                        Creator = creator
                    };
                }
                else
                {
                    throw new Exception("上传失败！");
                }
            });
            return Ok(result);
        }
        /// <summary>
        /// 删除 推荐新 标识
        /// </summary>
        [HttpGet]
        public IHttpActionResult DeleteRecommend(string code)
        {
            var retult = ControllerService.Run(() =>
            {
                WorkRegionMenuAdapter.Instance.DeleteRecommend(code);
            });
            return Ok(retult);
        }


        #endregion


        #region 获取菜单实体 - PC
        /// <summary>
        /// 获取菜单实体 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetModel(string code)
        {
            var list = WorkRegionMenuAdapter.Instance.Load(p => { p.AppendItem("Code", code); });
            if (list.Count <= 0)
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = false,
                    Message = "菜单编号错误！"
                });
            }
            var model = list[0];

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = new ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel()
                {
                    Code = model.Code,
                    Name = model.Name,
                    Type = model.Type,
                    Event = model.Event,
                    IcoUrl = model.IcoUrl,
                    CategoryId = model.CategoryId,
                    IsTop = model.IsTop,
                    IsEnable = model.IsEnable,
                    Sort = model.Sort,
                    AppKey = model.AppKey,
                    VersionType = model.VersionType
                }
            });
        }
        #endregion

        #region 获取菜单分类 - PC
        /// <summary>
        /// 获取菜单分类 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetCategoryList()
        {
            var list = Models.ModelExtention.GetEnumTYpeDesList<Enum.EnumWorkRegionMenuCategory>();
            DataTable dt = WorkRegionMenuAdapter.Instance.QueryRecommend();
            dt.Columns.Add("icoUrl_", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                if (row["RecommendIco"] != null)
                {
                    row["icoUrl_"] = FileService.DownloadFile(row["RecommendIco"].ToString());
                }
            }
            return Ok(new
            {
                State = true,
                Message = "success.",
                Data = list,
                Recommend = dt
            });
        }
        #endregion

        #region 获取菜单列表 - PC
        /// <summary>
        /// 获取菜单列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(string versionType)
        {
            var list = WorkRegionMenuAdapter.Instance.GetList(versionType);

            var view = new List<ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel>();
            foreach (var model in list)
            {
                view.Add(new ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel()
                {
                    Code = model.Code,
                    Name = model.Name,
                    Type = model.Type,
                    Event = model.Event,
                     IcoUrl = model.IcoUrl,
                    CategoryId = model.CategoryId,
                    IsTop = model.IsTop,
                    IsEnable = model.IsEnable,
                    Sort = model.Sort,
                    RecommendIco = FileService.DownloadFile(model.RecommendIco),
                    AppKey = model.AppKey,
                    VersionType = model.VersionType
                });
            }

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }
        #endregion

        #region 获取菜单列表 - APP
        /// <summary>
        /// 获取菜单列表 - APP
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForApp(string versionType = "")
        {
            if (string.IsNullOrWhiteSpace(versionType))
            {
                versionType = "oldVersion";
            }
            else
            {
                versionType = "newVersion";
            }
            Seagull2Identity currentUser = (Seagull2Identity)User.Identity;
            AppCommonPermissionModelCollection currentUserPermission = AppCommonPermissionAdapter.Instance.Load(w => w.AppendItem("Module", (int)EnumPermissionCategory.WorkRegionMenu).AppendItem("UserCode", currentUser.Id));
            List<string> allModulePermission = AppCommonPermissionAdapter.Instance.GetAllModule((int)EnumPermissionCategory.WorkRegionMenu);
            var view = new List<ViewsModel.WorkRegionMenu.WorkRegionMenuAPPViewModel>();
            var eipModule = ConfigAppSetting.EIPModuleForWorkRegion.Split(',');
            var menuList = WorkRegionMenuAdapter.Instance.GetList(versionType);
            var categoryList = Models.ModelExtention.GetEnumTYpeDesList<Enum.EnumWorkRegionMenuCategory>();
            foreach (var category in categoryList)
            {
                var find = menuList.Where(menu => menu.CategoryId == category.Value && menu.IsEnable == true);
                if (!User.IsInRole(ConfigAppSetting.EIPACCESSER))
                {
                    find = find.Where(menu => eipModule.Contains(menu.Event) == false);
                }
                if (find.Count() > 0)
                {
                    var menus = new List<ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel>();
                    foreach (var model in find)
                    {
                        bool isVisible = false;
                        if (allModulePermission.Exists(f => f == model.Code))
                        {
                            isVisible = currentUserPermission.Exists(f => f.RelationCode == model.Code && f.UserCode == currentUser.Id);
                        }
                        else
                        {
                            isVisible = true;
                        }
                        menus.Add(new ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel()
                        {
                            Code = model.Code,
                            Name = model.Name,
                            Type = model.Type,
                            Event = model.Event,
                            // IcoUrl = model.IcoUrl,
                            IcoUrl = "",
                            CategoryId = model.CategoryId,
                            IsTop = model.IsTop,
                            IsEnable = model.IsEnable,
                            Visible = isVisible,
                            RecommendIco = string.IsNullOrEmpty(model.RecommendIco) ? "" : FileService.DownloadFile(model.RecommendIco),
                            AppKey = model.AppKey,
                            Sort = model.Sort
                        });

                    }

                    view.Add(new ViewsModel.WorkRegionMenu.WorkRegionMenuAPPViewModel()
                    {
                        CategoryId = category.Value,
                        CategoryName = category.Text,
                        Menus = menus
                    });
                }
            }

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }

        /// <summary>
        /// 搜索菜单列表 - APP
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListSearchForApp(string key)
        {
            var menus = new List<ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel>();
            if (string.IsNullOrEmpty(key))
            {
                return Ok(new ViewsModel.BaseView()
                {
                    State = true,
                    Message = "success.",
                    Data = menus
                });
            }
            Seagull2Identity currentUser = (Seagull2Identity)User.Identity;
            AppCommonPermissionModelCollection currentUserPermission = AppCommonPermissionAdapter.Instance.Load(w => w.AppendItem("Module", (int)EnumPermissionCategory.WorkRegionMenu).AppendItem("UserCode", currentUser.Id));
            List<string> allModulePermission = AppCommonPermissionAdapter.Instance.GetAllModule((int)EnumPermissionCategory.WorkRegionMenu);
            var view = new List<ViewsModel.WorkRegionMenu.WorkRegionMenuAPPViewModel>();
            var eipModule = ConfigAppSetting.EIPModuleForWorkRegion.Split(',');
            var menuList = WorkRegionMenuAdapter.Instance.GetListBySearch(key);
            var categoryList = Models.ModelExtention.GetEnumTYpeDesList<Enum.EnumWorkRegionMenuCategory>();
            foreach (var category in categoryList)
            {
                var find = menuList.Where(menu => menu.CategoryId == category.Value && menu.IsEnable == true);
                if (!User.IsInRole(ConfigAppSetting.EIPACCESSER))
                {
                    find = find.Where(menu => eipModule.Contains(menu.Event) == false);
                }
                if (find.Count() > 0)
                {
                    foreach (var model in find)
                    {
                        bool isVisible = false;
                        if (allModulePermission.Exists(f => f == model.Code))
                        {
                            isVisible = currentUserPermission.Exists(f => f.RelationCode == model.Code && f.UserCode == currentUser.Id);
                        }
                        else
                        {
                            isVisible = true;
                        }
                        if (isVisible)
                        {
                            menus.Add(new ViewsModel.WorkRegionMenu.WorkRegionMenuViewModel()
                            {
                                Code = model.Code,
                                Name = model.Name,
                                Type = model.Type,
                                Event = model.Event,
                                //IcoUrl = model.IcoUrl,
                                IcoUrl="",
                                CategoryId = model.CategoryId,
                                IsTop = model.IsTop,
                                IsEnable = model.IsEnable,
                                Visible = isVisible,
                                RecommendIco = string.IsNullOrEmpty(model.RecommendIco) ? "" : FileService.DownloadFile(model.RecommendIco),
                                AppKey = model.AppKey,
                                Sort = model.Sort
                            });
                        }
                    }
                }
            }

            return Ok(new ViewsModel.BaseView()
            {
                State = true,
                Message = "success.",
                Data = menus
            });
        }

        #endregion
    }
    public class AppModuleVisible
    {
        public string module { get; set; }
        public List<string> org { get; set; }

        public List<string> user { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 业绩报表控制器
    /// </summary>
    public class PerformanceReportController : ApiController
    {
        #region 获取海鸥2全部菜单
        /// <summary>
        /// 获取海鸥2全部菜单
        /// </summary>
        [HttpGet]
        public Task<IHttpActionResult> GetSeagull2List()
        {
            //获取海鸥Ⅱ菜单
            var seagList = EIPMenuAdapter.Instance.GetList();

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList
            {
                State = true,
                Message = "操作成功！",
                Data = seagList
            }));
        }
        #endregion

        #region 添加和修改菜单
        /// <summary>
        /// 添加和修改菜单
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> Edit(PerformanceReportMenuViewModelPC model)
        {
            //添加菜单
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                //判断菜单是否存在
                bool isExists = PerformanceReportMenuAdapter.Instance.Exists(p => { p.AppendItem("MenuId", model.MenuId); });
                if (isExists)
                {
                    return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull
                    {
                        State = false,
                        Message = "要添加的菜单已存在！"
                    }));
                }

                //组织model
                PerformanceReportMenuModel dbModel = new PerformanceReportMenuModel();
                dbModel.Code = Guid.NewGuid().ToString();
                dbModel.MenuId = model.MenuId;
                dbModel.Href = model.Href;
                dbModel.Sort = model.Sort;
                dbModel.Status = model.Status;
                dbModel.IconSrc = FileService.UploadFile(model.IconResourceSrc);
                dbModel.IconResourceSrc = model.IconResourceSrc;
                dbModel.Creator = ((Seagull2Identity)User.Identity).Id;
                dbModel.CreateTime = DateTime.Now;
                dbModel.ValidStatus = true;
                dbModel.ModuleType = model.ModuleType;

                string msg;
                bool isAdd = PerformanceReportMenuAdapter.Instance.UpdateMenu(dbModel, out msg);

                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList
                {
                    State = isAdd,
                    Message = msg
                }));
            }
            else
            {
                //根据Code获取数据库model
                var list = PerformanceReportMenuAdapter.Instance.Load(m => { m.AppendItem("Code", model.Code); });
                if (list.Count < 1)
                {
                    return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull
                    {
                        State = false,
                        Message = "Code错误！"
                    }));
                }

                //判断是否修改了菜单Id
                if (model.MenuId != list[0].MenuId)
                {
                    bool isExists = PerformanceReportMenuAdapter.Instance.Exists(p => { p.AppendItem("MenuId", model.MenuId); });
                    if (isExists)
                    {
                        return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull
                        {
                            State = false,
                            Message = "要修改的菜单已存在！"
                        }));
                    }
                }

                //组织model
                PerformanceReportMenuModel dbModel = new PerformanceReportMenuModel();
                dbModel.Code = list[0].Code;
                dbModel.MenuId = model.MenuId;
                dbModel.Href = model.Href;
                dbModel.Sort = model.Sort;
                dbModel.Status = model.Status;
                //判断图片是否更改
                if (list[0].IconResourceSrc != model.IconResourceSrc)
                {
                    dbModel.IconSrc = FileService.UploadFile(model.IconResourceSrc);
                    dbModel.IconResourceSrc = model.IconResourceSrc;
                }
                else
                {
                    dbModel.IconSrc = list[0].IconSrc;
                    dbModel.IconResourceSrc = list[0].IconSrc;
                }
                dbModel.Creator = list[0].Creator;
                dbModel.CreateTime = list[0].CreateTime;
                dbModel.Modifier = ((Seagull2Identity)User.Identity).Id;
                dbModel.ModifyTime = DateTime.Now;
                dbModel.ValidStatus = true;
                dbModel.ModuleType = model.ModuleType;

                string msg;
                bool isUpdate = PerformanceReportMenuAdapter.Instance.UpdateMenu(dbModel, out msg);

                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList
                {
                    State = isUpdate,
                    Message = msg
                }));
            }
        }
        #endregion

        #region 获取业绩报表详情
        /// <summary>
        /// 获取业绩报表详情
        /// </summary>
        [HttpGet]
        public Task<IHttpActionResult> GetModel(string code)
        {
            var list = PerformanceReportMenuAdapter.Instance.Load(p =>
            {
                p.AppendItem("Code", code);
            });

            if (list.Count < 1)
            {
                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull
                {
                    State = false,
                    Message = "获取失败！"
                }));
            }

            //获取海鸥Ⅱ菜单
            var seagList = EIPMenuAdapter.Instance.GetList();
            var menu = seagList.Find(f => f.ID == list[0].MenuId);

            var model = new PerformanceReportMenuViewModelPC();
            model.Code = list[0].Code;
            model.MenuId = list[0].MenuId;
            model.MenuName = menu == null ? "" : menu.Name;
            model.Href = list[0].Href;
            model.Sort = list[0].Sort;
            model.Status = list[0].Status;
            model.IconSrc = FileService.DownloadFile(list[0].IconSrc);
            model.IconResourceSrc = list[0].IconResourceSrc;
            model.ModuleType = list[0].ModuleType;

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList
            {
                State = true,
                Message = "操作成功！",
                Data = model
            }));
        }
        #endregion

        #region 获取业绩报表列表
        /// <summary>
        /// 获取业绩报表列表
        /// </summary>
        [HttpGet]
        public Task<IHttpActionResult> GetList()
        {
            //获取全部菜单
            var funcList = PerformanceReportMenuAdapter.Instance.Load(p => { });

            //获取海鸥Ⅱ菜单
            var seagList = EIPMenuAdapter.Instance.GetList();

            //合并菜单
            var margList = new List<PerformanceReportMenuViewModelPC>();
            funcList.ForEach(m =>
            {
                var find = seagList.Find(f => f.ID == m.MenuId && !string.IsNullOrWhiteSpace(f.ParentID));
                if (find != null)
                {
                    margList.Add(new ViewsModel.PerformanceReportMenuViewModelPC()
                    {
                        Code = m.Code,
                        MenuId = m.MenuId,
                        MenuName = find.Name,
                        Href = m.Href,
                        Sort = m.Sort,
                        Status = m.Status,
                        IconSrc = FileService.DownloadFile(m.IconSrc),
                        ModuleType = m.ModuleType
                    });
                }
            });

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList
            {
                State = true,
                Message = "操作成功！",
                Data = margList.OrderBy(p => p.Sort)
            }));
        }
        #endregion

        //APP 接口

        #region 获取用户菜单
        /// <summary>
        /// 获取用户菜单
        /// </summary>
        [HttpGet]
        public Task<IHttpActionResult> GetMenuByUserCode()
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            //获取所有菜单 EIP（ID,Name,ParentID）
            var listEIPAll = EIPMenuAdapter.Instance.GetList();

            //获取用户菜单 EIP（ID,Name,ParentID）
            var listEIPUser = EIPMenuAdapter.Instance.GetList(userCode);

            //获取所有菜单 本地
            var listDBAll = PerformanceReportMenuAdapter.Instance.Load(p => { p.AppendItem("Status", 1); });

            //获取用户关注的菜单 本地
            var listDBUser = PerformanceReportMenuAdapter.Instance.GetList(userCode);

            //所有菜单
            var appAll = CreateMenuTree(listEIPUser, listDBAll, listDBUser);

            //用户关注的菜单
            var appFocus = new List<PerformanceReportMenuViewModelAPP>();
            appFocus.Add(new PerformanceReportMenuViewModelAPP()
            {
                MenuName = "我的关注"
            });
            listDBUser.ForEach(f =>
            {
                if (f.Status == 1)
                {
                    var menu = listEIPAll.Find(m => m.ID == f.MenuId);
                    if (menu != null)
                    {
                        appFocus[0].ChildNodes.Add(new PerformanceReportMenuViewModelAPP()
                        {
                            MenuId = f.MenuId,
                            MenuName = menu.Name,
                            Href = f.Href,
                            IconSrc = FileService.DownloadFile(f.IconSrc),
                            IsFocus = true
                        });
                    }
                }
            });

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseList
            {
                State = true,
                Message = "操作成功！",
                Data = new PerformanceReportMenuViewModelAPPMenuList
                {
                    ListAll = appAll,
                    ListFocus = appFocus
                }
            }));
        }
        #endregion

        #region 添加关注
        /// <summary>
        /// 添加关注
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> AddUserFavourateMenu(PerformanceReportMenuViewModelAPPPost model)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;
            //var userCode = "80f4464f-e912-40c9-9502-c369a0d935ee";

            //判断菜单是否存在
            var allList = EIPMenuAdapter.Instance.GetList(userCode);
            var menu = allList.Find(p => p.ID == model.MenuId);
            if (menu == null)
            {
                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "该菜单不存在或没有权限！"
                }));
            }

            //判断是否已经关注
            bool isExists = PerformanceReportUserFocusAdapter.Instance.Exists(p =>
            {
                p.AppendItem("ReportCode", model.MenuId);
                p.AppendItem("UserCode", userCode);
            });
            if (isExists)
            {
                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "不能重复关注！"
                }));
            }

            //添加关注
            PerformanceReportUserFocusAdapter.Instance.Update(new Models.PerformanceReportUserFocusModel()
            {
                Code = Guid.NewGuid().ToString(),
                ReportCode = model.MenuId,
                UserCode = userCode,
                Sort = 1,
                Creator = userCode,
                CreateTime = DateTime.Now,
                ValidStatus = true
            });

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
            {
                State = true,
                Message = "关注成功！"
            }));
        }
        #endregion

        #region 取消关注
        /// <summary>
        /// 取消关注
        /// </summary>
        [HttpPost]
        public Task<IHttpActionResult> DeleteUserFavourateMenu(PerformanceReportMenuViewModelAPPPost model)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;
            //var userCode = "80f4464f-e912-40c9-9502-c369a0d935ee";

            //判断是否已经关注
            bool isExists = PerformanceReportUserFocusAdapter.Instance.Exists(p =>
            {
                p.AppendItem("ReportCode", model.MenuId);
                p.AppendItem("UserCode", userCode);
            });
            if (isExists == false)
            {
                return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
                {
                    State = false,
                    Message = "取消失败，你没有关注此菜单！"
                }));
            }

            //删除关注信息
            PerformanceReportUserFocusAdapter.Instance.Delete(p =>
            {
                p.AppendItem("ReportCode", model.MenuId);
                p.AppendItem("UserCode", userCode);
            });

            return Task.FromResult<IHttpActionResult>(Ok(new ViewModelBaseNull()
            {
                State = true,
                Message = "取消成功！"
            }));
        }
        #endregion

        #region 菜单转化成树形结构
        /// <summary>
        /// 菜单转化成树形结构
        /// </summary>
        public List<PerformanceReportMenuViewModelAPP> CreateMenuTree(EIPMenuCollection listEIPUser, PerformanceReportMenuCollection listDBAll, PerformanceReportMenuCollection listDBUser)
        {
            var tree = new List<PerformanceReportMenuViewModelAPP>();

            //获取父级菜单
            var listParent = listEIPUser.FindAll(p => string.IsNullOrWhiteSpace(p.ParentID)).OrderBy(p => p.ID);
            foreach (var parent in listParent)
            {
                //获取对应的本地菜单
                var menuParent = listDBAll.Find(m => m.MenuId == parent.ID);
                if (menuParent == null)
                {
                    continue;
                }

                var app = new PerformanceReportMenuViewModelAPP();
                app.MenuName = parent.Name;

                //获取子级菜单
                var listChild = listEIPUser.FindAll(p => p.ParentID == parent.ID);
                foreach (var child in listChild)
                {
                    //获取对应的本地菜单
                    var menuChild = listDBAll.Find(mc => mc.MenuId == child.ID);
                    if (menuChild == null)
                    {
                        continue;
                    }

                    app.ChildNodes.Add(new PerformanceReportMenuViewModelAPP()
                    {
                        MenuId = child.ID,
                        MenuName = child.Name,
                        Href = menuChild.Href,
                        IconSrc = FileService.DownloadFile(menuChild.IconSrc),
                        IsFocus = listDBUser.Find(focus => focus.MenuId == child.ID) == null ? false : true
                    });
                }

                if (app.ChildNodes.Count > 0)
                {
                    tree.Add(app);
                }
            }
            return tree;
        }
        #endregion
    }
}
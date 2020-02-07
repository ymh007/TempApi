using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Newtonsoft.Json;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Share;
using Seagull2.YuanXin.AppApi.Models.Share;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Share;

namespace Seagull2.YuanXin.AppApi.Controllers.Share
{
    /// <summary>
    /// 菜单控制器
    /// </summary>
    public class Share_MenuController : ApiController
    {
        log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region 编辑菜单
        /// <summary>
        /// 编辑菜单
        /// </summary>
        [HttpPost]
        public IHttpActionResult Save(List<MenuViewModel> list)
        {
            var userCode = ((Seagull2Identity)User.Identity).Id;

            // 现有菜单
            var menus = MenuAdapter.Instance.Load(w => { });
            // 新菜单List<Code>
            var posts = new List<string>();

            // 更新菜单
            foreach (var model in list)
            {
                posts.Add(model.Code);
                if (model.Code.StartsWith("temp-menu-"))
                {
                    model.Code = Guid.NewGuid().ToString();
                }
                MenuAdapter.Instance.Update(new MenuModel()
                {
                    Code = model.Code,
                    Name = model.Name,
                    ParentCode = string.Empty,
                    Sort = model.Sort,
                    Creator = userCode,
                    CreateTime = DateTime.Now,
                    Modifier = userCode,
                    ModifyTime = DateTime.Now,
                    ValidStatus = true,
                });

                foreach (var subModel in model.SubMenu)
                {
                    posts.Add(subModel.Code);
                    if (subModel.Code.StartsWith("temp-menu-"))
                    {
                        subModel.Code = Guid.NewGuid().ToString();
                    }
                    MenuAdapter.Instance.Update(new MenuModel()
                    {
                        Code = subModel.Code,
                        Name = subModel.Name,
                        ParentCode = model.Code,
                        Sort = subModel.Sort,
                        Creator = userCode,
                        CreateTime = DateTime.Now,
                        Modifier = userCode,
                        ModifyTime = DateTime.Now,
                        ValidStatus = true
                    });
                }
            }

            // 从库中删除多余的菜单
            menus.ForEach(menu =>
            {
                var find = posts.Where(w => w == menu.Code);
                if (find.Count() < 1)
                {
                    MenuAdapter.Instance.Delete(w => w.AppendItem("Code", menu.Code));
                }
            });
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "操作了应用管理-营销学院-菜单管理-菜单");
            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
            });
        }
        #endregion

        #region 查询菜单列表
        /// <summary>
        /// 查询菜单列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetList()
        {
            var view = new List<MenuViewModel>();
            var menus = MenuAdapter.Instance.Load(p => { });
            // 根菜单
            var root = menus.Where(p => p.ParentCode == string.Empty).OrderBy(p => p.Sort).ToList();
            root.ForEach(modelRoot =>
            {
                var menu = new MenuViewModel()
                {
                    Code = modelRoot.Code,
                    Name = modelRoot.Name,
                    Sort = modelRoot.Sort,
                    SubMenu = new List<MenuBaseViewModel>()
                };
                // 子菜单
                var child = menus.Where(p => p.ParentCode == modelRoot.Code).OrderBy(p => p.Sort).ToList();
                child.ForEach(modelChild =>
                {
                    menu.SubMenu.Add(new MenuBaseViewModel()
                    {
                        Code = modelChild.Code,
                        Name = modelChild.Name,
                        Sort = modelChild.Sort
                    });
                });
                view.Add(menu);
            });

            return Ok(new BaseView()
            {
                State = true,
                Message = "success.",
                Data = view
            });
        }
        #endregion
    }
}

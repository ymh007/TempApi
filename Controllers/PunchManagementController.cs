using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Adapter.PunchManagement;
using Seagull2.YuanXin.AppApi.Models.PunchManagement;
using Seagull2.YuanXin.AppApi.ViewsModel.PunchManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 打卡管理
    /// </summary>
    public class PunchManagementController : ApiController
    {
        #region 新增或编辑打卡管理单元
        /// <summary>
        /// 新增或编辑打卡管理单元
        /// </summary>
        [HttpPost]
        public IHttpActionResult PunchManagementSave(PunchManagementViewModel model)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;

                // 添加
                if (string.IsNullOrEmpty(model.Code))
                {
                    // 判断打卡管理单元组合是否存在
                    var departCodeList = model.PunchDepartmentList.Select(f => f.Id).OrderBy(o => o).ToList();
                    var isExist = PunchDepartmentAdapter.Instance.IsExist(string.Empty, departCodeList);
                    if (isExist)
                    {
                        throw new Exception("该部门/人员已添加管理员，请在对应打卡管理单元添加管理员！");
                    }

                    // 判断打卡管理单元名称是否存在
                    var dbModel = PunchManagementAdapter.Instance.Load(w => w.AppendItem("Name", model.Title)).FirstOrDefault();
                    if (dbModel != null)
                    {
                        throw new Exception("此名称已被使用！");
                    }

                    var code = Guid.NewGuid().ToString();

                    // 添加打卡管理单元
                    PunchManagementAdapter.Instance.Update(new PunchManagementModel
                    {
                        Code = code,
                        Name = model.Title,
                        Type = model.Type,
                        CreatorName = user.DisplayName,
                        OnTime = model.OnTime,
                        OffTime = model.OffTime,
                        PunchArea = model.PunchArea,
                        IsChange = model.IsChange,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true,
                    });

                    // 更新子表信息
                    UpdateChildren(code, model);
                }
                else
                {
                    // 判断打卡管理单元组合是否存在
                    var departCodeList = model.PunchDepartmentList.Select(f => f.Id).OrderBy(o => o).ToList();
                    var isExist = PunchDepartmentAdapter.Instance.IsExist(model.Code, departCodeList);
                    if (isExist)
                    {
                        throw new Exception("该部门/人员已添加管理员，请在对应打卡管理单元添加管理员！");
                    }

                    // 判断打卡管理单元名称是否存在
                    var dbModel = PunchManagementAdapter.Instance.Load(w => w.AppendItem("Name", model.Title).AppendItem("Code", model.Code, "<>")).FirstOrDefault();
                    if (dbModel != null)
                    {
                        throw new Exception("此名称已被使用！");
                    }

                    // 根据编码查询已有数据
                    var punchManagement = PunchManagementAdapter.Instance.Load(w => w.AppendItem("Code", model.Code)).FirstOrDefault();
                    if (punchManagement == null)
                    {
                        throw new Exception("编码错误!");
                    }

                    //编辑打卡单元
                    punchManagement.Name = model.Title;
                    punchManagement.Type = model.Type;
                    punchManagement.OnTime = model.OnTime;
                    punchManagement.OffTime = model.OffTime;
                    punchManagement.PunchArea = model.PunchArea;
                    punchManagement.IsChange = model.IsChange;
                    punchManagement.ValidStatus = model.ValidStatus;
                    punchManagement.Modifier = user.Id;
                    punchManagement.ModifyTime = DateTime.Now;
                    PunchManagementAdapter.Instance.Update(punchManagement);

                    // 更新子表信息
                    UpdateChildren(model.Code, model); 
                }
                ControllerService.UploadLog(user.Id, "操作了应用管理-考勤打卡-打卡管理单元");
            });
            return Ok(result);
        }
        #endregion

        #region 获取打卡管理列表及搜索
        /// <summary>
        /// 获取打卡管理列表及搜索
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetPunchManagementList(int pageIndex, int pageSize, string name = "", string conName = "")
        {
            var result = ControllerService.Run(() =>
            {
                var identity = (Seagull2Identity)User.Identity;

                var count = PunchManagementAdapter.Instance.GetListByPage(name, conName);
                var table = PunchManagementAdapter.Instance.GetListByPage(pageIndex, pageSize, name, conName);
                var lists = DataConvertHelper<PunchManagementModel>.ConvertToList(table);
                var checkList = new List<PunchManagementViewModel>();
                var configPersons = PunchManagementAdapter.Instance.GetConfigPersons().ToList().ConvertAll(c => c.CreatorName);
                //数据格式化
                lists.ForEach(m =>
                {
                    var punchCode = m.Code;
                    //获取部门列表
                    var PunchDepartmentList = PunchDepartmentAdapter.Instance.Load(e =>
                    {
                        e.AppendItem("PunchCode", punchCode);
                    }, o =>
                    {
                        o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                    }).ToList();

                    //获取管理人员列表
                    var PunchManagerList = PunchManagerAdapter.Instance.Load(e =>
                    {
                        e.AppendItem("PunchCode", punchCode);
                    }, o =>
                    {
                        o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                    });


                    //格式化部门
                    var checkPunchDepartmentList = new List<PunchDepartmentListItem>();

                    PunchDepartmentList.ForEach(PunchDepartmentitem =>
                    {
                        var PunchDepartment = new PunchDepartmentListItem
                        {
                            Id = PunchDepartmentitem.ConcatCode,
                            DisplayName = PunchDepartmentitem.Name,
                            ObjectType = PunchDepartmentitem.Type,
                        };
                        checkPunchDepartmentList.Add(PunchDepartment);
                    });

                    //格式化管理人员列表
                    var checkPunchManagerList = new List<PunchManagerListItem>();

                    PunchManagerList.ForEach(PunchManageritem =>
                    {
                        var PunchManager = new PunchManagerListItem
                        {
                            Id = PunchManageritem.ConcatCode,
                            DisplayName = PunchManageritem.Name,
                        };
                        checkPunchManagerList.Add(PunchManager);
                    });

                    var item = new PunchManagementViewModel
                    {
                        Code = m.Code,
                        Title = m.Name,
                        Creator = m.Creator,
                        CreatorName = m.CreatorName,
                        Type = m.Type,
                        OnTime = m.OnTime,
                        OffTime = m.OffTime,
                        PunchArea = m.PunchArea,
                        IsChange = m.IsChange,
                        PunchDepartmentList = checkPunchDepartmentList,
                        PunchManagerList = checkPunchManagerList,
                        ValidStatus = m.ValidStatus
                    };
                    checkList.Add(item);

                });
                return new
                {
                    PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
                    Data = checkList,
                    configPersons
                };
            });
            return Ok(result);
        }
        #endregion

        #region 删除打卡管理单元
        /// <summary>
        /// 删除打卡管理单元
        /// </summary>
        [HttpGet]
        public IHttpActionResult PunchManagementDelete(string code)
        {
            var result = ControllerService.Run(() =>
            {
                // 删除考勤对象
                PunchDepartmentAdapter.Instance.Delete(m => { m.AppendItem("PunchCode", code); });

                // 删除考勤管理员
                PunchManagerAdapter.Instance.Delete(m => { m.AppendItem("PunchCode", code); });

                // 删除所有考勤人员
                PunchPersonnelAdapter.Instance.Delete(code);

                // 删除打卡单元
                PunchManagementAdapter.Instance.Delete(m => { m.AppendItem("Code", code); });

               ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了应用管理-考勤打卡-打卡管理单元");
            });
            return Ok(result);
        }
        #endregion

        #region 更新打卡部门/人员是否改变的状态
        /// <summary>
        /// 更新打卡部门/人员是否改变的状态
        /// </summary>
        [HttpGet]
        public IHttpActionResult IsChange(string code)
        {
            var result = ControllerService.Run(() =>
            {
                var model = PunchManagementAdapter.Instance.Load(w => w.AppendItem("Code", code)).SingleOrDefault();
                if (model == null)
                {
                    throw new Exception("编码错误！");
                }
                model.IsChange = false;
                PunchManagementAdapter.Instance.Update(model);
            });
            return Ok(result);
        }
        #endregion

        #region 手动更新某个打卡管理单元的所有人员 - 测试时使用
        /// <summary>
        /// 手动更新某个打卡管理单元的所有人员 - 测试时使用
        /// </summary>
        [HttpGet]
        public IHttpActionResult UpdatePerson(string code)
        {
            var result = ControllerService.Run(() =>
            {
                PunchPersonnelAdapter.Instance.UpdatePserson(code);
            });
            return Ok(result);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 更新 - 考勤对象、考勤管理员、所有考勤人员
        /// </summary>
        void UpdateChildren(string Code, PunchManagementViewModel model)
        {
            var user = (Seagull2Identity)User.Identity;

            // 删除考勤对象
            PunchDepartmentAdapter.Instance.Delete(m => { m.AppendItem("PunchCode", Code); });

            // 删除考勤管理员
            PunchManagerAdapter.Instance.Delete(m => { m.AppendItem("PunchCode", Code); });

            var i = 0;
            // 新增考勤对象
            model.PunchDepartmentList.ForEach(item =>
            {
                var punchDepartment = new PunchDepartmentModel
                {
                    Code = Guid.NewGuid().ToString(),
                    PunchCode = Code,
                    Type = item.ObjectType,
                    Name = item.DisplayName,
                    ConcatCode = item.Id,
                    Creator = user.Id,
                    CreateTime = DateTime.Now.AddSeconds(i),
                    ValidStatus = true,
                };
                i++;
                PunchDepartmentAdapter.Instance.Update(punchDepartment);
            });
            var j = 0;

            // 新增考勤管理员
            model.PunchManagerList.ForEach(item =>
            {

                var punchManager = new PunchManagerModel
                {
                    Code = Guid.NewGuid().ToString(),
                    PunchCode = Code,
                    Name = item.DisplayName,
                    ConcatCode = item.Id,
                    Creator = user.Id,
                    CreateTime = DateTime.Now.AddSeconds(j),
                    ValidStatus = true,
                };
                j++;
                PunchManagerAdapter.Instance.Update(punchManager);
            });

            // 更新所有人员信息
            System.Threading.Tasks.Task.Run(() =>
            {
                PunchPersonnelAdapter.Instance.UpdatePserson(Code);
            });
        }
        #endregion
    }
}
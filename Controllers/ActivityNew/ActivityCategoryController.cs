using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ActivityNew;
using Seagull2.YuanXin.AppApi.Models.ActivityNew;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.ActivityNew;

namespace Seagull2.YuanXin.AppApi.Controllers.ActivityNew
{
    /// <summary>
    /// 活动分类 Controller
    /// </summary>
    public class ActivityCategoryController : ApiController
    {
        #region 编辑活动分类 - PC
        /// <summary>
        /// 编辑活动分类 - PC
        /// </summary>
        [HttpPost]
        public IHttpActionResult Edit(ActivityCategorySaveViewModel model)
        {
            var user = (Seagull2Identity)User.Identity;

            //上传图片
            var isIcon = false;
            if (model.Icon.StartsWith("data:image"))
            {
                model.Icon = FileService.UploadFile(model.Icon);
                isIcon = true;
            }

            //添加
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                ActivityCategoryAdapter.Instance.Update(new ActivityCategoryModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    Icon = isIcon ? model.Icon : string.Empty,
                    IsEnable = model.IsEnable,
                    Sort = model.Sort,
                    Creator = user.Id,
                    CreateTime = DateTime.Now,
                    ValidStatus = true
                });
            }
            //修改
            else
            {
                var find = ActivityCategoryAdapter.Instance.Load(p => { p.AppendItem("Code", model.Code); }).SingleOrDefault();
                if (find == null)
                {
                    return Ok(new BaseView()
                    {
                        State = false,
                        Message = "not find."
                    });
                }
                ActivityCategoryAdapter.Instance.Update(new ActivityCategoryModel()
                {
                    Code = model.Code,
                    Name = model.Name,
                    Icon = isIcon ? model.Icon : find.Icon,
                    IsEnable = model.IsEnable,
                    Sort = model.Sort,
                    Creator = find.Creator,
                    CreateTime = find.CreateTime,
                    Modifier = user.Id,
                    ModifyTime = DateTime.Now,
                    ValidStatus = true
                });
            }
            ControllerService.UploadLog(user.Id, "操作了应用管理-活动-活动类型");
            return Ok(new BaseView
            {
                State = true,
                Message = "保存成功！"
            });
        }
        #endregion

        #region 获取活动分类列表 - PC
        /// <summary>
        /// 获取活动分类列表 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetList(int pageSize, int pageIndex, string name = "")
        {
            var dataCount = ActivityCategoryAdapter.Instance.GetList(name);
            var dataList = ActivityCategoryAdapter.Instance.GetList(pageSize, pageIndex, name);

            var view = new List<ActivityCategoryViewModel>();
            dataList.ForEach(item =>
            {
                view.Add(new ViewsModel.ActivityNew.ActivityCategoryViewModel()
                {
                    Code = item.Code,
                    Name = item.Name,
                    Icon = item.Icon,
                    IsEnable = item.IsEnable,
                    Sort = item.Sort
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

        #region 获取活动分类详情 - PC
        /// <summary>
        /// 获取活动分类详情 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetModel(string code)
        {
            var find = ActivityCategoryAdapter.Instance.Load(m => m.AppendItem("Code", code)).SingleOrDefault();
            if (find == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "not find."
                });
            }
            var model = new ActivityCategoryViewModel
            {
                Code = find.Code,
                Name = find.Name,
                Icon = find.Icon,
                IsEnable = find.IsEnable,
                Sort = find.Sort
            };
            return Ok(new BaseView
            {
                State = true,
                Message = "success.",
                Data = model
            });
        }
        #endregion

        #region 删除活动分类 - PC
        /// <summary>
        /// 删除活动分类 - PC
        /// </summary>
        [HttpGet]
        public IHttpActionResult Delete(string code)
        {
            var model = ActivityCategoryAdapter.Instance.Load(m => m.AppendItem("Code", code)).SingleOrDefault();
            if (model == null)
            {
                return Ok(new BaseView
                {
                    State = false,
                    Message = "not find."
                });
            }
            ActivityCategoryAdapter.Instance.Delete(w => w.AppendItem("Code", code));
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了应用管理-活动-活动类型" + model.Name);
            return Ok(new BaseView
            {
                State = true,
                Message = "删除成功"
            });
        }
        #endregion

        #region 获取活动分类列表 - APP
        /// <summary>
        /// 获取活动分类列表 - APP
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForApp()
        {
            var list = new List<ActivityCategoryForAppViewModel>();
            var data = ActivityCategoryAdapter.Instance.Load(
                 where =>
                 {
                     where.AppendItem("IsEnable", true);
                 },
                 orderby =>
                 {
                     orderby.AppendItem("Sort", MCS.Library.Data.Builder.FieldSortDirection.Ascending);
                 });
            data.ForEach(item =>
            {
                list.Add(new ActivityCategoryForAppViewModel()
                {
                    Code = item.Code,
                    Name = item.Name,
                    Icon = item.Icon
                });
            });

            return Ok(new BaseView
            {
                State = true,
                Message = "success.",
                Data = list
            });
        }
        #endregion
    }
}
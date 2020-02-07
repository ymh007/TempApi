using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Channel;
using Seagull2.YuanXin.AppApi.Models.Channel;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.Channel;

namespace Seagull2.YuanXin.AppApi.Controllers
{
    /// <summary>
    /// 频道管理
    /// </summary>
    public class ChannelController : ApiController
    {
        // PC端

        #region 推荐频道新增/编辑
        /// <summary>
        /// 新增/编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Save(ChannelModel model)
        {
            var user = (Seagull2Identity)User.Identity;
            if (string.IsNullOrEmpty(model.Code))//新增
            {
                model.Code = Guid.NewGuid().ToString();
                model.Creator = user.Id;
                model.Modifier = user.Id;
                model.ModifyTime = DateTime.Now;
                model.CreateTime = DateTime.Now;
                model.ValidStatus = true;
                ChannelAdapter.Instance.Update(model);
            }
            else //编辑
            {
                var edModel = ChannelAdapter.Instance.Load(m => m.AppendItem("Code", model.Code)).SingleOrDefault();
                edModel.Name = model.Name;
                edModel.Keys = model.Keys;
                edModel.Sort = model.Sort;
                edModel.IsDefault = model.IsDefault;
                edModel.IsEnable = model.IsEnable;
                edModel.Modifier = user.Id;
                edModel.ModifyTime = DateTime.Now;
                ChannelAdapter.Instance.Update(edModel);
            }
            ControllerService.UploadLog(user.Id, "操作了工具-资讯频道管理");
            return Ok(new BaseView
            {
                State = true,
                Message = "保存成功！"
            });
        }
        #endregion

        #region 查询频道列表
        /// <summary>
        /// 查询频道列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForPC()
        {
            var coll = ChannelAdapter.Instance.Load(m => m.AppendItem("ValidStatus", true)).OrderBy(m => m.Sort).ToList();
            List<GetListForPCViewModel> result = new List<GetListForPCViewModel>();
            coll.ForEach(m =>
            {
                var viewModel = new GetListForPCViewModel();
                viewModel.Code = m.Code;
                viewModel.Name = m.Name;
                viewModel.Sort = m.Sort;
                viewModel.IsDefault = m.IsDefault;
                viewModel.IsEnable = m.IsEnable;
                viewModel.Key = m.Keys;
                result.Add(viewModel);
            });
            return Ok(new BaseView
            {
                State = true,
                Message = "查询成功",
                Data = result
            });
        }
        #endregion

        #region 根据Code查询频道
        [HttpGet]
        public IHttpActionResult GetChannelByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Ok(new BaseView
                {
                    State = true,
                    Message = "参数不能为空"
                });
            }
            var model = ChannelAdapter.Instance.Load(m => m.AppendItem("Code", code)).SingleOrDefault();
            return Ok(new BaseView
            {
                State = true,
                Message = "查询成功",
                Data = model
            });
        }
        #endregion

        #region 删除推荐频道
        [HttpGet]
        public IHttpActionResult Delete(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Ok(new BaseView
                {
                    State = true,
                    Message = "参数不能为空"
                });
            }
            ChannelAdapter.Instance.Delete(m => m.AppendItem("Code", Code));
            ControllerService.UploadLog(((Seagull2Identity)User.Identity).Id, "删除了工具-资讯频道管理");
            return Ok(new BaseView
            {
                State = true,
                Message = "删除成功"
            });
        }
        #endregion

        // APP端

        #region 查询频道列表
        /// <summary>
        /// 查询频道列表
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetListForAPP()
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                var listForAll = ChannelAdapter.Instance.Load(w => { w.AppendItem("IsEnable", true); });
                var listForFav = ChannelFavAdapter.Instance.Load(w => { w.AppendItem("UserCode", user.Id); });
                var list = new ChannelForAppViewModel(listForAll, listForFav);
                return list;
            });
            return Ok(result);
        }
        #endregion

        #region 收藏频道
        /// <summary>
        /// 收藏频道
        /// </summary>
        [HttpPost]
        public IHttpActionResult CollectionChannel(List<SaveChannelViewModel> list)
        {
            var result = ControllerService.Run(() =>
            {
                var user = (Seagull2Identity)User.Identity;
                ChannelFavAdapter.Instance.Delete(m => m.AppendItem("UserCode", user.Id));
                var group = list.GroupBy(item => item.Code);
                foreach (var g in group)
                {
                    var model = g.ToList()[0];
                    ChannelFavAdapter.Instance.Update(new ChannelFavModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        UserCode = user.Id,
                        ChannelCode = model.Code,
                        Sort = model.Sort,
                        Creator = user.Id,
                        CreateTime = DateTime.Now,
                        ValidStatus = true
                    });
                }
            });
            return Ok(result);
        }
        #endregion
    }
}

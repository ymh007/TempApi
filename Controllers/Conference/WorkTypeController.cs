using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.Conference.WorkTypeViewModel;

namespace Seagull2.YuanXin.AppApi.Controllers.Conference
{
	/// <summary>
	/// 工作人员类型-Controller
	/// </summary>
	public class WorkTypeController : ApiController
	{
		/// <summary>
		/// 保存编辑
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult Save(SaveModel model)
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				if (string.IsNullOrEmpty(model.ID))
				{
					//新增
					WorkerTypeAdapter.Instance.Update(new Models.WorkerTypeModel
					{
						ID = Guid.NewGuid().ToString(),
						Name = model.Name,
						Sort = model.Sort,
                        ContactId=model.ContactId,
						Creator = user.LogonName,
						CreateTime = DateTime.Now,
						ValidStatus = true
					});
				}
				else
				{
					//编辑
					var workTypeModel = WorkerTypeAdapter.Instance.Load(m => m.AppendItem("ID", model.ID)).FirstOrDefault();
					if (workTypeModel == null)
					{
						throw new Exception(string.Format("无法找到ID为{0}的数据", model.ID));
					}
					WorkerTypeAdapter.Instance.Update(new Models.WorkerTypeModel
					{
						ID = model.ID,
						Name = model.Name,
						Sort = model.Sort,
                        ContactId = workTypeModel.ContactId,
                        Creator = workTypeModel.Creator,
						CreateTime = DateTime.Now,
						ValidStatus = true
					});
				}
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取列表
		/// </summary>
		/// <returns></returns>
		public IHttpActionResult GetList(string ContactId)
		{
			var result = ControllerService.Run(() =>
			{
				List<GetModel> list = new List<GetModel>();
				var coll = WorkerTypeAdapter.Instance.Load(m => m.AppendItem("ContactId", ContactId)).OrderBy(o => o.Sort).ToList();
				coll.ForEach(m =>
				{
					list.Add(new GetModel
					{
						ID = m.ID,
						Name = m.Name,
						Creator = m.Creator,
                        ContactId=m.ContactId,
						CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
						Sort = m.Sort
					});
				});
				return list;
			});
			return Ok(result);
		}

		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult DeleteByID(string id)
		{
			var result = ControllerService.Run(() =>
			{
				var coll = WorkerTypeAdapter.Instance.Load(m => m.AppendItem("ID", id));
				if (coll.Count > 0)
				{
					WorkerTypeAdapter.Instance.Delete(m => m.AppendItem("ID", id));
				}
				else
				{
					throw new Exception("无效的id");
				}

			});
			return Ok(result);
		}
	}
}
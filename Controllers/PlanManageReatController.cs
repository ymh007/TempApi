using Seagull2.Core.Models;
using Seagull2.Owin.Organization.Controllers;
using Seagull2.Owin.Organization.Services;
using Seagull2.Permission.Organization;
using Seagull2.YuanXin.AppApi.Services;
using Seagull2.YuanXin.AppApi.ViewsModel.PlanManage;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;


namespace Seagull2.YuanXin.AppApi.Controllers
{
	[AllowAnonymous]
	public class PlanManageReatController : ApiController
	{
		public readonly IPlanManageRate _planManageRate;

		public PlanManageReatController(IPlanManageRate planManageRate)
		{
			_planManageRate = planManageRate;
		}
		/// <summary>
		/// 加载事业节点达成率
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IHttpActionResult> GetGroupYearEipKeyPointRateTable()
		{
			string stareTime = "";
			return Ok(await _planManageRate.GetGroupYearEipKeyPointRateTable(stareTime));
		}

		/// <summary>
		/// 加载事业节点达成率
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IHttpActionResult> WeiCeshi()
		{
			return Ok(await _planManageRate.WeiCeshi());
		}

		/// <summary>
		/// 获取项目进度
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetProjectProgress(string planCode = "")
		{
			var result = ControllerService.Run(() =>
			{
				//定义返回数据格式
				PlanManageViewModel data = new PlanManageViewModel();
				List<PointItem> list = new List<PointItem>();
				List<IndegreeItem> inList = new List<IndegreeItem>();
				//返回数据
				List<DataHelper> dataList = Adapter.PlanManage.PlanManageAdapter.Instance.GetPointList(planCode);
				//遍历返回数据
				List<DataHelper> dataListRoot = dataList.FindAll(m => m.ParentCode == "ROOT");
				var tasks = new List<System.Threading.Tasks.Task>();
				tasks.Add(System.Threading.Tasks.Task.Run(() =>
				{
					//遍历根节点数据
					dataListRoot.ForEach(m =>
					{
						#region 添加根级任务
						list.Add(new PointItem
						{
							Task = new ViewsModel.PlanManage.Task
							{
								TaskName = m.TaskName
							},
							Plan = new Plan
							{
								PlanName = m.CnName,
								PlanCode = m.PlanCode
							},
							Relation = new Relation
							{
								StartPoint = "-1",
								EndPoint = m.Code,
								Length = ((m.EndTime - m.StartTime).TotalDays + 1).ToString()
							}
						});

						//添加code
						inList.Add(new IndegreeItem
						{
							Code = m.Code,
							Name = m.TaskName + "完成"
						});
						#endregion

						#region 非根级任务
						List<DataHelper> dataHelper = new List<DataHelper>();
						var nodeList = RecursionHelper(m.Code, dataList, dataHelper);
						//如果有子集
						if (nodeList.Count > 0)
						{
							nodeList.ForEach(item =>
							{
								//添加任务
								list.Add(new PointItem
								{
									Task = new ViewsModel.PlanManage.Task
									{
										TaskName = item.TaskName
									},
									Plan = new Plan
									{
										PlanName = item.CnName,
										PlanCode = item.PlanCode
									},
									Relation = new Relation
									{
										StartPoint = item.ParentCode,
										EndPoint = item.Code,
										Length = ((item.EndTime - item.StartTime).TotalDays + 1).ToString()
									}
								});
								//添加code
								inList.Add(new IndegreeItem
								{
									Code = item.Code,
									Name = item.TaskName + "完成"
								});
								if (dataList.FindAll(o => o.ParentCode == item.Code).Count == 0)
								{
									//添加计划
									list.Add(new PointItem
									{
										Task = new ViewsModel.PlanManage.Task
										{
											TaskName = item.CnName
										},
										Plan = new Plan
										{
											PlanName = item.CnName,
											PlanCode = item.PlanCode
										},
										Relation = new Relation
										{
											StartPoint = item.Code,
											EndPoint = item.PlanCode,
											Length = "0"
										}
									});
								}
							});
						}
						else
						{
							//添加计划
							list.Add(new PointItem
							{
								Task = new ViewsModel.PlanManage.Task
								{
									TaskName = m.CnName
								},
								Plan = new Plan
								{
									PlanName = m.CnName,
									PlanCode = m.PlanCode
								},
								Relation = new Relation
								{
									StartPoint = m.Code,
									EndPoint = m.PlanCode,
									Length = "0"
								}
							});
						}
						#endregion

					});

					#region 添加计划
					dataList.ForEach(m =>
					{
						if (inList.FindAll(o => o.Code == m.PlanCode).Count == 0)
						{
							list = list.FindAll(t => t.Relation != null);
							var model = list.FindAll(t => t.Relation.EndPoint == m.PlanCode);
							//计划code
							inList.Add(new IndegreeItem
							{
								Code = m.PlanCode,
								//获取项目名称
								Name = m.CnName + "完成"
							});
						}
					});
					//获取所有的计划
					var planList = dataList.Select(p => p.PlanCode);
					planList = planList.Distinct().ToList();
					foreach (var id in planList)
					{
						//添加计划
						list.Add(new PointItem
						{
							Relation = new Relation
							{
								StartPoint = id,
								EndPoint = "0",
								Length = "0"
							}
						});
					}
					#endregion

				}));
				tasks.Add(System.Threading.Tasks.Task.Run(() =>
				{
					//添加起点code
					inList.Add(new IndegreeItem
					{
						Code = "-1",
						Name = "项目开始"
					});
					//终结点code
					inList.Add(new IndegreeItem
					{
						Code = "0",
						Name = "项目结束"
					});
				}));
				System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
				data.EdgeList = list;
				data.VertexList = inList;
				return data;
			});
			return Ok(result);
		}

		/// <summary>
		/// 递归帮助类
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public List<DataHelper> RecursionHelper(string code, List<DataHelper> dataList, List<DataHelper> list)
		{
			var coll = dataList.FindAll(o => o.ParentCode == code);
			if (coll.Count > 0)
			{
				//如果能找到该code的子节点
				list.AddRange(coll);
				coll.ForEach(m =>
				{
					RecursionHelper(m.Code, dataList, list);
				});
			}
			return list;
		}

		/// <summary>
		/// 获取项目进度
		/// </summary>
		/// <returns></returns>
	}
}
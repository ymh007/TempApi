using MCS.Library.Data.Builder;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ResearchData;
using Seagull2.YuanXin.AppApi.Domain.ResearchData;
using Seagull2.YuanXin.AppApi.Domain.YuanXinOfficeCommon;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using Seagull2.YuanXin.AppApi.ViewsModel.ResearchData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Http;
using static Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon.YuanXinOfficeCommon;

namespace Seagull2.YuanXin.AppApi.Controllers
{
	/// <summary>
	/// 客研数据Controller
	/// </summary>
	public class ResearchDataController : ApiController
	{
		/// <summary>
		/// 保存编辑基础信息和政策
		/// </summary>
		/// <returns>IHttpActionResult</returns>
		[HttpPost]
		public IHttpActionResult SavePolicy(SavePolicyViewModel post)
		{
			var result = ControllerService.Run(() =>
			{
				if (string.IsNullOrWhiteSpace(post.RD_Info.Code))
				{
					var list = RD_InfoAdapter.Instance.Load(w => w.AppendItem("CityName", post.RD_Info.CityName.Trim()));
					if (list.Count > 0)
					{
						throw new Exception("已经存在该城市，不能重复添加！");
					}
				}
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.SavePolicy(post, user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-城市与政策");
            });
			return Ok(result);
		}

		/// <summary>
		/// 保存编辑经济
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult SaveEconomic(List<RD_Economic> list)
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.Save_Economic(list, user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-经济");
            });
			return Ok(result);
		}

		/// <summary>
		/// 保存编辑对标
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult SaveBenchmark(SaveBenchmarkViewModel post)
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.SaveRD_Benchmark(post, user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-对标");
            });
			return Ok(result);
		}

		/// <summary>
		/// 保存编辑新房市场
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult SaveNewHousingMarket(SaveNewHousingMarketViewModel post)
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.SaveNewHousingMarket(post, user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-新房市场");
            });
			return Ok(result);
		}

		/// <summary>
		/// 保存编辑土地市场
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult SaveLandMarket(List<RD_LandMarket> list)
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.SaveRD_LandMarket(list, user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-土地市场");
            });
			return Ok(result);
		}

		/// <summary>
		/// 保存编辑人口
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult SavePopulation(List<RD_Population> list)
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.SaveRD_Population(list, user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-人口");
            });
			return Ok(result);
		}

		/// <summary>
		/// 获取城市列表
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetList(int pageIndex, int pageSize, string cityName = "")
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				var count = RD_InfoAdapter.Instance.GetCount(cityName);
				var data = domain.GetList(cityName, pageIndex, pageSize);
				var dataResult = new BaseViewPage
				{
					DataCount = count,
					PageCount = count % pageSize == 0 ? count / pageSize : count / pageSize + 1,
					PageData = data
				};
				return dataResult;
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取城市政策
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetCityPolicy(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetCityPolicy(code);
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取经济
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetEconomic(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetEconomic(code);
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取人口信息
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetPopulation(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetPopulation(code);
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取土地市场信息
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetLandMarket(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetLandMarket(code);
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取新房市场
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetNewHousingMarket(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetNewHousingMarket(code);
			});
			return Ok(result);
		}

		/// <summary>
		/// 获取对标信息
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetBenchmark(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetBenchmark(code);
			});
			return Ok(result);
		}

		/// <summary>
		/// 根据客研编码删除城市
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult DeleteCityByCode(string code)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				domain.DeleteCityByCode(code);
                var user = (Seagull2Identity)User.Identity;
                ControllerService.UploadLog(user.Id, "删除了应用管理-城市数据-城市");
            });
			return Ok(result);
		}

		/// <summary>
		/// Excel导入
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult ExcelImport()
		{
			var result = ControllerService.Run(() =>
			{
				var user = (Seagull2Identity)User.Identity;
				var domain = new ResearchDataDomain();
				domain.ExcelImport(user);
                ControllerService.UploadLog(user.Id, "操作了应用管理-城市数据-导入表格");
            });
			return Ok(result);
		}

		/// <summary>
		/// APP获取客研数据
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetRD_DataForAPP(string cityName)
		{
			var result = ControllerService.Run(() =>
			{
				var domain = new ResearchDataDomain();
				return domain.GetRD_DataForAPP(cityName);

			});
			return Ok(result);
		}

		/// <summary>
		/// APP获取有效的城市列表
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IHttpActionResult GetValidCityListv1(string cityName = "")
		{
			var result = ControllerService.Run(() =>
			{
                var groupData =new  List<object>();
				var domain = new ResearchDataDomain();
                IEnumerable<RD_InfoModel> source=domain.GetValidCityList(cityName);
                source.ToList().ForEach(f=> {
                    f.PinYin = NPinyin.Pinyin.GetPinyin(f.CityName.ToCharArray()[0]).FirstOrDefault().ToString().ToUpper();
                });
                source.GroupBy(g => g.PinYin).OrderBy(o=>o.Key).ToList().ForEach(f=> {
                    var p =new { pinYin=f.Key,data=new List< RD_InfoModel>() };
                    f.ToList().ForEach(x=> {
                        p.data.Add(x);
                    });
                    groupData.Add(p);
                });
              
                return groupData;
            });
			return Ok(result);
		}

        /// <summary>
		/// APP获取有效的城市列表
		/// </summary>
		/// <returns></returns>
		[HttpGet]
        public IHttpActionResult GetValidCityList(string cityName = "")
        {
            var result = ControllerService.Run(() =>
            {
                var domain = new ResearchDataDomain();
                return domain.GetValidCityList(cityName);
            });
            return Ok(result);
        }


        /// <summary>
        /// APP城市变更
        /// </summary>
        [HttpGet]
		public IHttpActionResult UrbanChange(string cityName)
		{
			var result = ControllerService.Run(() =>
			{   // 过滤字符串
				cityName = cityName.Replace("市", string.Empty);
				var info = RD_InfoAdapter.Instance.Load(m =>
				{
					m.AppendItem("CityName", cityName);
					m.AppendItem("ValidStatus", true);
				});
				if (info.Count > 0)
				{
					var user = (Seagull2Identity)User.Identity;
					PushMessageBody post = new PushMessageBody
					{
						Code = string.Empty,
						Content = $"{cityName}市城市数据，点击查看详情",
						Creaotr = user.Id,
						CreatorName = user.DisplayName,
						MoudleType = EnumMessageModuleType.ResearchData,
						PushType = 0,
						Title = "城市数据",
						TitleType = EnumMessageTitle.System,
						UserList = new List<string> { user.Id }
					};
					var domain = new PushDomain();
					domain.Push(post);
				}
			});
			return Ok(result);
		}
	}
}

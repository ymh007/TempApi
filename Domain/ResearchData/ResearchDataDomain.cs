using MCS.Library.Data;
using MCS.Library.OGUPermission;
using NPOI.SS.Formula.Functions;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Adapter.ResearchData;
using Seagull2.YuanXin.AppApi.Common;
using Seagull2.YuanXin.AppApi.Models.ResearchData;
using Seagull2.YuanXin.AppApi.ViewsModel.ResearchData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Domain.ResearchData
{
	public class ResearchDataDomain
	{
		/// <summary>
		/// 获取列表
		/// </summary>
		/// <param name="cityName">城市名称</param>
		/// <param name="pageIndex">页索引</param>
		/// <param name="pageSize">页大小</param>
		/// <returns></returns>
		public List<ListViewModel> GetList(string cityName, int pageIndex, int pageSize)
		{
			var coll = RD_InfoAdapter.Instance.GetList(cityName, pageIndex, pageSize);
			var list = new List<ListViewModel>();
			coll.ForEach(m =>
			{
				var item = new ListViewModel
				{
					Code = m.Code,
					CityName = m.CityName,
					CreateTime = m.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
					ValidStatus = m.ValidStatus
				};
				list.Add(item);
			});
			return list;
		}

		/// <summary>
		/// 保存基础信息和政策
		/// </summary>
		/// <param name="post">保存viewmodel</param>
		public void SavePolicy(SavePolicyViewModel post, Seagull2Identity user)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				var code = this.SaveRD_Info(post.RD_Info, user);
				this.SaveRD_Policy(post.RD_Policy, user, code);
				scope.Complete();
			}
		}

		/// <summary>
		/// 保存经济
		/// </summary>
		/// <param name="list">RD_Economic</param>
		/// <param name="user">user</param>
		public void Save_Economic(List<RD_Economic> list, Seagull2Identity user)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				var coll = new List<RD_EconomicModel>();
				list.ForEach(m =>
				{
					var item = new RD_EconomicModel();
					item.Code = Guid.NewGuid().ToString();
					item.Creator = user.Id;
					item.CreateTime = DateTime.Now;
					item.ValidStatus = true;
					item.ResearchData_Code = m.ResearchData_Code;
					item.Annual = m.Annual;
					item.GDPTotal = m.GDPTotal;
					item.GDPIncrement = m.GDPIncrement;
					item.GovernmentReceipts = m.GovernmentReceipts;
					item.ThirdIndustry = m.ThirdIndustry;
					item.InvestmentProportion = m.InvestmentProportion;
					coll.Add(item);
				});
				if (list.Count > 0)
				{
					RD_EconomicAdapter.Instance.DeleteByRd_Code(list.FirstOrDefault().ResearchData_Code);
				}
				RD_EconomicAdapter.Instance.BatchInsert(coll);
				scope.Complete();
			}
		}

		/// <summary>
		/// 保存客研数据-基础信息
		/// </summary>
		/// <param name="model">RD_Info</param>
		/// <param name="user">user</param>
		public string SaveRD_Info(RD_Info model, Seagull2Identity user)
		{
			var item = new RD_InfoModel();
			var result = string.Empty;
			if (!string.IsNullOrEmpty(model.Code))
			{
				item = RD_InfoAdapter.Instance.GetInfoByCode(model.Code);
				if (item == null) { throw new Exception("无效的code"); }
			}
			if (string.IsNullOrEmpty(model.Code))
			{
				item.Code = Guid.NewGuid().ToString();
				item.Creator = user.Id;

				item.CreateTime = DateTime.Now;
			}
			item.CityName = model.CityName;
			item.ValidStatus = model.ValidStatus;
			RD_InfoAdapter.Instance.Update(item);
			result = item.Code;
			return result;
		}

		/// <summary>
		/// 保存客研数据-政策
		/// </summary>
		/// <param name="model">model</param>
		/// <param name="user">user</param>
		/// <param name="code">客研数据编码</param>
		public void SaveRD_Policy(RD_Policy model, Seagull2Identity user, string code)
		{
			if (model != null)
			{
				var item = new RD_PolicyModel();
				if (!string.IsNullOrEmpty(model.Code))
				{
					item = RD_PolicyAdapter.Instance.GetInfoByRD_Code(code);
				}
				if (string.IsNullOrEmpty(model.Code))
				{
					item.Code = Guid.NewGuid().ToString();
					item.ResearchData_Code = code;
					item.Creator = user.Id;
					item.CreateTime = DateTime.Now;
					item.ValidStatus = true;
				}
				item.LimitPurchase = model.LimitPurchase;
				item.LimitLoan = model.LimitLoan;
				item.MortgageRate = model.MortgageRate;
				item.LimitSell = model.LimitSell;
				item.LimitCommercial = model.LimitCommercial;
				item.LimitPrice = model.LimitPrice;
				item.LimitSign = model.LimitSign;
				item.ShakingNumber = model.ShakingNumber;
				item.PreSaleConditions = model.PreSaleConditions;
				item.LimitCompanyPurchase = model.LimitCompanyPurchase;
				RD_PolicyAdapter.Instance.Update(item);
			}
		}

		/// <summary>
		/// 新增编辑-对标
		/// </summary>
		public void SaveRD_Benchmark(SaveBenchmarkViewModel post, Seagull2Identity user)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				if (post.RD_Benchmark_EnterpriseSaleStatistics.Count > 0)
				{
					RD_Benchmark_ProjectSaleStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_Benchmark_EnterpriseSaleStatistics.FirstOrDefault().ResearchData_Code);
					RD_Benchmark_EnterpriseSaleStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_Benchmark_EnterpriseSaleStatistics.FirstOrDefault().ResearchData_Code);
				}
				if (post.RD_Benchmark_ProjectSaleStatistics.Count > 0)
				{
					RD_Benchmark_ProjectSaleStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_Benchmark_ProjectSaleStatistics.FirstOrDefault().ResearchData_Code);
					RD_Benchmark_EnterpriseSaleStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_Benchmark_ProjectSaleStatistics.FirstOrDefault().ResearchData_Code);
				}
				this.SaveRD_Benchmark_EnterpriseSaleStatistics(post.RD_Benchmark_EnterpriseSaleStatistics, user);
				this.SaveRD_Benchmark_ProjectSaleStatistics(post.RD_Benchmark_ProjectSaleStatistics, user);
				scope.Complete();
			}
		}

		/// <summary>
		/// 新增对标_企业销售统计
		/// </summary>
		public void SaveRD_Benchmark_EnterpriseSaleStatistics(List<RD_Benchmark_EnterpriseSaleStatistics> list, Seagull2Identity user)
		{
			var listData = new List<RD_Benchmark_EnterpriseSaleStatisticsModel>();
			list.ForEach(m =>
			{
				var item = new RD_Benchmark_EnterpriseSaleStatisticsModel
				{
					Code = Guid.NewGuid().ToString(),
					ResearchData_Code = m.ResearchData_Code,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true,
					Enterprise = m.Enterprise,
					SalesVolume = m.SalesVolume,
					MarketShare = m.MarketShare,
					Sort = m.Sort,
					Type = m.Type
				};
				listData.Add(item);
			});

			RD_Benchmark_EnterpriseSaleStatisticsAdapter.Instance.BatchInsert(listData);
		}

		/// <summary>
		/// 新增对标_项目销售统计
		/// </summary>
		public void SaveRD_Benchmark_ProjectSaleStatistics(List<RD_Benchmark_ProjectSaleStatistics> list, Seagull2Identity user)
		{
			var listData = new List<RD_Benchmark_ProjectSaleStatisticsModel>();
			list.ForEach(m =>
			{
				var item = new RD_Benchmark_ProjectSaleStatisticsModel
				{
					Code = Guid.NewGuid().ToString(),
					ResearchData_Code = m.ResearchData_Code,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true,
					Project = m.Project,
					SalesVolume = m.SalesVolume,
					Sort = m.Sort,
					Type = m.Type
				};
				listData.Add(item);
			});
			RD_Benchmark_ProjectSaleStatisticsAdapter.Instance.BatchInsert(listData);
		}

		/// <summary>
		/// 新增编辑新房市场
		/// </summary>
		/// <param name="post"></param>
		/// <param name="user"></param>
		public void SaveNewHousingMarket(SaveNewHousingMarketViewModel post, Seagull2Identity user)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				if (post.RD_NHM_AnnualStatistics.Count > 0)
				{
					RD_NHM_AnnualStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_NHM_AnnualStatistics.FirstOrDefault().ResearchData_Code);
					RD_NHM_MonthlyStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_NHM_AnnualStatistics.FirstOrDefault().ResearchData_Code);
				}
				if (post.RD_NHM_MonthlyStatistics.Count > 0)
				{
					RD_NHM_AnnualStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_NHM_MonthlyStatistics.FirstOrDefault().ResearchData_Code);
					RD_NHM_MonthlyStatisticsAdapter.Instance.DeleteByRD_Code(post.RD_NHM_MonthlyStatistics.FirstOrDefault().ResearchData_Code);
				}
				this.SaveRD_NHM_AnnualStatistics(post.RD_NHM_AnnualStatistics, user);
				this.SaveRD_NHM_MonthlyStatistics(post.RD_NHM_MonthlyStatistics, user);
				scope.Complete();
			}
		}

		/// <summary>
		/// 新增新房市场_年度统计
		/// </summary>
		public void SaveRD_NHM_AnnualStatistics(List<RD_NHM_AnnualStatistics> list, Seagull2Identity user)
		{
			var listData = new List<RD_NHM_AnnualStatisticsModel>();
			list.ForEach(m =>
			{
				var item = new RD_NHM_AnnualStatisticsModel
				{
					Code = Guid.NewGuid().ToString(),
					ResearchData_Code = m.ResearchData_Code,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true,
					Annual = m.Annual,
					MarketTransactionArea = m.MarketTransactionArea,
					NewSupplyArea = m.NewSupplyArea,
					MarketTransactionAmount = m.MarketTransactionAmount,
					APOMT = m.APOMT,
					MarketingRatio = m.MarketingRatio
				};
				listData.Add(item);
			});
			RD_NHM_AnnualStatisticsAdapter.Instance.BatchInsert(listData);
		}

		/// <summary>
		/// 新增新房市场_月度统计
		/// </summary>
		public void SaveRD_NHM_MonthlyStatistics(List<RD_NHM_MonthlyStatistics> list, Seagull2Identity user)
		{
			var listData = new List<RD_NHM_MonthlyStatisticsModel>();
			list.ForEach(m =>
			{
				var item = new RD_NHM_MonthlyStatisticsModel
				{
					Code = Guid.NewGuid().ToString(),
					ResearchData_Code = m.ResearchData_Code,
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true,
					Monthly = DataConvertHelper<DateTime>.ConvertToBaseType(m.Monthly),
					MarketTransactionArea = m.MarketTransactionArea,
					NewSupplyArea = m.NewSupplyArea,
					MarketTransactionAmount = m.MarketTransactionAmount,
					APOMT = m.APOMT,
					MarketingRatio = m.MarketingRatio,
					SupplyStock = m.SupplyStock,
					MarketCycle = m.MarketCycle
				};
				listData.Add(item);
			});
			RD_NHM_MonthlyStatisticsAdapter.Instance.BatchInsert(listData);
		}

		/// <summary>
		/// 新增编辑土地市场
		/// </summary>
		/// <param name="list">list</param>
		public void SaveRD_LandMarket(List<RD_LandMarket> list, Seagull2Identity user)
		{
			var coll = new List<RD_LandMarketModel>();
			list.ForEach(m =>
			{
				var item = new RD_LandMarketModel
				{
					Code = Guid.NewGuid().ToString(),
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true,
					ResearchData_Code = m.ResearchData_Code,
					Monthly = DataConvertHelper<DateTime>.ConvertToBaseType(m.Monthly),
					LandTransactionScale = m.LandTransactionScale,
					FloorPrice = m.FloorPrice,
					PremiumRate = m.PremiumRate,
					LandGrant = m.LandGrant,
					LowPriceRatio = m.LowPriceRatio
				};
				coll.Add(item);
			});
			if (coll.Count > 0)
			{
				RD_LandMarketAdapter.Instance.DeleteByRD_Code(coll.FirstOrDefault().ResearchData_Code);
			}
			RD_LandMarketAdapter.Instance.BatchInsert(coll);
		}

		/// <summary>
		/// 新增编辑人口
		/// </summary>
		/// <param name="list">list</param>
		public void SaveRD_Population(List<RD_Population> list, Seagull2Identity user)
		{
			var coll = new List<RD_PopulationModel>();
			list.ForEach(m =>
			{
				var item = new RD_PopulationModel
				{
					Code = Guid.NewGuid().ToString(),
					Creator = user.Id,
					CreateTime = DateTime.Now,
					ValidStatus = true,
					ResearchData_Code = m.ResearchData_Code,
					Annual = m.Annual,
					PermanentPopulationTotal = m.PermanentPopulationTotal,
					PermanentResidentsTotal = m.PermanentResidentsTotal,
					PermanentPopulationIncrement = m.PermanentPopulationIncrement,
					NetPopulationInflow = m.NetPopulationInflow,
					PerCapitaSavings = m.PerCapitaSavings,
					PrimarySchoolEnrollment = m.PrimarySchoolEnrollment,
					UniversitySchoolEnrollment = m.UniversitySchoolEnrollment,
					UrbanizationRate = m.UrbanizationRate
				};
				coll.Add(item);
			});
			if (coll.Count > 0)
			{
				RD_PopulationAdapter.Instance.DeleteByRD_Code(coll.FirstOrDefault().ResearchData_Code);
			}
			RD_PopulationAdapter.Instance.BatchInsert(coll);
		}

		/// <summary>
		/// 获取城市政策
		/// </summary>
		/// <param name="code">客研数据编码</param>
		/// <returns></returns>
		public CityPolicyInfo GetCityPolicy(string code)
		{
			var info = RD_InfoAdapter.Instance.GetInfoByCode(code);
			var policy = RD_PolicyAdapter.Instance.GetInfoByRD_Code(code);
			var result = new CityPolicyInfo();
			if (info != null)
			{

				result.RD_Info = ObjectCopy.Copy<RD_InfoModel, RD_Info>(info);
			}
			if (policy != null)
			{
				result.RD_Policy = ObjectCopy.Copy<RD_PolicyModel, RD_Policy>(policy);
			}
			return result;

		}

		/// <summary>
		/// 获取经济
		/// </summary>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_Economic> GetEconomic(string code)
		{
			var listData = new List<RD_Economic>();
			var ecommic = RD_EconomicAdapter.Instance.GetEconomicByRD_Code(code);
			ecommic.ForEach(m =>
			{
				listData.Add(ObjectCopy.Copy<RD_EconomicModel, RD_Economic>(m));
			});
			return listData;
		}

		/// <summary>
		/// 获取人口
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public List<RD_Population> GetPopulation(string code)
		{
			var listData = new List<RD_Population>();
			var populattion = RD_PopulationAdapter.Instance.GetPopulationByRD_Code(code);
			if (populattion.Count > 0)
			{
				populattion.ForEach(m =>
				{
					listData.Add(ObjectCopy.Copy<RD_PopulationModel, RD_Population>(m));
				});
			}
			return listData;
		}

		/// <summary>
		/// 获取土地市场
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public List<RD_LandMarket> GetLandMarket(string code)
		{
			var listData = new List<RD_LandMarket>();
			var landMarket = RD_LandMarketAdapter.Instance.GetLandMarketByRD_Code(code);
			landMarket.ForEach(m =>
			{
				var item = ObjectCopy.Copy<RD_LandMarketModel, RD_LandMarket>(m);
				item.Monthly = m.Monthly.ToString("yyyy-MM");
				listData.Add(item);
			});
			return listData;
		}

		/// <summary>
		/// 获取新房市场
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public NewHousingMarketInfo GetNewHousingMarket(string code)
		{
			var data = new NewHousingMarketInfo();
			var annual = RD_NHM_AnnualStatisticsAdapter.Instance.GetNHM_AnnualStatisticsByRD_Code(code);
			var monthly = RD_NHM_MonthlyStatisticsAdapter.Instance.GetNHM_MonthlyStatisticsByRD_Code(code);
			var annualList = new List<RD_NHM_AnnualStatistics>();
			var monthlyList = new List<RD_NHM_MonthlyStatistics>();
			if (annual.Count > 0)
			{
				annual.ForEach(m =>
				{
					annualList.Add(ObjectCopy.Copy<RD_NHM_AnnualStatisticsModel, RD_NHM_AnnualStatistics>(m));
				});
			}
			if (monthly.Count > 0)
			{
				monthly.ForEach(m =>
				{
					var item = ObjectCopy.Copy<RD_NHM_MonthlyStatisticsModel, RD_NHM_MonthlyStatistics>(m);
					item.Monthly = m.Monthly.ToString("yyyy-MM");
					monthlyList.Add(item);
				});
			}
			data.RD_NHM_AnnualStatistics = annualList;
			data.RD_NHM_MonthlyStatistics = monthlyList;
			return data;
		}

		/// <summary>
		/// 获取对标信息
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public BenchmarkInfo GetBenchmark(string code)
		{
			var data = new BenchmarkInfo();
			var enterprise = RD_Benchmark_EnterpriseSaleStatisticsAdapter.Instance.GetNHM_AnnualStatisticsByRD_Code(code);
			var project = RD_Benchmark_ProjectSaleStatisticsAdapter.Instance.GetNHM_AnnualStatisticsByRD_Code(code);
			var enterpriseList = new List<RD_Benchmark_EnterpriseSaleStatistics>();
			var projectList = new List<RD_Benchmark_ProjectSaleStatistics>();
			if (enterprise.Count > 0)
			{
				enterprise.ForEach(m =>
				{
					enterpriseList.Add(ObjectCopy.Copy<RD_Benchmark_EnterpriseSaleStatisticsModel, RD_Benchmark_EnterpriseSaleStatistics>(m));
				});
			}
			if (project.Count > 0)
			{
				project.ForEach(m =>
				{
					projectList.Add(ObjectCopy.Copy<RD_Benchmark_ProjectSaleStatisticsModel, RD_Benchmark_ProjectSaleStatistics>(m));
				});
			}
			data.RD_Benchmark_EnterpriseSaleStatistics = enterpriseList;
			data.RD_Benchmark_ProjectSaleStatistics = projectList;
			return data;
		}

		/// <summary>
		/// Excel导入数据库
		/// </summary>
		public void ExcelImport(Seagull2Identity user)
		{
			ExcelService service = new ExcelService();
			var infoTable = service.GetExcelData("ResearchData", 0, 0, 0);
			var policyTable = service.GetExcelData("ResearchData", 1, 1, 0);
			var economicTabel = service.GetExcelData("ResearchData", 2, 1, 0);
			var populationTabel = service.GetExcelData("ResearchData", 3, 1, 0);
			var nhmTable = service.GetExcelData("ResearchData", 4, 1, 0);
			var landMarketTabel = service.GetExcelData("ResearchData", 5, 1, 0);
			var benchmarkTabel = service.GetExcelData("ResearchData", 6, 2, 0);
			var infoModel = ConvertToRD_InfoModel(infoTable, user);
			var policyModel = ConvertRD_PolicyModel(policyTable, user, infoModel.Code);
			var economicColl = ConvertRD_EconomicModel(economicTabel, user, infoModel.Code);
			var populationColl = ConvertRD_PopulationModel(populationTabel, user, infoModel.Code);
			var nhm_Annua = ConvertRD_NHM_AnnualStatistics(nhmTable, user, infoModel.Code);
			var nhm_monthly = ConvertRD_NHM_MonthlyStatistics(nhmTable, user, infoModel.Code);
			var landMarketColl = ConvertRD_LandMarketModel(landMarketTabel, user, infoModel.Code);
			var benchmark_Annua = ConvertRD_Benchmark_EnterpriseSaleStatisticsModel(benchmarkTabel, user, infoModel.Code);
			var benchmark_monthly = ConvertRD_Benchmark_ProjectSaleStatisticsModel(benchmarkTabel, user, infoModel.Code);
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				var data = RD_InfoAdapter.Instance.GetRD_InfoByCity(infoModel.CityName);
				if (data != null)
				{
					RD_InfoAdapter.Instance.DeleteAllInfo(data.Code);
				}
				if (infoModel != null)
				{
					RD_InfoAdapter.Instance.Update(infoModel);
				}
				if (policyModel != null)
				{
					RD_PolicyAdapter.Instance.Update(policyModel);
				}
				if (economicColl.Count > 0)
				{
					RD_EconomicAdapter.Instance.BatchInsert(economicColl);
				}
				if (populationColl.Count > 0)
				{
					RD_PopulationAdapter.Instance.BatchInsert(populationColl);
				}
				if (nhm_Annua.Count > 0)
				{
					RD_NHM_AnnualStatisticsAdapter.Instance.BatchInsert(nhm_Annua);
				}
				if (nhm_monthly.Count > 0)
				{
					RD_NHM_MonthlyStatisticsAdapter.Instance.BatchInsert(nhm_monthly);
				}
				if (landMarketColl.Count > 0)
				{
					RD_LandMarketAdapter.Instance.BatchInsert(landMarketColl);
				}
				if (benchmark_Annua.Count > 0)
				{
					RD_Benchmark_EnterpriseSaleStatisticsAdapter.Instance.BatchInsert(benchmark_Annua);
				}
				if (benchmark_monthly.Count > 0)
				{
					RD_Benchmark_ProjectSaleStatisticsAdapter.Instance.BatchInsert(benchmark_monthly);
				}
				scope.Complete();
			}
		}

		/// <summary>
		/// Tabel转化为基础信息Model
		/// </summary>
		/// <param name="table">table</param>
		/// <param name="user">当前请求用户信息</param>
		/// <returns></returns>
		public RD_InfoModel ConvertToRD_InfoModel(DataTable table, Seagull2Identity user)
		{
			if (string.IsNullOrEmpty(table.Rows[0][0].ToString())) { throw new Exception("城市名称必须填写"); }
			var model = BaseAttributeAssignment.Assignment<RD_InfoModel>(user);
			model.CityName = table.Rows[0][0].ToString();
			return model;
		}

		/// <summary>
		/// table转化为政策model
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public RD_PolicyModel ConvertRD_PolicyModel(DataTable table, Seagull2Identity user, string code)
		{
			if (table.Rows.Count < 1) { return null; }
			var model = BaseAttributeAssignment.Assignment<RD_PolicyModel>(user);
			model.ResearchData_Code = code;
			model.LimitPurchase = table.Rows[0][0].ToString();
			model.LimitLoan = table.Rows[0][1].ToString();
			model.MortgageRate = table.Rows[0][2].ToString();
			model.LimitSell = table.Rows[0][3].ToString();
			model.LimitCommercial = table.Rows[0][4].ToString();
			model.LimitPrice = table.Rows[0][5].ToString();
			model.LimitSign = table.Rows[0][6].ToString();
			model.ShakingNumber = table.Rows[0][7].ToString();
			model.PreSaleConditions = table.Rows[0][8].ToString();
			model.LimitCompanyPurchase = table.Rows[0][9].ToString();
			return model;
		}

		/// <summary>
		/// table转化为经济Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_EconomicModel> ConvertRD_EconomicModel(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_EconomicModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 5; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][0].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_EconomicModel>(user);
				model.ResearchData_Code = code;
				model.Annual = DataConvertHelper<Int32>.ConvertToBaseType(table.Rows[i][0].ToString());
				model.GDPTotal = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][1].ToString());
				model.GDPIncrement = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][2].ToString());
				model.GovernmentReceipts = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][3].ToString());
				model.ThirdIndustry = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][4].ToString());
				model.InvestmentProportion = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][5].ToString());
				list.Add(model);
			}
			return list;
		}

		/// <summary>
		/// table转化为人口Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_PopulationModel> ConvertRD_PopulationModel(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_PopulationModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 5; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][0].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_PopulationModel>(user);
				model.ResearchData_Code = code;
				model.Annual = DataConvertHelper<Int32>.ConvertToBaseType(table.Rows[i][0].ToString());
				model.PermanentPopulationTotal = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][1].ToString());
				model.PermanentResidentsTotal = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][2].ToString());
				model.PermanentPopulationIncrement = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][3].ToString());
				model.NetPopulationInflow = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][4].ToString());
				model.PerCapitaSavings = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][5].ToString());
				model.PrimarySchoolEnrollment = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][6].ToString());
				model.UniversitySchoolEnrollment = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][7].ToString());
				model.UrbanizationRate = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][8].ToString());
				list.Add(model);
			}
			return list;
		}

		/// <summary>
		/// table转化为新房市场_年度统计Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_NHM_AnnualStatisticsModel> ConvertRD_NHM_AnnualStatistics(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_NHM_AnnualStatisticsModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 5; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][0].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_NHM_AnnualStatisticsModel>(user);
				model.ResearchData_Code = code;
				model.Annual = DataConvertHelper<Int32>.ConvertToBaseType(table.Rows[i][0].ToString());
				model.MarketTransactionArea = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][1].ToString());
				model.NewSupplyArea = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][2].ToString());
				model.MarketTransactionAmount = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][3].ToString());
				model.APOMT = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][4].ToString());
				model.MarketingRatio = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][5].ToString());
				list.Add(model);
			}
			return list;
		}

		/// <summary>
		/// table转化为新房市场_月度统计Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_NHM_MonthlyStatisticsModel> ConvertRD_NHM_MonthlyStatistics(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_NHM_MonthlyStatisticsModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 12; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][7].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_NHM_MonthlyStatisticsModel>(user);
				model.ResearchData_Code = code;
				model.Monthly = DataConvertHelper<DateTime>.ConvertToBaseType(table.Rows[i][7].ToString());
				model.MarketTransactionArea = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][8].ToString());
				model.NewSupplyArea = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][9].ToString());
				model.MarketTransactionAmount = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][10].ToString());
				model.APOMT = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][11].ToString());
				model.MarketingRatio = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][12].ToString());
				model.SupplyStock = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][13].ToString());
				model.MarketCycle = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][14].ToString());
				list.Add(model);
			}
			return list;
		}

		/// <summary>
		/// table转化为土地市场Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_LandMarketModel> ConvertRD_LandMarketModel(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_LandMarketModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 12; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][0].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_LandMarketModel>(user);
				model.ResearchData_Code = code;
				model.Monthly = DataConvertHelper<DateTime>.ConvertToBaseType(table.Rows[i][0].ToString());
				model.LandTransactionScale = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][1].ToString());
				model.FloorPrice = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][2].ToString());
				model.PremiumRate = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][3].ToString());
				model.LandGrant = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][4].ToString());
				model.LowPriceRatio = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][5].ToString());
				list.Add(model);
			}
			return list;
		}

		/// <summary>
		/// table转化为对标_企业统计Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_Benchmark_EnterpriseSaleStatisticsModel> ConvertRD_Benchmark_EnterpriseSaleStatisticsModel(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_Benchmark_EnterpriseSaleStatisticsModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 10; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][0].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_Benchmark_EnterpriseSaleStatisticsModel>(user);
				model.ResearchData_Code = code;
				model.Enterprise = table.Rows[i][0].ToString();
				model.SalesVolume = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][1].ToString());
				model.MarketShare = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][2].ToString());
				model.Sort = i + 1;
				model.Type = 0;
				var item = BaseAttributeAssignment.Assignment<RD_Benchmark_EnterpriseSaleStatisticsModel>(user);
				item.ResearchData_Code = code;
				item.Enterprise = Convert.ToString(table.Rows[i][3].ToString());
				item.SalesVolume = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][4].ToString());
				item.MarketShare = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][5].ToString());
				item.Sort = i + 1;
				item.Type = 1;
				list.Add(model);
				list.Add(item);
			}
			return list;
		}

		/// <summary>
		/// table转化为对标_项目统计Collection
		/// </summary>
		/// <param name="table">tabel</param>
		/// <param name="user">当前请求用户信息</param>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_Benchmark_ProjectSaleStatisticsModel> ConvertRD_Benchmark_ProjectSaleStatisticsModel(DataTable table, Seagull2Identity user, string code)
		{
			var list = new List<RD_Benchmark_ProjectSaleStatisticsModel>();
			if (table.Rows.Count < 1) { return list; }
			for (int i = 0; i < 10; i++)
			{
				if (string.IsNullOrEmpty(table.Rows[i][7].ToString())) { continue; }
				var model = BaseAttributeAssignment.Assignment<RD_Benchmark_ProjectSaleStatisticsModel>(user);
				model.ResearchData_Code = code;
				model.Project = table.Rows[i][7].ToString();
				model.SalesVolume = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][8].ToString());
				model.Sort = i + 1;
				model.Type = 0;
				var item = BaseAttributeAssignment.Assignment<RD_Benchmark_ProjectSaleStatisticsModel>(user);
				item.ResearchData_Code = code;
				item.Project = table.Rows[i][9].ToString();
				item.SalesVolume = DataConvertHelper<Decimal>.ConvertToBaseType(table.Rows[i][10].ToString());
				item.Sort = i + 1;
				item.Type = 1;
				list.Add(model);
				list.Add(item);
			}
			return list;
		}

		/// <summary>
		/// 根据城市名称获取所有数据
		/// </summary>
		/// <param name="cityName"></param>
		/// <returns></returns>
		public RD_DataForAPP GetRD_DataForAPP(string cityName)
		{
			var result = new RD_DataForAPP();
			var data = RD_InfoAdapter.Instance.GetRD_InfoByCity(cityName);
			if (data != null)
			{
				var CityPolicyInfo = Task<CityPolicyInfo>.Factory.StartNew(() =>
				{
					return GetCityPolicy(data.Code);
				});
				var RD_Economic = Task<List<RD_Economic>>.Factory.StartNew(() =>
				{
					return GetEconomic(data.Code);
				});
				var RD_Population = Task<List<RD_Population>>.Factory.StartNew(() =>
				{
					return GetPopulation(data.Code);
				});
				var RD_LandMarket = Task<List<RD_LandMarket>>.Factory.StartNew(() =>
				{
					return GetLandMarket(data.Code);
				});
				var NewHousingMarketInfo = Task<NewHousingMarketInfo>.Factory.StartNew(() =>
				{
					return GetNewHousingMarket(data.Code);
				});
				var BenchmarkInfo = Task<BenchmarkInfo>.Factory.StartNew(() =>
				{
					return GetBenchmark(data.Code);
				});
				result.CityPolicyInfo = CityPolicyInfo.Result;
				result.RD_Economic = RD_Economic.Result;
				result.RD_Population = RD_Population.Result;
				result.RD_LandMarket = RD_LandMarket.Result;
				result.NewHousingMarketInfo = NewHousingMarketInfo.Result;
				result.BenchmarkInfo = BenchmarkInfo.Result;
			}
			return result;
		}

		/// <summary>
		/// 获取有效的城市列表
		/// </summary>
		/// <param name="city"></param>
		/// <returns></returns>
		public IEnumerable<RD_InfoModel> GetValidCityList(string city)
		{
			return RD_InfoAdapter.Instance.Load(where =>
			{
				where.AppendItem("ValidStatus", true);
				if (!string.IsNullOrEmpty(city))
				{
					where.AppendItem("CityName", "%" + city + "%", "Like");
				}
			}).OrderBy(order => order.CityName);
		}

		/// <summary>
		/// 根据客研编码删除城市
		/// </summary>
		/// <param name="code"></param>
		public void DeleteCityByCode(string code)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				RD_InfoAdapter.Instance.DeleteAllInfo(code);
				scope.Complete();
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.ResearchData
{
	public class ListViewModel
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 城市名称
		/// </summary>
		public string CityName { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreateTime { get; set; }

		/// <summary>
		/// 是否启用
		/// </summary>
		public bool ValidStatus { get; set; }
	}

	public class SavePolicyViewModel
	{
		/// <summary>
		/// 客研数据
		/// </summary>
		public RD_Info RD_Info { get; set; }

		/// <summary>
		/// 政策
		/// </summary>
		public RD_Policy RD_Policy { get; set; }
	}

	public class SaveBenchmarkViewModel
	{
		/// <summary>
		/// 对标_企业销售统计
		/// </summary>
		public List<RD_Benchmark_EnterpriseSaleStatistics> RD_Benchmark_EnterpriseSaleStatistics { get; set; }

		/// <summary>
		/// 对标_项目销售统计
		/// </summary>
		public List<RD_Benchmark_ProjectSaleStatistics> RD_Benchmark_ProjectSaleStatistics { get; set; }
	}

	public class SaveNewHousingMarketViewModel
	{
		/// <summary>
		/// 客研数据_新房市场_月度统计
		/// </summary>
		public List<RD_NHM_MonthlyStatistics> RD_NHM_MonthlyStatistics { get; set; }

		/// <summary>
		/// 客研数据_新房市场_年度统计
		/// </summary>
		public List<RD_NHM_AnnualStatistics> RD_NHM_AnnualStatistics { get; set; }
	}

	public class RD_DataForAPP
	{
		public CityPolicyInfo CityPolicyInfo { get; set; }

		public List<RD_Economic> RD_Economic { get; set; }

		public List<RD_Population> RD_Population { get; set; }

		public List<RD_LandMarket> RD_LandMarket { get; set; }

		public NewHousingMarketInfo NewHousingMarketInfo { get; set; }

		public BenchmarkInfo BenchmarkInfo { get; set; }
	}


	public class CityPolicyInfo
	{
		public RD_Info RD_Info { get; set; }

		public RD_Policy RD_Policy { get; set; }
	}

	public class NewHousingMarketInfo
	{
		public List<RD_NHM_MonthlyStatistics> RD_NHM_MonthlyStatistics { get; set; }

		public List<RD_NHM_AnnualStatistics> RD_NHM_AnnualStatistics { get; set; }
	}

	public class BenchmarkInfo
	{
		public List<RD_Benchmark_EnterpriseSaleStatistics> RD_Benchmark_EnterpriseSaleStatistics { get; set; }

		public List<RD_Benchmark_ProjectSaleStatistics> RD_Benchmark_ProjectSaleStatistics { get; set; }
	}


	public class RD_Info
	{
		/// <summary>
		/// 主键编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 城市名称
		/// </summary>
		[Required]
		public string CityName { get; set; }
		/// <summary>
		/// 有效状态
		/// </summary>
		public bool ValidStatus { get; set; }
	}

	public class RD_Policy
	{
		/// <summary>
		/// 主键编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }
		/// <summary>
		/// 限购
		/// </summary>
		public string LimitPurchase { get; set; }
		/// <summary>
		/// 限贷
		/// </summary>
		public string LimitLoan { get; set; }
		/// <summary>
		/// 房贷利率
		/// </summary>
		public string MortgageRate { get; set; }
		/// <summary>
		/// 限售
		/// </summary>
		public string LimitSell { get; set; }
		/// <summary>
		/// 限商
		/// </summary>
		public string LimitCommercial { get; set; }
		/// <summary>
		/// 限价
		/// </summary>
		public string LimitPrice { get; set; }
		/// <summary>
		/// 限签
		/// </summary>
		public string LimitSign { get; set; }
		/// <summary>
		/// 摇号
		/// </summary>
		public string ShakingNumber { get; set; }
		/// <summary>
		/// 预售条件
		/// </summary>
		public string PreSaleConditions { get; set; }
		/// <summary>
		/// 限企业购房
		/// </summary>
		public string LimitCompanyPurchase { get; set; }
	}

	public class RD_Economic
	{
		/// <summary>
		/// 主键编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }
		/// <summary>
		/// 年份
		/// </summary>
		public int Annual { get; set; }
		/// <summary>
		/// GDP总量
		/// </summary>
		public decimal GDPTotal { get; set; }
		/// <summary>
		/// GDP增量
		/// </summary>
		public decimal GDPIncrement { get; set; }
		/// <summary>
		/// 财政收入
		/// </summary>
		public decimal GovernmentReceipts { get; set; }
		/// <summary>
		/// 第三产业占比
		/// </summary>
		public decimal ThirdIndustry { get; set; }
		/// <summary>
		/// 房投/固投比重
		/// </summary>
		public decimal InvestmentProportion { get; set; }
	}

	public class RD_NHM_MonthlyStatistics
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }
		/// <summary>
		/// 月度
		/// </summary>
		public string Monthly { get; set; }
		/// <summary>
		/// 市场成交面积
		/// </summary>
		public decimal MarketTransactionArea { get; set; }
		/// <summary>
		/// 新增供应面积
		/// </summary>
		public decimal NewSupplyArea { get; set; }
		/// <summary>
		/// 市场成交金额
		/// </summary>
		public decimal MarketTransactionAmount { get; set; }
		/// <summary>
		/// 市场成交均价
		/// </summary>
		public decimal APOMT { get; set; }
		/// <summary>
		/// 销供比
		/// </summary>
		public decimal MarketingRatio { get; set; }
		/// <summary>
		/// 市场存量
		/// </summary>
		public decimal SupplyStock { get; set; }
		/// <summary>
		/// 市场去化周期
		/// </summary>
		public decimal MarketCycle { get; set; }
	}

	public class RD_NHM_AnnualStatistics
	{
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }
		/// <summary>
		/// 年度
		/// </summary>
		public int Annual { get; set; }
		/// <summary>
		/// 市场成交面积
		/// </summary>
		public decimal MarketTransactionArea { get; set; }
		/// <summary>
		/// 新增供应面积
		/// </summary>
		public decimal NewSupplyArea { get; set; }
		/// <summary>
		/// 市场成交金额
		/// </summary>
		public decimal MarketTransactionAmount { get; set; }
		/// <summary>
		/// 市场成交均价
		/// </summary>
		public decimal APOMT { get; set; }
		/// <summary>
		/// 销供比
		/// </summary>
		public decimal MarketingRatio { get; set; }
	}

	public class RD_Benchmark_EnterpriseSaleStatistics
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }
		/// <summary>
		/// 企业
		/// </summary>
		public string Enterprise { get; set; }
		/// <summary>
		/// 销售额
		/// </summary>
		public decimal SalesVolume { get; set; }
		/// <summary>
		/// 市占率
		/// </summary>
		public decimal MarketShare { get; set; }

		/// <summary>
		/// 排序
		/// </summary>
		public int Sort { get; set; }

		/// <summary>
		/// 类型（1:今年；0：去年）
		/// </summary>
		public int Type { get; set; }
	}

	public class RD_Benchmark_ProjectSaleStatistics
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }
		/// <summary>
		/// 项目
		/// </summary>
		public string Project { get; set; }

		/// <summary>
		/// 排序
		/// </summary>
		public int Sort { get; set; }

		/// <summary>
		/// 销售额
		/// </summary>
		public decimal SalesVolume { get; set; }
		/// <summary>
		/// 类型（1:今年；0：去年）
		/// </summary>
		public int Type { get; set; }
	}

	public class RD_Population
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }

		/// <summary>
		/// 年份
		/// </summary>
		public int Annual { get; set; }

		/// <summary>
		/// 常住人口数
		/// </summary>
		public decimal PermanentPopulationTotal { get; set; }

		/// <summary>
		/// 户籍人口数
		/// </summary>
		public decimal PermanentResidentsTotal { get; set; }

		/// <summary>
		/// 常住人口增量
		/// </summary>
		public decimal PermanentPopulationIncrement { get; set; }

		/// <summary>
		/// 人口净流入量
		/// </summary>
		public decimal NetPopulationInflow { get; set; }

		/// <summary>
		/// 人均储蓄
		/// </summary>
		public decimal PerCapitaSavings { get; set; }

		/// <summary>
		/// 小学在校生人数
		/// </summary>
		public decimal PrimarySchoolEnrollment { get; set; }

		/// <summary>
		/// 大学在校生人数
		/// </summary>
		public decimal UniversitySchoolEnrollment { get; set; }

		/// <summary>
		/// 城镇化率
		/// </summary>
		public decimal UrbanizationRate { get; set; }
	}

	public class RD_LandMarket
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 客研数据编码
		/// </summary>
		public string ResearchData_Code { get; set; }

		/// <summary>
		/// 时间（年-月）
		/// </summary>
		public string Monthly { get; set; }

		/// <summary>
		/// 土地成交规模
		/// </summary>
		public decimal LandTransactionScale { get; set; }

		/// <summary>
		/// 楼板价
		/// </summary>
		public decimal FloorPrice { get; set; }

		/// <summary>
		/// 溢价率
		/// </summary>
		public decimal PremiumRate { get; set; }

		/// <summary>
		/// 土地出让金
		/// </summary>
		public decimal LandGrant { get; set; }

		/// <summary>
		/// 低价房价比
		/// </summary>
		public decimal LowPriceRatio { get; set; }
	}
}
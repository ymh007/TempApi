using System;
using System.Collections.Generic;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;

/// <summary>
/// 科研数据_新房市场_月度统计Model
/// </summary>
[ORTableMapping("office.RD_NHM_MonthlyStatistics")]
public class RD_NHM_MonthlyStatisticsModel : BaseModel
{
	/// <summary>
	/// 客研数据编码
	/// </summary>
	[ORFieldMapping("ResearchData_Code")]
	public string ResearchData_Code { get; set; }
	/// <summary>
	/// 月度
	/// </summary>
	[ORFieldMapping("Monthly")]
	public DateTime Monthly { get; set; }
	/// <summary>
	/// 市场成交面积
	/// </summary>
	[ORFieldMapping("MarketTransactionArea")]
	public decimal MarketTransactionArea { get; set; }
	/// <summary>
	/// 新增供应面积
	/// </summary>
	[ORFieldMapping("NewSupplyArea")]
	public decimal NewSupplyArea { get; set; }
	/// <summary>
	/// 市场成交金额
	/// </summary>
	[ORFieldMapping("MarketTransactionAmount")]
	public decimal MarketTransactionAmount { get; set; }
	/// <summary>
	/// 市场成交均价
	/// </summary>
	[ORFieldMapping("APOMT")]
	public decimal APOMT { get; set; }
	/// <summary>
	/// 销供比
	/// </summary>
	[ORFieldMapping("MarketingRatio")]
	public decimal MarketingRatio { get; set; }
	/// <summary>
	/// 市场存量
	/// </summary>
	[ORFieldMapping("SupplyStock")]
	public decimal SupplyStock { get; set; }
	/// <summary>
	/// 市场去化周期
	/// </summary>
	[ORFieldMapping("MarketCycle")]
	public decimal MarketCycle { get; set; }
}
/// <summary>
/// 科研数据_新房市场_月度统计Collection
/// </summary>
public class RD_NHM_MonthlyStatisticsCollection : EditableDataObjectCollectionBase<RD_NHM_MonthlyStatisticsModel>
{

}
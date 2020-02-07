using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;

/// <summary>
/// 科研数据_新房市场_年度统计Model
/// </summary>
[ORTableMapping("office.RD_NHM_AnnualStatistics")]
public class RD_NHM_AnnualStatisticsModel : BaseModel
{
   /// <summary>
   /// 客研数据编码
   /// </summary>
   [ORFieldMapping("ResearchData_Code")]
   public string ResearchData_Code{ get; set; }
   /// <summary>
   /// 年度
   /// </summary>
   [ORFieldMapping("Annual")]
   public int Annual{ get; set; }
   /// <summary>
   /// 市场成交面积
   /// </summary>
   [ORFieldMapping("MarketTransactionArea")]
   public decimal MarketTransactionArea{ get; set; }
   /// <summary>
   /// 新增供应面积
   /// </summary>
   [ORFieldMapping("NewSupplyArea")]
   public decimal NewSupplyArea{ get; set; }
   /// <summary>
   /// 市场成交金额
   /// </summary>
   [ORFieldMapping("MarketTransactionAmount")]
   public decimal MarketTransactionAmount{ get; set; }
   /// <summary>
   /// 市场成交均价
   /// </summary>
   [ORFieldMapping("APOMT")]
   public decimal APOMT { get; set; }
   /// <summary>
   /// 销供比
   /// </summary>
   [ORFieldMapping("MarketingRatio")]
   public decimal MarketingRatio{ get; set; }
}

/// <summary>
/// 科研数据_新房市场_月度统计
/// </summary>
public class RD_NHM_AnnualStatisticsCollection : EditableDataObjectCollectionBase<RD_NHM_AnnualStatisticsModel>
{

}
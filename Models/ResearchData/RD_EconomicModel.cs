using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;


/// <summary>
/// 客研数据_经济Model
/// </summary>
[ORTableMapping("office.RD_Economic")]
public class RD_EconomicModel : BaseModel
{
   /// <summary>
   /// 客研数据编码
   /// </summary>
   [ORFieldMapping("ResearchData_Code")]
   public string ResearchData_Code{ get; set; }
   /// <summary>
   /// 年份
   /// </summary>
   [ORFieldMapping("Annual")]
   public int Annual{ get; set; }
   /// <summary>
   /// GDP总量
   /// </summary>
   [ORFieldMapping("GDPTotal")]
   public decimal GDPTotal{ get; set; }
   /// <summary>
   /// GDP增量
   /// </summary>
   [ORFieldMapping("GDPIncrement")]
   public decimal GDPIncrement{ get; set; }
   /// <summary>
   /// 财政收入
   /// </summary>
   [ORFieldMapping("GovernmentReceipts")]
   public decimal GovernmentReceipts{ get; set; }
   /// <summary>
   /// 第三产业占比
   /// </summary>
   [ORFieldMapping("ThirdIndustry")]
   public decimal ThirdIndustry{ get; set; }
   /// <summary>
   /// 房投/固投比重
   /// </summary>
   [ORFieldMapping("InvestmentProportion")]
   public decimal InvestmentProportion{ get; set; }
}

/// <summary>
/// 客研数据_经济Collection
/// </summary>
public class RD_EconomicCollection : EditableDataObjectCollectionBase<RD_EconomicModel>
{

}
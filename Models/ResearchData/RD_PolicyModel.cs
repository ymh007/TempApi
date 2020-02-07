using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;

/// <summary>
/// 客研数据_政策Model
/// </summary>
[ORTableMapping("office.RD_Policy")]
public class RD_PolicyModel : BaseModel
{
   /// <summary>
   /// 客研数据编码
   /// </summary>
   [ORFieldMapping("ResearchData_Code")]
   public string ResearchData_Code{ get; set; }
   /// <summary>
   /// 限购
   /// </summary>
   [ORFieldMapping("LimitPurchase")]
   public string LimitPurchase{ get; set; }
   /// <summary>
   /// 限贷
   /// </summary>
   [ORFieldMapping("LimitLoan")]
   public string LimitLoan{ get; set; }
   /// <summary>
   /// 房贷利率
   /// </summary>
   [ORFieldMapping("MortgageRate")]
   public string MortgageRate{ get; set; }
   /// <summary>
   /// 限售
   /// </summary>
   [ORFieldMapping("LimitSell")]
   public string LimitSell{ get; set; }
   /// <summary>
   /// 限商
   /// </summary>
   [ORFieldMapping("LimitCommercial")]
   public string LimitCommercial{ get; set; }
   /// <summary>
   /// 限价
   /// </summary>
   [ORFieldMapping("LimitPrice")]
   public string LimitPrice{ get; set; }
   /// <summary>
   /// 限签
   /// </summary>
   [ORFieldMapping("LimitSign")]
   public string LimitSign{ get; set; }
   /// <summary>
   /// 摇号
   /// </summary>
   [ORFieldMapping("ShakingNumber")]
   public string ShakingNumber{ get; set; }
   /// <summary>
   /// 预售条件
   /// </summary>
   [ORFieldMapping("PreSaleConditions")]
   public string PreSaleConditions{ get; set; }
   /// <summary>
   /// 限企业购房
   /// </summary>
   [ORFieldMapping("LimitCompanyPurchase")]
   public string LimitCompanyPurchase{ get; set; }
}

/// <summary>
/// 客研数据_政策Collection
/// </summary>
public class RD_PolicyCollection : EditableDataObjectCollectionBase<RD_PolicyModel>
{

}
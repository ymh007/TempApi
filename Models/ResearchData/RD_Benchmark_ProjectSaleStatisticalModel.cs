using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Models;

/// <summary>
/// 客研数据_对标_项目销售统计Model
/// </summary>
[ORTableMapping("office.RD_Benchmark_ProjectSaleStatistics")]
public class RD_Benchmark_ProjectSaleStatisticsModel : BaseModel
{
   /// <summary>
   /// 客研数据编码
   /// </summary>
   [ORFieldMapping("ResearchData_Code")]
   public string ResearchData_Code{ get; set; }
   /// <summary>
   /// 项目
   /// </summary>
   [ORFieldMapping("Project")]
   public string Project{ get; set; }
   /// <summary>
   /// 销售额
   /// </summary>
   [ORFieldMapping("SalesVolume")]
   public decimal SalesVolume{ get; set; }

	/// <summary>
	/// 排序
	/// </summary>
	[ORFieldMapping("Sort")]
	public int Sort { get; set; }

	/// <summary>
	/// 类型（1:今年；0：去年）
	/// </summary>
	[ORFieldMapping("Type")]
   public int Type{ get; set; }
}
/// <summary>
/// 客研数据_对标_项目销售统计Collection
/// </summary>
public class RD_Benchmark_ProjectSaleStatisticsCollection : EditableDataObjectCollectionBase<RD_Benchmark_ProjectSaleStatisticsModel>
{

}
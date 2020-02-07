using System;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.ResearchData
{
	/// <summary>
	/// 客研数据_土地市场Model
	/// </summary>
	[ORTableMapping("office.RD_LandMarket")]
	public class RD_LandMarketModel : BaseModel
	{
		/// <summary>
		/// 客研数据编码
		/// </summary>
		[ORFieldMapping("ResearchData_Code")]
		public string ResearchData_Code { get; set; }

		/// <summary>
		/// 时间（年-月）
		/// </summary>
		[ORFieldMapping("Monthly")]
		public DateTime Monthly { get; set; }

		/// <summary>
		/// 土地成交规模
		/// </summary>
		[ORFieldMapping("LandTransactionScale")]
		public decimal LandTransactionScale { get; set; }

		/// <summary>
		/// 楼板价
		/// </summary>
		[ORFieldMapping("FloorPrice")]
		public decimal FloorPrice { get; set; }

		/// <summary>
		/// 溢价率
		/// </summary>
		[ORFieldMapping("PremiumRate")]
		public decimal PremiumRate { get; set; }

		/// <summary>
		/// 土地出让金
		/// </summary>
		[ORFieldMapping("LandGrant")]
		public decimal LandGrant { get; set; }

		/// <summary>
		/// 低价房价比
		/// </summary>
		[ORFieldMapping("LowPriceRatio")]
		public decimal LowPriceRatio { get; set; }
	}

	/// <summary>
	/// 科研数据_土地市场Collection
	/// </summary>
	public class RD_LandMarketCollection : EditableDataObjectCollectionBase<RD_LandMarketModel>
	{

	}
}
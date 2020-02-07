using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.ResearchData
{
	/// <summary>
	/// 客研数据_人口Model
	/// </summary>
	[ORTableMapping("office.RD_Population")]
	public class RD_PopulationModel : BaseModel
	{
		/// <summary>
		/// 客研数据编码
		/// </summary>
		[ORFieldMapping("ResearchData_Code")]
		public string ResearchData_Code { get; set; }

		/// <summary>
		/// 年份
		/// </summary>
		[ORFieldMapping("Annual")]
		public int Annual { get; set; }

		/// <summary>
		/// 常住人口数
		/// </summary>
		[ORFieldMapping("PermanentPopulationTotal")]
		public decimal PermanentPopulationTotal { get; set; }

		/// <summary>
		/// 户籍人口数
		/// </summary>
		[ORFieldMapping("PermanentResidentsTotal")]
		public decimal PermanentResidentsTotal { get; set; }

		/// <summary>
		/// 常住人口增量
		/// </summary>
		[ORFieldMapping("PermanentPopulationIncrement")]
		public decimal PermanentPopulationIncrement { get; set; }

		/// <summary>
		/// 人口净流入量
		/// </summary>
		[ORFieldMapping("NetPopulationInflow")]
		public decimal NetPopulationInflow { get; set; }

		/// <summary>
		/// 人均储蓄
		/// </summary>
		[ORFieldMapping("PerCapitaSavings")]
		public decimal PerCapitaSavings { get; set; }

		/// <summary>
		/// 小学在校生人数
		/// </summary>
		[ORFieldMapping("PrimarySchoolEnrollment")]
		public decimal PrimarySchoolEnrollment { get; set; }

		/// <summary>
		/// 大学在校生人数
		/// </summary>
		[ORFieldMapping("UniversitySchoolEnrollment")]
		public decimal UniversitySchoolEnrollment { get; set; }

		/// <summary>
		/// 城镇化率
		/// </summary>
		[ORFieldMapping("UrbanizationRate")]
		public decimal UrbanizationRate { get; set; }
	}

	/// <summary>
	/// 客研数据_人口Collection
	/// </summary>
	public class RD_PopulationCollection : EditableDataObjectCollectionBase<RD_PopulationModel>
	{

	}
}
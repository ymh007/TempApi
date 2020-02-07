using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.UrgeRecord;

namespace Seagull2.YuanXin.AppApi.Adapter.UrgeRecordAdapter
{
	/// <summary>
	/// 催办记录-Adapter
	/// </summary>
	public class UrgeRecordAdapter : UpdatableAndLoadableAdapterBase<UrgeRecordModel, UrgeRecordCollection>
	{
		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		protected override string GetConnectionName()
		{
			return ConnectionNameDefine.YuanXinBusiness;
		}

		/// <summary>
		/// 实例化
		/// </summary>
		public static readonly UrgeRecordAdapter Instance = new UrgeRecordAdapter();

	}
}
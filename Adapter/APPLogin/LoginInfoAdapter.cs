using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.APPLogin;

namespace Seagull2.YuanXin.AppApi.Adapter.APPLogin
{
	/// <summary>
	/// 登录信息-Adapter
	/// </summary>
	public class LoginInfoAdapter : UpdatableAndLoadableAdapterBase<LoginInfoModel, LoginInfoCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly LoginInfoAdapter Instance = new LoginInfoAdapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName()
		{
			return Models.ConnectionNameDefine.YuanXinBusiness;
		}
	}
}
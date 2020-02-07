using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.APPLogin;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter.APPLogin
{
	/// <summary>
	/// 连接Token-Adapter
	/// </summary>
	public class ConnectionTokenAdapter : UpdatableAndLoadableAdapterBase<ConnectionTokenModel, ConnectionTokenCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly ConnectionTokenAdapter Instance = new ConnectionTokenAdapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName()
		{
			return Models.ConnectionNameDefine.YuanXinBusiness;
		}

		public bool IsFirstLogin(string applicationID, string userCode)
		{
			var sql = @"select distinct A.userID from [OAuth].[LoginInfo] A
                        join [OAuth].[ConnectionToken] B
                        on A.ConnectionID = B.ConnectionID
                        where A.UserID = @UserID and A.ApplicationID = @ApplicationID";
			SqlParameter[] parameters = {
				new SqlParameter("@UserID",SqlDbType.NVarChar,36),
				new SqlParameter("@ApplicationID",SqlDbType.NVarChar,36),
			};
			parameters[0].Value = userCode;
			parameters[1].Value = applicationID;
			SqlDbHelper helper = new SqlDbHelper();
			var count = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
			if (count.Rows.Count > 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
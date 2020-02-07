using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.APPLogin
{
	/// <summary>
	/// 登录信息-Model
	/// </summary>
	[ORTableMapping("OAuth.LoginInfo")]
	public class LoginInfoModel
	{
		/// <summary>
		/// 连接ID
		/// </summary>
		[ORFieldMapping("ConnectionID")]
		public string ConnectionID { get; set; }

		/// <summary>
		/// 用户ID
		/// </summary>
		[ORFieldMapping("UserID")]
		public string UserID { get; set; }

		/// <summary>
		/// AppID
		/// </summary>
		[ORFieldMapping("ApplicationID")]
		public string ApplicationID { get; set; }

		/// <summary>
		/// 设备ID
		/// </summary>
		[ORFieldMapping("ClientID")]
		public string ClientID { get; set; }

		/// <summary>
		/// 设备类型
		/// </summary>
		[ORFieldMapping("ClientType")]
		public int ClientType { get; set; }

		/// <summary>
		/// 登录时间
		/// </summary>
		[ORFieldMapping("LoginTime")]
		public DateTime LoginTime { get; set; }

		/// <summary>
		/// 登出时间
		/// </summary>
		[ORFieldMapping("LogoutTime")]
		public DateTime LogoutTime { get; set; }
	}

	/// <summary>
	/// 登录信息-Collection
	/// </summary>
	public class LoginInfoCollection : EditableDataObjectCollectionBase<LoginInfoModel> { }
}
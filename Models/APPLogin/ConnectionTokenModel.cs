using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;

namespace Seagull2.YuanXin.AppApi.Models.APPLogin
{
	/// <summary>
	/// 连接Token-Model
	/// </summary>
	[ORTableMapping("OAuth.ConnectionToken")]
	public class ConnectionTokenModel
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
		/// 票据
		/// </summary>
		[ORFieldMapping("Ticket")]
		public string Ticket { get; set; }

		/// <summary>
		/// 有效状态
		/// </summary>
		[ORFieldMapping("VaildStatus")]
		public string VaildStatus { get; set; }
	}

	/// <summary>
	/// 连接Token-Collection
	/// </summary>
	public class ConnectionTokenCollection : EditableDataObjectCollectionBase<ConnectionTokenModel> { }
}
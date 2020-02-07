using MCS.Library.OGUPermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Common
{
	/// <summary>
	/// 数据过滤公共类
	/// </summary>
	public class DataFilter
	{
		/// <summary>
		/// 过滤有效的人员
		/// </summary>
		/// <returns></returns>
		public List<IUser> FilterValidUsers(List<string> ids)
		{
			ids = ids.Distinct().ToList();//去重
			if (ids.Count < 1 || ids.FirstOrDefault() == null)
			{
				return new List<IUser>();
			}
			var users = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, ids.ToArray());
			return users.ToList().FindAll(m => m.IsSideline == false);//取出用户主职信息
		}
	}
}
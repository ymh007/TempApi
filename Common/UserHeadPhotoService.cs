using MCS.Library.Data.Builder;
using Seagull2.YuanXin.AppApi.Adapter.UserHeadPhoto;
using Seagull2.YuanXin.AppApi.ViewsModel.UserHeadPhoto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
	/// <summary>
	/// 用户头像公共类
	/// </summary>
	public class UserHeadPhotoService
	{
		#region 获取用户头像
		/// <summary>
		/// 获取用户头像
		/// </summary>
		public static string GetUserHeadPhoto(string userCode)
		{
			try
			{
				var list = UserHeadPhotoAdapter.Instance.Load(m => m.AppendItem("UserCode", userCode).AppendItem("IsAudit", true)).OrderByDescending(o => o.CreateTime).ToList();
				if (list.Count() > 0)
				{
					return FileService.DownloadFile(list[0].Url);
				}
				else
				{
					return string.Format(ConfigAppSetting.SignImgPath, userCode);
				}
			}
			catch
			{
				return string.Format(ConfigAppSetting.SignImgPath, "abc");
			}
		}
        #endregion

        #region 批量获取用户头像
        /// <summary>
        /// 批量获取用户头像
        /// </summary>
        public static List<UserHeadPhotoList> GetUserHeadPhoto(string[] ids)
		{
			try
			{
				List<UserHeadPhotoList> result = new List<UserHeadPhotoList>();
				{
					InSqlClauseBuilder inSql = new InSqlClauseBuilder();
					inSql.AppendItem(ids);
					var where = new WhereSqlClauseBuilder();
					where.AppendItem("UserCode", inSql.ToSqlStringWithInOperator(TSqlBuilder.Instance), "", true);
					var whereSql = where.ToSqlString(TSqlBuilder.Instance);
					var list = UserHeadPhotoAdapter.Instance.GetList(whereSql);
					ids.ToList().ForEach(m =>
					{
						var childList = list.FindAll(o => o.UserCode == m);
						var item = new UserHeadPhotoList
						{
							UserCode = m,
							UserHeadPhoto = childList.Count > 0 ? FileService.DownloadFile(childList.FirstOrDefault().Url) : string.Format(ConfigAppSetting.SignImgPath, m)
						};
						result.Add(item);
					});
					return result;
				}
			}
			catch
			{
				return null;
			}
		}
		#endregion
	}
}
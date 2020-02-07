using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.Core.Models;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.UserHeadPhoto;
using Seagull2.YuanXin.AppApi.Services.Meeting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Seagull2.YuanXin.AppApi.Adapter.UserHeadPhoto
{
	/// <summary>
	/// 用户头像适配器
	/// </summary>
	public class UserHeadPhotoAdapter : UpdatableAndLoadableAdapterBase<UserHeadPhotoModel, UserHeadPhotoCollection>
	{
		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		protected override string GetConnectionName()
		{
			return ConnectionNameDefine.YuanXinBusiness;
		}
		/// <summary>
		/// 日志实例化
		/// </summary>
		ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// 实例化
		/// </summary>
		public static readonly UserHeadPhotoAdapter Instance = new UserHeadPhotoAdapter();

		#region 获取DataCount
		/// <summary>
		/// 获取DataCount
		/// </summary>
		/// <returns></returns>
		public int GetListForPC(int state, string userName)
		{
			var sql = new StringBuilder();
			var count = 0;
			switch (state)
			{
				case -1: //查询全部
					sql.Append(@"SELECT COUNT(0) FROM [office].[UserHeadPhoto] ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"Where UserName like '%{0}%'", userName);
					}
					count = Convert.ToInt32(DbHelper.RunSqlReturnScalar(sql.ToString(), GetConnectionName()));
					break;
				case 0: //查询待处理
					sql.Append(@"SELECT COUNT(0) FROM [office].[UserHeadPhoto] a Where IsOperate = 'false' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%'", userName);
					}
					count = Convert.ToInt32(DbHelper.RunSqlReturnScalar(sql.ToString(), GetConnectionName()));
					break;
				case 1: //查询已经处理
					sql.Append(@"SELECT COUNT(0) FROM [office].[UserHeadPhoto] a Where  IsOperate = 'true' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%'", userName);
					}
					count = Convert.ToInt32(DbHelper.RunSqlReturnScalar(sql.ToString(), GetConnectionName()));
					break;
				case 2: //查询已经审核
					sql.Append(@"SELECT COUNT(0) FROM [office].[UserHeadPhoto] a Where IsOperate = 'true' And IsAudit = 'true' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%' ", userName);
					}
					count = Convert.ToInt32(DbHelper.RunSqlReturnScalar(sql.ToString(), GetConnectionName()));
					break;
				case 3: //查询审核未通过
					sql.Append(@"SELECT COUNT(0) FROM [office].[UserHeadPhoto] a Where IsOperate = 'true' And IsAudit = 'false' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%'", userName);
					}
					count = Convert.ToInt32(DbHelper.RunSqlReturnScalar(sql.ToString(), GetConnectionName()));
					break;
			}
			return count;
		}
		#endregion

		#region 分页查询用户头像数据
		/// <summary>
		/// 分页查询用户头像数据
		/// </summary>
		/// <param name="state"></param>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <param name="userName"></param>
		/// <returns></returns>
		public List<UserHeadPhotoModel> GetList(int state, int pageIndex, int pageSize, string userName)
		{
			pageIndex--;
			var sql = new StringBuilder();
			DataTable table = new DataTable();
			switch (state)
			{
				case -1: //查询全部
					sql.Append(@"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[UserHeadPhoto] ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@" WHERE UserName like '%{0}%'", userName);
					}
					sql.AppendFormat(@" )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}", pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);
					table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
					break;
				case 0: //查询待处理
					sql.Append(@"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[UserHeadPhoto] 
                            WHERE IsOperate = 'false'  ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@" And UserName like '%{0}%'", userName);
					}
					sql.AppendFormat(@" )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}", pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);
					table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
					break;
				case 1: //查询已经处理
					sql.Append(@"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[UserHeadPhoto] 
                            WHERE IsOperate = 'true' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%'", userName);
					}
					sql.AppendFormat(@" )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}", pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);
					table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
					break;
				case 2: //查询已经审核
					sql.Append(@"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[UserHeadPhoto] 
                            Where IsOperate = 'true' And IsAudit = 'true' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%'", userName);
					}
					sql.AppendFormat(@" )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}", pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);
					table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
					break;
				case 3: //查询审核未通过
					sql.Append(@"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[UserHeadPhoto] 
                            Where IsOperate = 'true' And IsAudit = 'false' ");
					if (!string.Format(userName).IsEmptyOrNull())
					{
						sql.AppendFormat(@"And UserName like '%{0}%'", userName);
					}
					sql.AppendFormat(@" )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {0} AND {1}", pageSize * pageIndex + 1, pageSize * pageIndex + pageSize);
					table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
					break;
			}
			return DataConvertHelper<UserHeadPhotoModel>.ConvertToList(table);
		}
		#endregion

		#region 查询审核通过的用户头像数据
		/// <summary>
		/// 查询审核通过的用户头像数据
		/// </summary>
		/// <returns></returns>
		public List<UserHeadPhotoModel> GetList(string whereSql)
		{
			var sql = new StringBuilder();
			sql.Append(string.Format(@"SELECT * FROM [office].[UserHeadPhoto] 
                         Where IsOperate = 'true' And IsAudit = 'true' And {0}", whereSql));
			var table = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0];
			return DataConvertHelper<UserHeadPhotoModel>.ConvertToList(table);
		}
		#endregion
	}
}
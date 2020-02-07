using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.MessagePush;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Seagull2.YuanXin.AppApi.Adapter.MessagePush
{
	/// <summary>
	/// 消息推送群组 Adapter
	/// </summary>
	public class MessagePushGroupAdapter : UpdatableAndLoadableAdapterBase<MessagePushGroupModel, MessagePushGroupCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly MessagePushGroupAdapter Instance = new MessagePushGroupAdapter();

		/// <summary>
		/// 数据库连接名称
		/// </summary>
		protected override string GetConnectionName()
		{
			return Models.ConnectionNameDefine.YuanXinBusiness;
		}

		public DataTable GetList(string name,int pageIndex,int pageSize,string sourceType="1")
		{
			pageIndex--;
			var sql = @"WITH [Temp] AS
                        (
                          SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row],*
						
                          FROM  [office].[MessagePushGroup]

						  WHERE SourceType=@SourceType and   Name like '%' + @Name + '%'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize";
			SqlParameter[] parameters = {
				new SqlParameter("@Name",SqlDbType.NVarChar,36),
				new SqlParameter("@PageIndex",SqlDbType.NVarChar,36),
				new SqlParameter("@PageSize",SqlDbType.NVarChar,36),
                new SqlParameter("@SourceType",SqlDbType.NVarChar,36),
            };
			parameters[0].Value = name;
			parameters[1].Value = pageSize * pageIndex + 1;
			parameters[2].Value = pageSize * pageIndex + pageSize;
            parameters[3].Value = sourceType;
            SqlDbHelper helper = new SqlDbHelper();
			return helper.ExecuteDataTable(sql, CommandType.Text, parameters);
		}

		public int GetCount(string name, string sourceType = "1")
		{
			var sql = @"  SELECT COUNT(*)
						
                          FROM  [office].[MessagePushGroup]

						  WHERE SourceType=@SourceType and    Name like '%' + @Name + '%'";
			SqlParameter[] parameters = {
				new SqlParameter("@Name",SqlDbType.NVarChar,36),
                new SqlParameter("@SourceType",SqlDbType.NVarChar,36),
            };
			parameters[0].Value = name;
            parameters[1].Value = sourceType;
            SqlDbHelper helper = new SqlDbHelper();
			return Convert.ToInt32(helper.ExecuteScalar(sql, CommandType.Text, parameters));
		}
	}
}
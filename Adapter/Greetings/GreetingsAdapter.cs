using MCS.Library.Data;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Greetings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Greetings
{
    /// <summary>
    /// 
    /// </summary>
    public class GreetingsAdapter : UpdatableAndLoadableAdapterBase<GreetingsModel, GreetingsCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly GreetingsAdapter Instance = new GreetingsAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string ConnectionString = DbConnectionManager.GetConnectionString(ConnectionNameDefine.YuanXinBusiness);

        /// <summary>
        /// 获取单条问候语
        /// </summary>
        public DataRow GetSignle()
        {
            var sql = @"
                IF EXISTS(SELECT * FROM [office].[Greetings] WHERE DATEDIFF(DAY, GETDATE(), [Time]) = 0)
	                SELECT TOP 1 [A].[Title], [A].[TitleType], [A].[Time], [B].[Content] FROM [office].[Greetings] A INNER JOIN [office].[GreetingsContent] B ON [A].[Code] = [B].[GreetingsCode] WHERE DATEDIFF(DAY, GETDATE(), [A].[Time]) = 0 ORDER BY NEWID();
                ELSE
	                SELECT TOP 1 [A].[Title], [A].[TitleType], [A].[Time], [B].[Content] FROM [office].[Greetings] A INNER JOIN [office].[GreetingsContent] B ON [A].[Code] = [B].[GreetingsCode] WHERE [A].[Time] = '' ORDER BY NEWID();";

            var helper = new SqlDbHelper(ConnectionString);
            var dt = helper.ExecuteDataTable(sql, CommandType.Text);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
        }
    }
}
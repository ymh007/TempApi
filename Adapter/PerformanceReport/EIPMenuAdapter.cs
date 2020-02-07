using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;

namespace Seagull2.YuanXin.AppApi.Adapter
{
    /// <summary>
    /// 海鸥Ⅱ菜单适配器
    /// </summary>
    public class EIPMenuAdapter : UpdatableAndLoadableAdapterBase<EIPMenuModel, EIPMenuCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly EIPMenuAdapter Instance = new EIPMenuAdapter();

        /// <summary>
        /// 根据ID获取菜单（ID,Name,ParentID）
        /// </summary>
        public EIPMenuCollection GetListById(string id)
        {
            return this.QueryData("SELECT [ID],[Name],[ParentID] FROM [EIP_ND_MySeagull2Menu] WHERE [ID]='" + id + "'");
        }

        /// <summary>
        /// 获取所有菜单（ID,Name,ParentID）
        /// </summary>
        public EIPMenuCollection GetList()
        {
            return this.QueryData("SELECT [ID],[Name],[ParentID] FROM [EIP_ND_MySeagull2Menu] ORDER BY [Sort] ASC");
        }

        /// <summary>
        /// 获取用户菜单（ID,Name,ParentID）
        /// </summary>
        public EIPMenuCollection GetList(string userCode)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("IF EXISTS(SELECT * FROM [dbo].[UserProfile] WHERE [UserId]='" + userCode + "' AND [UsingPanel]='TRUE')");
            strSql.Append(" BEGIN");
            strSql.Append("  SELECT DISTINCT A.[ID], A.[Name], A.[ParentID]");
            strSql.Append("  FROM [dbo].[EIP_ND_MySeagull2Menu] A, [dbo].[Panel_EIP_ND_MySeagull2Menu] B, [dbo].[Panel] C");
            strSql.Append("   INNER JOIN [dbo].[UserProfile] D ON C.[Id]=D.[PanelId]");
            strSql.Append("  WHERE");
            strSql.Append("   A.[ID]=B.[MenuId] AND B.[PanelId]=C.[Id] AND D.[UserId]='" + userCode + "' AND D.[UsingPanel]='TRUE'");
            strSql.Append(" END ");
            strSql.Append("ELSE");
            strSql.Append(" BEGIN");
            strSql.Append("  SELECT DISTINCT A.[ID], A.[Name], A.[ParentID]");
            strSql.Append("  FROM [dbo].[EIP_ND_MySeagull2Menu] A, [dbo].[UserProfile_EIP_ND_MySeagull2Menu] B");
            strSql.Append("  WHERE");
            strSql.Append("   A.[ID]=B.[MenuId] AND B.[UserId]='" + userCode + "' ");
            strSql.Append("END");

            var list = this.QueryData(strSql.ToString());
            if (list.Count > 0)
            {
                return list;
            }

            //如果没有菜单，则取默认面板的菜单
            strSql.Clear();
            strSql.Append("SELECT DISTINCT A.[ID], A.[Name], A.[ParentID] ");
            strSql.Append("FROM [dbo].[EIP_ND_MySeagull2Menu] A, [dbo].[Panel_EIP_ND_MySeagull2Menu] B ");
            strSql.Append("WHERE");
            strSql.Append(" A.[ID]=B.[MenuId] AND B.[PanelId] IN");
            strSql.Append(" (");
            strSql.Append("  SELECT TOP 1 [Id] FROM [dbo].[Panel] WHERE [IsDefault]='TRUE'");
            strSql.Append(" )");

            return this.QueryData(strSql.ToString());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.WorkRegionMenu;

namespace Seagull2.YuanXin.AppApi.Adapter.WorkRegionMenu
{
    /// <summary>
    /// 工作通菜单 Adapter
    /// </summary>
    public class WorkRegionMenuAdapter : UpdatableAndLoadableAdapterBase<WorkRegionMenuModel, WorkRegionMenuCollection>
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly WorkRegionMenuAdapter Instance = new WorkRegionMenuAdapter();

        /// <summary>
        /// 获取所有菜单列表
        /// </summary>
        public WorkRegionMenuCollection GetList(string versionType)
        {
            var sql = @"SELECT * FROM [office].[WorkRegionMenu] WHERE [VersionType] = '" + versionType + "' ORDER BY [IsTop] DESC, [Sort] ASC";
            return QueryData(sql);
        }
        /// <summary>
        /// 搜索菜单列表
        /// </summary>
        public WorkRegionMenuCollection GetListBySearch(string key)
        {
            var sql = @"SELECT * FROM [office].[WorkRegionMenu] WHERE [VersionType] ='newVersion' and  [NAME] LIKE '%" + key + "%' ";
            return QueryData(sql);
        }

        #region  菜单对应的推荐 新


        /// <summary>
        /// 获取推荐新 标识
        /// </summary>
        public DataTable QueryRecommend()
        {
            List<object> result = new List<object>();
            var sql = @"SELECT * FROM [office].[AppRecommends]";
            SqlDbHelper sqlDbHelper = new SqlDbHelper();
            return sqlDbHelper.ExecuteDataTable(sql);
        }


        /// <summary>
        /// 添加推荐新 标识
        /// </summary>
        public int AddRecommend(string code,string url,int recommendType, string creator)
        {
            var sql = @"INSERT INTO [office].[AppRecommends] VALUES('"+ code + "','"+ url + "',"+ recommendType + ",'"+ creator + "')";
            SqlDbHelper sqlDbHelper = new SqlDbHelper();
            return sqlDbHelper.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 删除 推荐新 标识
        /// </summary>
        public int DeleteRecommend(string code)
        {
            SqlDbHelper sqlDbHelper = new SqlDbHelper();
            var sql = @" Delete [office].[AppRecommends] WHERE Code ='"+ code+"'";
            return sqlDbHelper.ExecuteNonQuery(sql);
        }
        #endregion
    }
}
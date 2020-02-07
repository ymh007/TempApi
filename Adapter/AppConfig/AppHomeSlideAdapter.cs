using System;
using MCS.Library.SOA.DataObjects;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AppConfig;

namespace Seagull2.YuanXin.AppApi.Adapter.AppConfig
{
    /// <summary>
    /// 首页轮播图 Adapter
    /// </summary>
    public class AppHomeSlideAdapter : UpdatableAndLoadableAdapterBase<AppHomeSlideModel, AppHomeSlideCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static readonly AppHomeSlideAdapter Instance = new AppHomeSlideAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取所有轮播图列表
        /// </summary>
        public AppHomeSlideCollection GetList()
        {
            var sql = @"SELECT * FROM [office].[AppHomeSlide] ORDER BY [Sort] ASC";
            return QueryData(sql);
        }
    }
}
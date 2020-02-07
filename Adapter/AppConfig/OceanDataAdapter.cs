using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.AppConfig;

namespace Seagull2.YuanXin.AppApi.Adapter.AppConfig
{
    /// <summary>
    /// 数字远洋Adapter
    /// </summary>
    public class OceanDataAdapter : UpdatableAndLoadableAdapterBase<OceanDataModel, OceanDataCollection>
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
        public static readonly OceanDataAdapter Instance = new OceanDataAdapter();

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public OceanDataCollection GetList()
        {
            var sql = @"WITH Temp AS
                        (
	                        SELECT ROW_NUMBER() OVER (PARTITION BY [id] ORDER BY [Version] DESC) AS [Row], * 
	                        FROM [dbo].[OceanData] 
	                        WHERE [id] IN (SELECT [OceanDataId] FROM [dbo].[DigitalPanel_OceanData] WHERE [DigitalPanelId] = '{0}')
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] = 1 ORDER BY [Temp].[SortId] ASC;";
            sql = string.Format(sql, ConfigAppSetting.DigitalPanelId);
            return QueryData(sql);
        }
    }
}
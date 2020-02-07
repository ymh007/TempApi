using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ManagementReport;

namespace Seagull2.YuanXin.AppApi.Adapter.ManagementReport
{
    /// <summary>
    /// 访问记录Adapter
    /// </summary>
    public class SiteVisitLogAdapter : UpdatableAndLoadableAdapterBase<SiteVisitLogModel, SiteVisitLogCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static SiteVisitLogAdapter Instance = new SiteVisitLogAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }

        /// <summary>
        /// 添加浏览记录
        /// </summary>
        public int Add(SiteVisitLogModel model)
        {
            var sql = @"INSERT INTO [dbo].[SITE_VISIT_LOG]
                                (
                                 [URL],
                                 [USER_NAME],
                                 [DROP_TIME],
                                 [TITLE],
                                 [SOURCE]
                                )
                        VALUES  (
                                 N'{0}', -- URL - nvarchar(250)
                                 N'{1}', -- USER_NAME - nvarchar(100)
                                 '{2}', -- DROP_TIME - datetime
                                 N'{3}', -- TITLE - nvarchar(200)
                                 N'{4}'  -- SOURCE - nvarchar(200)
                                )";
            sql = string.Format(sql, model.Url, model.UserName, model.DropTime, model.Title, model.Source);
            var result = DbHelper.RunSql(sql, GetConnectionName());
            return result;
        }
    }
}
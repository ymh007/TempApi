using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System.Reflection;
using System.Collections.Generic;
using Seagull2.YuanXin.AppApi.Models.BusinessCard;

namespace Seagull2.YuanXin.AppApi.Adapter.BusinessCard
{
    /// <summary>
    /// 个人名片
    /// </summary>
    public class BusinessCardAdapter : UpdatableAndLoadableAdapterBase<BusinessCardModel, BusinessCardCollection>
    {
        /// <summary>
        /// 适配器实例化
        /// </summary>
        public static readonly BusinessCardAdapter Instance = new BusinessCardAdapter();

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
        /// 获取名片数量
        /// </summary>
        public int GetCount(string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[BusinessCard] WHERE [Creator] = '{0}'";
            sql = string.Format(sql, userCode);
            var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            int count;
            if (int.TryParse(result.ToString(), out count))
            {
                return count;
            }
            return 0;
        }
    }
}
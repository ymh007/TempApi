using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Invoice;

namespace Seagull2.YuanXin.AppApi.Adapter.Invoice
{
    /// <summary>
    /// 发票 Adapter
    /// </summary>
    public class InvoiceAdapter : UpdatableAndLoadableAdapterBase<InvoiceModel, InvoiceCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly InvoiceAdapter Instance = new InvoiceAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }

        #region 获取用户发票列表

        /// <summary>
        /// 获取用户发票列表 - 总记录数
        /// </summary>
        public int GetList(string userCode)
        {
            var sql = @"SELECT COUNT(0) FROM [office].[Invoice] WHERE [Creator] = '{0}'";
            sql = string.Format(sql, userCode);
            var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            int count;
            if (int.TryParse(result.ToString(), out count))
            {
                return count;
            }
            return 0;
        }

        /// <summary>
        /// 获取用户发票列表 - 当前页数据
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        public InvoiceCollection GetList(string userCode, int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row], * FROM [office].[Invoice] WHERE [Creator] = '{0}'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {1} AND {2}";
            sql = string.Format(sql, userCode, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);
            return QueryData(sql);
        }

        #endregion
    }
}
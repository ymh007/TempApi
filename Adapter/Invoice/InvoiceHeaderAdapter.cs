using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Invoice;

namespace Seagull2.YuanXin.AppApi.Adapter.Invoice
{
    /// <summary>
    /// 海鸥二发票 Adapter
    /// </summary>
    public class InvoiceHeaderAdapter : UpdatableAndLoadableAdapterBase<InvoiceHeaderModel, InvoiceHeaderCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly InvoiceHeaderAdapter Instance = new InvoiceHeaderAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.SubjectDB_InvoiceHeader;
        }

        #region 查询发票列表

        /// <summary>
        /// 查询发票列表 - 总记录数
        /// </summary>
        public int GetList(string companyName)
        {
            var sql = @"SELECT COUNT(0) FROM [Finance].[InvoiceHeader] WHERE [CompanyName] LIKE '%{0}%'";
            sql = string.Format(sql, companyName);
            var result = DbHelper.RunSqlReturnScalar(sql, GetConnectionName());
            int count;
            if (int.TryParse(result.ToString(), out count))
            {
                return count;
            }
            return 0;
        }

        /// <summary>
        /// 查询发票列表 - 当前页数据
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">页索引，从1开始</param>
        public InvoiceHeaderCollection GetList(string companyName, int pageSize, int pageIndex)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
	                        SELECT ROW_NUMBER() OVER (ORDER BY [CompanyName] ASC) AS [Row], * FROM [Finance].[InvoiceHeader] WHERE [CompanyName] LIKE '%{0}%'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN {1} AND {2}";
            sql = string.Format(sql, companyName, pageIndex * pageSize + 1, pageIndex * pageSize + pageSize);
            return QueryData(sql);
        }

        #endregion
    }
}
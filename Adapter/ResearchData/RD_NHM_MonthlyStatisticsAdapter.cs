using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Adapter.ResearchData
{
    /// <summary>
    /// 科研数据_新房市场_月度统计Adapter
    /// </summary>
    public class RD_NHM_MonthlyStatisticsAdapter : UpdatableAndLoadableAdapterBase<RD_NHM_MonthlyStatisticsModel, RD_NHM_MonthlyStatisticsCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly RD_NHM_MonthlyStatisticsAdapter Instance = new RD_NHM_MonthlyStatisticsAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        public void BatchInsert(List<RD_NHM_MonthlyStatisticsModel> list)
        {
			if (list.Count < 1) { throw new Exception("至少保存一条数据"); }
			//构造DataTable
			var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("ResearchData_Code", System.Type.GetType("System.String"));
            dt.Columns.Add("Monthly", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("MarketTransactionArea", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("NewSupplyArea", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("MarketTransactionAmount", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("APOMT", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("MarketingRatio", System.Type.GetType("System.String"));
            dt.Columns.Add("SupplyStock", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("MarketCycle", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            list.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["ResearchData_Code"] = m.ResearchData_Code;
                dr["Monthly"] = m.Monthly;
                dr["MarketTransactionArea"] = m.MarketTransactionArea;
                dr["NewSupplyArea"] = m.NewSupplyArea;
                dr["MarketTransactionAmount"] = m.MarketTransactionAmount;
                dr["APOMT"] = m.APOMT;
                dr["MarketingRatio"] = m.MarketingRatio;
                dr["SupplyStock"] = m.SupplyStock;
                dr["MarketCycle"] = m.MarketCycle;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "office.RD_NHM_MonthlyStatistics", GetConnectionName());
        }

        /// <summary>
        /// 获取新房市场信息
        /// </summary>
        /// <param name="code">客研编码</param>
        /// <returns></returns>
        public List<RD_NHM_MonthlyStatisticsModel> GetNHM_MonthlyStatisticsByRD_Code(string code)
        {
            return Instance.Load(m => m.AppendItem("ResearchData_Code", code)).OrderByDescending(o => o.Monthly).Take(12).ToList();
        }

        /// <summary>
        /// 根据RD_Code删除数据
        /// </summary>
        /// <param name="code"></param>
        public void DeleteByRD_Code(string code)
        {
            Instance.Delete(m => m.AppendItem("ResearchData_Code", code));
        }
    }
}
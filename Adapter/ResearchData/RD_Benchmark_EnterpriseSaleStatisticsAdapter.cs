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
    /// 客研数据_对标_企业销售统计Adapter
    /// </summary>
    public class RD_Benchmark_EnterpriseSaleStatisticsAdapter : UpdatableAndLoadableAdapterBase<RD_Benchmark_EnterpriseSaleStatisticsModel, RD_Benchmark_EnterpriseSaleStatisticsCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly RD_Benchmark_EnterpriseSaleStatisticsAdapter Instance = new RD_Benchmark_EnterpriseSaleStatisticsAdapter();

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
        public void BatchInsert(List<RD_Benchmark_EnterpriseSaleStatisticsModel> list)
        {
			if (list.Count < 1) { throw new Exception("至少保存一条数据"); }
			//构造DataTable
			var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("ResearchData_Code", System.Type.GetType("System.String"));
            dt.Columns.Add("Enterprise", System.Type.GetType("System.String"));
            dt.Columns.Add("SalesVolume", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("MarketShare", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("Type", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Sort", System.Type.GetType("System.Int32"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            list.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["ResearchData_Code"] = m.ResearchData_Code;
                dr["Enterprise"] = m.Enterprise;
                dr["SalesVolume"] = m.SalesVolume;
                dr["MarketShare"] = m.MarketShare;
                dr["Type"] = m.Type;
                dr["Sort"] = m.Sort;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "office.RD_Benchmark_EnterpriseSaleStatistics", GetConnectionName());
        }

        /// <summary>
        /// 获取对标信息
        /// </summary>
        /// <param name="code">客研编码</param>
        /// <returns></returns>
        public List<RD_Benchmark_EnterpriseSaleStatisticsModel> GetNHM_AnnualStatisticsByRD_Code(string code)
        {
            var list = new List<RD_Benchmark_EnterpriseSaleStatisticsModel>();
            var list1 = Instance.Load(m => m.AppendItem("ResearchData_Code", code).AppendItem("Type", 0)).OrderBy(o => o.Sort).Take(10).ToList();
            var list2 = Instance.Load(m => m.AppendItem("ResearchData_Code", code).AppendItem("Type", 1)).OrderBy(o => o.Sort).Take(10).ToList();
            list.AddRange(list1);
            list.AddRange(list2);
            return list;
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
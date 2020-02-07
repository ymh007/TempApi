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
    /// 客研数据_经济
    /// </summary>
    public class RD_EconomicAdapter : UpdatableAndLoadableAdapterBase<RD_EconomicModel, RD_EconomicCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly RD_EconomicAdapter Instance = new RD_EconomicAdapter();

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
        /// <param name="model"></param>
        /// <param name="userCode"></param>
        public void BatchInsert(List<RD_EconomicModel> list)
        {
            if (list.Count < 1) { throw new Exception("至少保存一条数据"); }
            //构造DataTable
            var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("ResearchData_Code", System.Type.GetType("System.String"));
            dt.Columns.Add("Annual", System.Type.GetType("System.Int32"));
            dt.Columns.Add("GDPTotal", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("GDPIncrement", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("GovernmentReceipts", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("ThirdIndustry", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("InvestmentProportion", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            list.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["ResearchData_Code"] = m.ResearchData_Code;
                dr["Annual"] = m.Annual;
                dr["GDPTotal"] = m.GDPTotal;
                dr["GDPIncrement"] = m.GDPIncrement;
                dr["GovernmentReceipts"] = m.GovernmentReceipts;
                dr["ThirdIndustry"] = m.ThirdIndustry;
                dr["InvestmentProportion"] = m.InvestmentProportion;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "office.RD_Economic", GetConnectionName());
        }

        /// <summary>
        /// 根据客研数据Code获取经济数据
        /// </summary>
        /// <param name="code">客研数据</param>
        /// <returns></returns>
        public List<RD_EconomicModel> GetEconomicByRD_Code(string code)
        {
            return Instance.Load(m => m.AppendItem("ResearchData_Code", code)).OrderByDescending(o => o.Annual).Take(5).ToList();
        }

        public void DeleteByRd_Code(string code)
        {
            Instance.Delete(m => m.AppendItem("ResearchData_Code", code));
        }
    }
}
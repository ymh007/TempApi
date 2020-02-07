using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ResearchData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.ResearchData
{
    /// <summary>
    /// 客研数据_人口Adapter
    /// </summary>
    public class RD_PopulationAdapter : UpdatableAndLoadableAdapterBase<RD_PopulationModel, RD_PopulationCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly RD_PopulationAdapter Instance = new RD_PopulationAdapter();

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
        public void BatchInsert(List<RD_PopulationModel> list)
        {
			if (list.Count < 1) { throw new Exception("至少保存一条数据"); }
			//构造DataTable
			var dt = new DataTable();
            dt.Columns.Add("Code", System.Type.GetType("System.String"));
            dt.Columns.Add("ResearchData_Code", System.Type.GetType("System.String"));
            dt.Columns.Add("Annual", System.Type.GetType("System.Int32"));
            dt.Columns.Add("PermanentPopulationTotal", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("PermanentResidentsTotal", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("PermanentPopulationIncrement", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("NetPopulationInflow", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("PerCapitaSavings", System.Type.GetType("System.String"));
            dt.Columns.Add("PrimarySchoolEnrollment", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("UniversitySchoolEnrollment", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("UrbanizationRate", System.Type.GetType("System.Decimal"));
            dt.Columns.Add("Creator", System.Type.GetType("System.String"));
            dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
            dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
            list.ForEach(m =>
            {
                DataRow dr = dt.NewRow();
                dr["Code"] = m.Code;
                dr["ResearchData_Code"] = m.ResearchData_Code;
                dr["Annual"] = m.Annual;
                dr["PermanentPopulationTotal"] = m.PermanentPopulationTotal;
                dr["PermanentResidentsTotal"] = m.PermanentResidentsTotal;
                dr["PermanentPopulationIncrement"] = m.PermanentPopulationIncrement;
                dr["NetPopulationInflow"] = m.NetPopulationInflow;
                dr["PerCapitaSavings"] = m.PerCapitaSavings;
                dr["PrimarySchoolEnrollment"] = m.PrimarySchoolEnrollment;
                dr["UniversitySchoolEnrollment"] = m.UniversitySchoolEnrollment;
                dr["UrbanizationRate"] = m.UrbanizationRate;
                dr["Creator"] = m.Creator;
                dr["CreateTime"] = m.CreateTime;
                dr["ValidStatus"] = m.ValidStatus;
                dt.Rows.Add(dr);
            });
            SqlDbHelper.BulkInsertData(dt, "office.RD_Population", GetConnectionName());
        }

        /// <summary>
        /// 获取人口
        /// </summary>
        /// <param name="code">客研编码</param>
        /// <returns></returns>
        public List<RD_PopulationModel> GetPopulationByRD_Code(string code)
        {
            return Instance.Load(m => m.AppendItem("ResearchData_Code", code)).OrderByDescending(o => o.Annual).Take(5).ToList();
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
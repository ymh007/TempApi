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
	/// 客研数据_土地市场Adapter
	/// </summary>
	public class RD_LandMarketAdapter : UpdatableAndLoadableAdapterBase<RD_LandMarketModel, RD_LandMarketCollection>
	{
		/// <summary>
		/// 实例
		/// </summary>
		public static readonly RD_LandMarketAdapter Instance = new RD_LandMarketAdapter();

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
		public void BatchInsert(List<RD_LandMarketModel> list)
		{
			if (list.Count < 1) { throw new Exception("至少保存一条数据"); }
			//构造DataTable
			var dt = new DataTable();
			dt.Columns.Add("Code", System.Type.GetType("System.String"));
			dt.Columns.Add("ResearchData_Code", System.Type.GetType("System.String"));
			dt.Columns.Add("Monthly", System.Type.GetType("System.DateTime"));
			dt.Columns.Add("LandTransactionScale", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("FloorPrice", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("PremiumRate", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("LandGrant", System.Type.GetType("System.Decimal"));
			dt.Columns.Add("LowPriceRatio", System.Type.GetType("System.String"));
			dt.Columns.Add("Creator", System.Type.GetType("System.String"));
			dt.Columns.Add("CreateTime", System.Type.GetType("System.DateTime"));
			dt.Columns.Add("ValidStatus", System.Type.GetType("System.Boolean"));
			list.ForEach(m =>
			{
				DataRow dr = dt.NewRow();
				dr["Code"] = m.Code;
				dr["ResearchData_Code"] = m.ResearchData_Code;
				dr["Monthly"] = m.Monthly;
				dr["LandTransactionScale"] = m.LandTransactionScale;
				dr["FloorPrice"] = m.FloorPrice;
				dr["PremiumRate"] = m.PremiumRate;
				dr["LandGrant"] = m.LandGrant;
				dr["LowPriceRatio"] = m.LowPriceRatio;
				dr["Creator"] = m.Creator;
				dr["CreateTime"] = m.CreateTime;
				dr["ValidStatus"] = m.ValidStatus;
				dt.Rows.Add(dr);
			});
			SqlDbHelper.BulkInsertData(dt, "office.RD_LandMarket", GetConnectionName());
		}

		/// <summary>
		/// 获取土地列表
		/// </summary>
		/// <param name="code">客研编码</param>
		/// <returns></returns>
		public List<RD_LandMarketModel> GetLandMarketByRD_Code(string code)
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
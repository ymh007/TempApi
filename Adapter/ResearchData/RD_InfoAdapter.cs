using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi;
using Seagull2.YuanXin.AppApi.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Adapter.ResearchData
{
    /// <summary>
    /// 客研数据_信息Adapter
    /// </summary>
    public class RD_InfoAdapter : UpdatableAndLoadableAdapterBase<RD_InfoModel, RD_InfoCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly RD_InfoAdapter Instance = new RD_InfoAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public List<RD_InfoModel> GetList(string cityName, int pageIndex, int pageSize)
        {
            pageIndex--;
            var sql = @"WITH [Temp] AS
                        (
                          SELECT ROW_NUMBER() OVER (ORDER BY [CreateTime] DESC) AS [Row],*
						
                          FROM  office.RD_Info

						  WHERE  CityName like '%' + @CityName + '%'
                        )
                        SELECT * FROM [Temp] WHERE [Temp].[Row] BETWEEN @PageIndex AND @PageSize";
            SqlParameter[] parameters = {
                new SqlParameter("@CityName",SqlDbType.NVarChar,36),
                new SqlParameter("@PageIndex",SqlDbType.NVarChar,36),
                new SqlParameter("@PageSize",SqlDbType.NVarChar,36),
            };
            parameters[0].Value = cityName;
            parameters[1].Value = pageSize * pageIndex + 1;
            parameters[2].Value = pageSize * pageIndex + pageSize;
            SqlDbHelper helper = new SqlDbHelper();
            var table = helper.ExecuteDataTable(sql, CommandType.Text, parameters);
            var list = DataConvertHelper<RD_InfoModel>.ConvertToList(table);
            return list;
        }


        public int GetCount(string cityName)
        {
            var sql = @"SELECT COUNT(*) AS [Count] FROM office.RD_Info WHERE [CityName] LIKE '%' + @CityName + '%'";

            SqlParameter[] parameters = { new SqlParameter("@CityName", SqlDbType.NVarChar, 36) };
            parameters[0].Value = cityName;

            SqlDbHelper helper = new SqlDbHelper();
            var count = (int)helper.ExecuteScalar(sql, CommandType.Text, parameters);
            return count;
        }

        /// <summary>
        /// 根据编码获取信息
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public RD_InfoModel GetInfoByCode(string code)
        {
            return Instance.Load(m => m.AppendItem("Code", code)).FirstOrDefault();
        }

        /// <summary>
        /// 根据城市查询是否存在数据
        /// </summary>
        /// <param name="cityName"></param>
        /// <returIsExistsns></returns>
        public RD_InfoModel GetRD_InfoByCity(string cityName)
        {
            return Instance.Load(m => { m.AppendItem("CityName", cityName); }).SingleOrDefault();
        }

        /// <summary>
        /// 根据编码删除客研数据
        /// </summary>
        /// <param name="code"></param>
        public void DeleteAllInfo(string code)
        {
            var sql = @"begin
                                DELETE office.RD_Info
                                WHERE code = @Code;
                                DELETE	office.RD_Policy
                                WHERE ResearchData_Code = @Code;
                                DELETE	office.RD_Economic
                                WHERE ResearchData_Code = @Code;
                                DELETE office.RD_Population
                                WHERE ResearchData_Code = @Code;
                                DELETE office.RD_LandMarket
                                WHERE ResearchData_Code = @Code;
                                DELETE	office.RD_NHM_AnnualStatistics
                                WHERE ResearchData_Code = @Code;
                                DELETE office.RD_NHM_MonthlyStatistics
                                WHERE ResearchData_Code = @Code;
                                DELETE	office.RD_Benchmark_EnterpriseSaleStatistics
                                WHERE ResearchData_Code = @Code;
                                DELETE	office.RD_Benchmark_ProjectSaleStatistics
                                WHERE ResearchData_Code = @Code;
                                end";
            SqlParameter[] parameters = { new SqlParameter("@Code", SqlDbType.NVarChar, 36) };
            parameters[0].Value = code;
            SqlDbHelper helper = new SqlDbHelper();
            helper.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }
    }
}
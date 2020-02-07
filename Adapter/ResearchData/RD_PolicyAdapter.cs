using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Adapter.ResearchData
{
    /// <summary>
    /// 客研数据_信息Adapter
    /// </summary>
    public class RD_PolicyAdapter : UpdatableAndLoadableAdapterBase<RD_PolicyModel, RD_PolicyCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly RD_PolicyAdapter Instance = new RD_PolicyAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据客研编码获取信息
        /// </summary>
        /// <param name="code">客研编码</param>
        /// <returns></returns>
        public RD_PolicyModel GetInfoByRD_Code(string code)
        {
            return Instance.Load(m => m.AppendItem("ResearchData_Code", code)).FirstOrDefault();
        }
    }
}
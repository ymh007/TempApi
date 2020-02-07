using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Sign;
using System;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Adapter.Sign
{
    /// <summary>
    /// 极速打卡开关设置 Adapter
    /// </summary>
    public class PunchQuickSettingAdapter : UpdatableAndLoadableAdapterBase<PunchQuickSettingModel, PunchQuickSettingCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly PunchQuickSettingAdapter Instance = new PunchQuickSettingAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取用户的极速打卡设置
        /// </summary>
        public PunchQuickSettingModel GetModelByUserCode(string userCode)
        {
            return Load(w =>
            {
                w.AppendItem("Creator", userCode);
            }).FirstOrDefault();
        }
    }
}
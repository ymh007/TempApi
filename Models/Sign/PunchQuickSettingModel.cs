using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.Sign
{
    /// <summary>
    /// 极速打卡开关设置 Model
    /// </summary>
    [ORTableMapping("office.PunchQuickSetting")]
    public class PunchQuickSettingModel : Models.BaseModel
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }
    }

    /// <summary>
    /// 极速打卡开关设置 Collection
    /// </summary>
    public class PunchQuickSettingCollection : EditableDataObjectCollectionBase<PunchQuickSettingModel>
    {

    }
}
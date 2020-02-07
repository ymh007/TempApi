using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.Sign
{
    /// <summary>
    /// 打卡提醒设置 Model
    /// </summary>
    [ORTableMapping("office.PunchRemindSetting")]
    public class PunchRemindSettingModel : Models.BaseModel
    {
        /// <summary>
        /// 周一提醒
        /// </summary>
        [ORFieldMapping("Monday")]
        public bool Monday { get; set; }

        /// <summary>
        /// 周二提醒
        /// </summary>
        [ORFieldMapping("Tuesday")]
        public bool Tuesday { get; set; }

        /// <summary>
        /// 周三提醒
        /// </summary>
        [ORFieldMapping("Wednesday")]
        public bool Wednesday { get; set; }

        /// <summary>
        /// 周四提醒
        /// </summary>
        [ORFieldMapping("Thursday")]
        public bool Thursday { get; set; }

        /// <summary>
        /// 周五提醒
        /// </summary>
        [ORFieldMapping("Friday")]
        public bool Friday { get; set; }

        /// <summary>
        /// 周六提醒
        /// </summary>
        [ORFieldMapping("Saturday")]
        public bool Saturday { get; set; }

        /// <summary>
        /// 周日提醒
        /// </summary>
        [ORFieldMapping("Sunday")]
        public bool Sunday { get; set; }

        /// <summary>
        /// 提醒时间
        /// </summary>
        [ORFieldMapping("RemindTime")]
        public string RemindTime { get; set; }

        /// <summary>
        /// 类型 0=上午 1=下午
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [ORFieldMapping("IsEnable")]
        public bool IsEnable { get; set; }
    }

    /// <summary>
    /// 打卡提醒设置 Collection
    /// </summary>
    public class PunchRemindSettingCollection : EditableDataObjectCollectionBase<PunchRemindSettingModel>
    {

    }
}
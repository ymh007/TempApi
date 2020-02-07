using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.ScheduleManage
{
    /// <summary>
    /// 日程类
    /// </summary>
    [ORTableMapping("office.Schedule")]
    public class ScheduleModel : BaseModel
    {

        /// <summary>
        /// 日程code
        /// </summary>
        public string ScheduleCode { get; set; }


        /// <summary>
        /// 日程类型 : 未跨天数据 1  原始数据 0  拆分数据  2  
        /// </summary>
        public int ScheduleType { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 查询时间
        /// </summary>
        public int SearchTime { get; set; }
        /// <summary>
        /// 日程开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 日程结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 日程地点
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// 提醒时间
        /// </summary>
        public ReminderTimeType ReminderTime { get; set; }
        /// <summary>
        /// 日程内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        ///  outlook  数据id
        /// </summary>
        public string OutlookId { get; set; }

        ///// <summary>
        /////  outlook   
        ///// </summary>
        //public bool Synchronization { get; set; }

        /// <summary>
        /// 参与人员
        /// </summary>
        [NoMapping]
        public ScheduleParticipantsCollection Participants { get; set; }

        /// <summary>
        /// 分组值
        /// </summary>
        [NoMapping]
        public string GroupKey
        {
            get
            {
                return StartTime.ToString("yyyy-MM-dd");
            }
            set { }
        }
        /// <summary>
        /// 分组值
        /// </summary>
        [NoMapping]
        public int CoressDay
        {
            get
            {
                return (EndTime - StartTime).Days;
            }
            set { }
        }
        [NoMapping]
        public bool IsOutlook { get; set; }
        [NoMapping]
        public bool IsMeeting { get; set; }
        [NoMapping]
        public bool IsCancelled { get; set; }
        [NoMapping]
        public long ReminderDueBy { get; set; }
        [NoMapping]
        public bool IsFromMe { get; set; }
    }

    /// <summary>
    /// 日程提醒枚举
    /// </summary>
    public enum ReminderTimeType
    {
        /// <summary>
        /// 不提醒
        /// </summary>
        NoNotify = 0,

        /// <summary>
        /// 事件开始时
        /// </summary>
        EventBeginning = 1,

        /// <summary>
        /// 提前15分钟
        /// </summary>
        Beforehand15 = 2,

        /// <summary>
        /// 提前30分钟
        /// </summary>
        Beforehand30 = 3,

        /// <summary>
        /// 提前一个小时
        /// </summary>
        Beforehand60 = 4,

        /// <summary>
        /// 提前一天
        /// </summary>
        BeforehandOneDay = 5,

        /// <summary>
        /// 其他
        /// </summary>
        Other = 6


    }

    public class ScheduleViewModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public int ReminderTime { get; set; }
        public DateTime StartTime { get; set; }
        public string Place { get; set; }
        public string Creator { get; set; }
    }






    /// <summary>
    /// 日程集合
    /// </summary>
    public class ScheduleModelCollection : EditableDataObjectCollectionBase<ScheduleModel>
    {

    }
}
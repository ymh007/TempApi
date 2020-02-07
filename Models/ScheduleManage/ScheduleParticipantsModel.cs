using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.ScheduleManage
{
    /// <summary>
    /// 日程参与人员
    /// </summary>
    [ORTableMapping("office.ScheduleParticipants")]
    public class ScheduleParticipantsModel 
    {
        /// <summary>
        /// 主键编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 日程code
        /// </summary>
        public string ScheduleCode { get; set; }
        /// <summary>
        /// 参与人员名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参与人员code
        /// </summary>
        public string PersonnelCode { get; set; }

        /// <summary>
        /// 人员部门
        /// </summary>
        public string Department { get; set; }
    }


    /// <summary>
    /// 参与人员集合
    /// </summary>
    public class ScheduleParticipantsCollection : EditableDataObjectCollectionBase<ScheduleParticipantsModel>
    {

    }
}
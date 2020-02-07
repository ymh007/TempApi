using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Message;
using Seagull2.YuanXin.AppApi.Models.ScheduleManage;
using System;

namespace Seagull2.YuanXin.AppApi.Adapter.ScheduleManage
{
    /// <summary>
    /// 日程管理Adapter
    /// </summary>
    public class ScheduleAdapter : UpdatableAndLoadableAdapterBase<ScheduleModel, ScheduleModelCollection>
    {

        public static readonly ScheduleAdapter Instance = new ScheduleAdapter();

        /// <summary>
        /// 
        /// </summary>
        public ScheduleAdapter()
        {

        }

        /// <summary>
        /// 获取链接名称
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 获取要提示的今天和明天的日程
        /// </summary>
        /// <returns></returns>
        public ScheduleModelCollection GetNotifyData()
        {
            string sql= $"SELECT* FROM [YuanXinBusiness].[office].[Schedule]  where ScheduleType <=1 and  ReminderTime>0  and  StartTime between '{DateTime.Now.ToString("yyyy-MM-dd")}' and '{DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")} 23:55'  order by StartTime";
           return  this.QueryData(sql);
        }
        /// <summary>
        /// 获取跨天原始数据
        /// </summary>
        /// <returns></returns>
        public ScheduleModelCollection GetHastenData(string creator)
        {
            string sql = $"select  ScheduleCode,StartTime,EndTime from  [YuanXinBusiness].[office].[Schedule] where Creator='{creator}' and  ScheduleType=0 ";
            return this.QueryData(sql);
        }

        /// <summary>
        /// 获取单个日程对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public MessageModel GetScheduleModel(string code)
        {
            ScheduleModelCollection source = this.Load(p => p.AppendItem("Code", code));
            if (source.Count > 0)
            {
                var temp = source[0];
                return  new MessageModel()
                {
                    Code = Guid.NewGuid().ToString(),
                    MeetingCode = temp.Code,
                    MessageContent = $"您的“{temp.Title}”将于“{temp.StartTime.ToString("yyyy-MM-dd HH:mm")}”{(string.IsNullOrEmpty(temp.Place) ? "开始" : "在“" + temp.Place + "”开始")}，点击进入详情",
                    MessageStatusCode = EnumMessageStatus.New,
                    MessageTypeCode = "2",
                    MessageTitleCode = EnumMessageTitle.ScheduleManage,
                    ModuleType = EnumMessageModuleType.ScheduleManage.ToString(),
                    Creator = temp.Creator,
                    ReceivePersonCode = temp.Creator,
                    ReceivePersonName = temp.Creator,
                    ReceivePersonMeetingTypeCode = "",
                    OverdueTime = DateTime.Now.AddDays(1),
                    ValidStatus = true,
                    CreateTime = DateTime.Now
                };
            }
            else {
                return null;
            }
        }
    }


    /// <summary>
    /// 日程参与人员Adapter
    /// </summary>
    public class ScheduleParticipantsAdapter : UpdatableAndLoadableAdapterBase<ScheduleParticipantsModel, ScheduleParticipantsCollection>
    {
        public static readonly ScheduleParticipantsAdapter Instance = new ScheduleParticipantsAdapter();

        /// <summary>
        /// 
        /// </summary>
        public ScheduleParticipantsAdapter()
        {

        }

        /// <summary>
        /// 获取链接名称
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

    }
}
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 参会人员 Adapter
    /// </summary>
    public class AttendeeAdapter : UpdatableAndLoadableAdapterBase<AttendeeModel, AttendeeCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly AttendeeAdapter Instance = new AttendeeAdapter();

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据主键编码获取参会人员
        /// </summary>
        public AttendeeModel LoadByID(string id)
        {
            return Load(m => m.AppendItem("ID", id)).SingleOrDefault();
        }

        /// <summary>
        /// 根据人员编码和会议编码获取参会人员
        /// </summary>
        public AttendeeModel LoadByAttendeeID(string attendeeId, string conferenceId)
        {
            return Load(m => m.AppendItem("AttendeeID", attendeeId).AppendItem("ConferenceID", conferenceId)).FirstOrDefault();
        }

        /// <summary>
        /// 更新参会人员集合
        /// </summary>
        public void UpdateAttendeeColl(AttendeeCollection attendeeCollection)
        {
            attendeeCollection.ForEach(attendee =>
            {
                Update(attendee);
            });
        }

        /// <summary>
        /// 根据会议ID删除所有参会人数据
        /// </summary>
        public void DelByConferceId(string conferenceId)
        {
            Delete(p =>
            {
                p.AppendItem("ConferenceId", conferenceId);
            });
        }

        /// <summary>
        /// 更改参会人状态
        /// </summary>
        /// <param name="id">主键编码</param>
        /// <param name="validStatus">状态</param>
        public void UpdateStatus(string id, bool validStatus = true)
        {
            var model = LoadByID(id);
            if (model != null)
            {
                model.ValidStatus = validStatus;
                Update(model);
            }
        }

        /// <summary>
        /// 判断是否存在该参会人
        /// </summary>
        public AttendeeModel IsContainsAttendee(string Email, string conferenceID)
        {
            return Load(m =>
            {
                m.AppendItem("Email", Email);
                m.AppendItem("ConferenceID", conferenceID);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据编码查询参会人数据
        /// </summary>
        /// <param name="attendeeID">海鸥二编码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="AttendeeType">参会人类型</param>    
        /// <returns></returns>
        public AttendeeModel GetAttendeeByAttendaID(string attendeeID, string conferenceID, int AttendeeType)
        {
            return this.Load(p =>
            {
                p.AppendItem("AttendeeID", attendeeID);
                p.AppendItem("ConferenceID", conferenceID);
                p.AppendItem("AttendeeType", AttendeeType);
            }).FirstOrDefault();
        }
        /// <summary>
        /// 根据编码查询参会人数据
        /// </summary>
        /// <param name="attendeeID">海鸥二编码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        public AttendeeCollection GetAttendeeByAttendaID(string attendeeID, string conferenceID)
        {
            return this.Load(p =>
            {
                p.AppendItem("AttendeeID", attendeeID);
                p.AppendItem("ConferenceID", conferenceID);
            });
        }
        /// <summary>
        /// 根据邮箱和会议编码查询参会人
        /// </summary>
        /// <param name="email">海鸥二编码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        public AttendeeModel GetByEmailAndConferenceID(string email, string conferenceID, int attType)
        {
            return this.Load(p =>
            {
                p.AppendItem("Email", email);
                p.AppendItem("ConferenceID", conferenceID);
                p.AppendItem("AttendeeType", attType);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据邮箱查询参会人数据
        /// </summary>
        /// <param name="email"></param>
        public AttendeeModel GetAttendeeByEmail(string email)
        {
            return this.Load(p =>
            {
                p.AppendItem("Email", email);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 根据会议编码查询参会人
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <returns></returns>
        public AttendeeCollection GetAttendeeCollectionByConference(string conferenceId, int attendeeType = 1)
        {

            return this.Load(p =>
            {
                p.AppendItem("ConferenceId", conferenceId);
                p.AppendItem("AttendeeType", attendeeType);
            });
        }


        /// <summary>
        /// 根据会议编码查询参会人
        /// </summary>
        /// <param name="conferenceId">会议id</param>
        /// <param name="attendeeType">参会人类型</param>
        /// <param name="where">筛选条件</param>
        /// <returns></returns>
        public AttendeeCollection GetAttendeeCollectionByConference(string conferenceId, int attendeeType,string where)
        {
            string sql =$"select * from  office.Attendee  where ConferenceID = '{conferenceId}' and AttendeeType ={attendeeType}" ;
            if (!string.IsNullOrEmpty(where))
            {
                sql = $"select * from  YuanXinBusiness.office.Attendee  where ConferenceID = '{conferenceId}' and AttendeeType ={attendeeType}  and (name like '%{where}%' or email like '%{where}%')";
            }
            return this.QueryData(sql);
        }

    }
}
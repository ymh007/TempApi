using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TD = SinoOcean.Seagull2.TransactionData.Meeting;
using MD = SinoOcean.Seagull2.Framework.MasterData;

namespace Seagull2.YuanXin.AppApi.Services.Meeting
{
    /// <summary>
    /// 会议类(远洋平台模拟)
    /// </summary>
    public class MeetingsYuanXinAdapter : TD.MeetingAdapter
    {
        public static MeetingsYuanXinAdapter Instance = new MeetingsYuanXinAdapter();

        /// <summary>
        /// 查询某个会议室下和某个时间段有关联的所有会议信息
        /// </summary>
        public static TD.MeetingsCollection getAllByMeetRoomCodeAndTime(string meetRoomCode, DateTime startTime, DateTime endTime)
        {
            string sTime = startTime.ToString("yyyy-MM-dd HH:mm:ss");
            string eTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = string.Format(@"SELECT meet.* FROM SecretaryAdministration.Secretary.Meetings meet
                                        INNER JOIN SecretaryAdministration.Secretary.MeetingRoomUseRecord mrus ON mrus.MeetingsCode=meet.Code
                                        INNER JOIN SubjectDB.Secretary.MeetingRoom mr ON mr.Code=mrus.MeetingRoomCode
                                        WHERE mr.VersionEndTime IS NULL AND mr.IsOuter=0 AND mr.UseStateCode={0} AND mr.Code='{1}' AND ((meet.StartTime<='{2}' AND meet.EndTime>='{3}') OR (meet.StartTime<='{4}' AND meet.EndTime>='{5}') OR (meet.StartTime>='{6}' AND meet.EndTime<='{7}'))",
                                        Convert.ToString((int)MD.MeetingUseStateEnum.Available), meetRoomCode, sTime, sTime, eTime, eTime, sTime, eTime);
            TD.MeetingsCollection mColl = Instance.QueryData(sql);
            return mColl;
        }
        /// <summary>
        /// 查询某个会议室下某个时间段范围内的所有会议信息
        /// </summary>
        public static TD.MeetingsCollection getAllByRoomCodeAndBetweenTime(string meetRoomCode, DateTime startTime, DateTime endTime)
        {
            string sTime = startTime.ToString("yyyy-MM-dd HH:mm:ss");
            string eTime = endTime.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = string.Format(@"SELECT meet.* FROM SecretaryAdministration.Secretary.Meetings meet
                                        INNER JOIN SecretaryAdministration.Secretary.MeetingRoomUseRecord mrus ON mrus.MeetingsCode=meet.Code
                                        INNER JOIN SubjectDB.Secretary.MeetingRoom mr ON mr.Code=mrus.MeetingRoomCode
                                        WHERE mr.VersionEndTime IS NULL AND mr.IsOuter=0 AND mr.UseStateCode={0} AND mr.Code='{1}' AND ((meet.StartTime>='{2}' and meet.EndTime <='{3}')
 or ('{2}' between meet.StartTime and meet.Endtime ) or ('{3}' between meet.StartTime and meet.Endtime))",
                                        Convert.ToString((int)MD.MeetingUseStateEnum.Available), meetRoomCode, sTime, eTime);
            TD.MeetingsCollection mColl = Instance.QueryData(sql);
            return mColl;
        }

        /// <summary>
        /// 根据登录人code查询曾经预定过的记账公司
        /// </summary>
        /// <param name="createrCode"></param>
        /// <returns></returns>
        public static TD.MeetingsCollection LoadByCreaterAndCnCode(string createrCode)
        {

            string sql = string.Format(@"SELECT TOP(1)* FROM Secretary.Meetings WHERE CreatorCode='{0}' ORDER BY CreateTime DESC",
                                       createrCode);
            TD.MeetingsCollection mColl = Instance.QueryData(sql);
            return mColl;
        }
    }
}
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    public class SignInViewModel : ViewModelBase
    {
        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime SignDate { get; set; }
    }

    /// <summary>
    /// 签到详情
    /// </summary>
    public class SignInDetailViewModelList
    {
        /// <summary>
        /// 签到时间
        /// </summary>
        public DateTime SignDate { get; set; }
        /// <summary>
        /// 签到时间Str
        /// </summary>
        public string SignDateStr
        {
            get
            {
                return this.SignDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// 参会人编码
        /// </summary>
        public string AttendeeID { get; set; }

        /// <summary>
        /// 参会人部门
        /// </summary>
        public string OrganizationStructure { get; set; }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string PhotoAddress { get; set; }

        /// <summary>
        /// 参会人姓名
        /// </summary>
        public string AttendeeName { get; set; }

        /// <summary>
        /// 议程ID
        /// </summary>
        public string AgendaID { get; set; }
        /// <summary>
        /// 议程标题
        /// </summary>
        public string AgendaTitle { get; set; }
        /// <summary>
        /// 是否签到
        /// </summary>
        public bool IsSigned { get; set; }
        /// <summary>
        /// 签到来源
        /// </summary>
        public int SignSourceType { get; set; }
        /// <summary>
        /// 签到来源名称
        /// </summary>
        public string SignSourceTypeName
        {
            get
            {
                if (SignSourceType != 0 && SignSourceType != 1)
                {
                    return EnumItemDescriptionAttribute.GetDescription(EnumSignSourceType.App);
                }
                else
                {
                    return EnumItemDescriptionAttribute.GetDescription((EnumSignSourceType)Convert.ToInt32(this.SignSourceType));
                }
            }
        }
    }
    public class SignInDetailViewModelListAdapter : ViewBaseAdapter<SignInDetailViewModelList, List<SignInDetailViewModelList>>
    {
        private static string ConnectionString = "yuanxin";
        public static SignInDetailViewModelListAdapter Instance = new SignInDetailViewModelListAdapter();
        public SignInDetailViewModelListAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 分页查询签到列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="agendaID">议程编码</param>
        /// <param name="organizationName">组织名称</param>
        /// <param name="SelectSignIn">0-都查，1-签到，2,-未签到</param>
        /// <param name="IsWorkMan">是否排除工作人员</param>
        /// <returns></returns>
        public ViewPageBase<List<SignInDetailViewModelList>> GetSignInDetailViewModelByPage(int pageIndex, string conferenceID, string agendaID, string organizationName, string SelectSignIn, DateTime searchTime, string AttendeeName, bool IsWorkMan,int AttendeeType)
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "SELECT age.[ID] as AgendaID,att.AttendeeID,att.PhotoAddress ,age.[ConferenceID],att.[Name] as AttendeeName,att.[OrganizationStructure],sig.SignDate,age.Title AS AgendaTitle,(case when sig.SignDate  IS NULL  then 0 else  1 end ) as IsSigned,sig.SignSourceType";
            string fromAndWhereSQL = string.Format(@"FROM office.Attendee att  --参会人
                                                    INNER JOIN office.Conference con ON con.ID=att.ConferenceID  --会议
                                                    INNER JOIN office.Agenda age ON age.ConferenceID=con.ID and age.NeedSign=1  --议程
													 LEFT JOIN office.SignDetail sig ON sig.AgendaID=age.ID AND sig.AttendeeID=att.ID --签到详情
                                                    WHERE att.ValidStatus=1 AND  AttendeeType={0} AND att.CreateTime<='{1}'", AttendeeType,searchTime);
            if (!conferenceID.IsEmptyOrNull())
            {
                fromAndWhereSQL += string.Format(@" AND con.ID='{0}'", conferenceID);
            }
            if (!agendaID.IsEmptyOrNull() && agendaID != "null")
            {
                fromAndWhereSQL += string.Format(@" AND age.ID='{0}'", agendaID);
            }
            if (!organizationName.IsEmptyOrNull() && organizationName != "null")
            {
                fromAndWhereSQL += string.Format(@" AND att.OrganizationStructure LIKE '%{0}%'", organizationName);
            }
            if (!AttendeeName.IsEmptyOrNull() && AttendeeName != "null")
            {
                fromAndWhereSQL += string.Format(@" AND att.Name LIKE '%{0}%'", AttendeeName);
            }
            //0-所有，1-签到，2-未签到
            if (SelectSignIn == "1")
            {
                fromAndWhereSQL += string.Format(@" AND sig.SignDate IS NOT NULL");
            }
            else if (SelectSignIn == "2")
            {
                fromAndWhereSQL += string.Format(@" AND sig.SignDate IS NULL");
            }
            if (IsWorkMan)
            {
                if (!conferenceID.IsEmptyOrNull())
                {
                    fromAndWhereSQL += string.Format(@" AND att.AttendeeID NOT IN (SELECT UserID FROM office.Worker WHERE ConferenceID='{0}')", conferenceID);
                }
                else
                {
                    fromAndWhereSQL += string.Format(@" AND att.AttendeeID NOT IN (SELECT UserID FROM office.Worker)");
                }
            }
            string orderSQL = "ORDER BY att.NAME,att.PhotoAddress";
            ViewPageBase<List<SignInDetailViewModelList>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>
        /// 查询签到数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="conferenceID">会议编码</param>
        /// <param name="agendaID">议程编码</param>
        /// <param name="organizationName">组织名称</param>
        /// <param name="SelectSignIn">0-都查，1-签到，2,-未签到</param>
        /// <returns></returns>
        public List<SignInDetailViewModelList> GetSignInData(string conferenceID, string agendaID, string SelectSignIn, string AttendeeName,int AttendeeType)
        {
            string selectSQL = "SELECT age.[ID] as AgendaID,att.AttendeeID,att.PhotoAddress ,age.[ConferenceID],att.[Name] as AttendeeName,att.[OrganizationStructure],sig.SignDate,age.Title AS AgendaTitle,(case when sig.SignDate  IS NULL  then 0 else  1 end ) as IsSigned,sig.SignSourceType";
            string fromAndWhereSQL = string.Format(@"FROM office.Attendee att  --参会人
                                                    INNER JOIN office.Conference con ON con.ID=att.ConferenceID  --会议
                                                    INNER JOIN office.Agenda age ON age.ConferenceID=con.ID and age.NeedSign=1  --议程
													 LEFT JOIN office.SignDetail sig ON sig.AgendaID=age.ID AND sig.AttendeeID=att.ID --签到详情
                                                    WHERE att.ValidStatus=1 AND AttendeeType={0}", AttendeeType);
            if (!conferenceID.IsEmptyOrNull())
            {
                fromAndWhereSQL += string.Format(@" AND con.ID='{0}'", conferenceID);
            }
            if (!agendaID.IsEmptyOrNull() && agendaID != "null")
            {
                fromAndWhereSQL += string.Format(@" AND age.ID='{0}'", agendaID);
            }
            if (!AttendeeName.IsEmptyOrNull() && AttendeeName != "null")
            {
                fromAndWhereSQL += string.Format(@" AND att.Name LIKE '%{0}%'", AttendeeName);
            }
            //0-所有，1-签到，2-未签到
            if (SelectSignIn == "1")
            {
                fromAndWhereSQL += string.Format(@" AND sig.SignDate IS NOT NULL");
            }
            else if (SelectSignIn == "2")
            {
                fromAndWhereSQL += string.Format(@" AND sig.SignDate IS NULL");
            }
            string orderSQL = "ORDER BY att.NAME,att.PhotoAddress";

            List<SignInDetailViewModelList> result = LoadTColl(selectSQL + " " + fromAndWhereSQL + " " + orderSQL);
            return result;
        }
    }
}


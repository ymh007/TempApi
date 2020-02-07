using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.Sign
{
    /// <summary>
    /// 打卡统计
    /// </summary>
    public class SignStatisticsViewModel
    {
        /// <summary>
        /// 组织或人员编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 组织或人员名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 所属组织名称，针对人员
        /// </summary>
        public string OrganizationName
        {
            set { _OrganizationName = value; }
            get
            {
                if (string.IsNullOrWhiteSpace(_OrganizationName)) return string.Empty;
                var arr = _OrganizationName.Split('\\');
                if (arr.Length >= 2)
                {
                    return arr[arr.Length - 2];
                }
                return _OrganizationName;
            }
        }
        string _OrganizationName;
        /// <summary>
        /// 平均工作时长
        /// </summary>
        public string AvgHour { set; get; }
        /// <summary>
        /// 迟到次数
        /// </summary>
        public string LateCount { set; get; }
        /// <summary>
        /// 早退次数
        /// </summary>
        public string EarlyCount { set; get; }
        /// <summary>
        /// 异常次数
        /// </summary>
        public string UnusualCount { set; get; }
        /// <summary>
        /// 未签到次数
        /// </summary>
        public string NoSignCount { set; get; }
        /// <summary>
        /// 人员数量
        /// </summary>
        public int UserCount { set; get; }
    }

    /// <summary>
    /// 搜索选项列表
    /// </summary>
    public class GetSearchListViewModel
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
    }
    /// <summary>
    /// 打卡统计 Plus
    /// </summary>
    public class SignStatisticsPlusViewModel
    {
        /// <summary>
        /// 单元、组织或人员编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 单元、组织或人员名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 类型（Management=打卡管理单元 Organization=组织机构 User=人员）
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 是否为自定义打卡管理单元
        /// </summary>
        public bool IsCustomize { set; get; }
        /// <summary>
        /// 人员数量
        /// </summary>
        public int UserCount { set; get; }
        /// <summary>
        /// 平均工作时长
        /// </summary>
        public string AvgHour { set; get; }
        /// <summary>
        /// 迟到次数
        /// </summary>
        public string LateCount { set; get; }
        /// <summary>
        /// 早退次数
        /// </summary>
        public string EarlyCount { set; get; }
        /// <summary>
        /// 异常次数
        /// </summary>
        public string UnusualCount { set; get; }
        /// <summary>
        /// 未签到次数
        /// </summary>
        public string NoSignCount { set; get; }
        /// <summary>
        /// 默认首个考勤对象的编码
        /// </summary>
        public string DefaultCode { set; get; }
        /// <summary>
        /// 默认首个考勤对象的类型（Organizations=组织机构 Users=用户）
        /// </summary>
        public string DefaultType { set; get; }
        /// <summary>
        /// 打卡管理单元创建人编码
        /// </summary>
        public string Creator { set; get; }
    }

    /// <summary>
    /// 个人签到明细
    /// </summary>
    public class SignDetailViewModel
    {
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnName { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string CnName { set; get; }
        /// <summary>
        /// 所属组织名称
        /// </summary>
        public string OrganizationName
        {
            set { _OrganizationName = value; }
            get
            {
                if (string.IsNullOrWhiteSpace(_OrganizationName)) return string.Empty;
                var arr = _OrganizationName.Split('\\');
                if (arr.Length >= 2)
                {
                    return arr[arr.Length - 2];
                }
                return _OrganizationName;
            }
        }
        string _OrganizationName;
        /// <summary>
        /// 打卡地点
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 日期
        /// </summary>
        public string PunchDate { set; get; }
        /// <summary>
        /// 上午
        /// </summary>
        public string PunchTime1 { set; get; }
        /// <summary>
        /// 下午
        /// </summary>
        public string PunchTime2 { set; get; }
        /// <summary>
        /// 工作时长
        /// </summary>
        public string WorkHour { set; get; }
        /// <summary>
        /// 迟到
        /// </summary>
        public string IsLate { set; get; }
        /// <summary>
        /// 早退
        /// </summary>
        public string IsEarly { set; get; }
        /// <summary>
        /// 未签次数
        /// </summary>
        public string NoSign { set; get; }
        /// <summary>
        /// 异常原因
        /// </summary>
        public string UnusualType { set; get; }
        /// <summary>
        /// 异常说明
        /// </summary>
        public string UnusualDesc { set; get; }
    }

    // ***********************************************************

    /// <summary>
    /// 打卡 ViewModel
    /// </summary>
    public class SignViewModel
    {

        public string Code;
        /// <summary>
        /// 地理位置
        /// </summary>
        public string Address;
        /// <summary>
        /// 纬度
        /// </summary>
        public string Lat;
        /// <summary>
        /// 经度
        /// </summary>
        public string Lng;
        /// <summary>
        /// 是否异常
        /// </summary>
        public bool IsUnusual;
        /// <summary>
        /// 异常原因
        /// </summary>
        public string UnusualType;
        /// <summary>
        /// 异常说明
        /// </summary>
        public string UnusualDesc;
        /// <summary>
        /// 是否急速打卡
        /// </summary>
        public bool IsQuickPunch;
    }

    /// <summary>
    /// 当月考勤 ViewModel
    /// </summary>
    public class SignResultByMonthViewModel
    {
        /// <summary>
        /// 当天日期
        /// </summary>
        public string Day;
        /// <summary>
        /// 上班打卡
        /// </summary>
        public SignResultByMonthOfDayViewModel AM;
        /// <summary>
        /// 下班打卡
        /// </summary>
        public SignResultByMonthOfDayViewModel PM;
    }

    /// <summary>
    /// 当天签到或签退 ViewModel
    /// </summary>
    public class SignResultByMonthOfDayViewModel
    {
        /// <summary>
        /// 是否打卡
        /// </summary>
        public bool IsPunch;
        /// <summary>
        /// 打卡记录编码
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code;
        /// <summary>
        /// 标准打卡地点名称
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StandardAddress;
        /// <summary>
        /// 打卡位置
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PunchAddress;
        /// <summary>
        /// 打卡时间
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PunchTime;
        /// <summary>
        /// 是否迟到/是否早退
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRegular;
        /// <summary>
        /// 是否异常
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsUnusual;
        /// <summary>
        /// 异常原因
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UnusualType;
        /// <summary>
        /// 异常说明
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UnusualDesc;
        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsValid;

        /// <summary>
        /// 是否有效
        /// </summary>
        public int DescStatus;
    }

    /// <summary>
    /// 更新异常信息 ViewModel
    /// </summary>
    public class UpdateUnusualViewModel
    {
        /// <summary>
        /// 打卡编码
        /// </summary>
        public string Code;
        /// <summary>
        /// 打卡日期
        /// </summary>
        public string PunchDate;
        /// <summary>
        /// 0：签到、1：签退
        /// </summary>
        public int PunchType;
        /// <summary>
        /// 异常原因
        /// </summary>
        public string UnusualType;
        /// <summary>
        /// 异常说明
        /// </summary>
        public string UnusualDesc;
    }

    public class UserReport
    {
        /// <summary>
        /// 异常类型
        /// </summary>
        public string ut { get; set; }

        /// <summary>
        /// 打卡日期
        /// </summary>
        public string pdate { get; set; }

        /// <summary>
        /// 打卡时间   未打卡的时候显示 未签到 未签退
        /// </summary>
        public string ptime { get; set; }

        /// <summary>
        /// 星期几
        /// </summary>
        public string wd { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string desc { get; set; }
        
    }
    /// <summary>
    /// 打卡返回对象
    /// </summary>
    public class Temp 
    {
      public string CurrentTime { get; set; }
      public bool isRedirect { get; set; }
      public string msg { get; set; }
    }
}
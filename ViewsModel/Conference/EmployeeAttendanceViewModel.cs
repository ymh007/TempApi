using Seagull2.YuanXin.AppApi.Adapter;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    #region 员工考勤视图

    /// <summary>
    /// 员工考勤视图类
    /// </summary>
    public class EmployeeAttendanceViewModel
    {
        /// <summary>
        /// 员工考勤编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 打卡人
        /// </summary>
        public string CnName { get; set; }
        /// <summary>
        /// 组织部门
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 打卡时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 打卡时间Str
        /// </summary>
        public string CreateTimeStr
        {
            get
            {
                return this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 打卡日期Date
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 打卡类型Str
        /// </summary>
        public string PunchTypeStr { get; set; }
        /// <summary>
        /// 打卡位置
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 考勤设置编码
        /// </summary>
        public string StandardPunchCode { get; set; }
        //public StandardPunch StandardPunch
        //{
        //    get
        //    {
        //        StandardPunch data = StandardPunchAdapater.Instance.LoadByCode(this.StandardPunchCode);
        //        return data;
        //    }
        //}
    }
    public class EmployeeAttendanceViewModelAdapter : ViewBaseAdapter<EmployeeAttendanceViewModel, List<EmployeeAttendanceViewModel>>
    {
        private static string ConnectionString = ConnectionNameDefine.EmployeeAttendance;
        public static EmployeeAttendanceViewModelAdapter Instance = new EmployeeAttendanceViewModelAdapter();

        public EmployeeAttendanceViewModelAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 分页查询员工考勤数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="searchTime"></param>
        /// <param name="userName"></param>
        /// <param name="signAddress"></param>
        /// <param name="signDate"></param>
        /// <param name="fullPathName"></param>
        /// <returns></returns>
        public ViewPageBase<List<EmployeeAttendanceViewModel>> GetEmployeeAttendanceViewByPage(int pageIndex, DateTime searchTime, string userName = "", string signAddress = "", string signStartDate = "", string signEndDate = "", string fullPathName = "")
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            //分页查询考勤记录：一个人在一个地点上午打卡只取最早时间，下午取最晚时间
            string selectSQL = "SELECT * ";
            string fromAndWhereSQL = string.Format(@"FROM (
                                                    SELECT CnName,Min(FullPath) as FullPath,CreateDate,PunchTypeStr,Address,CASE PunchTypeStr WHEN '上午' THEN MIN(CreateTime) ELSE MAX(CreateTime) END AS CreateTime 
                                                    FROM 
                                                    (
                                                    SELECT es.CnName,es.CreateTime,es.FullPath,CONVERT(varchar(100), es.CreateTime, 111) CreateDate,CASE WHEN CONVERT(INT,DATENAME(HOUR,es.CreateTime))<12 THEN '上午' ELSE '下午' END AS PunchTypeStr,sp.Address
                                                    FROM dbo.EmployeeServices es
                                                    LEFT JOIN dbo.StandardPunch sp ON sp.Code=es.StandardPunchCode
                                                    ) A WHERE A.CreateTime is not NULL {0} {1} {2} {3} {4}
                                                    GROUP BY CnName,PunchTypeStr,Address,CreateDate
                                                    ) B
                                                    ", userName != "null" && !userName.IsEmptyOrNull() ? "" : "",
                                                    signAddress != "null" && !signAddress.IsEmptyOrNull() ? "AND A.Address='" + signAddress + "'" : "",
                                                    signStartDate != "null" && !signStartDate.IsEmptyOrNull() ? "AND A.CreateTime>'" + Convert.ToDateTime(signStartDate).ToString("yyyy/MM/dd") + "'" : "",
                                                    signEndDate != "null" && !signEndDate.IsEmptyOrNull() ? "AND A.CreateTime<'" + Convert.ToDateTime(signEndDate).AddDays(1).ToString("yyyy/MM/dd") + "'" : "",
                                                    fullPathName != "null" && !fullPathName.IsEmptyOrNull() ? "AND A.FullPath like '%" + fullPathName + "%'" : ""
                                                    );

            string orderSQL = "ORDER BY Address DESC,CreateTime DESC";
            ViewPageBase<List<EmployeeAttendanceViewModel>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return result;
        }
        /// <summary>
        /// 查询全部员工考勤数据
        /// </summary>
        /// <returns></returns>
        public List<EmployeeAttendanceViewModel> GetAllEmployeeAttendance(string userName = "", string signAddress = "", string signStartDate = "", string signEndDate = "", string fullPathName = "")
        {
            //分页查询考勤记录：一个人在一个地点上午打卡只取最早时间，下午取最晚时间
            string sql = string.Format(@"SELECT CnName,Min(FullPath) as FullPath,CreateDate,PunchTypeStr,Address,CASE PunchTypeStr WHEN '上午' THEN MIN(CreateTime) ELSE MAX(CreateTime) END AS CreateTime 
                                        FROM 
                                        (
                                        SELECT es.CnName,es.CreateTime,CONVERT(varchar(100), es.CreateTime, 111) CreateDate,CASE WHEN CONVERT(INT,DATENAME(HOUR,es.CreateTime))<12 THEN '上午' ELSE '下午' END AS PunchTypeStr,sp.Address,es.FullPath
                                        FROM MobileBusiness.dbo.EmployeeServices es
                                        LEFT JOIN MobileBusiness.dbo.StandardPunch sp ON sp.Code=es.StandardPunchCode
                                        ) A WHERE A.CreateTime is not NULL {0} {1} {2} {3} {4}
                                        GROUP BY CnName,PunchTypeStr,Address,CreateDate
                                        ORDER BY Address DESC,CreateTime DESC",
                                        userName != "null" && !userName.IsEmptyOrNull() ? "" : "",
                                        signAddress != "null" && !signAddress.IsEmptyOrNull() ? "AND A.Address='" + signAddress + "'" : "",
                                        signStartDate != "null" && !signStartDate.IsEmptyOrNull() ? "AND A.CreateTime>'" + Convert.ToDateTime(signStartDate).ToString("yyyy/MM/dd") + "'" : "",
                                        signEndDate != "null" && !signEndDate.IsEmptyOrNull() ? "AND A.CreateTime<'" + Convert.ToDateTime(signEndDate).AddDays(1).ToString("yyyy/MM/dd") + "'" : "",
                                        fullPathName != "null" && !fullPathName.IsEmptyOrNull() ? "AND A.FullPath like '%" + fullPathName + "%'" : ""
                                     );
            List<EmployeeAttendanceViewModel> dataList = LoadTColl(sql);

            return dataList;
        }
    }
    #endregion

    #region 员工统计考勤视图
    /// <summary>
    /// 员工统计考勤视图类
    /// </summary>
    public class EmployeeStatisticsViewModel
    {
        /// <summary>
        /// 打卡人
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LOGON_NAME { get; set; }
        /// <summary>
        /// 组织机构
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// 1号
        /// </summary>
        public string Day1 { get; set; }
        public string Day2 { get; set; }
        public string Day3 { get; set; }
        public string Day4 { get; set; }
        public string Day5 { get; set; }
        public string Day6 { get; set; }
        public string Day7 { get; set; }
        public string Day8 { get; set; }
        public string Day9 { get; set; }
        public string Day10 { get; set; }
        /// <summary>
        /// 11号
        /// </summary>
        public string Day11 { get; set; }
        public string Day12 { get; set; }
        public string Day13 { get; set; }
        public string Day14 { get; set; }
        public string Day15 { get; set; }
        public string Day16 { get; set; }
        public string Day17 { get; set; }
        public string Day18 { get; set; }
        public string Day19 { get; set; }
        public string Day20 { get; set; }
        /// <summary>
        /// 21号
        /// </summary>
        public string Day21 { get; set; }
        public string Day22 { get; set; }
        public string Day23 { get; set; }
        public string Day24 { get; set; }
        public string Day25 { get; set; }
        public string Day26 { get; set; }
        public string Day27 { get; set; }
        public string Day28 { get; set; }
        public string Day29 { get; set; }
        public string Day30 { get; set; }
        /// <summary>
        /// 31号
        /// </summary>
        public string Day31 { get; set; }
    }
    public class EmployeeStatisticsViewModelAdapter : ViewBaseAdapter<EmployeeStatisticsViewModel, List<EmployeeStatisticsViewModel>>
    {
        private static string ConnectionString = ConnectionNameDefine.EmployeeAttendance;
        public static EmployeeStatisticsViewModelAdapter Instance = new EmployeeStatisticsViewModelAdapter();

        public EmployeeStatisticsViewModelAdapter() : base(ConnectionString)
        {

        }
        /// <summary>
        /// 查询全部员工考勤数据
        /// </summary>
        /// <param name="attendanceMonth">统计月份</param>
        /// <returns></returns>
        public List<EmployeeStatisticsViewModel> GetAllEmployeeAttendance(string attendanceMonth = "")
        {
            //准备有效的人员数据
            string sql = string.Format(@"SELECT DisplayName,LOGON_NAME,FullPath INTO #User FROM YuanXinBusiness.OAuth.Seagull2 WHERE DisplayName NOT LIKE '%已离职%' ;");
            //准备整理后的考勤数据
            sql += string.Format(@"SELECT EnName,CreateDate,MIN(createTime1) AS CreateTime INTO #companySingn FROM (
                                    select CnName, EnName,CreateTime createTime1,CONVERT(nvarchar(100), CreateTime, 111) CreateDate,CreateTime
                                    from dbo.EmployeeServices
                                    ) A
                                    WHERE CreateDate LIKE '{0}%'   --2017/02
                                    GROUP BY EnName,CreateDate", Convert.ToDateTime(attendanceMonth).ToString("yyyy/MM"));
            //准备查询SQL
            sql += string.Format(@"
                                    DECLARE @year INT={0};
                                    DECLARE @month INT={1};
                                    SELECT DisplayName,LOGON_NAME,Min(FullPath) as FullPath,
                                    MAX(day1) day1,
                                    MAX(day2) day2,
                                    MAX(day3) day3,
                                    MAX(day4) day4,
                                    MAX(day5) day5,
                                    MAX(day6) day6,
                                    MAX(day7) day7,
                                    MAX(day8) day8,
                                    MAX(day9) day9,
                                    MAX(day10) day10,
                                    MAX(day11) day11,
                                    MAX(day12) day12,
                                    MAX(day13) day13,
                                    MAX(day14) day14,
                                    MAX(day15) day15,
                                    MAX(day16) day16,
                                    MAX(day17) day17,
                                    MAX(day18) day18,
                                    MAX(day19) day19,
                                    MAX(day20) day20,
                                    MAX(day21) day21,
                                    MAX(day22) day22,
                                    MAX(day23) day23,
                                    MAX(day24) day24,
                                    MAX(day25) day25,
                                    MAX(day26) day26,
                                    MAX(day27) day27,
                                    MAX(day28) day28,
                                    MAX(day29) day29,
                                    MAX(day30) day30,
                                    MAX(day31) day31
                                    FROM (
                                    SELECT sea.DisplayName,sea.LOGON_NAME,sea.FullPath,
                                    CASE WHEN es1.EnName IS NOT NULL THEN 1 ELSE 0 END AS day1,
                                    CASE WHEN es2.EnName IS NOT NULL THEN 1 ELSE 0 END AS day2,
                                    CASE WHEN es3.EnName IS NOT NULL THEN 1 ELSE 0 END AS day3,
                                    CASE WHEN es4.EnName IS NOT NULL THEN 1 ELSE 0 END AS day4,
                                    CASE WHEN es5.EnName IS NOT NULL THEN 1 ELSE 0 END AS day5,
                                    CASE WHEN es6.EnName IS NOT NULL THEN 1 ELSE 0 END AS day6,
                                    CASE WHEN es7.EnName IS NOT NULL THEN 1 ELSE 0 END AS day7,
                                    CASE WHEN es8.EnName IS NOT NULL THEN 1 ELSE 0 END AS day8,
                                    CASE WHEN es9.EnName IS NOT NULL THEN 1 ELSE 0 END AS day9,
                                    CASE WHEN es10.EnName IS NOT NULL THEN 1 ELSE 0 END AS day10,
                                    CASE WHEN es11.EnName IS NOT NULL THEN 1 ELSE 0 END AS day11,
                                    CASE WHEN es12.EnName IS NOT NULL THEN 1 ELSE 0 END AS day12,
                                    CASE WHEN es13.EnName IS NOT NULL THEN 1 ELSE 0 END AS day13,
                                    CASE WHEN es14.EnName IS NOT NULL THEN 1 ELSE 0 END AS day14,
                                    CASE WHEN es15.EnName IS NOT NULL THEN 1 ELSE 0 END AS day15,
                                    CASE WHEN es16.EnName IS NOT NULL THEN 1 ELSE 0 END AS day16,
                                    CASE WHEN es17.EnName IS NOT NULL THEN 1 ELSE 0 END AS day17,
                                    CASE WHEN es18.EnName IS NOT NULL THEN 1 ELSE 0 END AS day18,
                                    CASE WHEN es19.EnName IS NOT NULL THEN 1 ELSE 0 END AS day19,
                                    CASE WHEN es20.EnName IS NOT NULL THEN 1 ELSE 0 END AS day20,
                                    CASE WHEN es21.EnName IS NOT NULL THEN 1 ELSE 0 END AS day21,
                                    CASE WHEN es22.EnName IS NOT NULL THEN 1 ELSE 0 END AS day22,
                                    CASE WHEN es23.EnName IS NOT NULL THEN 1 ELSE 0 END AS day23,
                                    CASE WHEN es24.EnName IS NOT NULL THEN 1 ELSE 0 END AS day24,
                                    CASE WHEN es25.EnName IS NOT NULL THEN 1 ELSE 0 END AS day25,
                                    CASE WHEN es26.EnName IS NOT NULL THEN 1 ELSE 0 END AS day26,
                                    CASE WHEN es27.EnName IS NOT NULL THEN 1 ELSE 0 END AS day27,
                                    CASE WHEN es28.EnName IS NOT NULL THEN 1 ELSE 0 END AS day28,
                                    CASE WHEN es29.EnName IS NOT NULL THEN 1 ELSE 0 END AS day29,
                                    CASE WHEN es30.EnName IS NOT NULL THEN 1 ELSE 0 END AS day30,
                                    CASE WHEN es31.EnName IS NOT NULL THEN 1 ELSE 0 END AS day31
                                    FROM #User  sea
                                    LEFT JOIN #companySingn es1 ON sea.LOGON_NAME=es1.EnName AND YEAR(es1.CreateTime)=@year AND MONTH(es1.CreateTime)=@month AND DAY(es1.CreateTime)=1
                                    LEFT JOIN #companySingn es2 ON sea.LOGON_NAME=es2.EnName AND YEAR(es2.CreateTime)=@year AND MONTH(es2.CreateTime)=@month AND DAY(es2.CreateTime)=2
                                    LEFT JOIN #companySingn es3 ON sea.LOGON_NAME=es3.EnName AND YEAR(es3.CreateTime)=@year AND MONTH(es3.CreateTime)=@month AND DAY(es3.CreateTime)=3
                                    LEFT JOIN #companySingn es4 ON sea.LOGON_NAME=es4.EnName AND YEAR(es4.CreateTime)=@year AND MONTH(es4.CreateTime)=@month AND DAY(es4.CreateTime)=4
                                    LEFT JOIN #companySingn es5 ON sea.LOGON_NAME=es5.EnName AND YEAR(es5.CreateTime)=@year AND MONTH(es5.CreateTime)=@month AND DAY(es5.CreateTime)=5
                                    LEFT JOIN #companySingn es6 ON sea.LOGON_NAME=es6.EnName AND YEAR(es6.CreateTime)=@year AND MONTH(es6.CreateTime)=@month AND DAY(es6.CreateTime)=6
                                    LEFT JOIN #companySingn es7 ON sea.LOGON_NAME=es7.EnName AND YEAR(es7.CreateTime)=@year AND MONTH(es7.CreateTime)=@month AND DAY(es7.CreateTime)=7
                                    LEFT JOIN #companySingn es8 ON sea.LOGON_NAME=es8.EnName AND YEAR(es8.CreateTime)=@year AND MONTH(es8.CreateTime)=@month AND DAY(es8.CreateTime)=8
                                    LEFT JOIN #companySingn es9 ON sea.LOGON_NAME=es9.EnName AND YEAR(es9.CreateTime)=@year AND MONTH(es9.CreateTime)=@month AND DAY(es9.CreateTime)=9
                                    LEFT JOIN #companySingn es10 ON sea.LOGON_NAME=es10.EnName AND YEAR(es10.CreateTime)=@year AND MONTH(es10.CreateTime)=@month AND DAY(es10.CreateTime)=10
                                    LEFT JOIN #companySingn es11 ON sea.LOGON_NAME=es11.EnName AND YEAR(es11.CreateTime)=@year AND MONTH(es11.CreateTime)=@month AND DAY(es11.CreateTime)=11
                                    LEFT JOIN #companySingn es12 ON sea.LOGON_NAME=es12.EnName AND YEAR(es12.CreateTime)=@year AND MONTH(es12.CreateTime)=@month AND DAY(es12.CreateTime)=12
                                    LEFT JOIN #companySingn es13 ON sea.LOGON_NAME=es13.EnName AND YEAR(es13.CreateTime)=@year AND MONTH(es13.CreateTime)=@month AND DAY(es13.CreateTime)=13
                                    LEFT JOIN #companySingn es14 ON sea.LOGON_NAME=es14.EnName AND YEAR(es14.CreateTime)=@year AND MONTH(es14.CreateTime)=@month AND DAY(es14.CreateTime)=14
                                    LEFT JOIN #companySingn es15 ON sea.LOGON_NAME=es15.EnName AND YEAR(es15.CreateTime)=@year AND MONTH(es15.CreateTime)=@month AND DAY(es15.CreateTime)=15
                                    LEFT JOIN #companySingn es16 ON sea.LOGON_NAME=es16.EnName AND YEAR(es16.CreateTime)=@year AND MONTH(es16.CreateTime)=@month AND DAY(es16.CreateTime)=16
                                    LEFT JOIN #companySingn es17 ON sea.LOGON_NAME=es17.EnName AND YEAR(es17.CreateTime)=@year AND MONTH(es17.CreateTime)=@month AND DAY(es17.CreateTime)=17
                                    LEFT JOIN #companySingn es18 ON sea.LOGON_NAME=es18.EnName AND YEAR(es18.CreateTime)=@year AND MONTH(es18.CreateTime)=@month AND DAY(es18.CreateTime)=18
                                    LEFT JOIN #companySingn es19 ON sea.LOGON_NAME=es19.EnName AND YEAR(es19.CreateTime)=@year AND MONTH(es19.CreateTime)=@month AND DAY(es19.CreateTime)=19
                                    LEFT JOIN #companySingn es20 ON sea.LOGON_NAME=es20.EnName AND YEAR(es20.CreateTime)=@year AND MONTH(es20.CreateTime)=@month AND DAY(es20.CreateTime)=20
                                    LEFT JOIN #companySingn es21 ON sea.LOGON_NAME=es21.EnName AND YEAR(es21.CreateTime)=@year AND MONTH(es21.CreateTime)=@month AND DAY(es21.CreateTime)=21
                                    LEFT JOIN #companySingn es22 ON sea.LOGON_NAME=es22.EnName AND YEAR(es22.CreateTime)=@year AND MONTH(es22.CreateTime)=@month AND DAY(es22.CreateTime)=22
                                    LEFT JOIN #companySingn es23 ON sea.LOGON_NAME=es23.EnName AND YEAR(es23.CreateTime)=@year AND MONTH(es23.CreateTime)=@month AND DAY(es23.CreateTime)=23
                                    LEFT JOIN #companySingn es24 ON sea.LOGON_NAME=es24.EnName AND YEAR(es24.CreateTime)=@year AND MONTH(es24.CreateTime)=@month AND DAY(es24.CreateTime)=24
                                    LEFT JOIN #companySingn es25 ON sea.LOGON_NAME=es25.EnName AND YEAR(es25.CreateTime)=@year AND MONTH(es25.CreateTime)=@month AND DAY(es25.CreateTime)=25
                                    LEFT JOIN #companySingn es26 ON sea.LOGON_NAME=es26.EnName AND YEAR(es26.CreateTime)=@year AND MONTH(es26.CreateTime)=@month AND DAY(es26.CreateTime)=26
                                    LEFT JOIN #companySingn es27 ON sea.LOGON_NAME=es27.EnName AND YEAR(es27.CreateTime)=@year AND MONTH(es27.CreateTime)=@month AND DAY(es27.CreateTime)=27
                                    LEFT JOIN #companySingn es28 ON sea.LOGON_NAME=es28.EnName AND YEAR(es28.CreateTime)=@year AND MONTH(es28.CreateTime)=@month AND DAY(es28.CreateTime)=28
                                    LEFT JOIN #companySingn es29 ON sea.LOGON_NAME=es29.EnName AND YEAR(es29.CreateTime)=@year AND MONTH(es29.CreateTime)=@month AND DAY(es29.CreateTime)=29
                                    LEFT JOIN #companySingn es30 ON sea.LOGON_NAME=es30.EnName AND YEAR(es30.CreateTime)=@year AND MONTH(es30.CreateTime)=@month AND DAY(es30.CreateTime)=30
                                    LEFT JOIN #companySingn es31 ON sea.LOGON_NAME=es31.EnName AND YEAR(es31.CreateTime)=@year AND MONTH(es31.CreateTime)=@month AND DAY(es31.CreateTime)=31
                                    ) A GROUP BY DisplayName,LOGON_NAME
                                ", Convert.ToDateTime(attendanceMonth).Year, Convert.ToDateTime(attendanceMonth).Month);
            //删除临时表
            sql += string.Format(@"DROP TABLE #companySingn;
                                   DROP TABLE #User");


            List<EmployeeStatisticsViewModel> dataList = LoadTColl(sql);

            return dataList;
        }
    }
    #endregion

}
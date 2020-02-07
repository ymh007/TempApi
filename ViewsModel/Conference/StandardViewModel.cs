using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// 考勤打卡地点视图
    /// </summary>
    public class StandardViewModel
    {
        /// <summary>
        /// 打卡地点编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string OnTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public string OffTime { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Lng { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Lat { get; set; }
        /// <summary>
        /// 打卡地点
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 有效状态
        /// </summary>
        public bool ValidStatus { get; set; }

    }

    /// <summary>
    /// 考勤打卡地点操作
    /// </summary>
    public class StandardViewModelAdapter : ViewBaseAdapter<StandardViewModel, List<StandardViewModel>>
    {

        private static string ConnectionString = ConnectionNameDefine.EmployeeAttendance;
        public static StandardViewModelAdapter Instance = new StandardViewModelAdapter();

        public StandardViewModelAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 分页查询考勤打卡地点
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="searchTime">查询时间</param>
        /// <param name="signAddress">地点名称</param>
        /// <returns></returns>
        public ViewPageBase<List<StandardViewModel>> GetStandardDataViewByPage(int pageIndex, DateTime searchTime, string signAddress = "")
        {
            if (pageIndex == 1)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "SELECT * ";
            string fromAndWhereSQL = string.Format(@"from dbo.StandardPunch where ValidStatus =1 and  Address like '%" + signAddress + "%'");


            string orderSQL = "ORDER BY Address DESC,CreateTime DESC";
            ViewPageBase<List<StandardViewModel>> result = LoadViewModelCollByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);

            result.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return result;

        }
    }
}
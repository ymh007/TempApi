using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel
{
    /// <summary>
    /// 手机端会议工作人员 ViewModel
    /// </summary>
    public class ConferenceWorkerAppViewModel
    {
        /// <summary>
        /// 人员编码
        /// </summary>
        public string Code { set; get; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar
        {
            get
            {
                return UserHeadPhotoService.GetUserHeadPhoto(Code);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { set; get; }
    }

    /// <summary>
    /// 人员类型实体（包括人员列表）
    /// </summary>
    public class WorkerViewListModel
    {
        /// <summary>
        /// 类型编码
        /// </summary>
        public string WorkerTypeID { get; set; }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string WorkerTypeName { get; set; }
        /// <summary>
        /// 人员列表
        /// </summary>
        public List<WorkerList> WorkerListData = new List<WorkerList>();
    }

    /// <summary>
    /// 人员列表实体
    /// </summary>
    public class WorkerList
    {
        /// <summary>
        /// 自动编号
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 人员Code
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 人员电话
        /// </summary>
        public string UserTelPhone { get; set; }
        /// <summary>
        /// 人员头像 
        /// </summary>
        public string UserPhotoAddress { get; set; }
    }

    /// <summary>
    /// 获取人员类型列表数据适配器
    /// </summary>
    public class WorkerViewListModelAdapter : ViewBaseAdapter<WorkerViewListModel, List<WorkerViewListModel>>
    {
        private static string ConnectionString = "yuanxin";

        /// <summary>
        /// 实例
        /// </summary>
        public static new WorkerViewListModelAdapter Instance = new WorkerViewListModelAdapter();

        /// <summary>
        /// 构造
        /// </summary>
        public WorkerViewListModelAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 获取人员类型列表
        /// </summary>
        public List<WorkerViewListModel> GetWorkerViewListModelByPage(string conferenceID)
        {
            var sql = @"SELECT [ID] AS [WorkerTypeID], [Name] AS [WorkerTypeName]
                        FROM [office].[WorkerType]
                        WHERE [Name] IN (SELECT WorkerTypeName FROM [office].[Worker] WHERE [ConferenceID] = '{0}')
                        ORDER BY [Sort] ASC";
            sql = string.Format(sql, conferenceID);

            return LoadTColl(sql);
        }
    }

    /// <summary>
    /// 获取人员列表Adapter
    /// </summary>
    public class WorkerListAdapter : ViewBaseAdapter<WorkerList, List<WorkerList>>
    {
        private static string ConnectionString = "yuanxin";

        /// <summary>
        /// 实例
        /// </summary>
        public static new WorkerListAdapter Instance = new WorkerListAdapter();

        /// <summary>
        /// 构造
        /// </summary>
        public WorkerListAdapter() : base(ConnectionString)
        {

        }

        /// <summary>
        /// 获取人员列表
        /// </summary>
        public List<WorkerList> GetWorkerList(string ConferenceID, string WorkerTypeID)
        {
            var sql = @"SELECT [B].[ID], [B].[UserName], [B].[UserTelPhone], [B].[UserID], [B].[UserPhotoAddress]
                        FROM [office].[Worker] B LEFT JOIN [office].[WorkerType] A ON [A].[ID] = [B].[WorkerTypeName]
                        WHERE [B].[ConferenceID] = '{0}' AND [B].[WorkerTypeName] = '{1}'
                        ORDER BY [B].[Sort] ASC";
            sql = string.Format(sql, ConferenceID, WorkerTypeID);

            return LoadTColl(sql);
        }
    }
}
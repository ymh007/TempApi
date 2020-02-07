using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Services
{
    /// <summary>
    /// 流程实体 用户返回给APP端
    /// </summary>
    public class UserTaskModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("TASK_GUID")]
        public string TaskID { get; set; }

        /// <summary>
        /// 指引名称
        /// </summary>
        [ORFieldMapping("PROGRAM_NAME")]
        public string ProgramName { set; get; }

        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("TASK_TITLE")]
        public string TaskTitle { get; set; }

        /// <summary>
        /// 资源ID
        /// </summary>
        [ORFieldMapping("RESOURCE_ID")]
        public string ResourceID { get; set; }

        /// <summary>
        /// 流程ID
        /// </summary>
        [ORFieldMapping("PROCESS_ID")]
        public string ProcessID { get; set; }

        /// <summary>
        /// 活动编码
        /// </summary>
        [ORFieldMapping("ACTIVITY_ID")]
        public string ActivityID { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [ORFieldMapping("URL")]
        public string Url { get; set; }

        /// <summary>
        /// 接收人编码
        /// </summary>
        [ORFieldMapping("SEND_TO_USER")]
        public string SendToUser { get; set; }

        /// <summary>
        /// 阅读时间
        /// </summary>
        [ORFieldMapping("READ_TIME")]
        public DateTime ReadTime { get; set; }

        /// <summary>
        /// 交付时间（消息的发送时间）
        /// </summary>
        [ORFieldMapping("DELIVER_TIME")]
        public DateTime DeliverTime { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        [ORFieldMapping("COMPLETED_TIME")]
        public DateTime CompletedTime { get; set; }

        /// <summary>
        /// 是否收藏（1=已收藏）
        /// </summary>
        [ORFieldMapping("COLLECTION")]
        public int Collection { set; get; }

        /// <summary>
        /// 类型（1=待办、2和4=传阅和通知、Running=流转中、Completed=已办结）
        /// </summary>
        [ORFieldMapping("Type")]
        public string Type { set; get; }

        /// <summary>
        /// 是否新平台 老平台需要带Ticket
        /// </summary>
        public bool IsArchitecture { get; set; }

        /// <summary>
        /// 是否在移动端上线
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 搁置时间
        /// </summary>
        public string TotalTime
        {
            get
            {
                try
                {
                    if (DeliverTime == DateTime.MinValue)
                    {
                        return string.Empty;
                    }
                    var time = DateTime.Now - DeliverTime;
                    if (time.TotalSeconds < 60)
                    {
                        return time.Seconds + " 秒";
                    }
                    if (time.TotalMinutes < 60)
                    {
                        return time.Minutes + " 分 " + time.Seconds + " 秒";
                    }
                    if (time.TotalHours < 24)
                    {
                        return time.Hours + " 小时 " + time.Minutes + " 分";
                    }
                    if (time.TotalDays < 30)
                    {
                        return time.Days + " 天 " + time.Hours + " 小时";
                    }
                    if (time.TotalDays < 365)
                    {
                        return time.Days / 30 + " 个月 " + time.Days % 30 + " 天";
                    }
                    else
                    {
                        return time.Days / 365 + " 年 " + time.Days % 365 / 30 + " 个月";
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

    /// <summary>
    /// 获取用户待办数量响应实体
    /// </summary>
    public class GetUserTaskCountModel
    {
        /// <summary>
        /// 待办
        /// </summary>
        public int UnCompletedTaskNum { set; get; }
        /// <summary>
        /// 传阅和通知
        /// </summary>
        public int NoticeAndCirculationNum { set; get; }
        /// <summary>
        /// 流转中
        /// </summary>
        public int TransferTaskNum { set; get; }
        /// <summary>
        /// 已办结
        /// </summary>
        public int CompletedTask { set; get; }
    }

    /// <summary>
    /// 获取流程列表请求实体
    /// </summary>
    public class GetUserTaskPostModel
    {
        /// <summary>
        /// 排序
        /// </summary>
        public string BySort { get { return ""; } }
        /// <summary>
        /// 是否倒序
        /// </summary>
        public string IsDesc { get { return " desc "; } }
        /// <summary>
        /// 标题关键字
        /// </summary>
        public string TaskTitle { set; get; }
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageDataModel PageData { set; get; }
    }

    /// <summary>
    /// 获取流程列表响应实体
    /// </summary>
    public class GetUserTaskModel
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageDataModel PaginationInfo { set; get; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalItems { set; get; }
        /// <summary>
        /// 数据信息
        /// </summary>
        public List<GetUserTaskInfoModel> Data { set; get; }
        /// <summary>
        /// 数据转换
        /// </summary>
        public List<UserTaskModel> ToUserTaskList()
        {
            string urlMappingNG = ConfigurationManager.AppSettings["UrlMappingNG"];
            string whiteDomain = ConfigurationManager.AppSettings["WhiteDomain"];
            List<string> whiteDomains = whiteDomain.Split('|').ToList();
            var view = new List<UserTaskModel>();
            this.Data.ForEach(item =>
            {
                if (string.IsNullOrWhiteSpace(item.OriginalUrl))
                {
                    return;
                }
                // 屏蔽笔记本设备待办
                if (item.OriginalUrl.ToLower().Contains("OfficeSpace/default.htm#/ComputerApplyPrepare".ToLower()))
                {
                    return;
                }
                var isNewPlatform = false;
                item.OriginalUrl = item.OriginalUrl.Replace(" ", "");
                if (item.OriginalUrl.ToLower().Contains("thrwebapp"))
                {
                    item.OriginalUrl = urlMappingNG + item.OriginalUrl;
                    isNewPlatform = true;
                }
                whiteDomains.ForEach(f=> {
                    if (item.OriginalUrl.ToLower().Contains(f)) {
                        isNewPlatform = true;
                        return;
                    }
                });
                view.Add(new UserTaskModel
                {
                    TaskID = item.TaskId,
                    ProgramName = item.ProgramName,
                    TaskTitle = item.Title,
                    ResourceID = item.ResourceId,
                    ProcessID = item.ProcessId,
                    ActivityID = item.ActivityId,
                    Url = item.OriginalUrl,
                    ReadTime = item.ReadTime ?? DateTime.MinValue,
                    DeliverTime = item.DeliverTime ?? DateTime.MinValue,
                    Collection = item.Collection ?? 0,
                    Enabled = isNewPlatform,
                    IsArchitecture = isNewPlatform,
                    SendToUser = item.SendToUser,
                    Type = item.Type,
                });
            });
            return UrlConvertHelper.Convert(view);
        }
    }

    /// <summary>
    /// 流程实体 用于接收海鸥二返回数据（单个流程数据）
    /// </summary>
    public class GetUserTaskInfoModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string TaskId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ProgramName { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ResourceId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ProcessId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityId { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string OriginalUrl { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ReadTime { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeliverTime { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public int? Collection { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string SendToUser { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Type { set; get; }
    }

    /// <summary>
    /// 分页实体
    /// </summary>
    public class PageDataModel
    {
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { set; get; }
        /// <summary>
        /// 页索引
        /// </summary>
        public int PageIndex { set; get; }
    }

    /// <summary>
    /// 搜索 1
    /// </summary>
    public class SearchModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool State { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public SearchDataModel Data { set; get; }
    }

    /// <summary>
    /// 搜索 2
    /// </summary>
    public class SearchDataModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SearchDataItem Task { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SearchDataItem Running { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SearchDataItem Completed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SearchDataItem Notice { get; set; }
    }

    /// <summary>
    /// 搜索 3
    /// </summary>
    public class SearchDataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public GetUserTaskModel Data { set; get; }
    }

    /// <summary>
    /// 收藏
    /// </summary>
    public class CollectionModel
    {
        /// <summary>
        /// 流程编码
        /// </summary>
        public string TaskCode { set; get; }
        /// <summary>
        /// 状态 0=取消收藏 1=添加收藏
        /// </summary>
        public int Status { set; get; }
    }
}
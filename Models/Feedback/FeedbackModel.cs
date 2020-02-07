using System;
using System.Collections.Generic;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ViewsModel.Feedback;

namespace Seagull2.YuanXin.AppApi.Models.Feedback
{
    /// <summary>
    /// 意见反馈 Model
    /// </summary>
    [ORTableMapping("office.Feedback")]
    public class FeedbackModel : BaseModel
    {
        /// <summary>
        /// 操作系统
        /// </summary>
        [ORFieldMapping("System")]
        public string System { get; set; }

        /// <summary>
        /// 网络类型
        /// </summary>
        [ORFieldMapping("Network")]
        public string Network { get; set; }

        /// <summary>
        /// 手机型号
        /// </summary>
        [ORFieldMapping("Version")]
        public string Version { get; set; }

		/// <summary>
		/// APP版本
		/// </summary>
		[ORFieldMapping("AppVersion")]
		public string AppVersion { set; get; }

		/// <summary>
		/// 反馈内容
		/// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 是否回复 // 替代新的功能 : 新消息标识
        /// </summary>
        [ORFieldMapping("IsReply")]
        public bool IsReply { set; get; }

        /// <summary>
        /// 回复人编码
        /// </summary>
        [ORFieldMapping("ReplyUserCode")]
        public string ReplyUserCode { set; get; }

        /// <summary>
        /// 回复人名称
        /// </summary>
        [ORFieldMapping("ReplyUserName")]
        public string ReplyUserName { set; get; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [ORFieldMapping("ReplyContent")]
        public string ReplyContent { set; get; }

        /// <summary>
        /// 回复时间
        /// </summary>
        [ORFieldMapping("ReplyDateTime")]
        public DateTime ReplyDateTime { set; get; }

        /// <summary>
        /// 回复设备 1 app  0 pc 
        /// </summary>
        [ORFieldMapping("ReplyWay")]
        public int ReplyWay { get; set; }

        /// <summary>
        /// 是否标记
        /// </summary>
        [ORFieldMapping("Mark")]
        public bool Mark { get; set; }
    }



    /// <summary>
    /// 意见反馈 Model
    /// </summary>
    [ORTableMapping("office.FeedBackReply")]
    public class FeedbackReply
    {

        /// <summary>
        ///  编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { set; get; }

        /// <summary>
        /// 意见反馈编码
        /// </summary>
        [ORFieldMapping("FeedbackCode")]
        public string FeedbackCode { set; get; }


        /// <summary>
        /// 回复设备 1 app  0 pc 
        /// </summary>
        [ORFieldMapping("ReplyWay")]
        public int ReplyWay { get; set; }

        /// <summary>
        /// 回复人编码
        /// </summary>
        [ORFieldMapping("ReplyUserCode")]
        public string ReplyUserCode { set; get; }


        /// <summary>
        /// 回复人名称
        /// </summary>
        [ORFieldMapping("ReplyUserName")]
        public string ReplyUserName { set; get; }

        /// <summary>
        /// 回复内容
        /// </summary>
        [ORFieldMapping("ReplyContent")]
        public string ReplyContent { set; get; }

        /// <summary>
        /// 回复时间
        /// </summary>
        [ORFieldMapping("ReplyDateTime")]
        public DateTime ReplyDateTime { set; get; }
    }


    /// <summary>
    /// 意见反馈 Collection
    /// </summary>
    public class FeedbackCollection : EditableDataObjectCollectionBase<FeedbackModel>
    {
        /// <summary>
        /// 将 FeedbackCollection 转换为 FeedbackViewModel
        /// </summary>
        /// <returns></returns>
        public List<FeedbackViewModel> ToFeedbackViewModel()
        {
            var dataNew = new List<FeedbackViewModel>();
            ForEach(data =>
            {
                dataNew.Add(new FeedbackViewModel()
                {
                    Code = data.Code,
                    System = data.System,
                    Network = data.Network,
                    Version = data.Version,
                    Content = data.Content,
                    AppVersion = data.AppVersion,
                    DateTime = data.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserCode = data.UserCode,
                    UserName = data.UserName,
                    IsReply = data.IsReply,
                    ReplyUserCode = data.ReplyUserCode,
                    ReplyUserName = data.ReplyUserName,
                    ReplyContent = data.Content, // 部署后删除掉
                    ReplyWay = data.ReplyWay,
                    Mark = data.Mark,
                    ReplyDateTime = data.CreateTime.ToString("yyyy-MM-dd HH:mm:ss") //部署后删除掉
                });
            });
            return dataNew;
        }
    }


    /// <summary>
    /// 意见反馈回复 Collection
    /// </summary>
    public class FeedbackReplyCollection : EditableDataObjectCollectionBase<FeedbackReply>
    {
    }
}
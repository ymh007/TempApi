using System;
using System.Collections.Generic;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Notification
{
    /// <summary>
    /// 通知纪要实体
    /// </summary>
    public class NotificationsModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [NoMapping]
        public string Title { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        [NoMapping]
        public string PublishPeople { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        [NoMapping]
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 发布单位
        /// </summary>
        [NoMapping]
        public string PublishUnit { get; set; }

        /// <summary>
        /// 会议类型
        /// </summary>
        [NoMapping]
        public string Type { get; set; }

        public List<string> SrcAddress = new List<string>();
        
        public string Url { get; set; }

        public String Id { get; set; }
        public String ListId { get; set; }
        public String WebId { get; set; }
        public String AttachMent { get; set; }
        public string ShortOrganisation { get; set; }
        public List<string> Attachments = new List<string>();
        public String AttachmentStr { get; set; }
        public string Author { get; set; }

        public string WebName { get; set; }

        public string Body1 { get; set; }
        public string ResourceId { get; set; }
        public string CreateTimeString { get; set; }

        public string MossNoticeType { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { set; get; }
    }

    /// <summary>
    /// 通知纪要集合
    /// </summary>
    [Serializable]
    public class NotificationsCollection : EditableDataObjectCollectionBase<NotificationsModel>
    {

    }
}
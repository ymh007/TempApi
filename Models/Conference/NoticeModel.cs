using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会议通告
    /// </summary>
    [ORTableMapping("office.ConferenceNotice")]
    public class NoticeModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 会议编码
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }
        /// <summary>
        /// 通告内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }
        /// <summary>
        /// 通告时间
        /// </summary>
        [ORFieldMapping("PublicDate")]
        public DateTime PublicDate { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NoticeCollection : EditableDataObjectCollectionBase<NoticeModel>
    {

    }
}
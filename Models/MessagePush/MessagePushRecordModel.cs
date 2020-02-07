using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;

namespace Seagull2.YuanXin.AppApi.Models.MessagePush
{
    /// <summary>
    /// 消息推送记录 Model
    /// </summary>
    [ORTableMapping("[office].[MessagePushRecord]")]
    public class MessagePushRecordModel : BaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 群组Code
        /// </summary>
        [ORFieldMapping("PushGroupCode")]
        public string PushGroupCode { get; set; }

        /// <summary>
        /// 群组Name
        /// </summary>
        [ORFieldMapping("PushGroupName")]
        public string PushGroupName { get; set; }

        /// <summary>
        /// 推送内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 定时发送时间
        /// </summary>
        [ORFieldMapping("TimingSend")]
        public DateTime? TimingSend { get; set; }

        /// <summary>
        /// 是否定时发送
        /// </summary>
        [ORFieldMapping("IsTiming")]
        public bool IsTiming { get; set; }

        /// <summary>
        /// 发送状态  1 成功  2 失败  3 待发送 4  已撤销
        /// </summary>
        [ORFieldMapping("SendStatus")]
        public int SendStatus { get; set; }

        /// <summary>
        /// 添加数据时候记得加上备注，已备后面更改人员观看
        /// 数据源类型  1 消息管理  2.会议通告 
        /// </summary>
        [ORFieldMapping("SourceType")]
        public string SourceType { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [ORFieldMapping("MsgType")]
        public int MsgType { get; set; }
        /// <summary>
        /// 消息图片url
        /// </summary>
        [ORFieldMapping("ImgUrl")]
        public string ImgUrl { get; set; }

        /// <summary>
        /// 消息图片url
        /// </summary>
        [ORFieldMapping("ImgTitle")]
        public string ImgTitle { get; set; }

        /// <summary>
        /// 点击事件类型
        /// </summary>
        [ORFieldMapping("EventType")]
        public int EventType { get; set; }

    }

    /// <summary>
    /// 消息推送记录 Collection
    /// </summary>
    public class MessagePushRecordCollection : EditableDataObjectCollectionBase<MessagePushRecordModel>
	{

	}
}
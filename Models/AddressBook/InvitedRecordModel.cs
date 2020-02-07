using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook
{
    [ORTableMapping("[dbo].[InvitedRecord]")]
    public class InvitedRecordModel
    {

        /// <summary>
        /// code
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 对象的类型
        /// </summary>
        public string SendContent { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// 接收人Code
        /// </summary>
        public string RecipientCode { set; get; }
        

        /// <summary>
        /// 发送人
        /// </summary>
        public string SenderCode { set; get; }

        /// <summary>
        /// 接收人姓名
        /// </summary>
        public string RecipientName { set; get; }

      
    }

    public class InvitedRecordCollection : EditableDataObjectCollectionBase<InvitedRecordModel>
    {

    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models.Conference
{
    /// <summary>
    /// 会议企业会话表
    /// </summary>
    [ORTableMapping("office.ConferenceCompanyDialog")]
    public class ConferenceCompanyDialog
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 会议ID
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }
        /// <summary>
        /// 会话ID
        /// </summary>
        [ORFieldMapping("DialogCode")]
        public string DialogCode { get; set; }
        /// <summary>
        /// 会话类型ID（1--IM、2--WeiXin）
        /// </summary>
        [ORFieldMapping("DialogTypeID")]
        public string DialogTypeID { get; set; }
        /// <summary>
        /// 会话类型名称（1--IM、2--WeiXin）
        /// </summary>
        [NoMapping]
        public string DialogTypeName
        {
            get
            {
                return DialogTypeAdapter.Instance.GetTByID(this.DialogTypeID).Name;
            }
        }
        /// <summary>
        /// 会话内容类型ID（1--现场服务、2--预定车辆、3--会议交流）
        /// </summary>
        [ORFieldMapping("DialogContentTypeID")]
        public string DialogContentTypeID { get; set; }
        /// <summary>
        /// 会话内容类型名称（1--现场服务、2--预定车辆、3--会议交流）
        /// </summary>
        [NoMapping]
        public string DialogContentTypeName
        {
            get
            {
                return DialogContentTypeAdapter.Instance.GetTByID(this.DialogContentTypeID).Name;
            }
        }
    }

    /// <summary>
    /// 会话集合
    /// </summary>
    public class ConferenceCompanyDialogCollection : EditableDataObjectCollectionBase<ConferenceCompanyDialog>
    {

    }
}
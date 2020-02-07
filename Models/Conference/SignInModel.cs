using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Enum;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi
{
    /// <summary>
    /// 签到
    /// </summary>
    [ORTableMapping("Office.SignDetail")]
    public class SignInModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 议程ID
        /// </summary>
        [ORFieldMapping("AgendaID")]
        public string AgendaID { get; set; }

        /// <summary>
        /// 参会人ID
        /// </summary>
        [ORFieldMapping("AttendeeID")]
        public string AttendeeID { get; set; }

        /// <summary>
        /// 签到来源类型
        /// </summary>
        [ORFieldMapping("SignSourceType")]
        public EnumSignSourceType SignSourceType { get; set; }

        /// <summary>
        /// 签到来源类型名称
        /// </summary>
        [NoMapping]
        public string SignSourceTypeName
        {
            get
            {
                return EnumItemDescriptionAttribute.GetDescription(this.SignSourceType);
            }
        }

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
        /// <summary>
        /// 签到时间
        /// </summary>
        [ORFieldMapping("SignDate")]
        public DateTime SignDate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SignInModelCollection : EditableDataObjectCollectionBase<SignInModel>
    {
    }
}
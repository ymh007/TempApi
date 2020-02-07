using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.ActivityNew
{

    /// <summary>
    /// 活动报名
    /// </summary>
    [ORTableMapping("office.ActivityApplyInfo")]
    public class ActivityApplyInfoModel : BaseModel
    {
        /// <summary>
        /// 活动编码
        /// </summary>
        [ORFieldMapping("ActivityCode")]
        public string ActivityCode { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
         
        /// <summary>
        /// 电话号码
        /// </summary>
        [ORFieldMapping("PhoneNumber")]
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// 活动报名集合
    /// </summary>
    public class ActivityApplyInfoCollection : EditableDataObjectCollectionBase<ActivityApplyInfoModel>
    {

    }
}
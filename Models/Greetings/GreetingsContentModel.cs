using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Greetings
{
    /// <summary>
    /// 问候语内容
    /// </summary>
    [ORTableMapping("office.GreetingsContent")]
    public class GreetingsContentModel :BaseModel
    {
        /// <summary>
        /// 问候语编码
        /// </summary>
        [ORFieldMapping("GreetingsCode")]
        public string GreetingsCode { get; set; }

        /// <summary>
        /// 问候语内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GreetingsContentCollection : EditableDataObjectCollectionBase<GreetingsContentModel>
    {


    }
}
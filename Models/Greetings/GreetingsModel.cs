using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Greetings
{
    /// <summary>
    /// 问候语
    /// </summary>
    [ORTableMapping("office.Greetings")]
    public class GreetingsModel : BaseModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("TitleType")]
        public string TitleType { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        [ORFieldMapping("Time")]
        public string Time { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class GreetingsCollection : EditableDataObjectCollectionBase<GreetingsModel>
    {


    }
}
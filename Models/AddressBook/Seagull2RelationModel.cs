using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.AddressBook
{
    /// <summary>
    /// 
    /// </summary>
    [ORTableMapping("[OAuth].[Seagull2Relation]")]
    public class Seagull2RelationModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Id", PrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        [ORFieldMapping("UserId")]
        public string UserId { get; set; }

        
    }

    /// <summary>
    /// 
    /// </summary>
    public class Seagull2RelationModelCollection : EditableDataObjectCollectionBase<Seagull2RelationModel> { }
}
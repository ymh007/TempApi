using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models.Test
{
    /// <summary>
    /// Test实体
    /// </summary>
    [ORTableMapping("office.Test")]
    public class TestModel
    {
        /// <summary>
        /// test id
        /// </summary>
        [ORFieldMapping("id",PrimaryKey =true)]
        public string id { get; set; }

        /// <summary>
        /// test name
        /// </summary>
        [ORFieldMapping("name")]
        public string name { get; set; }

        /// <summary>
        /// test url
        /// </summary>
        [ORFieldMapping("url")]
        public string url { get; set; }

        /// <summary>
        /// test name
        /// </summary>
        [ORFieldMapping("alexa")]
        public int alexa { get; set; }


        /// <summary>
        /// test name
        /// </summary>
        [ORFieldMapping("country")]
        public string country { get; set; }


    }

    /// <summary>
    /// Test集合
    /// </summary>
    public class TestCollection : EditableDataObjectCollectionBase<TestModel>
    {

    }
}
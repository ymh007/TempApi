using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Test
{
    /// <summary>
    /// 测试-Adapter
    /// </summary>
    public class TestAdapter : UpdatableAndLoadableAdapterBase<TestModel, TestCollection>
    {

        /// <summary>
		/// 实例
		/// </summary>
		public static readonly TestAdapter Instance = new TestAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName() => Models.ConnectionNameDefine.YuanXinBusiness;


    }
}
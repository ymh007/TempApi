using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.ManagementReport;

namespace Seagull2.YuanXin.AppApi.Adapter.ManagementReport
{
    /// <summary>
    /// 评论Adapter
    /// </summary>
    public class CommentAdapter : UpdatableAndLoadableAdapterBase<CommentModel, CommentCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static CommentAdapter Instance = new CommentAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.SinooceanLandAddressList;
        }
    }
}
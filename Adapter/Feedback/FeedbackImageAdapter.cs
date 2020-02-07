using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Feedback;

namespace Seagull2.YuanXin.AppApi.Adapter.Feedback
{
    /// <summary>
    /// 意见反馈图片 Adapter
    /// </summary>
    public class FeedbackImageAdapter : UpdatableAndLoadableAdapterBase<FeedbackImageModel, FeedbackImageCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly FeedbackImageAdapter Instance = new FeedbackImageAdapter();

        /// <summary>
        /// 数据库连接名称
        /// </summary>
        protected override string GetConnectionName()
        {
            return Models.ConnectionNameDefine.YuanXinBusiness;
        }
    }
}
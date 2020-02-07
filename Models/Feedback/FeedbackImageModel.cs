using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models.Feedback
{
    /// <summary>
    /// 意见反馈图片 Model
    /// </summary>
    [Serializable]
    [ORTableMapping("office.FeedbackImage")]
    public class FeedbackImageModel : BaseModel
    {
        /// <summary>
        /// 反馈编码
        /// </summary>
        [ORFieldMapping("FeedbackCode")]
        public string FeedbackCode { set; get; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [ORFieldMapping("ImageUrl")]
        public string ImageUrl { set; get; }
    }

    /// <summary>
    /// 意见反馈图片 Collection
    /// </summary>
    [Serializable]
    public class FeedbackImageCollection : EditableDataObjectCollectionBase<FeedbackImageModel>
    {

    }
}
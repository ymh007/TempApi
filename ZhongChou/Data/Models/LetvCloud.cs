using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 活动场次
    /// </summary>
    [Serializable]
    [ORTableMapping("LetvCloud.LiveVideo")]
    public class LiveVideo
    {
        /// <summary>
        /// 直播数据编码
        /// </summary>
        [ORFieldMapping("Id", PrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 流编码
        /// </summary>
        [ORFieldMapping("StreamName")]
        public string StreamName { get; set; }

        /// <summary>
        /// 视频名称
        /// </summary>
        [ORFieldMapping("VideoName")]
        public string VideoName { get; set; }

        /// <summary>
        /// RTMP地址
        /// </summary>
        [ORFieldMapping("RTMP")]
        public string RTMP { get; set; }

        /// <summary>
        /// HLS地址
        /// </summary>
        [ORFieldMapping("HLS")]
        public string HLS { get; set; }

        /// <summary>
        /// FLV地址
        /// </summary>
        [ORFieldMapping("FLV")]
        public string FLV { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }
   
        /// <summary>
        /// 
        /// </summary>
        [ORFieldMapping("IsRecord")]
        public bool IsRecord { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [ORFieldMapping("BeginTime")]
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [ORFieldMapping("EndTime")]
        public DateTime EndTime { get; set; }
    }
     /// <summary>
    /// 活动直播集合
    /// </summary>
    [Serializable]
    public class LiveVideoCollection : EditableDataObjectCollectionBase<LiveVideo>
    {
    }

    /// <summary>
    /// 活动直播操作类
    /// </summary>
    public class LiveVideoAdapter : UpdatableAndLoadableAdapterBase<LiveVideo, LiveVideoCollection>
    {
        public static readonly LiveVideoAdapter Instance = new LiveVideoAdapter();

        private LiveVideoAdapter()
        {

        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        /// <summary>
        /// 加载活动Code
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public LiveVideo LoadByBroadCastCode(string Code)
        {
            return this.Load(where =>
            {
                where.AppendItem("StreamName", Code);
            }).OrderByDescending((item)=>item.BeginTime).FirstOrDefault();
        }
    }
      
}

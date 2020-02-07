using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会议详情
    /// </summary>
    [ORTableMapping("office.BannerConfig")]
    public class BannerConfig
    {
        /// <summary>
        /// 来源编码
        /// </summary>
        [ORFieldMapping("SourceId", PrimaryKey = true)]
        public string SourceId { get; set; }
        /// <summary>
        /// 来源名称
        /// </summary>
        [NoMapping]
        public string SourceName
        {
            get
            {
                string result = "";
                if (this.Type == EnumBannerType.Conference.GetHashCode())
                {
                    ConferenceModel model = Adapter.Conference.ConferenceAdapter.Instance.GetTByID(this.SourceId);
                    result = model == null ? "" : model.Name;
                }
                else if (this.Type == EnumBannerType.Topic.GetHashCode())
                {
                    TopicModel model = TopicModelAdapter.Instance.LoadByCode(this.SourceId);
                    result = model == null ? "" : model.Title;
                }
                return result;
            }
        }
        /// <summary>
        /// 图片路径
        /// </summary>
        [ORFieldMapping("ImageUrl")]
        public string ImageUrl { get; set; }
        /// <summary>
        /// 轮播类型
        /// </summary>
        [ORFieldMapping("Type")]
        public int Type { get; set; }
        /// <summary>
        /// 轮播类型名称
        /// </summary>
        [NoMapping]
        public string TypeName
        {
            get
            {
                string result = EnumItemDescriptionAttribute.GetDescription((EnumBannerType)this.Type);
                return result;
            }
        }
        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 置顶时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }
        /// <summary>
        /// 置顶时间Str
        /// </summary>
        [NoMapping]
        public string CreateTimeStr
        {
            get
            {
                return this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
    public class BannerConfigCollection : EditableDataObjectCollectionBase<BannerConfig> { }
}
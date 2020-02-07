using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects; 
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 活动直播
    /// </summary>
    [Serializable]
    [ORTableMapping("Business.ActivityBroadcast")]
    public class ActivityBroadcast
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 所属项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 直播名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        [ORFieldMapping("StartTime")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 直播封面图片
        /// </summary>
        [ORFieldMapping("CoverImg")]
        public string CoverImg { get; set; }

        /// <summary>
        /// 预告详情
        /// </summary>
        [ORFieldMapping("Detail")]
        public string Detail { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }


        [NoMapping]
        public OAuthUserInfo OAuthUserInfo
        {
            get {
                return OAuthUserInfoAdapter.Instance.LoadByUserCode(this.Creator);
            }
        }
    }
    /// <summary>
    /// 活动直播集合
    /// </summary>
    [Serializable]
    public class ActivityBroadcastCollection : EditableDataObjectCollectionBase<ActivityBroadcast>
    {
    }

    /// <summary>
    /// 活动直播操作类
    /// </summary>
    public class ActivityBroadcastAdapter : UpdatableAndLoadableAdapterBase<ActivityBroadcast, ActivityBroadcastCollection>
    {
        public static readonly ActivityBroadcastAdapter Instance = new ActivityBroadcastAdapter();

        private ActivityBroadcastAdapter()
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
        public ActivityBroadcast LoadByProjectCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
            }, order => {
                order.AppendItem("StartTime", FieldSortDirection.Descending);
            }).FirstOrDefault();
        }
      
    }

}

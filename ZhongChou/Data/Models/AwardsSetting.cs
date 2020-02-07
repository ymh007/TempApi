using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 奖项设置
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.AwardsSetting")]
    public class AwardsSetting
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 众筹项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 奖项名称
        /// </summary>
        [ORFieldMapping("AwardsName")]
        public string AwardsName { get; set; }

        /// <summary>
        /// 起始名次
        /// </summary>
        [ORFieldMapping("StartRanking")]
        public int StartRanking { get; set; }

        /// <summary>
        /// 结束名次
        /// </summary>
        [ORFieldMapping("StopRanking")]
        public int StopRanking { get; set; }

        /// <summary>
        /// 获奖数量
        /// </summary>
        [ORFieldMapping("AwardsCount")]
        public int AwardsCount { get; set; }

        /// <summary>
        /// 奖项内容
        /// </summary>
        [ORFieldMapping("AwardsContent")]
        public string AwardsContent { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }
        /// <summary>
        /// 获奖数量
        /// </summary>
        [NoMapping]
        public int GetAwarsCount {
            get {
                return GetAwardsList.Count;
            }
        }
        /// <summary>
        /// 获得该奖的作品
        /// </summary>
        [NoMapping]
        public WorksAwardsCollection GetAwardsList { 
            get {
                return WorksAwardsAdapter.Instance.LoadByAwardsSettingCode(Code);
            }
        }

        /// <summary>
        /// 奖项图片附件
        /// </summary>
        [NoMapping]
        public AttachmentCollection AwardImages
        {
            get
            {
                return AttachmentAdapter.Instance.LoadByResourceID(this.Code);
            }
        }
    }

    /// <summary>
    /// 奖项设置集合
    /// </summary>
    [Serializable]
    public class AwardsSettingCollection : EditableDataObjectCollectionBase<AwardsSetting>
    {
    }

    /// <summary>
    /// 奖项设置操作类
    /// </summary>
    public class AwardsSettingAdapter : UpdatableAndLoadableAdapterBase<AwardsSetting, AwardsSettingCollection>
    {
        public static readonly AwardsSettingAdapter Instance = new AwardsSettingAdapter();

        private AwardsSettingAdapter() { }


        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }
        public AwardsSetting LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public AwardsSettingCollection LoadList(string validStatus = "")
        {
            return this.Load(p =>
            {
                if (validStatus.IsNotEmpty())
                {
                    p.AppendItem("ValidStatus", validStatus);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public AwardsSettingCollection LoadByProjectCode(string projectCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("ProjectCode", projectCode);
            });
        }
        public void DeleteByProjectCode(string code)
        {
            //物理删除            
            this.Delete(where => where.AppendItem("ProjectCode", code));
        }
    }
}

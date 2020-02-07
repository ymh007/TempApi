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
    /// 获奖作品
    /// </summary>
    [ORTableMapping("zc.WorksAwards")]
    public class WorksAwards
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 活动作品编码
        /// </summary>
        [ORFieldMapping("ActivityWorksCode")]
        public string ActivityWorksCode { get; set; }

        /// <summary>
        /// 奖项设置编码
        /// </summary>
        [ORFieldMapping("AwardsSettingCode")]
        public string AwardsSettingCode { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

    }

    /// <summary>
    /// 获奖作品集合
    /// </summary>
    public class WorksAwardsCollection : EditableDataObjectCollectionBase<WorksAwards>
    {
    }

    /// <summary>
    /// 获奖作品操作类
    /// </summary>
    public class WorksAwardsAdapter : UpdatableAndLoadableAdapterBase<WorksAwards, WorksAwardsCollection>
    {
        public static readonly WorksAwardsAdapter Instance = new WorksAwardsAdapter();

        private WorksAwardsAdapter() { }


        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }
        public WorksAwards LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        public WorksAwardsCollection LoadByAwardsSettingCode(string AwardsSettingCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("AwardsSettingCode", AwardsSettingCode);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validStatus"></param>
        /// <returns></returns>
        public WorksAwardsCollection LoadList(string validStatus = "")
        {
            return this.Load(p =>
            {
                if (validStatus.IsNotEmpty())
                {
                    p.AppendItem("ValidStatus", validStatus);
                }
            });
        }
        public void DeleteByActivityWorksCode(string code)
        {
            this.Delete(o => o.AppendItem("ActivityWorksCode", code));
        }

        public WorksAwardsCollection LoadByProjectCode(string projectCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("ProjectCode", projectCode);
            });
        }

    }
}

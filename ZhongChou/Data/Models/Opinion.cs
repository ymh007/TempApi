using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using MCS.Library.Data.Builder;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 审核意见
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.Opinion")]
    public class Opinion
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 意见类型
        /// </summary>
        [ORFieldMapping("Type")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public OpinionType Type { get; set; }

        /// <summary>
        /// 资源ID
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID { get; set; }

        /// <summary>
        /// 意见内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        [ORFieldMapping("CreatorName")]
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建时间Str
        /// </summary>
        [NoMapping]
        public string CreateTimeStr
        {
            get
            {
                return this.CreateTime.ToString("yyyy-MM-dd HH:mm");
            }
        }
        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }

    }

    /// <summary>
    /// 审核意见集合
    /// </summary>
    [Serializable]
    public class OpinionCollection : EditableDataObjectCollectionBase<Opinion>
    {
    }

    /// <summary>
    /// 审核意见操作类
    /// </summary>
    public class OpinionAdapter : UpdatableAndLoadableAdapterBase<Opinion, OpinionCollection>
    {
        public static readonly OpinionAdapter Instance = new OpinionAdapter();

        private OpinionAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public Opinion LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code, bool trueDelete = false)
        {
            //逻辑删除
            this.SetFields("IsValid", false, where => where.AppendItem("Code", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }

        public OpinionCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public OpinionCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        public List<Opinion> LoadByResourceID(string resourceID)
        {
            return this.Load(where =>
            {
                where.AppendItem("ResourceID", resourceID);
            }).OrderByDescending(o => o.CreateTime).ToList();
        }

        public OpinionCollection LoadByResourceID(string resourceID, int type)
        {
            return this.Load(where =>
            {
                where.AppendItem("ResourceID", resourceID);
                where.AppendItem("Type", type.ToString());
            }, o => { o.AppendItem("CreateTime", FieldSortDirection.Ascending); });
        }
    }


}


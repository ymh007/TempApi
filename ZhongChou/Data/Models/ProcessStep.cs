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
    /// 流程步骤
    /// </summary>
    [Serializable]
    [ORTableMapping("zc.ProcessStep")]
    public class ProcessStep
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 资源编码
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID { get; set; }

        /// <summary>
        ///流程类型
        /// </summary>
        [ORFieldMapping("ProcessType")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public ProcessType ProcessType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("Remarks")]
        public string Remarks { get; set; }

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
        public string CreateTimeFormat { get { return this.CreateTime.ToString("MM-dd  HH:mm"); } }

    }

    /// <summary>
    /// 流程步骤集合
    /// </summary>
    public class ProcessStepCollection : EditableDataObjectCollectionBase<ProcessStep>
    {
    }

    /// <summary>
    /// 流程步骤操作类
    /// </summary>
    public class ProcessStepAdapter : UpdatableAndLoadableAdapterBase<ProcessStep, ProcessStepCollection>
    {
        public static readonly ProcessStepAdapter Instance = new ProcessStepAdapter();

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        private ProcessStepAdapter()
        {
        }

        public ProcessStep LoadByCode(string code)
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

            //实际删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
            }
        }

        public ProcessStepCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public ProcessStepCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        public ProcessStepCollection LoadByProcessType(string resourceID, ProcessType processType)
        {
            return this.Load(where =>
            {
                where.AppendItem("ResourceID", resourceID);
                where.AppendItem("ProcessType", processType.ToString("D"));
            }, o => { o.AppendItem("CreateTime", FieldSortDirection.Ascending); });
        }
    }


}


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
    /// 项目标签
    /// </summary>
    [ORTableMapping("zc.ProjectTag")]
    public class ProjectTag
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
        /// 标签编码
        /// </summary>
        [ORFieldMapping("TagCode")]
        public string TagCode { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        [ORFieldMapping("TagName")]
        public string TagName { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public string SortNo { get; set; }

    }

    /// <summary>
    /// 项目标签集合
    /// </summary>
    public class ProjectTagCollection : EditableDataObjectCollectionBase<ProjectTag>
    {
    }

    /// <summary>
    /// 项目标签操作类
    /// </summary>
    public class ProjectTagAdapter : UpdatableAndLoadableAdapterBase<ProjectTag, ProjectTagCollection>
    {
        public static readonly ProjectTagAdapter Instance = new ProjectTagAdapter();

        private ProjectTagAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public ProjectTag LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ProjectTagCollection LoadByprojectCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("ProjectCode", code);
            });
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
        public void DeleteByProjectCode(string code)
        {
            //物理删除            
            this.Delete(where => where.AppendItem("ProjectCode", code));
        }
        public ProjectTagCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public ProjectTagCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
    }


}


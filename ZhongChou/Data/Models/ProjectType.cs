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
    /// 项目类型
    /// </summary>
    [ORTableMapping("zc.ProjectType")]
    public class ProjectType
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [ORFieldMapping("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 总项目数
        /// </summary>
        [ORFieldMapping("TotalProjectNo")]
        public int TotalProjectNo { get; set; }

        /// <summary>
        /// 总参与人数
        /// </summary>
        [ORFieldMapping("TotalUserNo")]
        public int TotalUserNo { get; set; }

        /// <summary>
        /// 首页显示
        /// </summary>
        [ORFieldMapping("SetHome")]
        public bool SetHome { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [ORFieldMapping("Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [ORFieldMapping("Icon")]
        public string Icon { get; set; }

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

        /// <summary>
        /// 是否热门
        /// </summary>
        [ORFieldMapping("IsHot")]
        public bool IsHot { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("IsValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }

    }

    /// <summary>
    /// 项目类型集合
    /// </summary>
    public class ProjectTypeCollection : EditableDataObjectCollectionBase<ProjectType>
    {
    }

    /// <summary>
    /// 项目类型操作类
    /// </summary>
    public class ProjectTypeAdapter : UpdatableAndLoadableAdapterBase<ProjectType, ProjectTypeCollection>
    {
        public static readonly ProjectTypeAdapter Instance = new ProjectTypeAdapter();

        private ProjectTypeAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public ProjectType LoadByCode(string code)
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

        public ProjectTypeCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public ProjectTypeCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }

        public ProjectTypeCollection LoadHomeList()
        {
            return this.Load(where =>
            {
                where.AppendItem("IsValid", true);
                where.AppendItem("SetHome", true);
            }, order => { order.AppendItem("SortNo", MCS.Library.Data.Builder.FieldSortDirection.Ascending); });
        }

        public void SetIncTotalProjectNo(string projectType)
        {
            ProjectTypeAdapter.Instance.SetInc("TotalProjectNo", 1, where =>
            {
                where.AppendItem("Code", projectType);
            });
        }

        public void SetIncTotalUserNo(string projectType)
        {
            ProjectTypeAdapter.Instance.SetInc("TotalUserNo", 1, where =>
            {
                where.AppendItem("Code", projectType);
            });
        }
    }


}


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
    /// 话题
    /// </summary>
    [ORTableMapping("zc.Topic")]
    public class Topic
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 话题类型编码
        /// </summary>
        [ORFieldMapping("TopicTypeCode")]
        public string TopicTypeCode { get; set; }

        /// <summary>
        /// 众筹项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 话题时长编码
        /// </summary>
        [ORFieldMapping("TopicDurationCode")]
        public string TopicDurationCode { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [ORFieldMapping("Price")]
        public double Price { get; set; }

        /// <summary>
        /// 格式化价格
        /// </summary>
        [NoMapping]
        public string PriceFormat { get { return Price.ToString("0.00"); } }

        [NoMapping]
        public string TopicName
        {
            get
            {
                return ProjectAdapter.Instance.LoadByCode(this.ProjectCode).Name;
            }
        }

    }

    /// <summary>
    /// 话题集合
    /// </summary>
    public class TopicCollection : EditableDataObjectCollectionBase<Topic>
    {
    }

    /// <summary>
    /// 话题操作类
    /// </summary>
    public class TopicAdapter : UpdatableAndLoadableAdapterBase<Topic, TopicCollection>
    {
        public static readonly TopicAdapter Instance = new TopicAdapter();

        private TopicAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public Topic LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public Topic LoadByProjectCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
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

        public TopicCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public TopicCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
    }


}


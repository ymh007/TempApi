using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using System.Transactions;
using Newtonsoft.Json;
using MobileBusiness.Common.Data;
using MCS.Library.Data.Builder;
using System.Text;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using System.Configuration;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity
{
    /// <summary>
    /// 附件
    /// </summary>
    [Serializable]
    [ORTableMapping("Business.Attachment")]
    public class Attachment
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 附件类型编码
        /// </summary>
        [ORFieldMapping("AttachmentTypeCode")]
        public string AttachmentTypeCode { get; set; }

        /// <summary>
        /// 资源编码
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID { get; set; }

        /// <summary>
        /// 中文名
        /// </summary>
        [ORFieldMapping("CnName")]
        public string CnName { get; set; }

        /// <summary>
        /// 英文名
        /// </summary>
        [ORFieldMapping("Suffix")]
        public string Suffix { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [ORFieldMapping("URL")]
        public string URL { get; set; }
        /// <summary>
        /// 地址Str
        /// </summary>
        [NoMapping]
        public string URLStr
        {
            get
            {
                if (this.URL.Contains("http://"))
                {
                    return this.URL;
                }
                else
                {
                    return ConfigurationManager.AppSettings["GetProjectFileUrl"] + this.URL;
                }
            }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        [ORFieldMapping("FileSize")]
        public int FileSize { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        [JsonIgnore]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("VersionStartTime")]
        [JsonIgnore]
        public DateTime VersionStartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ORFieldMapping("VersionEndTime", PrimaryKey = true)]
        [JsonIgnore]
        public DateTime VersionEndTime { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        [JsonIgnore]
        public bool ValidStatus { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo { get; set; }



        public Attachment CopyToNewVersion(DateTime versionStartTime)
        {
            return new Attachment
            {
                Code = this.Code,
                AttachmentTypeCode = this.AttachmentTypeCode,
                CnName = this.CnName,
                ResourceID = this.ResourceID,
                FileSize = this.FileSize,
                Suffix = this.Suffix,
                URL = this.URL,
                Creator = this.Creator,
                ValidStatus = this.ValidStatus,
                SortNo = this.SortNo,
                VersionStartTime = versionStartTime,
                VersionEndTime = DatabaseUtility.GetDbDateTimeMaxValue()
            };
        }
    }

    /// <summary>
    /// 附件集合
    /// </summary>
    [Serializable]
    public class AttachmentCollection : EditableDataObjectCollectionBase<Attachment>
    {
    }

    /// <summary>
    /// 附件操作类
    /// </summary>
    public class AttachmentAdapter : UpdatableAndLoadableAdapterBase<Attachment, AttachmentCollection>
    {
        public static readonly AttachmentAdapter Instance = new AttachmentAdapter();

        private AttachmentAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return DatabaseUtility.CFOWDFUNDING;
        }

        public Attachment LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
                where.AppendItem("VersionEndTime", DatabaseUtility.GetDbDateTimeMaxValue());
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code)
        {
            var now = DateTime.Now;

            var entity = this.LoadByCode(code);
            entity.VersionEndTime = now;

            var newentity = entity.CopyToNewVersion(now);

            using (TransactionScope ts = TransactionScopeFactory.Create())
            {
                this.Update(entity);

                this.Update(newentity);

                ts.Complete();
            }
        }

        public void UpdateCollectionWithVersioned(string resourceID, AttachmentCollection attachmentCollection)
        {
            if (attachmentCollection.Count == 0) return;

            ISqlBuilder sqlBuilderInstance = TSqlBuilder.Instance;
            StringBuilder strSql = new StringBuilder();

            var existAttachments = this.LoadByResourceID(resourceID);

            var sealVersionTime = attachmentCollection.Select(it => it.VersionStartTime).FirstOrDefault();
            foreach (var item in existAttachments)
            {
                if (!attachmentCollection.Exists(it => it.Code == item.Code))
                {
                    var update = new UpdateSqlClauseBuilder();
                    update.AppendItem("VersionEndTime", sealVersionTime);
                    var where = new WhereSqlClauseBuilder();
                    where.AppendItem("Code", item.Code);
                    where.AppendItem("VersionEndTime", DatabaseUtility.GetDbDateTimeMaxValue());
                    strSql.Append(SqlHelper.GetUpdateSql<Attachment>(update, where));
                }
            }

            foreach (var item in attachmentCollection)
            {
                if (!existAttachments.Exists(it => it.Code == item.Code))
                {
                    strSql.Append(ORMapping.GetInsertSql(item, sqlBuilderInstance));
                }
            }

            if (strSql.Length > 0)
            {
                DbHelper.RunSqlWithTransaction(strSql.ToString(), this.GetConnectionName());
            }
        }

        public AttachmentCollection LoadByResourceID(string resourceID)
        {
            return this.Load(where =>
            {
                where.AppendItem("ResourceID", resourceID);
                where.AppendItem("VersionEndTime", DatabaseUtility.GetDbDateTimeMaxValue());
            });
        }

        public AttachmentCollection LoadByResourceIDAndType(string resourceID, AttachmentTypeEnum type)
        {
            return this.Load(where =>
            {
                where.AppendItem("ResourceID", resourceID);
                where.AppendItem("AttachmentTypeCode", type.ToString("D"));
                where.AppendItem("VersionEndTime", DatabaseUtility.GetDbDateTimeMaxValue());
            });
        }

    }


}

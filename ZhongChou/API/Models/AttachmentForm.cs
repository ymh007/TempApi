using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using System.ComponentModel.DataAnnotations;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouApi.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// 附件表单数据
    /// </summary>
    public class AttachmentForm
    {
        /// <summary>
        /// 附件编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string CnName { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        [Required]
        public string Suffix { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Required]
        public int FileSize { get; set; }

        /// <summary>
        /// 文件内容base64数据，提交数据时使用
        /// </summary>
        public string FileContent { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Required]
        public int SortNo { get; set; }

        /// <summary>
        /// 绑定数据时使用
        /// </summary>
        public string URL { get; set; }

        public string attachmentType { get; set; }

        public Attachment ToAttachment(AttachmentTypeEnum attachmentType, string resourceID, string creator)
        {
            string code = this.Code.IsNullOrEmpty() ? UuidHelper.NewUuidString() : this.Code;           

            return new Attachment
            {
                Code = code,
                CnName = this.CnName,
                ResourceID = resourceID,
                Suffix = this.Suffix,
                FileSize = this.FileSize,
                Creator = creator,
                VersionStartTime = DateTime.Now,
                VersionEndTime = DatabaseUtility.GetDbDateTimeMaxValue(),
                SortNo = this.SortNo,
                ValidStatus = true,

                AttachmentTypeCode = attachmentType.ToString("D"),
                URL = this.URL
            };
        }
        public Attachment ToAttachmentWithFile(AttachmentTypeEnum attachmentType, string resourceID, string creator)
        {
            string code = this.Code.IsNullOrEmpty() ? UuidHelper.NewUuidString() : this.Code;            
            return new Attachment
            {
                Code = code,
                CnName = GetAttachmentName2(code + "." + this.Suffix),
                ResourceID = resourceID,
                Suffix = this.Suffix,
                FileSize = this.FileSize,
                Creator = creator,
                VersionStartTime = DateTime.Now,
                VersionEndTime = DatabaseUtility.GetDbDateTimeMaxValue(),
                SortNo = this.SortNo,
                ValidStatus = true,
                AttachmentTypeCode = attachmentType.ToString("D"),
                URL = GetAttachmentUrl(code + "." + this.Suffix)
            };
        }
        private string GetAttachmentUrl(string cnname)
        {
            return string.Format("{0}/{1}/{2}",
                AliyunOSSHelper.OSS_Host,
                AliyunOSSHelper.OSS_Dir,
                cnname);
        }
        private string GetAttachmentName2(string cnname)
        {
            return string.Format("{0}/{1}",                
                AliyunOSSHelper.OSS_Dir,
                cnname);
        }

        public AttachmentForm CopyFrom(Attachment attachment)
        {
            if (attachment == null) return null;

            return new AttachmentForm { 
                Code = attachment.Code,
                FileSize = attachment.FileSize,
                SortNo = attachment.SortNo,
                Suffix = attachment.Suffix,
                URL = attachment.URL
            };
        }

        public string GetAttachmentName(string url)
        {
            return url.Substring(url.LastIndexOf("/") + 1);
        }
    }
}
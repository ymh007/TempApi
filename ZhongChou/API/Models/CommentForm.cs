using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    /// <summary>
    /// 评论数据表单
    /// </summary>
    public class CommentForm
    {        
        /// <summary>
        /// 项目编码
        /// </summary>
        public string projectCode { get; set; }
        /// <summary>
        /// 评论编码（被回复的）
        /// </summary>
        public string commentCode { get; set; }
        /// <summary>
        /// 作品编码
        /// </summary>
        public string worksCode { get; set; }
        /// <summary>
        /// 用户编码
        /// </summary>
        public string usercode { get; set; }       
        /// <summary>
        /// 消息内容 
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 上传图片
        /// </summary>
        public List<AttachmentForm> Attachments { get; set; }
        /// <summary>
        /// 转换成消息实体
        /// </summary>
        /// <returns></returns>
        public UserComment ToComment(out AttachmentCollection attachments)
        {
            string code = Guid.NewGuid().ToString();
            attachments = this.ToAttachmentCollection(code, this.usercode, AttachmentTypeEnum.UserComment);
            var result = new UserComment
            {
                Code = code,
                ProjectCode = projectCode,
                ParentCode = commentCode,
                WorksCode = worksCode,
                HavePicture = false,
                Creator = usercode,
                CreateTime = DateTime.Now,
                Content = content              
            };
            return result;
        }
        public UserComment ToComment()
        {
            string code = Guid.NewGuid().ToString();         
            var result = new UserComment
            {
                Code = code,
                ProjectCode = projectCode,
                ParentCode = commentCode,
                WorksCode = worksCode,
                HavePicture = false,
                Creator = usercode,
                CreateTime = DateTime.Now,
                Content = content
            };
            return result;
        }
        protected AttachmentCollection ToAttachmentCollection(string resourceID, string creator, AttachmentTypeEnum attachmentType)
        {
            var result = new AttachmentCollection();

            if (this.Attachments != null)
            {
                foreach (var item in this.Attachments)
                {
                    var attachment = item.ToAttachment(attachmentType, resourceID, creator);

                    result.Add(attachment);
                }
            }
            return result;
        }
    }
}
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class ActivityWorksForm
    {
        /// <summary>
        /// 众筹项目编码
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 作品内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否上传图
        /// </summary>
        public bool HaveImage { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 活动作品
        /// </summary>
        public List<AttachmentForm> Attachments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public ActivityWorks ToActivityWorks(out AttachmentCollection attachments)
        {
            string code = Guid.NewGuid().ToString();
            attachments = this.ToAttachmentCollection(code, this.Creator, AttachmentTypeEnum.ActivityWorks);
            return new ActivityWorks
            {
                Code = code,
                CommentNo = 0,
                HaveImage = this.Attachments != null && this.Attachments.Count > 0,
                Creator = this.Creator,
                CreateTime = DateTime.Now,
                Content = this.Content,
                VoteCount = 0,
                ProjectCode = this.ProjectCode
            };
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
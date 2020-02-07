using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.ZhongChouData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ZhongChouApi.Models
{
    public class UserEvaluationForm
    {
        /// <summary>
        /// 订单编码
        /// </summary>
        public string OrderCode { get; set; }

        /// <summary>
        /// 评价人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评价分数
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 评价类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 图片附件
        /// </summary>
        public List<AttachmentForm> Attachments { get; set; }

        public UserEvaluation ToUserEvaluation(out AttachmentCollection attachments)
        {
            string code = Guid.NewGuid().ToString();
            attachments = this.ToAttachmentCollection(code, this.Creator, AttachmentTypeEnum.UserEvaluation);

            return new UserEvaluation
            {
                Code = code,
                OrderCode = OrderCode,
                HavePicture = attachments != null && attachments.Count > 0,
                Creator = this.Creator,
                CreateTime = DateTime.Now,
                Content = this.Content,
                Score = double.Parse(this.Score),
                ParentID = ""
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
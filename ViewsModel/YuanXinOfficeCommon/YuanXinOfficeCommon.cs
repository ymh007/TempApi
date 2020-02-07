using Seagull2.YuanXin.AppApi.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.ViewsModel.YuanXinOfficeCommon
{
    public class YuanXinOfficeCommon
    {
        /// <summary>
        /// 消息提醒推送请求参数
        /// </summary>
        public class PushMessageBody
        {
            /// <summary>
            /// 标题
            /// </summary>
            [Required]
            public string Title { get; set; }

            /// <summary>
            /// 内容
            /// </summary>
            [Required]
            public string Content { get; set; }

            /// <summary>
            /// 消息模块类型（针对ModuleType字段）
            /// </summary>
            [Required]
            public EnumMessageModuleType MoudleType { get; set; }

            /// <summary>
            /// 消息标题类型（枚举）
            /// </summary>
            public EnumMessageTitle TitleType { get; set; }

            /// <summary>
            /// 推送编码
            /// </summary>
            public string Code { get; set; }

            /// <summary>
            /// 推送类型
            /// 0：推送、提醒
            /// 1：推送
            /// 2：提醒
            /// </summary>
            [Required]
            public int PushType { get; set; }

            /// <summary>
            /// 创建人编码
            /// </summary>
            [Required]
            public string Creaotr { get; set; }

            /// <summary>
            /// 创建人姓名
            /// </summary>
            [Required]
            public string CreatorName { get; set; }

            /// <summary>
            /// 推送人员
            /// </summary>
            [Required]
            public List<string> UserList { get; set; }
        }

        /// <summary>
        /// 邮件发送请求参数
        /// </summary>
        public class SeagullMail
        {
            /// <summary>
            /// 标题
            /// </summary>
            [Required]
            public string Subject { get; set; }

            /// <summary>
            /// 内容
            /// </summary>
            [Required]
            public string Body { get; set; }

            /// <summary>
            /// 是否是html
            /// </summary>
            [Required]
            public bool IsHtml { get; set; }

            /// <summary>
            /// 附件
            /// </summary>
            [Required]
            public List<string> Attachments { get; set; }

            /// <summary>
            /// 收件人
            /// </summary>
            [Required]
            public Dictionary<string, string> To { get; set; }

            /// <summary>
            /// 抄送人
            /// </summary>
            [Required]
            public Dictionary<string, string> CC { get; set; }

            /// <summary>
            /// 密送
            /// </summary>
            [Required]
            public Dictionary<string, string> BCC { get; set; }
        }
    }

    #region 海鸥2 获取个人请假信息的实体类


    /// <summary>
    /// 请休假接口返回对象
    /// </summary>
    public class LeaveInfo
    {
        /// <summary>
        /// 请休假数据
        /// </summary>
        public List<Leave> data { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool state { get; set; }

    }
    /// <summary>
    /// 请休假信息
    /// </summary>
    public class Leave
    {
        /// <summary>
        /// 开始请假类型
        /// </summary>
        public string startTypeName { get; set; }

        /// <summary>
        /// 结束请假类型
        /// </summary>
        public string endTypeName { get; set; }

        /// <summary>
        ///  假期类型
        /// </summary>
        public string attendenceItemName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime startDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endDate { get; set; }

        /// <summary>
        /// 请假天数
        /// </summary>
        public float daysCount { get; set; }

        /// <summary>
        /// 请假原因
        /// </summary>
        public string summary { get; set; }

    }

    public class PunchInfoModel
    {
        public DateTime punchDate { get; set; }

        public string attendenceItemName { get; set; }

        public string summary { get; set; }

        public bool AM { get; set; }
        public bool PM { get; set; }
    }
    #endregion
}
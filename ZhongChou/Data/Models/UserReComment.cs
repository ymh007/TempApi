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
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户评论
    /// </summary>
    [ORTableMapping("zc.UserComment")]
    public class UserReComment
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 父编码
        /// </summary>
        [ORFieldMapping("ParentCode")]
        public string ParentCode { get; set; }

        /// <summary>
        /// 作品编码
        /// </summary>
        [ORFieldMapping("WorksCode")]
        public string WorksCode { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 是否有图片
        /// </summary>
        [ORFieldMapping("HavePicture")]
        public bool HavePicture { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ORFieldMapping("Creator")]
        public string Creator { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        [NoMapping]
        public UserInfo UserInfo
        {
            get
            {
                if (Creator != "")
                {
                    return UserInfoAdapter.Instance.LoadByCode(Creator);
                }
                return null;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NoMapping]
        public string CreateTimeFormate
        {
            get
            {
                return CommonHelper.APPDateFormateDiff(CreateTime, DateTime.Now);
            }
        }

        [NoMapping]
        public AttachmentCollection AttachmentCollection
        {
            get
            {
                if (this.HavePicture)
                {
                    return AttachmentAdapter.Instance.LoadByResourceID(this.Code);
                }
                return null;
            }
        }

        [NoMapping]
        public UserReCommentCollection UserReCommentCollection {
            get {
                return UserReCommentAdapter.Instance.LoadByReComment(this.Code);
            }
        }
    }

    /// <summary>
    /// 用户评论集合
    /// </summary>
    public class UserReCommentCollection : EditableDataObjectCollectionBase<UserReComment>
    {
    }

    /// <summary>
    /// 用户评论操作类
    /// </summary>
    public class UserReCommentAdapter : UpdatableAndLoadableAdapterBase<UserReComment, UserReCommentCollection>
    {
        public static readonly UserReCommentAdapter Instance = new UserReCommentAdapter();

        private UserReCommentAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

     

        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public UserReCommentCollection LoadByReComment(string commentCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ParentCode", commentCode);
            });
        }       

    }


}


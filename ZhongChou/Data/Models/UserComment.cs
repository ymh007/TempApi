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
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户评论
    /// </summary>
    [ORTableMapping("zc.UserComment")]
    public class UserComment
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
        public ContactsModel UserInfo
        {
            get
            {
                if (Creator != "")
                {
                    return ContactsAdapter.Instance.LoadByCode(Creator);
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
        /// <summary>
        /// 获取直属子评论
        /// </summary>
        [NoMapping]
        public UserCommentCollection OneOfCommentColl
        {
            get
            {
                return UserCommentAdapter.Instance.LoadByReComment(this.Code);
            }
        }
    }

    /// <summary>
    /// 用户评论集合
    /// </summary>
    public class UserCommentCollection : EditableDataObjectCollectionBase<UserComment>
    {
    }

    /// <summary>
    /// 用户评论操作类
    /// </summary>
    public class UserCommentAdapter : UpdatableAndLoadableAdapterBase<UserComment, UserCommentCollection>
    {
        public static readonly UserCommentAdapter Instance = new UserCommentAdapter();

        private UserCommentAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserComment LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public UserCommentCollection LoadByProjectCode(string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ProjectCode", projectCode);
            });
        }

        /// <summary>
        /// 获取回复列表
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public UserCommentCollection LoadByReComment(string commentCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ParentCode", commentCode);
            });
        }
        public void DeleteByCode(string code, bool trueDelete = false)
        {
            ////逻辑删除
            //this.SetFields("IsValid", false, where => where.AppendItem("Code", code));
            //this.SetFields("IsValid", false, where => where.AppendItem("ParentCode", code));

            //物理删除
            if (trueDelete)
            {
                this.Delete(where => where.AppendItem("Code", code));
                this.Delete(where => where.AppendItem("ParentCode", code));
            }
        }

        public UserCommentCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public UserCommentCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
    }


}


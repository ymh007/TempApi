using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户关注
    /// </summary>
    [ORTableMapping("zc.UserFocus")]
    public class UserFocus
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        [ORFieldMapping("ProjectCode")]
        public string ProjectCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("Type")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public UserFocusType Type { get; set; }

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
        /// 项目信息
        /// </summary>
        [NoMapping]
        public Project Project
        {
            get
            {
                if (ProjectCode.IsNotEmpty())
                {
                    return ProjectAdapter.Instance.LoadByCode(ProjectCode);
                }

                return null;
            }
        }
    }

    /// <summary>
    /// 用户关注集合
    /// </summary>
    public class UserFocusCollection : EditableDataObjectCollectionBase<UserFocus>
    {
    }

    /// <summary>
    /// 用户关注操作类
    /// </summary>
    public class UserFocusAdapter : UpdatableAndLoadableAdapterBase<UserFocus, UserFocusCollection>
    {
        public static readonly UserFocusAdapter Instance = new UserFocusAdapter();

        private UserFocusAdapter()
        {
        }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserFocus LoadByCode(string code)
        {
            return this.Load(where =>
            {
                where.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public void DeleteByCode(string code, bool trueDelete = false)
        {
            this.Delete(where => where.AppendItem("Code", code));
        }

        public void Delete(string userCode, string projectCode)
        {
            this.Delete(where => {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode); 
            });
        }


        public UserFocusCollection LoadAll()
        {
            return this.Load(where => where.AppendItem("1", 1));
        }

        public UserFocusCollection LoadAll(bool isValid)
        {
            return this.Load(where => where.AppendItem("IsValid", isValid));
        }
        /// <summary>
        /// 多个项目关注数
        /// </summary>
        /// <param name="projectColl"></param>
        /// <returns></returns>
        public UserFocusCollection LoadByuserFocuse(ProjectCollection projectColl)
        {
            UserFocusCollection UserFocusColl = new UserFocusCollection();
            foreach (Project pro in projectColl)
            {
                UserFocusColl.Concat(
                   this.Load(where =>
                   {
                       where.AppendItem("ProjectCode", pro.Code);
                   }));
            }

            return UserFocusColl;
        }

        public UserFocus Load(string userCode, string projectCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
            }).FirstOrDefault();
        }

        public UserFocusCollection Load(string userCode, UserFocusType focusType)
        {
            return this.Load(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("Type", focusType.ToString("D"));
            });
        }

        public bool Exists(string userCode, string projectCode)
        {
            return this.Exists(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
            });
        }

        public bool Exists(string userCode, string projectCode, UserFocusType focusType)
        {
            return this.Exists(where =>
            {
                where.AppendItem("Creator", userCode);
                where.AppendItem("ProjectCode", projectCode);
                where.AppendItem("Type", focusType.ToString("D"));
            });
        }
        public void Update(string userCode, string projectCode, UserFocusType focusType)
        {
            var entity = new UserFocus
            {
                Code = UuidHelper.NewUuidString(),
                ProjectCode = projectCode,
                Creator = userCode,
                CreateTime = DateTime.Now,
                Type = focusType
            };

            this.Update(entity);
        }
    }


}


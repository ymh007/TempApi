using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MobileBusiness.Common.Data;
using System.Data;
using Newtonsoft.Json;
using Seagull2.YuanXin.AppApi.ZhongChouData.Enums;
using Seagull2.YuanXin.AppApi.ZhongChouData.Common;
using Seagull2.YuanXin.AppApi.ZhongChouData.BaseEntity;
using Seagull2.YuanXin.AppApi.Adapter.AddressBook;
using Seagull2.YuanXin.AppApi.Models.AddressBook;

namespace Seagull2.YuanXin.AppApi.ZhongChouData.Models
{
    /// <summary>
    /// 用户评价
    /// </summary>
    [ORTableMapping("zc.UserEvaluation")]
    public class UserEvaluation
    {
        /// <summary>
        /// 编码
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 父编码
        /// </summary>
        [ORFieldMapping("ParentID")]
        [JsonIgnore]
        public string ParentID { get; set; }

        /// <summary>
        /// 订单编码
        /// </summary>
        [ORFieldMapping("OrderCode")]
        [JsonIgnore]
        public string OrderCode { get; set; }

        /// <summary>
        /// 评价类型
        /// </summary>
        [ORFieldMapping("Type")]
        [SqlBehavior(EnumUsage = EnumUsageTypes.UseEnumValue)]
        public EvaluationType Type { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        [ORFieldMapping("Score")]
        public double Score { get; set; }
        /// <summary>
        /// 内容
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
        [JsonIgnore]
        public string Creator { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ORFieldMapping("CreateTime")]
        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        [NoMapping]

        public string CreateTimeFormat { get { return CommonHelper.APPDateFormateDiff(CreateTime, DateTime.Now); } }

        /// <summary>
        /// 用户信息
        /// </summary>
        [NoMapping]

        public ContactsModel UserInfo
        {
            get
            {
                var result = ContactsAdapter.Instance.LoadByCode(this.Creator);

                return result;
            }
        }
        /// <summary>
        /// 用户头像
        /// </summary>
        [NoMapping]

        public string UserHeadUrl
        {
            get
            {
                var result = UserHeadPhotoService.GetUserHeadPhoto(Creator); 

                return result;
            }
        }

        [NoMapping]

        public AttachmentCollection Images
        {
            get
            {
                if (this.HavePicture)
                {
                    return AttachmentAdapter.Instance.LoadByResourceIDAndType(this.Code, AttachmentTypeEnum.UserEvaluation);
                }
                return null;
            }
        }

        /// <summary>
        /// 项目名称
        /// </summary>
        [NoMapping]
        public string ProjectName { get; set; }

    }

    /// <summary>
    /// 用户评价集合
    /// </summary>
    public class UserEvaluationCollection : EditableDataObjectCollectionBase<UserEvaluation>
    {
    }

    /// <summary>
    /// 用户评价操作类
    /// </summary>
    public class UserEvaluationAdapter : UpdatableAndLoadableAdapterBase<UserEvaluation, UserEvaluationCollection>
    {
        public static readonly UserEvaluationAdapter Instance = new UserEvaluationAdapter();

        private UserEvaluationAdapter() { }

        protected override string GetConnectionName()
        {
            return CommonHelper.GetConnectionName();
        }

        public UserEvaluation LoadByCode(string code)
        {
            return this.Load(p =>
            {
                p.AppendItem("Code", code);
            }).FirstOrDefault();
        }

        public UserEvaluationCollection LoadByReEvaluation(string parentCode)
        {
            return this.Load(where =>
            {
                where.AppendItem("ParentID", parentCode);
            });
        }
        public UserEvaluationCollection LoadList(string validStatus = "")
        {
            return this.Load(p =>
            {
                if (validStatus.IsNotEmpty())
                {
                    p.AppendItem("ValidStatus", validStatus);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderCode"></param>
        /// <returns></returns>
        public UserEvaluation LoadByOrderCode(string orderCode)
        {
            return this.Load(p =>
            {
                p.AppendItem("OrderCode", orderCode);

            }).FirstOrDefault();
        }

        public UserEvaluationCollection LoadLastThree(string projectCode)
        {
            return this.LoadTopByProjectCode(3, projectCode);
        }
        public UserEvaluationCollection LoadAll(string projectCode)
        {
            return this.LoadTopByProjectCode(projectCode);
        }

        public UserEvaluationCollection LoadTopByProjectCode(int top, string projectCode)
        {
            var result = new UserEvaluationCollection();

            string sql = string.Format(@"select top {0} e.* 
                                         from zc.UserEvaluation e 
                                         left join zc.[Order] o 
                                         on e.OrderCode=o.Code  
                                         where ParentID='' and o.ProjectCode='{1}' 
                                         order by e.CreateTime desc"
                                        , top, projectCode);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result;
        }
        public UserEvaluationCollection LoadTopByProjectCode(string projectCode)
        {
            var result = new UserEvaluationCollection();

            string sql = string.Format(@"select e.* 
                                         from zc.UserEvaluation e 
                                         left join zc.[Order] o 
                                         on e.OrderCode=o.Code  
                                         where ParentID='' and o.ProjectCode='{0}' 
                                         order by e.CreateTime desc"
                                        , projectCode);

            DataView dv = DbHelper.RunSqlReturnDS(sql.ToString(), GetConnectionName()).Tables[0].DefaultView;
            ORMapping.DataViewToCollection(result, dv);

            return result;
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
                this.Delete(where => where.AppendItem("ParentID", code));
            }
        }

    }
}

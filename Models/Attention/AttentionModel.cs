using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects;
using System;
using System.Linq;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    [ORTableMapping("dbo.Attention")]
    public class AttentionModel
    {
        /// <summary>
        /// Code
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true)]
        public string Code { get; set; }

        /// <summary>
        /// 项目或事业部的ID
        /// </summary>
        [ORFieldMapping("BusinessProjectCode")]
        public string BusinessProjectCode { get; set; }

        /// <summary>
        /// 项目或事业部的Name
        /// </summary>
        [ORFieldMapping("BusinessProjectName")]
        public string BusinessProjectName { get; set; }

        /// <summary>
        /// 人员的ID
        /// </summary>
        [ORFieldMapping("UserCode")]
        public string UserCode { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }

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
        /// 封版时间
        /// </summary>
        [ORFieldMapping("VersionTime")]
        public DateTime VersionTime { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [ORFieldMapping("AttentionType")]
        public int AttentionType { get; set; }

        /// <summary>
        /// 事业部编码
        /// </summary>
        [ORFieldMapping("BusinessCode")]
        public string BusinessCode { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AttentionController : EditableDataObjectCollectionBase<AttentionModel> {
  
    }

    public class AttentionAdapter : UpdatableAndLoadableAdapterBase<AttentionModel, AttentionController> {

        public static readonly AttentionAdapter Instance = new AttentionAdapter();
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 查询我的关注列表
        /// </summary>
        /// <returns></returns>
        public AttentionController LoadAttentionUserCode(string userCode) {
            return base.Load( p =>{
                p.AppendItem("UserCode", userCode);
                p.AppendItem("ValidStatus",true);
            });
        }

        /// <summary>
        /// 查询关注
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="businessProjectCode"></param>
        /// <returns></returns>
        public AttentionModel LoadAttentionUserCodeAndBuss(string userCode,string businessProjectCode) {
            return base.Load( p=> {
                p.AppendItem("UserCode", userCode);
                p.AppendItem("ValidStatus", true);
                p.AppendItem("BusinessProjectCode", businessProjectCode);
            }).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        public new void Update(AttentionModel data) {
            base.Update(data);
        }
    }
}
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 点赞表
    /// </summary>
    [ORTableMapping("office.PraiseDetail")]
    public class PraiseDetailModel
    {

        /// <summary>
        /// 主键
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }

        /// <summary>
        /// 会议圈编码
        /// </summary>
        [ORFieldMapping("MomentID")]
        public string MomentID { get; set; }

        /// <summary>
        /// 点赞人
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }

        /// <summary>
        /// 点赞人名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }

        /// <summary>
        /// 点赞时间
        /// </summary>
        [ORFieldMapping("PraiseDate")]
        public DateTime PraiseDate { get; set; }

        /// <summary>
        /// 数据有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }
    public class PraiseDetailModelCollection : EditableDataObjectCollectionBase<PraiseDetailModel> { }
    public class PraiseDetailModelAdapter : BaseAdapter<PraiseDetailModel, PraiseDetailModelCollection>
    {
        public static PraiseDetailModelAdapter Instance = new PraiseDetailModelAdapter();
        public string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public PraiseDetailModelAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }
        /// <summary>
        /// 根据会议圈编码删除某人的点赞
        /// </summary>
        /// <param name=""></param>
        public void DeletePraiseDetailModel(string momentID, string userID)
        {
            string sql = string.Format(@"DELETE FROM office.PraiseDetail WHERE MomentID='{0}' AND UserID='{1}'", momentID, userID);
            ViewBaseAdapter<object, List<object>>.Instance.RunSQLByTransaction(sql, ConnectionNameDefine.YuanXinForDBHelp);
        }
        /// <summary>
        /// 根据会议圈编码删除点赞
        /// </summary>
        /// <param name="momentID"></param>
        public void DelbyMomentID(string momentID)
        {
            Delete(m => m.AppendItem("MomentID", momentID));
        }
    }
}
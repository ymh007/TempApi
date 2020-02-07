using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;

namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会议圈评论
    /// </summary>
    [ORTableMapping("office.MomentComment")]
    public class MomentCommentModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 评论编码
        /// </summary>
        [ORFieldMapping("MomentCommentID")]
        public string MomentCommentID { get; set; }
        /// <summary>
        /// 会议圈编码
        /// </summary>
        [ORFieldMapping("MomentID")]
        public string MomentID { get; set; }
        /// <summary>
        /// 评论人
        /// </summary>
        [ORFieldMapping("UserID")]
        public string UserID { get; set; }
        /// <summary>
        /// 评论人名称
        /// </summary>
        [ORFieldMapping("UserName")]
        public string UserName { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }
        /// <summary>
        /// 评论时间
        /// </summary>
        [ORFieldMapping("ContentDate")]
        public DateTime ContentDate { get; set; }
        /// <summary>
        /// 数据有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 企业圈集合
    /// </summary>
    public class MomentCommentModelCollection : EditableDataObjectCollectionBase<MomentCommentModel>
    {

    }

    /// <summary>
    /// 企业圈 Adapter
    /// </summary>
    public class MomentCommentModelAdapter : BaseAdapter<MomentCommentModel, MomentCommentModelCollection>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        public static MomentCommentModelAdapter Instance = new MomentCommentModelAdapter();

        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;

        /// <summary>
        /// 构造
        /// </summary>
        public MomentCommentModelAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        /// <summary>
        /// 根据会议圈编码删除
        /// </summary>
        public void DelByMomentID(string momentID)
        {
            Delete(m => m.AppendItem("MomentID", momentID));
        }

        /// <summary>
        /// 根据主键ID删除
        /// </summary>
        public int DelById(string id)
        {
            string delSql = "delete from office.MomentComment where ID='" + id + "'";
            int result = ViewBaseAdapter<object, List<object>>.Instance.RunSQLByTransaction(delSql, ConnectionNameDefine.YuanXinForDBHelp);
            return result;
        }
    }
}
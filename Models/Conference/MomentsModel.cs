using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 会议圈
    /// </summary>
    [ORTableMapping("office.Moment")]
    public class MomentModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [ORFieldMapping("ID", PrimaryKey = true)]
        public string ID { get; set; }
        /// <summary>
        /// 会议编码
        /// </summary>
        [ORFieldMapping("ConferenceID")]
        public string ConferenceID { get; set; }
        /// <summary>
        /// 发布内容
        /// </summary>
        [ORFieldMapping("Content")]
        public string Content { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        [ORFieldMapping("PraiseCount")]
        public int PraiseCount { get; set; }
        /// <summary>
        /// 发布人
        /// </summary>
        [ORFieldMapping("PublicUserID")]
        public string PublicUserID { get; set; }
        /// <summary>
        /// 发布人名称
        /// </summary>
        [ORFieldMapping("PublicUserName")]
        public string PublicUserName { get; set; }
        /// <summary>
        /// 发布日期
        /// </summary>
        [ORFieldMapping("PublicDate")]
        public DateTime PublicDate { get; set; }
        /// <summary>
        /// 会议圈类型
        /// </summary>
        [ORFieldMapping("MomentTypeId")]
        public Enum.EnumMomentType MomentTypeId { get; set; }
        /// <summary>
        /// 数据有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }
    public class MomentModelCollection : EditableDataObjectCollectionBase<MomentModel> { }
    public class MomentModelAdapter : BaseAdapter<MomentModel, MomentModelCollection>
    {
        public static MomentModelAdapter Instance = new MomentModelAdapter();
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public MomentModelAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }
        /// <summary>
        /// 分页查询会议圈列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="searchTime">首页查询时间</param>
        /// <returns></returns>
        public ViewPageBase<MomentModelCollection> GetConferenceModelListByPage(int pageIndex, DateTime searchTime)
        {
            if (searchTime == DateTime.MinValue || searchTime == null)
            {
                searchTime = DateTime.Now;
            }

            string selectSQL = "SELECT *";
            string fromAndWhereSQL = "FROM office.Moment WHERE CreateTime<'" + searchTime.ToString() + "'";
            string orderSQL = "order by CreateTime DESC";
            ViewPageBase<MomentModelCollection> pageData = GetTListByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            pageData.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return pageData;
        }

        /// <summary>
        /// 描述：根据时间查询是否有新的数据
        /// 作者：v-dengwh
        /// 邮箱：v-dengwh@sinooceanland.com
        /// 时间：2017-02-15 9:19
        /// </summary>
        /// <param name="lastTime"></param>
        /// <returns></returns>
        public MomentModelCollection GetMonentByLastTime(DateTime lastTime) {
            return Load( p =>  {
                p.AppendItem("PublicDate",lastTime,">");
                p.AppendItem("ValidStatus",true);
            });
        }

        /// <summary>
        /// 会议圈点赞数+1
        /// </summary>
        /// <param name="momentID"></param>
        public void AddMomentLikes(string momentID)
        {
            string sql = string.Format(@"UPDATE office.Moment SET PraiseCount=(PraiseCount+1) WHERE ID='{0}'", momentID);
            ViewBaseAdapter<object, List<object>>.Instance.RunSQLByTransaction(sql, ConnectionNameDefine.YuanXinForDBHelp);
        }
        /// <summary>
        /// 会议圈点赞数-1
        /// </summary>
        /// <param name="momentID"></param>
        public void DelMomentLikes(string momentID)
        {
            string sql = string.Format(@"UPDATE office.Moment SET PraiseCount=(PraiseCount-1) WHERE ID='{0}'", momentID);
            ViewBaseAdapter<object, List<object>>.Instance.RunSQLByTransaction(sql, ConnectionNameDefine.YuanXinForDBHelp);
        }
    }
}
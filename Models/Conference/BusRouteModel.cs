using MCS.Library.Data.Builder;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Seagull2.YuanXin.AppApi.Models
{
    /// <summary>
    /// 班车路线
    /// </summary>
    [ORTableMapping("office.BusRoute")]
    public class BusRouteModel
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
        /// 发车时间
        /// </summary>
        [ORFieldMapping("DepartDate")]
        public DateTime DepartDate { get; set; }
        /// <summary>
        /// 发车时间Str
        /// </summary>
        [NoMapping]
        public string DepartDateStr
        {
            get
            {
                return this.DepartDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        [ORFieldMapping("Title")]
        public string Title { get; set; }
        /// <summary>
        /// 发车地点
        /// </summary>
        [ORFieldMapping("BeginPlace")]
        public string BeginPlace { get; set; }
        /// <summary>
        /// 目的地
        /// </summary>
        [ORFieldMapping("EndPlace")]
        public string EndPlace { get; set; }
        /// <summary>
        /// 对接人姓名
        /// </summary>
        [ORFieldMapping("ContactsName")]
        public string ContactsName { get; set; }
        /// <summary>
        /// 对接人电话
        /// </summary>
        [ORFieldMapping("ContactsPhone")]
        public string ContactsPhone { get; set; }
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
        /// 数据有效性
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BusRouteModelCollection : EditableDataObjectCollectionBase<BusRouteModel>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class BusRouteModelAdapter : BaseAdapter<BusRouteModel, BusRouteModelCollection>
    {
        public static BusRouteModelAdapter Instance = new BusRouteModelAdapter();
        private string ConnectionString = ConnectionNameDefine.YuanXinBusiness;
        public BusRouteModelAdapter()
        {
            BaseConnectionStr = this.ConnectionString;
        }

        public BusRouteModelCollection LoadByConferenceCode(string code)
        {
            return Load(
                w =>
                {
                    w.AppendItem("ConferenceID", code);
                }
            );
        }

        public int AddBusRouteModelCollection(BusRouteModelCollection busRouteColl)
        {
            StringBuilder sql = new StringBuilder();
            busRouteColl.ForEach(bus =>
            {
                sql.Append(ORMapping.GetInsertSql<BusRouteModel>(bus, TSqlBuilder.Instance, "") + ";");
            });
            return ViewBaseAdapter<BusRouteModel, List<BusRouteModel>>.Instance.RunSQLByTransaction(sql.ToString(), ConnectionNameDefine.YuanXinForDBHelp);
        }

        /// <summary>
        /// 分页查询班车路线列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="searchTime">首页查询时间</param>
        /// <param name="conferenceCode">会议编码</param>
        /// <returns></returns>
        public ViewPageBase<BusRouteModelCollection> GetConferenceModelListByPage(int pageIndex, DateTime searchTime, string conferenceCode)
        {
          

            string selectSQL = "SELECT *";
            string fromAndWhereSQL = "FROM office.BusRoute WHERE CreateTime<='" + searchTime + "' AND ConferenceID='" + conferenceCode + "'";
            string orderSQL = "order by CreateTime DESC";
            ViewPageBase<BusRouteModelCollection> pageData = GetTListByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            pageData.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return pageData;
        }
        /// <summary>
        /// 根据会议编码删除班车路线
        /// </summary>
        /// <param name="conferenceID">会议编码</param>
        public void DelBusRouteByConferenceID(string conferenceID)
        {
            string sql = string.Format(@"DELETE FROM office.BusRoute where ConferenceID='{0}'", conferenceID);
            Delete(m => m.AppendItem("ConferenceID", conferenceID));
        }
    }
}
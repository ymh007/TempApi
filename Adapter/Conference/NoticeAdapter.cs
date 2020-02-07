using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 会议通告Adapter
    /// </summary>
    public class NoticeAdapter : BaseAdapter<NoticeModel, NoticeCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly NoticeAdapter Instance = new NoticeAdapter();
        
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据ID获取详情
        /// </summary>
        public NoticeModel LoadByID(string id)
        {
            return Load(m => m.AppendItem("ID", id)).SingleOrDefault();
        }

        /// <summary>
        /// 分页查询会议列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="searchTime">首页查询时间</param>
        /// <param name="conferenceID">会议编码</param>
        /// <returns></returns>
        public ViewPageBase<NoticeCollection> GetTopicsListsByPage(string conferenceID, int pageIndex, DateTime searchTime)
        {
           
            string selectSQL = "SELECT *";
            string fromAndWhereSQL = "FROM office.[ConferenceNotice] WHERE ConferenceID='" + conferenceID + "'and  CreateTime<='" + searchTime.ToString() + "'";
            string orderSQL = "order by CreateTime DESC";
            ViewPageBase<NoticeCollection> pageData = GetTListByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            pageData.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return pageData;
        }
        /// <summary>
        /// 查询会议下最新的通告
        /// </summary>
        /// <param name="conferenceID"></param>
        /// <returns></returns>
        public NoticeModel GetNewNotic(string conferenceID)
        {
            NoticeModel model = new NoticeModel();
            string sql = string.Format(@"SELECT TOP(1) * FROM office.Notic ORDER BY PublicDate DESC");
            NoticeCollection modelColl = QueryData(sql);
            if (modelColl.Count > 0)
            {
                model = modelColl[0];
            }
            return model;
        }
        /// <summary>
        /// 删除会议通告
        /// </summary>
        /// <param name="id"></param>
        public void DelNotic(string id)
        {
            Delete(m => m.AppendItem("ID", id));
        }
    }
}
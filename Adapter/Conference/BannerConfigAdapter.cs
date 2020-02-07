using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using Seagull2.YuanXin.AppApi.ViewsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 轮播图Adapter
    /// </summary>
    public class BannerConfigAdapter : BaseAdapter<BannerConfig, BannerConfigCollection>
    {
        public static readonly BannerConfigAdapter Instance = new BannerConfigAdapter();

        public BannerConfigAdapter()
        {

        }
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据编码查询
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public BannerConfig GetTopBanerBySourceID(string sourceId)
        {
            return Load(m => m.AppendItem("SourceId", sourceId)).SingleOrDefault();
        }
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public BannerConfigCollection GetAll()
        {
            return Load(m => m.AppendItem("ValidStatus", true));
        }

        /// <summary>
        /// 分页查询--轮播图数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="searchTime"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public ViewPageBase<BannerConfigCollection> GetTopBanerListByPage(int pageIndex, DateTime searchTime, string searchName)
        {
            if (searchTime == DateTime.MinValue || searchTime == null)
            {
                searchTime = DateTime.Now;
            }
            string selectSQL = "SELECT bc.*";
            string fromAndWhereSQL = string.Format(@"FROM office.BannerConfig bc
                                                    LEFT JOIN office.Conference con ON con.ID=bc.SourceId
                                                    LEFT JOIN office.Topic topic ON topic.TopicId=bc.SourceId 
                                                    WHERE bc.CreateTime<'" + searchTime.ToString() + "'");
            if (!searchName.IsEmptyOrNull() && searchName != "null")
            {
                fromAndWhereSQL += string.Format(@" AND (con.Name LIKE '%{0}%' OR topic.Title LIKE '%{1}%')", searchName, searchName);
            }
            string orderSQL = "order by SortNo";
            ViewPageBase<BannerConfigCollection> pageData = GetTListByPage(selectSQL, fromAndWhereSQL, orderSQL, pageIndex);
            pageData.FirstPageSearchTime = searchTime.ToString("yyyy-MM-dd HH:mm:ss");

            return pageData;
        }

    }
}
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    /// <summary>
    /// 现场服务
    /// </summary>
    public class SiteServiceAdapter : BaseAdapter<SiteServiceModel, SiteServiceCollection>
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static readonly SiteServiceAdapter Instance = new SiteServiceAdapter();

        /// <summary>
        /// 构造
        /// </summary>
        public SiteServiceAdapter()
        {
            BaseConnectionStr = ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void AddSiteService(SiteServiceModel siteService)
        {
            Update(siteService);
        }

        /// <summary>
        /// 根据ID获取
        /// </summary>
        public SiteServiceModel LoadByID(string id)
        {
            return Load(m => m.AppendItem("ID", id)).SingleOrDefault();
        }
    }
}
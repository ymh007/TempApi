using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using Seagull2.YuanXin.AppApi.Models.Conference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Adapter.Conference
{
    public class SiteServiceTypeAdapter : UpdatableAndLoadableAdapterBase<SiteServiceTypeModel, SiteServiceTypeCollection>
    {
        public static readonly SiteServiceTypeAdapter Instance = new SiteServiceTypeAdapter();

        public SiteServiceTypeAdapter()
        {

        }
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 查询会议服务类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SiteServiceTypeModel LoadSiteServiceType(string id)
        {
          return   this.Load(p => {
                p.AppendItem("ID",id);
            }).FirstOrDefault();
        }
    }
}
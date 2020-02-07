using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models.Adavertisement;
using Seagull2.YuanXin.AppApi.Models;
using log4net;
using System.Reflection;

namespace Seagull2.YuanXin.AppApi.Adapter.Advertisement
{
    /// <summary>
    /// 广告Adapter
    /// </summary>
    public class AdvertisementAdapter : UpdatableAndLoadableAdapterBase<AdvertisementModel, AdvertisementCollection>
    {
        /// <summary>
        /// 适配器实例化
        /// </summary>
        public static readonly AdvertisementAdapter Instance = new AdvertisementAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 根据类型获取数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AdvertisementCollection GetDataByType(int type)
        {
            return this.Load(
                        m =>
                        {
                            m.AppendItem("Type", type);
                            m.AppendItem("StartTime", DateTime.Now, "<");
                            m.AppendItem("EndTime", DateTime.Now, ">");
                        },
                        o =>
                        {
                            o.AppendItem("CreateTime", MCS.Library.Data.Builder.FieldSortDirection.Descending);
                        }
                    );
        }
    }
}
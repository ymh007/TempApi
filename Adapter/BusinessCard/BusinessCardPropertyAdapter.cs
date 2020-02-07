using log4net;
using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Seagull2.YuanXin.AppApi.Models.BusinessCard;

namespace Seagull2.YuanXin.AppApi.Adapter.BusinessCard
{
    /// <summary>
    /// 个人名片属性
    /// </summary>
    public class BusinessCardPropertyAdapter : UpdatableAndLoadableAdapterBase<BusinessCardPropertyModel, BusinessCardPropertyCollection>
    {
        /// <summary>
        /// 适配器实例化
        /// </summary>
        public static readonly BusinessCardPropertyAdapter Instance = new BusinessCardPropertyAdapter();

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionNameDefine.YuanXinBusiness;
        }

        /// <summary>
        /// 日志实例化
        /// </summary>
        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 保存属性列表数据
        /// </summary>
        public void SavePropertyList(List<string> list, int type, string cardCode, string userCode)
        {
            list.ForEach(item =>
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    Update(new BusinessCardPropertyModel()
                    {
                        Code = Guid.NewGuid().ToString(),
                        BusinessCardCode = cardCode,
                        Type = type,
                        TypeValue = item,
                        Creator = userCode,
                        CreateTime = DateTime.Now,
                        ValidStatus = true
                    });
                }
            });
        }
    }
}
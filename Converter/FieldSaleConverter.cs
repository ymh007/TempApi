using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using Seagull2.YuanXin.AppApi.Configuration;
using Seagull2.YuanXin.AppApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Converter
{
    public class FieldSaleConverter: IUrlConverter
    {
        public static readonly FieldSaleConverter Instance = new FieldSaleConverter();

        public string Convert(UserTaskModel task, UrlMappingElement settings)
        {
            IWfProcess process = WfRuntime.GetProcessByResourceID(task.ResourceID).FirstOrDefault();
            string sceneId = process.CurrentActivity.Descriptor.Scene;
            if (!string.IsNullOrEmpty(settings.ExceptScence))
            {
                string[] exceptList = settings.ExceptScence.Split(';');
                if (exceptList.Contains(sceneId))
                {
                    return string.Empty;
                }
                else
                {
                    return task.Url.ToLower().Replace(settings.PcUrl.ToLower(), settings.MobileUrl.ToLower());
                }
            }
            else
            {
                return task.Url.ToLower().Replace(settings.PcUrl.ToLower(), settings.MobileUrl.ToLower());

            }
        }
    }
}
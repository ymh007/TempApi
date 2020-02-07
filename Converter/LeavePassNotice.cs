using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Configuration;
using Seagull2.YuanXin.AppApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Converter
{
    public class LeavePassNotice: IUrlConverter
    {
        public static readonly LeavePassNotice Instance = new LeavePassNotice();

        public string Convert(UserTaskModel task, UrlMappingElement settings)
        {
            string urlString = UrlConvertHelper.ResolveUri(settings.MobileUrl);

            return string.Format("{0}?ResourceID={1}&TaskID={2}", urlString, task.ResourceID, task.TaskID);
        }
    }
}
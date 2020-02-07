using MCS.Library.SOA.DataObjects;
using Seagull2.YuanXin.AppApi.Configuration;
using Seagull2.YuanXin.AppApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull2.YuanXin.AppApi.Converter
{
    public class DefaultConverter: IUrlConverter
    {
        public static readonly DefaultConverter Instance = new DefaultConverter();

        public string Convert(UserTaskModel task, UrlMappingElement settings)
        {
            return task.Url.ToLower().Replace(settings.PcUrl.ToLower(), settings.MobileUrl.ToLower());
        }
    }
}